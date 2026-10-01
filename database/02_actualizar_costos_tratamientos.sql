USE HospitalDB;
GO

/*
   Los tratamientos empiezan en cero. El costo se calcula únicamente con
   cantidad prescrita × costo unitario de cada medicamento.
*/
IF EXISTS
(
    SELECT 1
    FROM sys.check_constraints
    WHERE name = 'CHK_Costo_Tratamiento'
      AND parent_object_id = OBJECT_ID('dbo.Tratamiento')
)
BEGIN
    ALTER TABLE dbo.Tratamiento
    DROP CONSTRAINT CHK_Costo_Tratamiento;
END;
GO

ALTER TABLE dbo.Tratamiento
ADD CONSTRAINT CHK_Costo_Tratamiento
CHECK (costo >= 0);
GO

/*
   Valida y descuenta el stock del hospital del paciente. Después actualiza
   el costo total de cada tratamiento afectado por la nueva prescripción.
*/
CREATE OR ALTER TRIGGER dbo.trg_ControlarStockMedicamento
ON dbo.Prescripcion
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Solicitud TABLE
    (
        id_hospital INT NOT NULL,
        id_medicamento INT NOT NULL,
        cantidad_solicitada INT NOT NULL,
        PRIMARY KEY (id_hospital, id_medicamento)
    );

    INSERT INTO @Solicitud
    (
        id_hospital,
        id_medicamento,
        cantidad_solicitada
    )
    SELECT
        paciente.id_hospital,
        inserted.id_medicamento,
        SUM(inserted.cantidad)
    FROM inserted
    INNER JOIN dbo.Tratamiento tratamiento
        ON tratamiento.id_tratamiento = inserted.id_tratamiento
    INNER JOIN dbo.Cita cita
        ON cita.id_cita = tratamiento.id_cita
    INNER JOIN dbo.Paciente paciente
        ON paciente.id_paciente = cita.id_paciente
    GROUP BY
        paciente.id_hospital,
        inserted.id_medicamento;

    IF EXISTS
    (
        SELECT 1
        FROM @Solicitud solicitud
        LEFT JOIN dbo.Inventario_Hospital inventario WITH (UPDLOCK, HOLDLOCK)
            ON inventario.id_hospital = solicitud.id_hospital
           AND inventario.id_medicamento = solicitud.id_medicamento
        WHERE ISNULL(inventario.cantidad_stock, 0)
              < solicitud.cantidad_solicitada
    )
    BEGIN
        RAISERROR
        (
            'Error: Stock insuficiente en el hospital del paciente.',
            16,
            1
        );
        ROLLBACK TRANSACTION;
        RETURN;
    END;

    UPDATE inventario
    SET cantidad_stock =
        inventario.cantidad_stock - solicitud.cantidad_solicitada
    FROM dbo.Inventario_Hospital inventario
    INNER JOIN @Solicitud solicitud
        ON solicitud.id_hospital = inventario.id_hospital
       AND solicitud.id_medicamento = inventario.id_medicamento;

    ;WITH TratamientosAfectados AS
    (
        SELECT DISTINCT id_tratamiento
        FROM inserted
    )
    UPDATE tratamiento
    SET costo = ISNULL
    (
        (
            SELECT SUM
            (
                CAST(prescripcion.cantidad AS DECIMAL(10, 2))
                * medicamento.costo_unitario
            )
            FROM dbo.Prescripcion prescripcion
            INNER JOIN dbo.Medicamento medicamento
                ON medicamento.id_medicamento = prescripcion.id_medicamento
            WHERE prescripcion.id_tratamiento = tratamiento.id_tratamiento
        ),
        0
    )
    FROM dbo.Tratamiento tratamiento
    INNER JOIN TratamientosAfectados afectados
        ON afectados.id_tratamiento = tratamiento.id_tratamiento;
END;
GO

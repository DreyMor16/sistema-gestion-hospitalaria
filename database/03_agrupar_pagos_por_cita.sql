USE HospitalDB;
GO

/*
   Un pendiente corresponde a una cita completa. El paciente ve y paga en
   conjunto todos los tratamientos pendientes de esa atención.
*/
CREATE OR ALTER PROCEDURE dbo.sp_HistorialPagosPendientesPaciente
    @IdPaciente INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        cita.id_cita,
        STUFF
        (
            (
                SELECT ' · ' + tratamientoInterno.descripcion
                FROM dbo.Tratamiento tratamientoInterno
                WHERE tratamientoInterno.id_cita = cita.id_cita
                  AND tratamientoInterno.costo > 0
                  AND NOT EXISTS
                  (
                      SELECT 1
                      FROM dbo.Pago pagoInterno
                      WHERE pagoInterno.id_tratamiento =
                          tratamientoInterno.id_tratamiento
                  )
                ORDER BY tratamientoInterno.id_tratamiento
                FOR XML PATH(''), TYPE
            ).value('.', 'VARCHAR(MAX)'),
            1,
            3,
            ''
        ) AS detalle_tratamientos,
        SUM(tratamiento.costo) AS monto_pendiente,
        cita.fecha AS fecha_cita
    FROM dbo.Tratamiento tratamiento
    INNER JOIN dbo.Cita cita
        ON cita.id_cita = tratamiento.id_cita
    WHERE cita.id_paciente = @IdPaciente
      AND tratamiento.costo > 0
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.Pago pago
          WHERE pago.id_tratamiento = tratamiento.id_tratamiento
      )
    GROUP BY cita.id_cita, cita.fecha
    HAVING SUM(tratamiento.costo) > 0
    ORDER BY cita.fecha DESC, cita.id_cita DESC;
END;
GO

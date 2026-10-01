CREATE DATABASE HospitalDB;
GO

USE HospitalDB;
GO

CREATE TABLE Hospital (
id_hospital INT IDENTITY(1,1) PRIMARY KEY,
nombre VARCHAR(100) NOT NULL,
direccion VARCHAR(200) NOT NULL,
telefono VARCHAR(20) NOT NULL
);
GO
CREATE TABLE Usuario (
    id_usuario INT IDENTITY(1,1) PRIMARY KEY,
    usuario VARCHAR(50) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL
);
GO
CREATE TABLE Persona (
    id_persona INT IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    apellido VARCHAR(50) NOT NULL,
    telefono VARCHAR(20),
    correo VARCHAR(100) UNIQUE,
    cedula VARCHAR(20) NOT NULL UNIQUE,
    id_usuario INT NULL,
    CONSTRAINT FK_Persona_Usuario
        FOREIGN KEY (id_usuario) REFERENCES Usuario(id_usuario) ON DELETE SET NULL 
);

CREATE TABLE Paciente (
    id_paciente INT IDENTITY(1,1) PRIMARY KEY,
    fecha_nacimiento DATE NOT NULL,
    genero VARCHAR(20) NOT NULL,
    direccion VARCHAR(200),
    id_persona INT NOT NULL UNIQUE,
    id_hospital INT NOT NULL,
    CONSTRAINT FK_Paciente_Persona
        FOREIGN KEY (id_persona) REFERENCES Persona(id_persona),
    CONSTRAINT FK_Paciente_Hospital
        FOREIGN KEY (id_hospital) REFERENCES Hospital(id_hospital),
    CONSTRAINT CHK_Genero CHECK (genero IN ('Masculino','Femenino','Otro'))
);
GO

CREATE TABLE Medico (
    id_medico INT IDENTITY(1,1) PRIMARY KEY,
    especialidad VARCHAR(100) NOT NULL,
    id_persona INT NOT NULL UNIQUE,
    id_hospital INT NOT NULL,

    CONSTRAINT FK_Medico_Persona
        FOREIGN KEY (id_persona) REFERENCES Persona(id_persona),

    CONSTRAINT FK_Medico_Hospital
        FOREIGN KEY (id_hospital) REFERENCES Hospital(id_hospital)
);
GO

CREATE TABLE Empleado (
    id_empleado INT IDENTITY(1,1) PRIMARY KEY,
    puesto VARCHAR(50) NOT NULL,
    id_persona INT NOT NULL UNIQUE,

    CONSTRAINT FK_Empleado_Persona
        FOREIGN KEY (id_persona) REFERENCES Persona(id_persona)

);
GO

CREATE TABLE Cita (
id_cita INT IDENTITY(1,1) PRIMARY KEY,
fecha DATE NOT NULL,
hora TIME NOT NULL,
diagnostico VARCHAR(255),
Estado VARCHAR(20) NOT NULL 
CONSTRAINT CHK_Cita_Estado CHECK (Estado IN ('En proceso', 'Finalizada', 'Cancelada')),
id_paciente INT NOT NULL,
id_medico INT NOT NULL,
CONSTRAINT FK_Cita_Paciente FOREIGN KEY (id_paciente)
    REFERENCES Paciente(id_paciente),
CONSTRAINT FK_Cita_Medico FOREIGN KEY (id_medico)
    REFERENCES Medico(id_medico),
CONSTRAINT UQ_Cita_Medico_FechaHora UNIQUE(id_medico, fecha, hora)
);
GO

CREATE TABLE Tratamiento (
id_tratamiento INT IDENTITY(1,1) PRIMARY KEY,
descripcion VARCHAR(500) NOT NULL,
costo DECIMAL(10,2) NOT NULL,
id_cita INT NOT NULL,
CONSTRAINT CHK_Costo_Tratamiento
     CHECK (costo >= 0),
CONSTRAINT FK_Tratamiento_Cita FOREIGN KEY (id_cita)
    REFERENCES Cita(id_cita)
);
GO

CREATE TABLE Medicamento (
id_medicamento INT IDENTITY(1,1) PRIMARY KEY,
nombre VARCHAR(100) NOT NULL,
descripcion VARCHAR(300),
costo_unitario DECIMAL(10,2) NOT NULL,
CONSTRAINT CHK_Costo_Medicamento
    CHECK (costo_unitario > 0)
);
GO

CREATE TABLE Inventario_Hospital (
id_hospital INT NOT NULL,
id_medicamento INT NOT NULL,
cantidad_stock INT NOT NULL,
PRIMARY KEY (id_hospital, id_medicamento),
CONSTRAINT CHK_Stock
    CHECK (cantidad_stock >= 0),
CONSTRAINT FK_Inventario_Hospital FOREIGN KEY (id_hospital)
    REFERENCES Hospital(id_hospital),
CONSTRAINT FK_Inventario_Medicamento FOREIGN KEY (id_medicamento)
    REFERENCES Medicamento(id_medicamento)
);
GO

CREATE TABLE Prescripcion (
id_prescripcion INT IDENTITY(1,1) PRIMARY KEY,
id_tratamiento INT NOT NULL,
id_medicamento INT NOT NULL,
cantidad INT NOT NULL,
dosis VARCHAR(100) NOT NULL,
CONSTRAINT CHK_Cantidad
    CHECK (cantidad > 0),
CONSTRAINT FK_Prescripcion_Tratamiento FOREIGN KEY (id_tratamiento)
    REFERENCES Tratamiento(id_tratamiento),
CONSTRAINT FK_Prescripcion_Medicamento FOREIGN KEY (id_medicamento)
    REFERENCES Medicamento(id_medicamento)
);
GO

CREATE TABLE Pago (
id_pago INT IDENTITY(1,1) PRIMARY KEY,
fecha_pago DATE NOT NULL,
monto DECIMAL(10,2) NOT NULL,
metodo_pago VARCHAR(50) NOT NULL,
id_tratamiento INT NOT NULL,
CONSTRAINT CHK_Monto
    CHECK (monto > 0),
CONSTRAINT FK_Pago_Tratamiento FOREIGN KEY (id_tratamiento)
    REFERENCES Tratamiento(id_tratamiento),
CONSTRAINT CHK_Metodo_Pago
    CHECK (
        metodo_pago IN ('Efectivo', 'Tarjeta', 'Sinpe')
          )
);
GO


-----------Procedimientos Amacenados y Trigger----------------------
/*1. Procedimiento almacenado para Obtener todos los pacientes atendidos por un médico
en un hospital específico durante un periodo determinado*/
CREATE PROCEDURE sp_PacientesAtendidosPorMedico
    @IdMedico INT,
    @IdHospital INT,
    @FechaInicio DATE = NULL,
    @FechaFin DATE = NULL,
    @Filtro VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @FiltroLimpio VARCHAR(100) =
        LTRIM(RTRIM(ISNULL(@Filtro, '')));

    SELECT
        p.id_paciente,
        per.cedula,
        per.nombre,
        per.apellido,
        MAX(c.fecha) AS fecha_ultima_atencion
    FROM Cita c
        INNER JOIN Paciente p
            ON c.id_paciente = p.id_paciente
        INNER JOIN Persona per
            ON p.id_persona = per.id_persona
        INNER JOIN Medico m
            ON c.id_medico = m.id_medico
    WHERE c.id_medico = @IdMedico
      AND m.id_hospital = @IdHospital
      AND c.Estado = 'Finalizada'
      AND (@FechaInicio IS NULL OR c.fecha >= @FechaInicio)
      AND (@FechaFin IS NULL OR c.fecha <= @FechaFin)
      AND
      (
          @FiltroLimpio = ''
          OR per.nombre LIKE '%' + @FiltroLimpio + '%'
          OR per.apellido LIKE '%' + @FiltroLimpio + '%'
          OR per.cedula LIKE '%' + @FiltroLimpio + '%'
          OR LTRIM(RTRIM(per.nombre + ' ' + per.apellido))
             LIKE '%' + @FiltroLimpio + '%'
          OR LTRIM(RTRIM(per.apellido + ' ' + per.nombre))
             LIKE '%' + @FiltroLimpio + '%'
      )
    GROUP BY
        p.id_paciente,
        per.cedula,
        per.nombre,
        per.apellido
    ORDER BY
        MAX(c.fecha) DESC,
        per.apellido,
        per.nombre;
END;
GO

/*2. Procedimiento almacenado Obtener el inventario de medicamentos y las prescripciones realizadas en 
los últimos 30 días para un hospital específico*/
CREATE PROCEDURE sp_InventarioYPrescripcionesUltimos30Dias
    @IdHospital INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        m.id_medicamento,
        m.nombre AS medicamento,
        ih.cantidad_stock AS stock_actual,
        ISNULL(
            SUM(
                CASE
                    WHEN c.fecha >= DATEADD(DAY, -30, CAST(GETDATE() AS DATE)) 
                         AND p.id_hospital = @IdHospital 
                    THEN pr.cantidad
                    ELSE 0
                END
            ), 0
        ) AS total_prescrito_30_dias

    FROM Inventario_Hospital ih
        INNER JOIN Medicamento m
            ON ih.id_medicamento = m.id_medicamento
        LEFT JOIN Prescripcion pr
            ON m.id_medicamento = pr.id_medicamento
        LEFT JOIN Tratamiento t
            ON pr.id_tratamiento = t.id_tratamiento
        LEFT JOIN Cita c
            ON t.id_cita = c.id_cita
        LEFT JOIN Paciente p
            ON c.id_paciente = p.id_paciente
    WHERE ih.id_hospital = @IdHospital

    GROUP BY
        m.id_medicamento,
        m.nombre,
        ih.cantidad_stock
    ORDER BY
        m.nombre;
END;
GO

/*3. Procedimiento almacenado Obtener el historial de pagos pendientes de un paciente:*/
CREATE PROCEDURE sp_HistorialPagosPendientesPaciente
    @IdPaciente INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        cita.id_cita,

        STUFF
        (
            (
                SELECT ' - ' + tratamientoInterno.descripcion
                FROM Tratamiento tratamientoInterno
                WHERE tratamientoInterno.id_cita = cita.id_cita
                  AND tratamientoInterno.costo > 0
                  AND NOT EXISTS
                  (
                      SELECT 1
                      FROM Pago pagoInterno
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

    FROM Tratamiento tratamiento
    INNER JOIN Cita cita
        ON cita.id_cita = tratamiento.id_cita

    WHERE cita.id_paciente = @IdPaciente
      AND tratamiento.costo > 0
      AND NOT EXISTS
      (
          SELECT 1
          FROM Pago pago
          WHERE pago.id_tratamiento = tratamiento.id_tratamiento
      )

    GROUP BY cita.id_cita, cita.fecha
    HAVING SUM(tratamiento.costo) > 0
    ORDER BY cita.fecha DESC, cita.id_cita DESC;
END;
GO

/*1. Procedimiento Almacenado para Registrar una Nueva Cita y Actualizar la Disponibilidad del Médico:*/

CREATE PROCEDURE sp_RegistrarCita
    @IdPaciente INT,
    @IdMedico INT,
    @Fecha DATE,
    @Hora TIME,
    @Diagnostico VARCHAR(255),
    @Estado VARCHAR(20) 
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Validar disponibilidad (verificar si ya está ocupado)
    -- Nota: Solo validamos citas que no estén canceladas
    IF EXISTS (
        SELECT 1 
        FROM Cita 
        WHERE id_medico = @IdMedico 
          AND fecha = @Fecha 
          AND hora = @Hora
          AND estado != 'Cancelada' 
    )
    BEGIN
        RAISERROR ('El médico ya tiene una cita activa asignada en este horario.', 16, 1);
        RETURN;
    END

    INSERT INTO Cita (fecha, hora, diagnostico, id_paciente, id_medico, estado)
    VALUES (@Fecha, @Hora, @Diagnostico, @IdPaciente, @IdMedico, @Estado);

    PRINT 'Cita registrada exitosamente.';
END;
GO

/*2.Trigger para Controlar el Stock de Medicamentos*/
CREATE TRIGGER trg_ControlarStockMedicamento
ON Prescripcion
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
    INNER JOIN Tratamiento tratamiento
        ON tratamiento.id_tratamiento = inserted.id_tratamiento
    INNER JOIN Cita cita
        ON cita.id_cita = tratamiento.id_cita
    INNER JOIN Paciente paciente
        ON paciente.id_paciente = cita.id_paciente
    GROUP BY
        paciente.id_hospital,
        inserted.id_medicamento;

    IF EXISTS
    (
        SELECT 1
        FROM @Solicitud solicitud
        LEFT JOIN Inventario_Hospital inventario
            ON inventario.id_hospital = solicitud.id_hospital
           AND inventario.id_medicamento = solicitud.id_medicamento
        WHERE ISNULL(inventario.cantidad_stock, 0)
              < solicitud.cantidad_solicitada
    )
    BEGIN
        RAISERROR(
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
    FROM Inventario_Hospital inventario
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
            FROM Prescripcion prescripcion
            INNER JOIN Medicamento medicamento
                ON medicamento.id_medicamento =
                    prescripcion.id_medicamento
            WHERE prescripcion.id_tratamiento =
                tratamiento.id_tratamiento
        ),
        0
    )
    FROM Tratamiento tratamiento
    INNER JOIN TratamientosAfectados afectados
        ON afectados.id_tratamiento =
            tratamiento.id_tratamiento;
END;
GO

/*Procedimiento Almacenado para Calcular el Total de Pagos Realizados por un Paciente en un Rango de Fechas*/
CREATE PROCEDURE sp_TotalPagadoPorPaciente
    @IdPaciente INT,
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ISNULL(SUM(p.monto), 0) AS total_pagado
    FROM Pago p
    INNER JOIN Tratamiento t ON p.id_tratamiento = t.id_tratamiento
    INNER JOIN Cita c ON t.id_cita = c.id_cita
    WHERE c.id_paciente = @IdPaciente
      AND p.fecha_pago BETWEEN @FechaInicio AND @FechaFin;
END;
GO

-----------------------Insertar Datos-------------------

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);

    ---------------------------------------------------------
    -- 1. HOSPITALES
    ---------------------------------------------------------

    INSERT INTO Hospital (nombre, direccion, telefono)
    VALUES
        ('Hospital Central', 'San Jose Centro', '22221111'),
        ('Hospital Cartago', 'Cartago Centro', '22221112'),
        ('Hospital Pacifico', 'Puntarenas Centro', '22221113');

    ---------------------------------------------------------
    -- 2. USUARIOS
    -- Contraseña de demostración: 123456 (PBKDF2-HMAC-SHA256)
    ---------------------------------------------------------

    INSERT INTO Usuario (usuario, password)
    VALUES
        ('admin1', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),
        ('recepcion1', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),
        ('recepcion2', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),

        ('drmartinez', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),
        ('drgarcia', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),
        ('drrojas', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),
        ('drsolano', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),
        ('drcampos', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),
        ('drvega', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),
        ('drcastro', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),
        ('drfuentes', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),
        ('drnavarro', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),
        ('drlopez', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),
        ('drherrera', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),
        ('drmora', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),
        ('drarias', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),

        ('paciente1', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),
        ('paciente2', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),
        ('paciente3', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),
        ('paciente4', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),
        ('paciente5', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),
        ('paciente6', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),
        ('paciente7', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),
        ('paciente8', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),
        ('paciente9', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY='),
        ('paciente10', 'PBKDF2$210000$FJN6+pmihW8L+wo+4MiVtw==$0Yv+57j3ftoLNyGIQadbe5Pvp5mdYDAC48l0Ds6SgUY=');

    ---------------------------------------------------------
    -- 3. PERSONAS
    -- Cédulas de 9 dígitos y teléfonos de 8 dígitos.
    ---------------------------------------------------------

    INSERT INTO Persona
    (nombre, apellido, telefono, correo, cedula, id_usuario)
    VALUES
        ('Laura', 'Ramirez', '80001001', 'laura@hospital.com',
         '101010101',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'admin1')),

        ('Sofia', 'Mora', '80001002', 'sofia@hospital.com',
         '101010102',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'recepcion1')),

        ('Daniela', 'Vargas', '80001003', 'daniela@hospital.com',
         '101010103',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'recepcion2')),

        ('Andres', 'Martinez', '81001001', 'andres@hospital.com',
         '201000001',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'drmartinez')),

        ('Monica', 'Garcia', '81001002', 'monica@hospital.com',
         '201000002',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'drgarcia')),

        ('Valeria', 'Rojas', '81001003', 'valeria@hospital.com',
         '201000003',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'drrojas')),

        ('Diego', 'Solano', '81001004', 'diego@hospital.com',
         '201000004',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'drsolano')),

        ('Paula', 'Campos', '81001005', 'paula@hospital.com',
         '201000005',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'drcampos')),

        ('Mariana', 'Vega', '81001006', 'mariana@hospital.com',
         '201000006',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'drvega')),

        ('Sofia', 'Castro', '81001007', 'sofiac@hospital.com',
         '201000007',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'drcastro')),

        ('Jorge', 'Fuentes', '81001008', 'jorge@hospital.com',
         '201000008',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'drfuentes')),

        ('Lucia', 'Navarro', '81001009', 'lucia@hospital.com',
         '201000009',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'drnavarro')),

        ('Ricardo', 'Lopez', '81001010', 'ricardo@hospital.com',
         '201000010',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'drlopez')),

        ('Elena', 'Herrera', '81001011', 'elena@hospital.com',
         '201000011',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'drherrera')),

        ('Marco', 'Mora', '81001012', 'marco@hospital.com',
         '201000012',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'drmora')),

        ('Karla', 'Arias', '81001013', 'karla@hospital.com',
         '201000013',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'drarias')),

        ('Ana', 'Torres', '82001001', 'ana@correo.com',
         '301000001',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'paciente1')),

        ('Bruno', 'Vargas', '82001002', 'bruno@correo.com',
         '301000002',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'paciente2')),

        ('Carla', 'Jimenez', '82001003', 'carla@correo.com',
         '301000003',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'paciente3')),

        ('David', 'Herrera', '82001004', 'david@correo.com',
         '301000004',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'paciente4')),

        ('Elena', 'Salas', '82001005', 'elena.salas@correo.com',
         '301000005',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'paciente5')),

        ('Fabio', 'Rojas', '82001006', 'fabio@correo.com',
         '301000006',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'paciente6')),

        ('Gabriela', 'Mendez', '82001007', 'gabriela@correo.com',
         '301000007',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'paciente7')),

        ('Hugo', 'Castro', '82001008', 'hugo@correo.com',
         '301000008',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'paciente8')),

        ('Isabel', 'Mora', '82001009', 'isabel@correo.com',
         '301000009',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'paciente9')),

        ('Javier', 'Chaves', '82001010', 'javier@correo.com',
         '301000010',
         (SELECT id_usuario FROM Usuario WHERE usuario = 'paciente10'));

    ---------------------------------------------------------
    -- 4. EMPLEADOS
    ---------------------------------------------------------

    INSERT INTO Empleado (puesto, id_persona)
    VALUES
        ('Administrador',
         (SELECT id_persona FROM Persona WHERE cedula = '101010101')),

        ('Recepcionista',
         (SELECT id_persona FROM Persona WHERE cedula = '101010102')),

        ('Recepcionista',
         (SELECT id_persona FROM Persona WHERE cedula = '101010103'));

    ---------------------------------------------------------
    -- 5. MEDICOS Y ESPECIALIDADES
    ---------------------------------------------------------

    INSERT INTO Medico (especialidad, id_persona, id_hospital)
    VALUES
        ('Medicina General',
         (SELECT id_persona FROM Persona WHERE cedula = '201000001'),
         (SELECT id_hospital FROM Hospital WHERE nombre = 'Hospital Central')),

        ('Medicina General',
         (SELECT id_persona FROM Persona WHERE cedula = '201000002'),
         (SELECT id_hospital FROM Hospital WHERE nombre = 'Hospital Central')),

        ('Cardiologia',
         (SELECT id_persona FROM Persona WHERE cedula = '201000003'),
         (SELECT id_hospital FROM Hospital WHERE nombre = 'Hospital Central')),

        ('Pediatria',
         (SELECT id_persona FROM Persona WHERE cedula = '201000004'),
         (SELECT id_hospital FROM Hospital WHERE nombre = 'Hospital Central')),

        ('Neurologia',
         (SELECT id_persona FROM Persona WHERE cedula = '201000005'),
         (SELECT id_hospital FROM Hospital WHERE nombre = 'Hospital Central')),

        ('Medicina General',
         (SELECT id_persona FROM Persona WHERE cedula = '201000006'),
         (SELECT id_hospital FROM Hospital WHERE nombre = 'Hospital Cartago')),

        ('Ginecologia',
         (SELECT id_persona FROM Persona WHERE cedula = '201000007'),
         (SELECT id_hospital FROM Hospital WHERE nombre = 'Hospital Cartago')),

        ('Ortopedia',
         (SELECT id_persona FROM Persona WHERE cedula = '201000008'),
         (SELECT id_hospital FROM Hospital WHERE nombre = 'Hospital Cartago')),

        ('Dermatologia',
         (SELECT id_persona FROM Persona WHERE cedula = '201000009'),
         (SELECT id_hospital FROM Hospital WHERE nombre = 'Hospital Cartago')),

        ('Medicina General',
         (SELECT id_persona FROM Persona WHERE cedula = '201000010'),
         (SELECT id_hospital FROM Hospital WHERE nombre = 'Hospital Pacifico')),

        ('Odontologia',
         (SELECT id_persona FROM Persona WHERE cedula = '201000011'),
         (SELECT id_hospital FROM Hospital WHERE nombre = 'Hospital Pacifico')),

        ('Oftalmologia',
         (SELECT id_persona FROM Persona WHERE cedula = '201000012'),
         (SELECT id_hospital FROM Hospital WHERE nombre = 'Hospital Pacifico')),

        ('Nutricion',
         (SELECT id_persona FROM Persona WHERE cedula = '201000013'),
         (SELECT id_hospital FROM Hospital WHERE nombre = 'Hospital Pacifico'));

    ---------------------------------------------------------
    -- 6. PACIENTES
    ---------------------------------------------------------

    INSERT INTO Paciente
    (fecha_nacimiento, genero, direccion, id_persona, id_hospital)
    VALUES
        ('1994-05-12', 'Femenino', 'San Jose',
         (SELECT id_persona FROM Persona WHERE cedula = '301000001'),
         (SELECT id_hospital FROM Hospital WHERE nombre = 'Hospital Central')),

        ('1987-09-20', 'Masculino', 'San Jose',
         (SELECT id_persona FROM Persona WHERE cedula = '301000002'),
         (SELECT id_hospital FROM Hospital WHERE nombre = 'Hospital Central')),

        ('2015-03-08', 'Femenino', 'Desamparados',
         (SELECT id_persona FROM Persona WHERE cedula = '301000003'),
         (SELECT id_hospital FROM Hospital WHERE nombre = 'Hospital Central')),

        ('1990-11-15', 'Masculino', 'Heredia',
         (SELECT id_persona FROM Persona WHERE cedula = '301000004'),
         (SELECT id_hospital FROM Hospital WHERE nombre = 'Hospital Central')),

        ('1992-06-18', 'Femenino', 'Cartago',
         (SELECT id_persona FROM Persona WHERE cedula = '301000005'),
         (SELECT id_hospital FROM Hospital WHERE nombre = 'Hospital Cartago')),

        ('1985-02-10', 'Masculino', 'Paraiso',
         (SELECT id_persona FROM Persona WHERE cedula = '301000006'),
         (SELECT id_hospital FROM Hospital WHERE nombre = 'Hospital Cartago')),

        ('2000-08-24', 'Femenino', 'Oreamuno',
         (SELECT id_persona FROM Persona WHERE cedula = '301000007'),
         (SELECT id_hospital FROM Hospital WHERE nombre = 'Hospital Cartago')),

        ('1978-12-02', 'Masculino', 'El Guarco',
         (SELECT id_persona FROM Persona WHERE cedula = '301000008'),
         (SELECT id_hospital FROM Hospital WHERE nombre = 'Hospital Cartago')),

        ('1996-04-30', 'Femenino', 'Puntarenas',
         (SELECT id_persona FROM Persona WHERE cedula = '301000009'),
         (SELECT id_hospital FROM Hospital WHERE nombre = 'Hospital Pacifico')),

        ('1989-10-19', 'Masculino', 'Esparza',
         (SELECT id_persona FROM Persona WHERE cedula = '301000010'),
         (SELECT id_hospital FROM Hospital WHERE nombre = 'Hospital Pacifico'));

    ---------------------------------------------------------
    -- 7. MEDICAMENTOS
    ---------------------------------------------------------

    INSERT INTO Medicamento (nombre, descripcion, costo_unitario)
    VALUES
        ('Paracetamol', 'Analgesico y antipiretico', 500),
        ('Ibuprofeno', 'Antiinflamatorio no esteroideo', 800),
        ('Amoxicilina', 'Antibiotico', 1200),
        ('Azitromicina', 'Antibiotico de amplio espectro', 1600),
        ('Omeprazol', 'Protector gastrico', 950),
        ('Loratadina', 'Antihistaminico', 650),
        ('Salbutamol', 'Broncodilatador', 2200),
        ('Losartan', 'Antihipertensivo', 1100),
        ('Atorvastatina', 'Control de colesterol', 1300),
        ('Metformina', 'Control de glucosa', 700),
        ('Diclofenaco', 'Antiinflamatorio y analgesico', 900),
        ('Clotrimazol', 'Antifungico', 1500),
        ('Aciclovir', 'Antiviral', 1700),
        ('Vitamina D', 'Suplemento vitaminico', 1000),
        ('Cefalexina', 'Antibiotico', 1350),
        ('Prednisona', 'Corticoide', 850),
        ('Gotas lubricantes', 'Lubricante oftalmico', 1800),
        ('Ciprofloxacino oftalmico', 'Antibiotico oftalmico', 2400),
        ('Clorhexidina bucal', 'Antiseptico bucal', 1200),
        ('Naproxeno', 'Antiinflamatorio', 950),
        ('Acido folico', 'Suplemento prenatal', 600),
        ('Hierro', 'Suplemento para anemia', 750),
        ('Insulina NPH', 'Control de diabetes', 6500),
        ('Suero oral', 'Sales de rehidratacion', 700);

    ---------------------------------------------------------
    -- 8. INVENTARIO
    -- Todos los hospitales tendrán medicamentos disponibles.
    ---------------------------------------------------------

    INSERT INTO Inventario_Hospital
    (id_hospital, id_medicamento, cantidad_stock)
    SELECT
        hospital.id_hospital,
        medicamento.id_medicamento,
        CASE
            WHEN medicamento.nombre = 'Insulina NPH' THEN 35
            WHEN medicamento.nombre = 'Salbutamol' THEN 50
            ELSE 150
        END
    FROM Hospital hospital
    CROSS JOIN Medicamento medicamento;

    ---------------------------------------------------------
    -- 9. CITAS FINALIZADAS, CANCELADAS Y FUTURAS
    ---------------------------------------------------------

    INSERT INTO Cita
    (fecha, hora, diagnostico, estado, id_paciente, id_medico)
    VALUES
        (DATEADD(DAY, -35, @Hoy), '09:00:00',
         'Infeccion respiratoria leve', 'Finalizada',
         (SELECT paciente.id_paciente
          FROM Paciente paciente
          INNER JOIN Persona persona
              ON paciente.id_persona = persona.id_persona
          WHERE persona.cedula = '301000001'),
         (SELECT medico.id_medico
          FROM Medico medico
          INNER JOIN Persona persona
              ON medico.id_persona = persona.id_persona
          WHERE persona.cedula = '201000001')),

        (DATEADD(DAY, -28, @Hoy), '10:00:00',
         'Control de hipertension arterial', 'Finalizada',
         (SELECT paciente.id_paciente FROM Paciente paciente
          INNER JOIN Persona persona ON paciente.id_persona = persona.id_persona
          WHERE persona.cedula = '301000002'),
         (SELECT medico.id_medico FROM Medico medico
          INNER JOIN Persona persona ON medico.id_persona = persona.id_persona
          WHERE persona.cedula = '201000003')),

        (DATEADD(DAY, -21, @Hoy), '11:00:00',
         'Control pediatrico anual', 'Finalizada',
         (SELECT paciente.id_paciente FROM Paciente paciente
          INNER JOIN Persona persona ON paciente.id_persona = persona.id_persona
          WHERE persona.cedula = '301000003'),
         (SELECT medico.id_medico FROM Medico medico
          INNER JOIN Persona persona ON medico.id_persona = persona.id_persona
          WHERE persona.cedula = '201000004')),

        (DATEADD(DAY, -18, @Hoy), '08:00:00',
         'Cefalea recurrente', 'Finalizada',
         (SELECT paciente.id_paciente FROM Paciente paciente
          INNER JOIN Persona persona ON paciente.id_persona = persona.id_persona
          WHERE persona.cedula = '301000004'),
         (SELECT medico.id_medico FROM Medico medico
          INNER JOIN Persona persona ON medico.id_persona = persona.id_persona
          WHERE persona.cedula = '201000005')),

        (DATEADD(DAY, -15, @Hoy), '09:00:00',
         'Gastritis leve', 'Finalizada',
         (SELECT paciente.id_paciente FROM Paciente paciente
          INNER JOIN Persona persona ON paciente.id_persona = persona.id_persona
          WHERE persona.cedula = '301000005'),
         (SELECT medico.id_medico FROM Medico medico
          INNER JOIN Persona persona ON medico.id_persona = persona.id_persona
          WHERE persona.cedula = '201000006')),

        (DATEADD(DAY, -12, @Hoy), '10:00:00',
         'Control prenatal', 'Finalizada',
         (SELECT paciente.id_paciente FROM Paciente paciente
          INNER JOIN Persona persona ON paciente.id_persona = persona.id_persona
          WHERE persona.cedula = '301000006'),
         (SELECT medico.id_medico FROM Medico medico
          INNER JOIN Persona persona ON medico.id_persona = persona.id_persona
          WHERE persona.cedula = '201000007')),

        (DATEADD(DAY, -10, @Hoy), '11:00:00',
         'Dolor lumbar mecanico', 'Finalizada',
         (SELECT paciente.id_paciente FROM Paciente paciente
          INNER JOIN Persona persona ON paciente.id_persona = persona.id_persona
          WHERE persona.cedula = '301000007'),
         (SELECT medico.id_medico FROM Medico medico
          INNER JOIN Persona persona ON medico.id_persona = persona.id_persona
          WHERE persona.cedula = '201000008')),

        (DATEADD(DAY, -8, @Hoy), '08:00:00',
         'Dermatitis alergica', 'Finalizada',
         (SELECT paciente.id_paciente FROM Paciente paciente
          INNER JOIN Persona persona ON paciente.id_persona = persona.id_persona
          WHERE persona.cedula = '301000008'),
         (SELECT medico.id_medico FROM Medico medico
          INNER JOIN Persona persona ON medico.id_persona = persona.id_persona
          WHERE persona.cedula = '201000009')),

        (DATEADD(DAY, -6, @Hoy), '09:00:00',
         'Profilaxis dental', 'Finalizada',
         (SELECT paciente.id_paciente FROM Paciente paciente
          INNER JOIN Persona persona ON paciente.id_persona = persona.id_persona
          WHERE persona.cedula = '301000009'),
         (SELECT medico.id_medico FROM Medico medico
          INNER JOIN Persona persona ON medico.id_persona = persona.id_persona
          WHERE persona.cedula = '201000011')),

        (DATEADD(DAY, -4, @Hoy), '10:00:00',
         'Irritacion ocular', 'Finalizada',
         (SELECT paciente.id_paciente FROM Paciente paciente
          INNER JOIN Persona persona ON paciente.id_persona = persona.id_persona
          WHERE persona.cedula = '301000010'),
         (SELECT medico.id_medico FROM Medico medico
          INNER JOIN Persona persona ON medico.id_persona = persona.id_persona
          WHERE persona.cedula = '201000012')),

        (DATEADD(DAY, -2, @Hoy), '11:00:00',
         'Dolor muscular post ejercicio', 'Finalizada',
         (SELECT paciente.id_paciente FROM Paciente paciente
          INNER JOIN Persona persona ON paciente.id_persona = persona.id_persona
          WHERE persona.cedula = '301000001'),
         (SELECT medico.id_medico FROM Medico medico
          INNER JOIN Persona persona ON medico.id_persona = persona.id_persona
          WHERE persona.cedula = '201000002')),

        (@Hoy, '08:00:00',
         'Control general de seguimiento', 'Finalizada',
         (SELECT paciente.id_paciente FROM Paciente paciente
          INNER JOIN Persona persona ON paciente.id_persona = persona.id_persona
          WHERE persona.cedula = '301000005'),
         (SELECT medico.id_medico FROM Medico medico
          INNER JOIN Persona persona ON medico.id_persona = persona.id_persona
          WHERE persona.cedula = '201000006')),

        (@Hoy, '10:00:00',
         'Revision odontologica preventiva', 'Finalizada',
         (SELECT paciente.id_paciente FROM Paciente paciente
          INNER JOIN Persona persona ON paciente.id_persona = persona.id_persona
          WHERE persona.cedula = '301000009'),
         (SELECT medico.id_medico FROM Medico medico
          INNER JOIN Persona persona ON medico.id_persona = persona.id_persona
          WHERE persona.cedula = '201000011')),

        (DATEADD(DAY, -1, @Hoy), '11:00:00',
         'Consulta cancelada por paciente', 'Cancelada',
         (SELECT paciente.id_paciente FROM Paciente paciente
          INNER JOIN Persona persona ON paciente.id_persona = persona.id_persona
          WHERE persona.cedula = '301000003'),
         (SELECT medico.id_medico FROM Medico medico
          INNER JOIN Persona persona ON medico.id_persona = persona.id_persona
          WHERE persona.cedula = '201000004')),

        (DATEADD(DAY, 1, @Hoy), '08:00:00',
         'Pendiente de valoracion general', 'En proceso',
         (SELECT paciente.id_paciente FROM Paciente paciente
          INNER JOIN Persona persona ON paciente.id_persona = persona.id_persona
          WHERE persona.cedula = '301000001'),
         (SELECT medico.id_medico FROM Medico medico
          INNER JOIN Persona persona ON medico.id_persona = persona.id_persona
          WHERE persona.cedula = '201000001')),

        (DATEADD(DAY, 1, @Hoy), '10:00:00',
         'Control cardiologico programado', 'En proceso',
         (SELECT paciente.id_paciente FROM Paciente paciente
          INNER JOIN Persona persona ON paciente.id_persona = persona.id_persona
          WHERE persona.cedula = '301000002'),
         (SELECT medico.id_medico FROM Medico medico
          INNER JOIN Persona persona ON medico.id_persona = persona.id_persona
          WHERE persona.cedula = '201000003')),

        (DATEADD(DAY, 2, @Hoy), '09:00:00',
         'Consulta ginecologica programada', 'En proceso',
         (SELECT paciente.id_paciente FROM Paciente paciente
          INNER JOIN Persona persona ON paciente.id_persona = persona.id_persona
          WHERE persona.cedula = '301000006'),
         (SELECT medico.id_medico FROM Medico medico
          INNER JOIN Persona persona ON medico.id_persona = persona.id_persona
          WHERE persona.cedula = '201000007')),

        (DATEADD(DAY, 2, @Hoy), '11:00:00',
         'Revision ortopedica programada', 'En proceso',
         (SELECT paciente.id_paciente FROM Paciente paciente
          INNER JOIN Persona persona ON paciente.id_persona = persona.id_persona
          WHERE persona.cedula = '301000007'),
         (SELECT medico.id_medico FROM Medico medico
          INNER JOIN Persona persona ON medico.id_persona = persona.id_persona
          WHERE persona.cedula = '201000008')),

        (DATEADD(DAY, 3, @Hoy), '08:00:00',
         'Consulta odontologica programada', 'En proceso',
         (SELECT paciente.id_paciente FROM Paciente paciente
          INNER JOIN Persona persona ON paciente.id_persona = persona.id_persona
          WHERE persona.cedula = '301000009'),
         (SELECT medico.id_medico FROM Medico medico
          INNER JOIN Persona persona ON medico.id_persona = persona.id_persona
          WHERE persona.cedula = '201000011')),

        (DATEADD(DAY, 3, @Hoy), '09:00:00',
         'Revision oftalmologica programada', 'En proceso',
         (SELECT paciente.id_paciente FROM Paciente paciente
          INNER JOIN Persona persona ON paciente.id_persona = persona.id_persona
          WHERE persona.cedula = '301000010'),
         (SELECT medico.id_medico FROM Medico medico
          INNER JOIN Persona persona ON medico.id_persona = persona.id_persona
          WHERE persona.cedula = '201000012'));

    ---------------------------------------------------------
    -- 10. TRATAMIENTOS
    -- El costo inicial es 1 para cumplir validaciones.
    -- El trigger lo recalcula con las prescripciones.
    ---------------------------------------------------------

    INSERT INTO Tratamiento (descripcion, costo, id_cita)
    SELECT 'Tratamiento de infeccion respiratoria', 1,
           id_cita
    FROM Cita
    WHERE diagnostico = 'Infeccion respiratoria leve';

    INSERT INTO Tratamiento (descripcion, costo, id_cita)
    SELECT 'Control cardiologico', 1, id_cita
    FROM Cita
    WHERE diagnostico = 'Control de hipertension arterial';

    INSERT INTO Tratamiento (descripcion, costo, id_cita)
    SELECT 'Tratamiento pediatrico', 1, id_cita
    FROM Cita
    WHERE diagnostico = 'Control pediatrico anual';

    INSERT INTO Tratamiento (descripcion, costo, id_cita)
    SELECT 'Manejo de cefalea', 1, id_cita
    FROM Cita
    WHERE diagnostico = 'Cefalea recurrente';

    INSERT INTO Tratamiento (descripcion, costo, id_cita)
    SELECT 'Tratamiento digestivo', 1, id_cita
    FROM Cita
    WHERE diagnostico = 'Gastritis leve';

    INSERT INTO Tratamiento (descripcion, costo, id_cita)
    SELECT 'Suplementacion prenatal', 1, id_cita
    FROM Cita
    WHERE diagnostico = 'Control prenatal';

    INSERT INTO Tratamiento (descripcion, costo, id_cita)
    SELECT 'Manejo de dolor lumbar', 1, id_cita
    FROM Cita
    WHERE diagnostico = 'Dolor lumbar mecanico';

    INSERT INTO Tratamiento (descripcion, costo, id_cita)
    SELECT 'Tratamiento dermatologico', 1, id_cita
    FROM Cita
    WHERE diagnostico = 'Dermatitis alergica';

    INSERT INTO Tratamiento (descripcion, costo, id_cita)
    SELECT 'Tratamiento odontologico', 1, id_cita
    FROM Cita
    WHERE diagnostico = 'Profilaxis dental';

    INSERT INTO Tratamiento (descripcion, costo, id_cita)
    SELECT 'Tratamiento oftalmologico', 1, id_cita
    FROM Cita
    WHERE diagnostico = 'Irritacion ocular';

    INSERT INTO Tratamiento (descripcion, costo, id_cita)
    SELECT 'Manejo de dolor muscular', 1, id_cita
    FROM Cita
    WHERE diagnostico = 'Dolor muscular post ejercicio';

    INSERT INTO Tratamiento (descripcion, costo, id_cita)
    SELECT 'Consulta general de seguimiento', 1, id_cita
    FROM Cita
    WHERE diagnostico = 'Control general de seguimiento';

    INSERT INTO Tratamiento (descripcion, costo, id_cita)
    SELECT 'Medicacion complementaria', 1, id_cita
    FROM Cita
    WHERE diagnostico = 'Control general de seguimiento';

    INSERT INTO Tratamiento (descripcion, costo, id_cita)
    SELECT 'Tratamiento odontologico preventivo', 1, id_cita
    FROM Cita
    WHERE diagnostico = 'Revision odontologica preventiva';

    ---------------------------------------------------------
    -- 11. PRESCRIPCIONES
    -- Una por una para que el trigger controle el stock.
    ---------------------------------------------------------

    INSERT INTO Prescripcion (id_tratamiento, id_medicamento, cantidad, dosis)
    VALUES
    (
        (SELECT id_tratamiento FROM Tratamiento
         WHERE descripcion = 'Tratamiento de infeccion respiratoria'),
        (SELECT id_medicamento FROM Medicamento WHERE nombre = 'Amoxicilina'),
        14, '1 capsula cada 8 horas'
    );

    INSERT INTO Prescripcion (id_tratamiento, id_medicamento, cantidad, dosis)
    VALUES
    (
        (SELECT id_tratamiento FROM Tratamiento
         WHERE descripcion = 'Tratamiento de infeccion respiratoria'),
        (SELECT id_medicamento FROM Medicamento WHERE nombre = 'Paracetamol'),
        10, '1 tableta cada 8 horas'
    );

    INSERT INTO Prescripcion (id_tratamiento, id_medicamento, cantidad, dosis)
    VALUES
    (
        (SELECT id_tratamiento FROM Tratamiento
         WHERE descripcion = 'Control cardiologico'),
        (SELECT id_medicamento FROM Medicamento WHERE nombre = 'Losartan'),
        30, '1 tableta al dia'
    );

    INSERT INTO Prescripcion (id_tratamiento, id_medicamento, cantidad, dosis)
    VALUES
    (
        (SELECT id_tratamiento FROM Tratamiento
         WHERE descripcion = 'Control cardiologico'),
        (SELECT id_medicamento FROM Medicamento WHERE nombre = 'Atorvastatina'),
        30, '1 tableta por la noche'
    );

    INSERT INTO Prescripcion (id_tratamiento, id_medicamento, cantidad, dosis)
    VALUES
    (
        (SELECT id_tratamiento FROM Tratamiento
         WHERE descripcion = 'Tratamiento pediatrico'),
        (SELECT id_medicamento FROM Medicamento WHERE nombre = 'Paracetamol'),
        10, 'Segun indicacion medica'
    );

    INSERT INTO Prescripcion (id_tratamiento, id_medicamento, cantidad, dosis)
    VALUES
    (
        (SELECT id_tratamiento FROM Tratamiento
         WHERE descripcion = 'Tratamiento pediatrico'),
        (SELECT id_medicamento FROM Medicamento WHERE nombre = 'Loratadina'),
        10, '1 dosis cada 24 horas'
    );

    INSERT INTO Prescripcion (id_tratamiento, id_medicamento, cantidad, dosis)
    VALUES
    (
        (SELECT id_tratamiento FROM Tratamiento
         WHERE descripcion = 'Manejo de cefalea'),
        (SELECT id_medicamento FROM Medicamento WHERE nombre = 'Ibuprofeno'),
        10, '1 tableta cada 8 horas'
    );

    INSERT INTO Prescripcion (id_tratamiento, id_medicamento, cantidad, dosis)
    VALUES
    (
        (SELECT id_tratamiento FROM Tratamiento
         WHERE descripcion = 'Tratamiento digestivo'),
        (SELECT id_medicamento FROM Medicamento WHERE nombre = 'Omeprazol'),
        14, '1 capsula antes del desayuno'
    );

    INSERT INTO Prescripcion (id_tratamiento, id_medicamento, cantidad, dosis)
    VALUES
    (
        (SELECT id_tratamiento FROM Tratamiento
         WHERE descripcion = 'Suplementacion prenatal'),
        (SELECT id_medicamento FROM Medicamento WHERE nombre = 'Acido folico'),
        30, '1 tableta al dia'
    );

    INSERT INTO Prescripcion (id_tratamiento, id_medicamento, cantidad, dosis)
    VALUES
    (
        (SELECT id_tratamiento FROM Tratamiento
         WHERE descripcion = 'Suplementacion prenatal'),
        (SELECT id_medicamento FROM Medicamento WHERE nombre = 'Hierro'),
        30, '1 tableta al dia'
    );

    INSERT INTO Prescripcion (id_tratamiento, id_medicamento, cantidad, dosis)
    VALUES
    (
        (SELECT id_tratamiento FROM Tratamiento
         WHERE descripcion = 'Manejo de dolor lumbar'),
        (SELECT id_medicamento FROM Medicamento WHERE nombre = 'Diclofenaco'),
        10, '1 tableta cada 12 horas'
    );

    INSERT INTO Prescripcion (id_tratamiento, id_medicamento, cantidad, dosis)
    VALUES
    (
        (SELECT id_tratamiento FROM Tratamiento
         WHERE descripcion = 'Tratamiento dermatologico'),
        (SELECT id_medicamento FROM Medicamento WHERE nombre = 'Clotrimazol'),
        1, 'Aplicar dos veces al dia'
    );

    INSERT INTO Prescripcion (id_tratamiento, id_medicamento, cantidad, dosis)
    VALUES
    (
        (SELECT id_tratamiento FROM Tratamiento
         WHERE descripcion = 'Tratamiento odontologico'),
        (SELECT id_medicamento FROM Medicamento WHERE nombre = 'Clorhexidina bucal'),
        1, 'Enjuague dos veces al dia'
    );

    INSERT INTO Prescripcion (id_tratamiento, id_medicamento, cantidad, dosis)
    VALUES
    (
        (SELECT id_tratamiento FROM Tratamiento
         WHERE descripcion = 'Tratamiento oftalmologico'),
        (SELECT id_medicamento FROM Medicamento WHERE nombre = 'Gotas lubricantes'),
        2, '2 gotas cada 8 horas'
    );

    INSERT INTO Prescripcion (id_tratamiento, id_medicamento, cantidad, dosis)
    VALUES
    (
        (SELECT id_tratamiento FROM Tratamiento
         WHERE descripcion = 'Manejo de dolor muscular'),
        (SELECT id_medicamento FROM Medicamento WHERE nombre = 'Naproxeno'),
        10, '1 tableta cada 12 horas'
    );

    INSERT INTO Prescripcion (id_tratamiento, id_medicamento, cantidad, dosis)
    VALUES
    (
        (SELECT id_tratamiento FROM Tratamiento
         WHERE descripcion = 'Consulta general de seguimiento'),
        (SELECT id_medicamento FROM Medicamento WHERE nombre = 'Paracetamol'),
        12, '1 tableta cada 8 horas'
    );

    INSERT INTO Prescripcion (id_tratamiento, id_medicamento, cantidad, dosis)
    VALUES
    (
        (SELECT id_tratamiento FROM Tratamiento
         WHERE descripcion = 'Medicacion complementaria'),
        (SELECT id_medicamento FROM Medicamento WHERE nombre = 'Suero oral'),
        5, '1 sobre despues de cada evacuacion'
    );

    INSERT INTO Prescripcion (id_tratamiento, id_medicamento, cantidad, dosis)
    VALUES
    (
        (SELECT id_tratamiento FROM Tratamiento
         WHERE descripcion = 'Tratamiento odontologico preventivo'),
        (SELECT id_medicamento FROM Medicamento WHERE nombre = 'Clorhexidina bucal'),
        1, 'Enjuague dos veces al dia'
    );

    ---------------------------------------------------------
    -- 12. PAGOS
    -- El monto se toma del costo calculado del tratamiento.
    ---------------------------------------------------------

    INSERT INTO Pago (fecha_pago, monto, metodo_pago, id_tratamiento)
    SELECT DATEADD(DAY, -35, @Hoy), costo, 'Sinpe', id_tratamiento
    FROM Tratamiento
    WHERE descripcion = 'Tratamiento de infeccion respiratoria';

    INSERT INTO Pago (fecha_pago, monto, metodo_pago, id_tratamiento)
    SELECT DATEADD(DAY, -28, @Hoy), costo, 'Tarjeta', id_tratamiento
    FROM Tratamiento
    WHERE descripcion = 'Control cardiologico';

    INSERT INTO Pago (fecha_pago, monto, metodo_pago, id_tratamiento)
    SELECT DATEADD(DAY, -21, @Hoy), costo, 'Efectivo', id_tratamiento
    FROM Tratamiento
    WHERE descripcion = 'Tratamiento pediatrico';

    INSERT INTO Pago (fecha_pago, monto, metodo_pago, id_tratamiento)
    SELECT DATEADD(DAY, -15, @Hoy), costo, 'Tarjeta', id_tratamiento
    FROM Tratamiento
    WHERE descripcion = 'Tratamiento digestivo';

    INSERT INTO Pago (fecha_pago, monto, metodo_pago, id_tratamiento)
    SELECT DATEADD(DAY, -12, @Hoy), costo, 'Sinpe', id_tratamiento
    FROM Tratamiento
    WHERE descripcion = 'Suplementacion prenatal';

    INSERT INTO Pago (fecha_pago, monto, metodo_pago, id_tratamiento)
    SELECT DATEADD(DAY, -8, @Hoy), costo, 'Efectivo', id_tratamiento
    FROM Tratamiento
    WHERE descripcion = 'Tratamiento dermatologico';

    INSERT INTO Pago (fecha_pago, monto, metodo_pago, id_tratamiento)
    SELECT DATEADD(DAY, -6, @Hoy), costo, 'Tarjeta', id_tratamiento
    FROM Tratamiento
    WHERE descripcion = 'Tratamiento odontologico';

    -- Dos tratamientos de la misma cita pagados juntos hoy.
    -- Se mostrarán agrupados en el historial de pagos.
    INSERT INTO Pago (fecha_pago, monto, metodo_pago, id_tratamiento)
    SELECT @Hoy, costo, 'Tarjeta', id_tratamiento
    FROM Tratamiento
    WHERE descripcion IN
    (
        'Consulta general de seguimiento',
        'Medicacion complementaria'
    );

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
    BEGIN
        ROLLBACK TRANSACTION;
    END;

    THROW;
END CATCH;
GO

---Procedimiento Almacenado Extra--
CREATE OR ALTER PROCEDURE sp_CancelarCitasVencidas
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Cita
    SET Estado = 'Cancelada'
    WHERE Estado = 'En proceso'
      AND DATEADD(
            HOUR,
            1,
            CONVERT(
                DATETIME,
                CONVERT(CHAR(10), fecha, 23)
                + ' '
                + CONVERT(CHAR(8), hora, 108),
                120
            )
          ) <= GETDATE();
END;
GO

--Mejora BD--
ALTER TABLE Cita
DROP CONSTRAINT UQ_Cita_Medico_FechaHora;
GO

CREATE UNIQUE INDEX UX_Cita_Medico_FechaHora_Activa
ON Cita (id_medico, fecha, hora)
WHERE Estado <> 'Cancelada';
GO

/*
EJEMPLOS DE RESPALDO Y RESTAURACIÓN (DESHABILITADOS)

Los siguientes bloques se conservan como referencia académica. No se ejecutan
con la instalación principal porque requieren una carpeta y una política de
respaldos configuradas por el administrador de SQL Server.

---Backup--
/*
POLÍTICA DE RESPALDOS - HOSPITALDB

1. Respaldo completo: todos los domingos a las 10:00 p. m.
2. Respaldo diferencial: de lunes a sábado a las 10:00 p. m.
3. Respaldo del log: cada 4 horas.
4. Respaldo COPY_ONLY: antes de realizar cambios importantes.

Los archivos se guardan en C:\Respaldos.
La carpeta debe existir y SQL Server debe tener permiso para escribir en ella.
*/

USE master;
GO

-- La recuperación FULL permite respaldos del log.
ALTER DATABASE HospitalDB
SET RECOVERY FULL;
GO

---------------------------------------------------------
-- 1. RESPALDO COMPLETO
-- Ejecutar los domingos a las 10:00 p. m.
---------------------------------------------------------

BACKUP DATABASE HospitalDB
TO DISK = 'C:\Respaldos\HospitalDB_Completo.bak'
WITH INIT,
     CHECKSUM,
     NAME = 'Respaldo completo de HospitalDB',
     STATS = 10;
GO

RESTORE VERIFYONLY
FROM DISK = 'C:\Respaldos\HospitalDB_Completo.bak';
GO

---------------------------------------------------------
-- 2. RESPALDO DIFERENCIAL
-- Ejecutar de lunes a sábado a las 10:00 p. m.
---------------------------------------------------------

BACKUP DATABASE HospitalDB
TO DISK = 'C:\Respaldos\HospitalDB_Diferencial.bak'
WITH DIFFERENTIAL,
     INIT,
     CHECKSUM,
     NAME = 'Respaldo diferencial de HospitalDB',
     STATS = 10;
GO

RESTORE VERIFYONLY
FROM DISK = 'C:\Respaldos\HospitalDB_Diferencial.bak';
GO

---------------------------------------------------------
-- 3. RESPALDO DEL LOG DE TRANSACCIONES
-- Ejecutar cada 4 horas.
---------------------------------------------------------

DECLARE @ArchivoLog NVARCHAR(4000);

SET @ArchivoLog =
    N'C:\Respaldos\HospitalDB_Log_' +
    CONVERT(CHAR(8), GETDATE(), 112) +
    N'_' +
    REPLACE(CONVERT(CHAR(8), GETDATE(), 108), ':', '') +
    N'.trn';

BACKUP LOG HospitalDB
TO DISK = @ArchivoLog
WITH CHECKSUM,
     NAME = 'Respaldo del log de HospitalDB',
     STATS = 10;

RESTORE VERIFYONLY
FROM DISK = @ArchivoLog;
GO

---------------------------------------------------------
-- 4. RESPALDO COMPLETO COPY_ONLY
-- Ejecutar antes de cambios importantes.
---------------------------------------------------------

BACKUP DATABASE HospitalDB
TO DISK = 'C:\Respaldos\HospitalDB_CopyOnly.bak'
WITH COPY_ONLY,
     INIT,
     CHECKSUM,
     NAME = 'Respaldo COPY_ONLY de HospitalDB',
     STATS = 10;
GO

RESTORE VERIFYONLY
FROM DISK = 'C:\Respaldos\HospitalDB_CopyOnly.bak';
GO

---Restaurar------------------------------------------------------
USE master;
GO

ALTER DATABASE HospitalDB
SET SINGLE_USER
WITH ROLLBACK IMMEDIATE;
GO

-- 1. Restaurar el último respaldo completo.
RESTORE DATABASE HospitalDB
FROM DISK = 'C:\Respaldos\HospitalDB_Completo.bak'
WITH NORECOVERY,
     REPLACE,
     STATS = 10;
GO

-- 2. Restaurar el último diferencial.
RESTORE DATABASE HospitalDB
FROM DISK = 'C:\Respaldos\HospitalDB_Diferencial.bak'
WITH NORECOVERY,
     STATS = 10;
GO

-- 3. Restaurar TODOS los respaldos de log posteriores
-- al respaldo diferencial, en orden de fecha y hora.

RESTORE LOG HospitalDB
FROM DISK = 'C:\Respaldos\HospitalDB_Log_20260803_020000.trn'
WITH NORECOVERY,
     STATS = 10;
GO

-- Repite el bloque anterior por cada log intermedio.
-- El último log debe terminar con RECOVERY.

RESTORE LOG HospitalDB
FROM DISK = 'C:\Respaldos\HospitalDB_Log_20260803_060000.trn'
WITH RECOVERY,
     STATS = 10;
GO

ALTER DATABASE HospitalDB
SET MULTI_USER;
GO
*/

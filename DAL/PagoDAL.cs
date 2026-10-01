using EDL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class PagoDAL
    {
        private readonly string cadenaConexion =
            ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        public List<PagoPendientePaciente> ObtenerPendientes(
            int idUsuario)
        {
            List<PagoPendientePaciente> pendientes =
                new List<PagoPendientePaciente>();

            int idPaciente =
                ObtenerIdPacientePorUsuario(idUsuario);

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(
                    "sp_HistorialPagosPendientesPaciente",
                    conexion))
            {
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.Add(
                    "@IdPaciente",
                    SqlDbType.Int).Value = idPaciente;

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        pendientes.Add(new PagoPendientePaciente
                        {
                            IdCita =
                                Convert.ToInt32(
                                    lector["id_cita"]),

                            DetalleTratamiento =
                                lector["detalle_tratamientos"]
                                .ToString(),

                            MontoPendiente =
                                Convert.ToDecimal(
                                    lector["monto_pendiente"]),

                            FechaCita =
                                Convert.ToDateTime(
                                    lector["fecha_cita"])
                        });
                    }
                }
            }

            return pendientes;
        }

        public decimal ObtenerTotalPagado(
            int idUsuario,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            int idPaciente =
                ObtenerIdPacientePorUsuario(idUsuario);

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(
                    "sp_TotalPagadoPorPaciente",
                    conexion))
            {
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.Add(
                    "@IdPaciente",
                    SqlDbType.Int).Value = idPaciente;

                comando.Parameters.Add(
                    "@FechaInicio",
                    SqlDbType.Date).Value = fechaInicio.Date;

                comando.Parameters.Add(
                    "@FechaFin",
                    SqlDbType.Date).Value = fechaFin.Date;

                conexion.Open();

                object resultado = comando.ExecuteScalar();

                return resultado == null || resultado == DBNull.Value
                    ? 0
                    : Convert.ToDecimal(resultado);
            }
        }

        public PagoPendientePaciente ObtenerPendiente(
            int idUsuario,
            int idCita)
        {
            PagoPendientePaciente pendiente = null;

            string consulta = @"
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
                FROM Persona persona
                INNER JOIN Paciente paciente
                    ON paciente.id_persona = persona.id_persona
                INNER JOIN Cita cita
                    ON cita.id_paciente = paciente.id_paciente
                INNER JOIN Tratamiento tratamiento
                    ON tratamiento.id_cita = cita.id_cita
                WHERE persona.id_usuario = @idUsuario
                  AND cita.id_cita = @idCita
                  AND tratamiento.costo > 0
                  AND NOT EXISTS
                  (
                      SELECT 1
                      FROM Pago pago
                      WHERE pago.id_tratamiento = tratamiento.id_tratamiento
                  )
                GROUP BY cita.id_cita, cita.fecha
                HAVING SUM(tratamiento.costo) > 0;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add(
                    "@idUsuario",
                    SqlDbType.Int).Value = idUsuario;

                comando.Parameters.Add(
                    "@idCita",
                    SqlDbType.Int).Value = idCita;

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        pendiente = new PagoPendientePaciente
                        {
                            IdCita =
                                Convert.ToInt32(
                                    lector["id_cita"]),

                            DetalleTratamiento =
                                lector["detalle_tratamientos"]
                                .ToString(),

                            MontoPendiente =
                                Convert.ToDecimal(
                                    lector["monto_pendiente"]),

                            FechaCita =
                                Convert.ToDateTime(
                                    lector["fecha_cita"])
                        };
                    }
                }
            }

            return pendiente;
        }

        public void RegistrarPago(
            int idUsuario,
            int idCita,
            string metodoPago)
        {
            string consulta = @"
                SET XACT_ABORT ON;

                BEGIN TRY
                    BEGIN TRANSACTION;

                    INSERT INTO Pago
                    (
                        fecha_pago,
                        monto,
                        metodo_pago,
                        id_tratamiento
                    )
                    SELECT
                        CAST(GETDATE() AS DATE),
                        tratamiento.costo,
                        @metodoPago,
                        tratamiento.id_tratamiento
                    FROM Persona persona
                    INNER JOIN Paciente paciente
                        ON paciente.id_persona = persona.id_persona
                    INNER JOIN Cita cita
                        ON cita.id_paciente = paciente.id_paciente
                    INNER JOIN Tratamiento tratamiento
                        ON tratamiento.id_cita = cita.id_cita
                    WHERE persona.id_usuario = @idUsuario
                      AND cita.id_cita = @idCita
                      AND tratamiento.costo > 0
                      AND NOT EXISTS
                      (
                          SELECT 1
                          FROM Pago pago WITH (UPDLOCK, HOLDLOCK)
                          WHERE pago.id_tratamiento =
                              tratamiento.id_tratamiento
                      );

                    IF @@ROWCOUNT = 0
                    BEGIN
                        RAISERROR
                        (
                            'La cita no tiene tratamientos pendientes de pago.',
                            16,
                            1
                        );
                    END;

                    COMMIT TRANSACTION;
                END TRY
                BEGIN CATCH
                    IF @@TRANCOUNT > 0
                    BEGIN
                        ROLLBACK TRANSACTION;
                    END;

                    THROW;
                END CATCH;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add(
                    "@idUsuario",
                    SqlDbType.Int).Value = idUsuario;

                comando.Parameters.Add(
                    "@idCita",
                    SqlDbType.Int).Value = idCita;

                comando.Parameters.Add(
                    "@metodoPago",
                    SqlDbType.VarChar,
                    50).Value = metodoPago;

                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public List<HistorialPagoPaciente> ListarHistorial(
            int idUsuario)
        {
            List<HistorialPagoPaciente> historial =
                new List<HistorialPagoPaciente>();

            string consulta = @"
                SELECT
                    pago.fecha_pago,
                    SUM(pago.monto) AS monto,
                    STUFF
                    (
                        (
                            SELECT ' - ' + tratamientoInterno.descripcion
                            FROM Pago pagoInterno
                            INNER JOIN Tratamiento tratamientoInterno
                                ON tratamientoInterno.id_tratamiento =
                                    pagoInterno.id_tratamiento
                            WHERE tratamientoInterno.id_cita = cita.id_cita
                              AND pagoInterno.fecha_pago = pago.fecha_pago
                            ORDER BY tratamientoInterno.id_tratamiento
                            FOR XML PATH(''), TYPE
                        ).value('.', 'VARCHAR(MAX)'),
                        1,
                        3,
                        ''
                    ) AS detalle_tratamientos,
                    STUFF
                    (
                        (
                            SELECT DISTINCT
                                ' · ' + pagoInterno.metodo_pago
                            FROM Pago pagoInterno
                            INNER JOIN Tratamiento tratamientoInterno
                                ON tratamientoInterno.id_tratamiento =
                                    pagoInterno.id_tratamiento
                            WHERE tratamientoInterno.id_cita = cita.id_cita
                              AND pagoInterno.fecha_pago = pago.fecha_pago
                            FOR XML PATH(''), TYPE
                        ).value('.', 'VARCHAR(MAX)'),
                        1,
                        3,
                        ''
                    ) AS metodo_pago,
                    cita.fecha AS fecha_cita
                FROM Persona persona
                INNER JOIN Paciente paciente
                    ON paciente.id_persona = persona.id_persona
                INNER JOIN Cita cita
                    ON cita.id_paciente = paciente.id_paciente
                INNER JOIN Tratamiento tratamiento
                    ON tratamiento.id_cita = cita.id_cita
                INNER JOIN Pago pago
                    ON pago.id_tratamiento = tratamiento.id_tratamiento
                WHERE persona.id_usuario = @idUsuario
                GROUP BY
                    cita.id_cita,
                    cita.fecha,
                    pago.fecha_pago
                ORDER BY pago.fecha_pago DESC, cita.fecha DESC;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add(
                    "@idUsuario",
                    SqlDbType.Int).Value = idUsuario;

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        historial.Add(new HistorialPagoPaciente
                        {
                            FechaPago =
                                Convert.ToDateTime(
                                    lector["fecha_pago"]),

                            Monto =
                                Convert.ToDecimal(
                                    lector["monto"]),

                            MetodoPago =
                                lector["metodo_pago"].ToString(),

                            DetalleTratamiento =
                                lector["detalle_tratamientos"]
                                .ToString(),

                            FechaCita =
                                Convert.ToDateTime(
                                    lector["fecha_cita"])
                        });
                    }
                }
            }

            return historial;
        }

        private int ObtenerIdPacientePorUsuario(int idUsuario)
        {
            string consulta = @"
                SELECT paciente.id_paciente
                FROM Persona persona
                INNER JOIN Paciente paciente
                    ON paciente.id_persona = persona.id_persona
                WHERE persona.id_usuario = @idUsuario;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add(
                    "@idUsuario",
                    SqlDbType.Int).Value = idUsuario;

                conexion.Open();

                object resultado = comando.ExecuteScalar();

                if (resultado == null)
                {
                    throw new InvalidOperationException(
                        "No se encontró un paciente asociado a esta cuenta.");
                }

                return Convert.ToInt32(resultado);
            }
        }
    }
}

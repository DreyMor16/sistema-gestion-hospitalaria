using EDL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class RecepcionistaPagoDAL
    {
        private readonly string cadenaConexion =
            ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        public PacientePagoRecepcion BuscarPacientePorCedula(
            string cedulaNormalizada)
        {
            PacientePagoRecepcion paciente = null;

            string consulta = @"
                SELECT
                    paciente.id_paciente,
                    persona.nombre + ' ' + persona.apellido
                        AS nombre_completo,
                    persona.cedula,
                    hospital.nombre AS hospital
                FROM Persona persona
                INNER JOIN Paciente paciente
                    ON paciente.id_persona = persona.id_persona
                INNER JOIN Hospital hospital
                    ON hospital.id_hospital = paciente.id_hospital
                WHERE REPLACE(REPLACE(persona.cedula, '-', ''), ' ', '')
                    = @cedula;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add(
                    "@cedula",
                    SqlDbType.VarChar,
                    20).Value = cedulaNormalizada;

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        paciente = new PacientePagoRecepcion
                        {
                            IdPaciente =
                                Convert.ToInt32(
                                    lector["id_paciente"]),

                            NombreCompleto =
                                lector["nombre_completo"].ToString(),

                            Cedula =
                                lector["cedula"].ToString(),

                            Hospital =
                                lector["hospital"].ToString()
                        };
                    }
                }
            }

            return paciente;
        }

        public List<CitaPendientePagoRecepcion>
            ObtenerPendientes(int idPaciente)
        {
            List<CitaPendientePagoRecepcion> pendientes =
                new List<CitaPendientePagoRecepcion>();

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
                        pendientes.Add(
                            new CitaPendientePagoRecepcion
                            {
                                IdCita =
                                    Convert.ToInt32(
                                        lector["id_cita"]),

                                FechaCita =
                                    Convert.ToDateTime(
                                        lector["fecha_cita"]),

                                DetalleTratamientos =
                                    lector["detalle_tratamientos"]
                                    .ToString(),

                                MontoPendiente =
                                    Convert.ToDecimal(
                                        lector["monto_pendiente"])
                            });
                    }
                }
            }

            return pendientes;
        }

        public CitaPendientePagoRecepcion ObtenerPendiente(
            int idPaciente,
            int idCita)
        {
            CitaPendientePagoRecepcion pendiente = null;

            string consulta = @"
                SELECT
                    cita.id_cita,
                    cita.fecha AS fecha_cita,

                    STUFF
                    (
                        (
                            SELECT ' - ' + interno.descripcion
                            FROM Tratamiento interno
                            WHERE interno.id_cita = cita.id_cita
                              AND interno.costo > 0
                              AND NOT EXISTS
                              (
                                  SELECT 1
                                  FROM Pago pagoInterno
                                  WHERE pagoInterno.id_tratamiento =
                                      interno.id_tratamiento
                              )
                            ORDER BY interno.id_tratamiento
                            FOR XML PATH(''), TYPE
                        ).value('.', 'VARCHAR(MAX)'),
                        1, 3, ''
                    ) AS detalle_tratamientos,

                    SUM(tratamiento.costo) AS monto_pendiente

                FROM Cita cita
                INNER JOIN Tratamiento tratamiento
                    ON tratamiento.id_cita = cita.id_cita

                WHERE cita.id_paciente = @idPaciente
                  AND cita.id_cita = @idCita
                  AND tratamiento.costo > 0
                  AND NOT EXISTS
                  (
                      SELECT 1
                      FROM Pago pago
                      WHERE pago.id_tratamiento =
                          tratamiento.id_tratamiento
                  )

                GROUP BY cita.id_cita, cita.fecha
                HAVING SUM(tratamiento.costo) > 0;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add(
                    "@idPaciente",
                    SqlDbType.Int).Value = idPaciente;

                comando.Parameters.Add(
                    "@idCita",
                    SqlDbType.Int).Value = idCita;

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        pendiente =
                            new CitaPendientePagoRecepcion
                            {
                                IdCita =
                                    Convert.ToInt32(
                                        lector["id_cita"]),

                                FechaCita =
                                    Convert.ToDateTime(
                                        lector["fecha_cita"]),

                                DetalleTratamientos =
                                    lector["detalle_tratamientos"]
                                    .ToString(),

                                MontoPendiente =
                                    Convert.ToDecimal(
                                        lector["monto_pendiente"])
                            };
                    }
                }
            }

            return pendiente;
        }

        public void RegistrarPago(
            int idPaciente,
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
                    FROM Cita cita
                    INNER JOIN Tratamiento tratamiento
                        ON tratamiento.id_cita = cita.id_cita
                    WHERE cita.id_paciente = @idPaciente
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
                        RAISERROR(
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
                    "@idPaciente",
                    SqlDbType.Int).Value = idPaciente;

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
    }
}
using EDL;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class ResumenJornadaMedicoDAL
    {
        private readonly string cadenaConexion =
            ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        public ResumenJornadaMedico ObtenerResumen(int idUsuario)
        {
            ResumenJornadaMedico resumen =
                new ResumenJornadaMedico();

            string consulta = @"
                SELECT
                    COUNT(c.id_cita) AS citas_hoy,

                    COUNT(DISTINCT c.id_paciente) AS pacientes_hoy,

                    ISNULL(SUM(
                        CASE
                            WHEN c.Estado = 'En proceso'
                            THEN 1
                            ELSE 0
                        END
                    ), 0) AS en_proceso_hoy,

                    ISNULL(SUM(
                        CASE
                            WHEN c.Estado = 'Finalizada'
                            THEN 1
                            ELSE 0
                        END
                    ), 0) AS finalizadas_hoy

                FROM Persona persona
                INNER JOIN Medico medico
                    ON medico.id_persona = persona.id_persona
                LEFT JOIN Cita c
                    ON c.id_medico = medico.id_medico
                   AND c.fecha = CAST(GETDATE() AS DATE)

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

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        resumen.CitasHoy =
                            Convert.ToInt32(lector["citas_hoy"]);

                        resumen.PacientesHoy =
                            Convert.ToInt32(lector["pacientes_hoy"]);

                        resumen.EnProcesoHoy =
                            Convert.ToInt32(
                                lector["en_proceso_hoy"]);

                        resumen.FinalizadasHoy =
                            Convert.ToInt32(
                                lector["finalizadas_hoy"]);
                    }
                }
            }

            return resumen;
        }
    }
}
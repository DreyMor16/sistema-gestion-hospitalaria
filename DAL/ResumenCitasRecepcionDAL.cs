using EDL;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace DAL
{
    public class ResumenCitasRecepcionDAL
    {
        private readonly string cadenaConexion =
            ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        public ResumenCitasRecepcion ObtenerResumen()
        {
            ResumenCitasRecepcion resumen =
                new ResumenCitasRecepcion();

            string consulta = @"
                SELECT COUNT(*)
                FROM Cita
                WHERE fecha = CAST(GETDATE() AS DATE)
                  AND estado <> 'Cancelada';";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                conexion.Open();

                resumen.CitasHoy =
                    Convert.ToInt32(
                        comando.ExecuteScalar());
            }

            return resumen;
        }
    }
}
using EDL;
using System.Configuration;
using System.Data.SqlClient;

namespace DAL
{
    public class DashboardDAL
    {
        private readonly string cadenaConexion =
            ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        public ResumenAdministrador ObtenerResumen()
        {
            ResumenAdministrador resumen =
                new ResumenAdministrador();

            string consulta = @"
                SELECT
                    (SELECT COUNT(*) FROM Paciente) AS total_pacientes,
                    (SELECT COUNT(*) FROM Medico) AS total_medicos,
                    (SELECT COUNT(*) FROM Medicamento) AS total_medicamentos;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        resumen.TotalPacientes =
                            lector.GetInt32(
                                lector.GetOrdinal("total_pacientes"));

                        resumen.TotalMedicos =
                            lector.GetInt32(
                                lector.GetOrdinal("total_medicos"));

                        resumen.TotalMedicamentos =
                            lector.GetInt32(
                                lector.GetOrdinal("total_medicamentos"));
                    }
                }
            }

            return resumen;
        }
    }
}
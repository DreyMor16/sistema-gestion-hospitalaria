using EDL;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class HospitalDAL
    {
        private readonly string cadenaConexion =
            ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        public List<Hospital> Listar()
        {
            List<Hospital> hospitales = new List<Hospital>();

            string consulta = @"
                SELECT id_hospital, nombre, direccion, telefono
                FROM Hospital
                ORDER BY nombre;";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        hospitales.Add(new Hospital
                        {
                            IdHospital = (int)lector["id_hospital"],
                            Nombre = lector["nombre"].ToString(),
                            Direccion = lector["direccion"].ToString(),
                            Telefono = lector["telefono"].ToString()
                        });
                    }
                }
            }

            return hospitales;
        }
        public List<Hospital> Buscar(string filtro)
        {
            List<Hospital> hospitales = new List<Hospital>();

            string consulta = @"
        SELECT id_hospital, nombre, direccion, telefono
        FROM Hospital
        WHERE nombre LIKE @filtro
           OR direccion LIKE @filtro
           OR telefono LIKE @filtro
        ORDER BY nombre;";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@filtro", SqlDbType.VarChar, 202).Value =
                    "%" + filtro.Trim() + "%";

                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        hospitales.Add(new Hospital
                        {
                            IdHospital = (int)lector["id_hospital"],
                            Nombre = lector["nombre"].ToString(),
                            Direccion = lector["direccion"].ToString(),
                            Telefono = lector["telefono"].ToString()
                        });
                    }
                }
            }

            return hospitales;
        }
        public Hospital ObtenerPorId(int idHospital)
        {
            Hospital hospital = null;

            string consulta = @"
                SELECT id_hospital, nombre, direccion, telefono
                FROM Hospital
                WHERE id_hospital = @idHospital;";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@idHospital", SqlDbType.Int).Value = idHospital;

                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        hospital = new Hospital
                        {
                            IdHospital = (int)lector["id_hospital"],
                            Nombre = lector["nombre"].ToString(),
                            Direccion = lector["direccion"].ToString(),
                            Telefono = lector["telefono"].ToString()
                        };
                    }
                }
            }

            return hospital;
        }

        public int Insertar(Hospital hospital)
        {
            string consulta = @"
                INSERT INTO Hospital (nombre, direccion, telefono)
                VALUES (@nombre, @direccion, @telefono);

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value = hospital.Nombre;
                comando.Parameters.Add("@direccion", SqlDbType.VarChar, 200).Value = hospital.Direccion;
                comando.Parameters.Add("@telefono", SqlDbType.VarChar, 20).Value = hospital.Telefono;

                conexion.Open();

                return (int)comando.ExecuteScalar();
            }
        }

        public void Actualizar(Hospital hospital)
        {
            string consulta = @"
                UPDATE Hospital
                SET nombre = @nombre,
                    direccion = @direccion,
                    telefono = @telefono
                WHERE id_hospital = @idHospital;";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@idHospital", SqlDbType.Int).Value = hospital.IdHospital;
                comando.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value = hospital.Nombre;
                comando.Parameters.Add("@direccion", SqlDbType.VarChar, 200).Value = hospital.Direccion;
                comando.Parameters.Add("@telefono", SqlDbType.VarChar, 20).Value = hospital.Telefono;

                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public void Eliminar(int idHospital)
        {
            string consulta = @"
                DELETE FROM Hospital
                WHERE id_hospital = @idHospital;";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@idHospital", SqlDbType.Int).Value = idHospital;

                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }
    }
}
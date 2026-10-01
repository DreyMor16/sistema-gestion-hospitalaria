using EDL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class PacienteDAL
    {
        private readonly string cadenaConexion =
            ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        public List<PacienteDetalle> Buscar(string filtro, int idHospital)
        {
            List<PacienteDetalle> pacientes = new List<PacienteDetalle>();

            string consulta = @"
                SELECT
                    pa.id_paciente,
                    per.id_persona,
                    per.nombre,
                    per.apellido,
                    per.telefono,
                    per.correo,
                    per.cedula,
                    ISNULL(u.usuario, '') AS usuario,
                    pa.fecha_nacimiento,
                    pa.genero,
                    ISNULL(pa.direccion, '') AS direccion,
                    h.id_hospital,
                    h.nombre AS nombre_hospital
                FROM Paciente pa
                INNER JOIN Persona per
                    ON pa.id_persona = per.id_persona
                INNER JOIN Hospital h
                    ON pa.id_hospital = h.id_hospital
                LEFT JOIN Usuario u
                    ON per.id_usuario = u.id_usuario
                WHERE
                (
                    @filtro = ''
                    OR per.nombre LIKE @patron
                    OR per.apellido LIKE @patron
                    OR LTRIM(RTRIM(per.nombre + ' ' + per.apellido)) LIKE @patron
                    OR LTRIM(RTRIM(per.apellido + ' ' + per.nombre)) LIKE @patron
                    OR ISNULL(per.correo, '') LIKE @patron
                    OR per.cedula LIKE @patron
                    OR ISNULL(per.telefono, '') LIKE @patron
                    OR ISNULL(u.usuario, '') LIKE @patron
                )
                AND (@idHospital = 0 OR pa.id_hospital = @idHospital)
                ORDER BY
                    h.nombre,
                    per.apellido,
                    per.nombre;";

            string filtroLimpio = filtro == null ? "" : filtro.Trim();

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@filtro", SqlDbType.VarChar, 100).Value =
                    filtroLimpio;

                comando.Parameters.Add("@patron", SqlDbType.VarChar, 202).Value =
                    "%" + filtroLimpio + "%";

                comando.Parameters.Add("@idHospital", SqlDbType.Int).Value =
                    idHospital;

                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        pacientes.Add(MapearPaciente(lector));
                    }
                }
            }

            return pacientes;
        }

        public PacienteDetalle ObtenerPorId(int idPaciente)
        {
            PacienteDetalle paciente = null;

            string consulta = @"
                SELECT
                    pa.id_paciente,
                    per.id_persona,
                    per.nombre,
                    per.apellido,
                    per.telefono,
                    per.correo,
                    per.cedula,
                    ISNULL(u.usuario, '') AS usuario,
                    pa.fecha_nacimiento,
                    pa.genero,
                    ISNULL(pa.direccion, '') AS direccion,
                    h.id_hospital,
                    h.nombre AS nombre_hospital
                FROM Paciente pa
                INNER JOIN Persona per
                    ON pa.id_persona = per.id_persona
                INNER JOIN Hospital h
                    ON pa.id_hospital = h.id_hospital
                LEFT JOIN Usuario u
                    ON per.id_usuario = u.id_usuario
                WHERE pa.id_paciente = @idPaciente;";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@idPaciente", SqlDbType.Int).Value =
                    idPaciente;

                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        paciente = MapearPaciente(lector);
                    }
                }
            }

            return paciente;
        }

        public void Insertar(PacienteDetalle paciente)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                SqlTransaction transaccion = conexion.BeginTransaction();

                try
                {
                    int idPersona;

                    string insertarPersona = @"
                        INSERT INTO Persona
                        (nombre, apellido, telefono, correo, cedula)
                        VALUES
                        (@nombre, @apellido, @telefono, @correo, @cedula);

                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using (SqlCommand comandoPersona =
                        new SqlCommand(insertarPersona, conexion, transaccion))
                    {
                        comandoPersona.Parameters.Add("@nombre", SqlDbType.VarChar, 50).Value =
                            paciente.Nombre;

                        comandoPersona.Parameters.Add("@apellido", SqlDbType.VarChar, 50).Value =
                            paciente.Apellido;

                        comandoPersona.Parameters.Add("@telefono", SqlDbType.VarChar, 20).Value =
                            string.IsNullOrWhiteSpace(paciente.Telefono)
                            ? (object)DBNull.Value
                            : paciente.Telefono;

                        comandoPersona.Parameters.Add("@correo", SqlDbType.VarChar, 100).Value =
                            string.IsNullOrWhiteSpace(paciente.Correo)
                            ? (object)DBNull.Value
                            : paciente.Correo;

                        comandoPersona.Parameters.Add("@cedula", SqlDbType.VarChar, 20).Value =
                            paciente.Cedula;

                        idPersona = (int)comandoPersona.ExecuteScalar();
                    }

                    string insertarPaciente = @"
                        INSERT INTO Paciente
                        (fecha_nacimiento, genero, direccion, id_persona, id_hospital)
                        VALUES
                        (@fechaNacimiento, @genero, @direccion, @idPersona, @idHospital);";

                    using (SqlCommand comandoPaciente =
                        new SqlCommand(insertarPaciente, conexion, transaccion))
                    {
                        comandoPaciente.Parameters.Add("@fechaNacimiento", SqlDbType.Date).Value =
                            paciente.FechaNacimiento;

                        comandoPaciente.Parameters.Add("@genero", SqlDbType.VarChar, 20).Value =
                            paciente.Genero;

                        comandoPaciente.Parameters.Add("@direccion", SqlDbType.VarChar, 200).Value =
                            string.IsNullOrWhiteSpace(paciente.Direccion)
                            ? (object)DBNull.Value
                            : paciente.Direccion;

                        comandoPaciente.Parameters.Add("@idPersona", SqlDbType.Int).Value =
                            idPersona;

                        comandoPaciente.Parameters.Add("@idHospital", SqlDbType.Int).Value =
                            paciente.IdHospital;

                        comandoPaciente.ExecuteNonQuery();
                    }

                    transaccion.Commit();
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }

        public void Actualizar(PacienteDetalle paciente)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                SqlTransaction transaccion = conexion.BeginTransaction();

                try
                {
                    string actualizarPersona = @"
                        UPDATE Persona
                        SET nombre = @nombre,
                            apellido = @apellido,
                            telefono = @telefono,
                            correo = @correo,
                            cedula = @cedula
                        WHERE id_persona = @idPersona;";

                    using (SqlCommand comandoPersona =
                        new SqlCommand(actualizarPersona, conexion, transaccion))
                    {
                        comandoPersona.Parameters.Add("@idPersona", SqlDbType.Int).Value =
                            paciente.IdPersona;

                        comandoPersona.Parameters.Add("@nombre", SqlDbType.VarChar, 50).Value =
                            paciente.Nombre;

                        comandoPersona.Parameters.Add("@apellido", SqlDbType.VarChar, 50).Value =
                            paciente.Apellido;

                        comandoPersona.Parameters.Add("@telefono", SqlDbType.VarChar, 20).Value =
                            string.IsNullOrWhiteSpace(paciente.Telefono)
                            ? (object)DBNull.Value
                            : paciente.Telefono;

                        comandoPersona.Parameters.Add("@correo", SqlDbType.VarChar, 100).Value =
                            string.IsNullOrWhiteSpace(paciente.Correo)
                            ? (object)DBNull.Value
                            : paciente.Correo;

                        comandoPersona.Parameters.Add("@cedula", SqlDbType.VarChar, 20).Value =
                            paciente.Cedula;

                        comandoPersona.ExecuteNonQuery();
                    }

                    string actualizarPaciente = @"
                        UPDATE Paciente
                        SET fecha_nacimiento = @fechaNacimiento,
                            genero = @genero,
                            direccion = @direccion,
                            id_hospital = @idHospital
                        WHERE id_paciente = @idPaciente;";

                    using (SqlCommand comandoPaciente =
                        new SqlCommand(actualizarPaciente, conexion, transaccion))
                    {
                        comandoPaciente.Parameters.Add("@idPaciente", SqlDbType.Int).Value =
                            paciente.IdPaciente;

                        comandoPaciente.Parameters.Add("@fechaNacimiento", SqlDbType.Date).Value =
                            paciente.FechaNacimiento;

                        comandoPaciente.Parameters.Add("@genero", SqlDbType.VarChar, 20).Value =
                            paciente.Genero;

                        comandoPaciente.Parameters.Add("@direccion", SqlDbType.VarChar, 200).Value =
                            string.IsNullOrWhiteSpace(paciente.Direccion)
                            ? (object)DBNull.Value
                            : paciente.Direccion;

                        comandoPaciente.Parameters.Add("@idHospital", SqlDbType.Int).Value =
                            paciente.IdHospital;

                        comandoPaciente.ExecuteNonQuery();
                    }

                    transaccion.Commit();
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }

        private PacienteDetalle MapearPaciente(SqlDataReader lector)
        {
            return new PacienteDetalle
            {
                IdPaciente = (int)lector["id_paciente"],
                IdPersona = (int)lector["id_persona"],
                Nombre = lector["nombre"].ToString(),
                Apellido = lector["apellido"].ToString(),
                Telefono = lector["telefono"].ToString(),
                Correo = lector["correo"].ToString(),
                Cedula = lector["cedula"].ToString(),
                NombreUsuario = lector["usuario"].ToString(),
                FechaNacimiento = (DateTime)lector["fecha_nacimiento"],
                Genero = lector["genero"].ToString(),
                Direccion = lector["direccion"].ToString(),
                IdHospital = (int)lector["id_hospital"],
                NombreHospital = lector["nombre_hospital"].ToString()
            };
        }
        public string ObtenerNombreHospitalPorUsuario(int idUsuario)
        {
            string consulta = @"
        SELECT h.nombre
        FROM Persona per
        INNER JOIN Paciente pa
            ON pa.id_persona = per.id_persona
        INNER JOIN Hospital h
            ON h.id_hospital = pa.id_hospital
        WHERE per.id_usuario = @idUsuario;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@idUsuario", SqlDbType.Int).Value =
                    idUsuario;

                conexion.Open();

                object resultado = comando.ExecuteScalar();

                return resultado == null || resultado == DBNull.Value
                    ? ""
                    : resultado.ToString();
            }
        }
    }
}

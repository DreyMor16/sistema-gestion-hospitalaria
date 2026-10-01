using EDL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class MedicoDAL
    {
        private readonly string cadenaConexion =
            ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        public List<MedicoDetalle> Buscar(string filtro, int idHospital)
        {
            List<MedicoDetalle> medicos = new List<MedicoDetalle>();

            string consulta = @"
                SELECT
                    m.id_medico,
                    per.id_persona,
                    per.nombre,
                    per.apellido,
                    per.telefono,
                    per.correo,
                    per.cedula,
                    ISNULL(per.id_usuario, 0) AS id_usuario,
                    ISNULL(u.usuario, '') AS usuario,
                    m.especialidad,
                    h.id_hospital,
                    h.nombre AS nombre_hospital
                FROM Medico m
                INNER JOIN Persona per
                    ON m.id_persona = per.id_persona
                INNER JOIN Hospital h
                    ON m.id_hospital = h.id_hospital
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
                        OR m.especialidad LIKE @patron
                    )
                    AND (@idHospital = 0 OR m.id_hospital = @idHospital)
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
                        medicos.Add(MapearMedico(lector));
                    }
                }
            }

            return medicos;
        }

        public MedicoDetalle ObtenerPorId(int idMedico)
        {
            MedicoDetalle medico = null;

            string consulta = @"
                SELECT
                    m.id_medico,
                    per.id_persona,
                    per.nombre,
                    per.apellido,
                    per.telefono,
                    per.correo,
                    per.cedula,
                    ISNULL(per.id_usuario, 0) AS id_usuario,
                    ISNULL(u.usuario, '') AS usuario,
                    m.especialidad,
                    h.id_hospital,
                    h.nombre AS nombre_hospital
                FROM Medico m
                INNER JOIN Persona per
                    ON m.id_persona = per.id_persona
                INNER JOIN Hospital h
                    ON m.id_hospital = h.id_hospital
                LEFT JOIN Usuario u
                    ON per.id_usuario = u.id_usuario
                WHERE m.id_medico = @idMedico;";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@idMedico", SqlDbType.Int).Value =
                    idMedico;

                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        medico = MapearMedico(lector);
                    }
                }
            }

            return medico;
        }

        public void Insertar(MedicoDetalle medico)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                SqlTransaction transaccion = conexion.BeginTransaction();

                try
                {
                    int idUsuario = InsertarUsuario(medico, conexion, transaccion);
                    int idPersona;

                    string insertarPersona = @"
                INSERT INTO Persona
                (nombre, apellido, telefono, correo, cedula, id_usuario)
                VALUES
                (@nombre, @apellido, @telefono, @correo, @cedula, @idUsuario);

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using (SqlCommand comandoPersona =
                        new SqlCommand(insertarPersona, conexion, transaccion))
                    {
                        comandoPersona.Parameters.Add("@nombre", SqlDbType.VarChar, 50).Value =
                            medico.Nombre;

                        comandoPersona.Parameters.Add("@apellido", SqlDbType.VarChar, 50).Value =
                            medico.Apellido;

                        comandoPersona.Parameters.Add("@telefono", SqlDbType.VarChar, 20).Value =
                            medico.Telefono;

                        comandoPersona.Parameters.Add("@correo", SqlDbType.VarChar, 100).Value =
                            string.IsNullOrWhiteSpace(medico.Correo)
                            ? (object)DBNull.Value
                            : medico.Correo;

                        comandoPersona.Parameters.Add("@cedula", SqlDbType.VarChar, 20).Value =
                            medico.Cedula;

                        comandoPersona.Parameters.Add("@idUsuario", SqlDbType.Int).Value =
                            idUsuario;

                        idPersona = (int)comandoPersona.ExecuteScalar();
                    }

                    string insertarMedico = @"
                INSERT INTO Medico
                (especialidad, id_persona, id_hospital)
                VALUES
                (@especialidad, @idPersona, @idHospital);";

                    using (SqlCommand comandoMedico =
                        new SqlCommand(insertarMedico, conexion, transaccion))
                    {
                        comandoMedico.Parameters.Add("@especialidad", SqlDbType.VarChar, 100).Value =
                            medico.Especialidad;

                        comandoMedico.Parameters.Add("@idPersona", SqlDbType.Int).Value =
                            idPersona;

                        comandoMedico.Parameters.Add("@idHospital", SqlDbType.Int).Value =
                            medico.IdHospital;

                        comandoMedico.ExecuteNonQuery();
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

        public void Actualizar(MedicoDetalle medico)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                SqlTransaction transaccion = conexion.BeginTransaction();

                try
                {
                    int idUsuario = ObtenerIdUsuarioPorPersona(
                        medico.IdPersona,
                        conexion,
                        transaccion);

                    if (idUsuario > 0)
                    {
                        medico.IdUsuario = idUsuario;
                        ActualizarUsuario(medico, conexion, transaccion);
                    }
                    else
                    {
                        idUsuario = InsertarUsuario(medico, conexion, transaccion);
                        medico.IdUsuario = idUsuario;
                    }

                    string actualizarPersona = @"
                UPDATE Persona
                SET nombre = @nombre,
                    apellido = @apellido,
                    telefono = @telefono,
                    correo = @correo,
                    cedula = @cedula,
                    id_usuario = @idUsuario
                WHERE id_persona = @idPersona;";

                    using (SqlCommand comandoPersona =
                        new SqlCommand(actualizarPersona, conexion, transaccion))
                    {
                        comandoPersona.Parameters.Add("@idPersona", SqlDbType.Int).Value =
                            medico.IdPersona;

                        comandoPersona.Parameters.Add("@nombre", SqlDbType.VarChar, 50).Value =
                            medico.Nombre;

                        comandoPersona.Parameters.Add("@apellido", SqlDbType.VarChar, 50).Value =
                            medico.Apellido;

                        comandoPersona.Parameters.Add("@telefono", SqlDbType.VarChar, 20).Value =
                            medico.Telefono;

                        comandoPersona.Parameters.Add("@correo", SqlDbType.VarChar, 100).Value =
                            string.IsNullOrWhiteSpace(medico.Correo)
                            ? (object)DBNull.Value
                            : medico.Correo;

                        comandoPersona.Parameters.Add("@cedula", SqlDbType.VarChar, 20).Value =
                            medico.Cedula;

                        comandoPersona.Parameters.Add("@idUsuario", SqlDbType.Int).Value =
                            idUsuario;

                        comandoPersona.ExecuteNonQuery();
                    }

                    string actualizarMedico = @"
                UPDATE Medico
                SET especialidad = @especialidad,
                    id_hospital = @idHospital
                WHERE id_medico = @idMedico;";

                    using (SqlCommand comandoMedico =
                        new SqlCommand(actualizarMedico, conexion, transaccion))
                    {
                        comandoMedico.Parameters.Add("@idMedico", SqlDbType.Int).Value =
                            medico.IdMedico;

                        comandoMedico.Parameters.Add("@especialidad", SqlDbType.VarChar, 100).Value =
                            medico.Especialidad;

                        comandoMedico.Parameters.Add("@idHospital", SqlDbType.Int).Value =
                            medico.IdHospital;

                        comandoMedico.ExecuteNonQuery();
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

        private MedicoDetalle MapearMedico(SqlDataReader lector)
        {
            return new MedicoDetalle
            {
                IdMedico = (int)lector["id_medico"],
                IdPersona = (int)lector["id_persona"],
                Nombre = lector["nombre"].ToString(),
                Apellido = lector["apellido"].ToString(),
                Telefono = lector["telefono"].ToString(),
                Correo = lector["correo"].ToString(),
                Cedula = lector["cedula"].ToString(),
                IdUsuario = (int)lector["id_usuario"],
                NombreUsuario = lector["usuario"].ToString(),
                Especialidad = lector["especialidad"].ToString(),
                IdHospital = (int)lector["id_hospital"],
                NombreHospital = lector["nombre_hospital"].ToString()
            };
        }
        public void Eliminar(int idMedico)
        {
            string consulta = @"
        DELETE FROM Medico
        WHERE id_medico = @idMedico;";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@idMedico", SqlDbType.Int).Value =
                    idMedico;

                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }
        private int InsertarUsuario(
    MedicoDetalle medico,
    SqlConnection conexion,
    SqlTransaction transaccion)
        {
            string consulta = @"
        INSERT INTO Usuario (usuario, password)
        VALUES (@usuario, @password);

        SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlCommand comando =
                new SqlCommand(consulta, conexion, transaccion))
            {
                comando.Parameters.Add("@usuario", SqlDbType.VarChar, 50).Value =
                    medico.NombreUsuario;

                comando.Parameters.Add("@password", SqlDbType.VarChar, 255).Value =
                    medico.Password;

                return (int)comando.ExecuteScalar();
            }
        }

        private int ObtenerIdUsuarioPorPersona(
            int idPersona,
            SqlConnection conexion,
            SqlTransaction transaccion)
        {
            const string consulta = @"
                SELECT ISNULL(id_usuario, 0)
                FROM Persona
                WHERE id_persona = @idPersona;";

            using (SqlCommand comando =
                new SqlCommand(consulta, conexion, transaccion))
            {
                comando.Parameters.Add("@idPersona", SqlDbType.Int).Value =
                    idPersona;

                object resultado = comando.ExecuteScalar();

                return resultado == null || resultado == DBNull.Value
                    ? 0
                    : Convert.ToInt32(resultado);
            }
        }

        private void ActualizarUsuario(
            MedicoDetalle medico,
            SqlConnection conexion,
            SqlTransaction transaccion)
        {
            string consulta;

            if (string.IsNullOrWhiteSpace(medico.Password))
            {
                consulta = @"
            UPDATE Usuario
            SET usuario = @usuario
            WHERE id_usuario = @idUsuario;";
            }
            else
            {
                consulta = @"
            UPDATE Usuario
            SET usuario = @usuario,
                password = @password
            WHERE id_usuario = @idUsuario;";
            }

            using (SqlCommand comando =
                new SqlCommand(consulta, conexion, transaccion))
            {
                comando.Parameters.Add("@idUsuario", SqlDbType.Int).Value =
                    medico.IdUsuario;

                comando.Parameters.Add("@usuario", SqlDbType.VarChar, 50).Value =
                    medico.NombreUsuario;

                if (!string.IsNullOrWhiteSpace(medico.Password))
                {
                    comando.Parameters.Add("@password", SqlDbType.VarChar, 255).Value =
                        medico.Password;
                }

                comando.ExecuteNonQuery();
            }
        }
    }
}

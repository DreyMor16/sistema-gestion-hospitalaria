using System;
using System.Data.SqlClient;
using System.Configuration;
using EDL;
using System.Data;

namespace DAL
{
    public class UsuarioDAL
    {
        private string cadenaConexion =
            ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        public Usuario ObtenerPorNombreUsuario(string nombreUsuario)
        {
            Usuario usuarioEncontrado = null;

            string query = @"
                SELECT
                    u.id_usuario,
                    u.usuario,
                    u.password,
                    per.nombre,
                    per.apellido,
                    CASE
                        WHEN m.id_medico IS NOT NULL THEN 'Medico'
                        WHEN pa.id_paciente IS NOT NULL THEN 'Paciente'
                        WHEN e.id_empleado IS NOT NULL
                             AND e.puesto = 'Administrador' THEN 'Administrador'
                        WHEN e.id_empleado IS NOT NULL
                             AND e.puesto = 'Recepcionista' THEN 'Recepcionista'
                        ELSE NULL
                    END AS rol
                FROM Usuario u
                INNER JOIN Persona per ON per.id_usuario = u.id_usuario
                LEFT JOIN Medico m ON m.id_persona = per.id_persona
                LEFT JOIN Paciente pa ON pa.id_persona = per.id_persona
                LEFT JOIN Empleado e ON e.id_persona = per.id_persona
                WHERE u.usuario = @usuario";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(query, conexion))
            {
                comando.Parameters.AddWithValue("@usuario", nombreUsuario);

                try
                {
                    conexion.Open();

                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            usuarioEncontrado = new Usuario();

                            usuarioEncontrado.IdUsuario =
                                Convert.ToInt32(lector["id_usuario"]);

                            usuarioEncontrado.NombreUsuario =
                                lector["usuario"].ToString();

                            usuarioEncontrado.Password =
                                lector["password"].ToString();

                            usuarioEncontrado.Nombre = lector["nombre"].ToString();
                            usuarioEncontrado.Apellido = lector["apellido"].ToString();

                            usuarioEncontrado.Rol =
                                lector["rol"] == DBNull.Value
                                ? null
                                : lector["rol"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        "Error en la capa DAL al validar usuario: " + ex.Message);
                }
            }

            return usuarioEncontrado;
        }
        public EstadoRegistroPaciente VerificarCedulaPaciente(string cedula)
        {
            EstadoRegistroPaciente estado =
                new EstadoRegistroPaciente();

            string consulta = @"
        SELECT persona.id_usuario
        FROM Persona persona
        INNER JOIN Paciente paciente
            ON paciente.id_persona = persona.id_persona
        WHERE persona.cedula = @cedula;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@cedula", SqlDbType.VarChar, 20).Value =
                    cedula;

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        estado.PacienteRegistrado = true;

                        estado.YaTieneUsuario =
                            lector["id_usuario"] != DBNull.Value;
                    }
                }
            }

            return estado;
        }
        public void CrearUsuarioParaPaciente(
    RegistroPaciente registro)
        {
            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                SqlTransaction transaccion =
                    conexion.BeginTransaction();

                try
                {
                    int idPersona = 0;

                    string buscarPersona = @"
                SELECT persona.id_persona
                FROM Persona persona WITH (UPDLOCK, HOLDLOCK)
                INNER JOIN Paciente paciente
                    ON paciente.id_persona = persona.id_persona
                WHERE persona.cedula = @cedula
                  AND persona.id_usuario IS NULL;";

                    using (SqlCommand comandoBuscar =
                        new SqlCommand(
                            buscarPersona,
                            conexion,
                            transaccion))
                    {
                        comandoBuscar.Parameters.Add(
                            "@cedula",
                            SqlDbType.VarChar,
                            20).Value = registro.Cedula;

                        object resultado = comandoBuscar.ExecuteScalar();

                        if (resultado != null)
                        {
                            idPersona = Convert.ToInt32(resultado);
                        }
                    }

                    if (idPersona <= 0)
                    {
                        throw new InvalidOperationException(
                            "No es posible activar una cuenta para esta cédula.");
                    }

                    int idUsuario;

                    string crearUsuario = @"
                INSERT INTO Usuario (usuario, password)
                VALUES (@usuario, @password);

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using (SqlCommand comandoUsuario =
                        new SqlCommand(
                            crearUsuario,
                            conexion,
                            transaccion))
                    {
                        comandoUsuario.Parameters.Add(
                            "@usuario",
                            SqlDbType.VarChar,
                            50).Value = registro.NombreUsuario;

                        comandoUsuario.Parameters.Add(
                            "@password",
                            SqlDbType.VarChar,
                            255).Value = registro.Password;

                        idUsuario =
                            Convert.ToInt32(
                                comandoUsuario.ExecuteScalar());
                    }

                    string asociarUsuario = @"
                UPDATE Persona
                SET id_usuario = @idUsuario
                WHERE id_persona = @idPersona
                  AND id_usuario IS NULL;";

                    using (SqlCommand comandoAsociar =
                        new SqlCommand(
                            asociarUsuario,
                            conexion,
                            transaccion))
                    {
                        comandoAsociar.Parameters.Add(
                            "@idUsuario",
                            SqlDbType.Int).Value = idUsuario;

                        comandoAsociar.Parameters.Add(
                            "@idPersona",
                            SqlDbType.Int).Value = idPersona;

                        if (comandoAsociar.ExecuteNonQuery() != 1)
                        {
                            throw new InvalidOperationException(
                                "No fue posible asociar el usuario al paciente.");
                        }
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
        public PerfilUsuario ObtenerPerfil(int idUsuario)
        {
            PerfilUsuario perfil = null;

            string consulta = @"
        SELECT
            u.id_usuario,
            u.usuario,
            p.telefono,
            p.correo
        FROM Usuario u
        INNER JOIN Persona p
            ON p.id_usuario = u.id_usuario
        WHERE u.id_usuario = @idUsuario;";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@idUsuario", SqlDbType.Int).Value =
                    idUsuario;

                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        perfil = new PerfilUsuario
                        {
                            IdUsuario =
                                Convert.ToInt32(lector["id_usuario"]),

                            NombreUsuario =
                                lector["usuario"].ToString(),

                            Telefono =
                                lector["telefono"].ToString(),

                            Correo =
                                lector["correo"].ToString()
                        };
                    }
                }
            }

            return perfil;
        }

        public void ActualizarPerfil(PerfilUsuario perfil)
        {
            string consulta = @"
        UPDATE Usuario
        SET usuario = @usuario,
            password = CASE
                WHEN @nuevaPassword IS NULL THEN password
                ELSE @nuevaPassword
            END
        WHERE id_usuario = @idUsuario;

        UPDATE Persona
        SET telefono = @telefono,
            correo = @correo
        WHERE id_usuario = @idUsuario;";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                SqlTransaction transaccion = conexion.BeginTransaction();

                try
                {
                    using (SqlCommand comando =
                        new SqlCommand(consulta, conexion, transaccion))
                    {
                        comando.Parameters.Add("@idUsuario", SqlDbType.Int).Value =
                            perfil.IdUsuario;

                        comando.Parameters.Add("@usuario", SqlDbType.VarChar, 50).Value =
                            perfil.NombreUsuario;

                        comando.Parameters.Add("@telefono", SqlDbType.VarChar, 20).Value =
                            string.IsNullOrWhiteSpace(perfil.Telefono)
                            ? (object)DBNull.Value
                            : perfil.Telefono;

                        comando.Parameters.Add("@correo", SqlDbType.VarChar, 100).Value =
                            string.IsNullOrWhiteSpace(perfil.Correo)
                            ? (object)DBNull.Value
                            : perfil.Correo;

                        comando.Parameters.Add("@nuevaPassword", SqlDbType.VarChar, 255).Value =
                            string.IsNullOrWhiteSpace(perfil.NuevaPassword)
                            ? (object)DBNull.Value
                            : perfil.NuevaPassword;

                        comando.ExecuteNonQuery();
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
    }
}

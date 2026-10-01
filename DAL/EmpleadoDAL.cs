using EDL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class EmpleadoDAL
    {
        private readonly string cadenaConexion =
            ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        public List<EmpleadoDetalle> Buscar(string filtro, string puesto)
        {
            List<EmpleadoDetalle> empleados = new List<EmpleadoDetalle>();

            string consulta = @"
                SELECT
                    e.id_empleado,
                    per.id_persona,
                    ISNULL(per.id_usuario, 0) AS id_usuario,
                    per.nombre,
                    per.apellido,
                    per.telefono,
                    per.correo,
                    per.cedula,
                    ISNULL(u.usuario, '') AS usuario,
                    e.puesto
                FROM Empleado e
                INNER JOIN Persona per
                    ON e.id_persona = per.id_persona
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
                    AND (@puesto = '' OR e.puesto = @puesto)
                ORDER BY
                    e.puesto,
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

                comando.Parameters.Add("@puesto", SqlDbType.VarChar, 50).Value =
                    puesto ?? "";

                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        empleados.Add(MapearEmpleado(lector));
                    }
                }
            }

            return empleados;
        }

        public EmpleadoDetalle ObtenerPorId(int idEmpleado)
        {
            EmpleadoDetalle empleado = null;

            string consulta = @"
                SELECT
                    e.id_empleado,
                    per.id_persona,
                    ISNULL(per.id_usuario, 0) AS id_usuario,
                    per.nombre,
                    per.apellido,
                    per.telefono,
                    per.correo,
                    per.cedula,
                    ISNULL(u.usuario, '') AS usuario,
                    e.puesto
                FROM Empleado e
                INNER JOIN Persona per
                    ON e.id_persona = per.id_persona
                LEFT JOIN Usuario u
                    ON per.id_usuario = u.id_usuario
                WHERE e.id_empleado = @idEmpleado;";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@idEmpleado", SqlDbType.Int).Value =
                    idEmpleado;

                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        empleado = MapearEmpleado(lector);
                    }
                }
            }

            return empleado;
        }

        public void Insertar(EmpleadoDetalle empleado)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                SqlTransaction transaccion = conexion.BeginTransaction();

                try
                {
                    int idUsuario = InsertarUsuario(
                        empleado,
                        conexion,
                        transaccion);

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
                            empleado.Nombre;

                        comandoPersona.Parameters.Add("@apellido", SqlDbType.VarChar, 50).Value =
                            empleado.Apellido;

                        comandoPersona.Parameters.Add("@telefono", SqlDbType.VarChar, 20).Value =
                            empleado.Telefono;

                        comandoPersona.Parameters.Add("@correo", SqlDbType.VarChar, 100).Value =
                            string.IsNullOrWhiteSpace(empleado.Correo)
                            ? (object)DBNull.Value
                            : empleado.Correo;

                        comandoPersona.Parameters.Add("@cedula", SqlDbType.VarChar, 20).Value =
                            empleado.Cedula;

                        comandoPersona.Parameters.Add("@idUsuario", SqlDbType.Int).Value =
                            idUsuario;

                        idPersona = (int)comandoPersona.ExecuteScalar();
                    }

                    string insertarEmpleado = @"
                        INSERT INTO Empleado (puesto, id_persona)
                        VALUES (@puesto, @idPersona);";

                    using (SqlCommand comandoEmpleado =
                        new SqlCommand(insertarEmpleado, conexion, transaccion))
                    {
                        comandoEmpleado.Parameters.Add("@puesto", SqlDbType.VarChar, 50).Value =
                            empleado.Puesto;

                        comandoEmpleado.Parameters.Add("@idPersona", SqlDbType.Int).Value =
                            idPersona;

                        comandoEmpleado.ExecuteNonQuery();
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

        public void Actualizar(EmpleadoDetalle empleado)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                SqlTransaction transaccion = conexion.BeginTransaction();

                try
                {
                    int idUsuario = empleado.IdUsuario;

                    if (idUsuario > 0)
                    {
                        ActualizarUsuario(empleado, conexion, transaccion);
                    }
                    else
                    {
                        idUsuario = InsertarUsuario(
                            empleado,
                            conexion,
                            transaccion);
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
                            empleado.IdPersona;

                        comandoPersona.Parameters.Add("@nombre", SqlDbType.VarChar, 50).Value =
                            empleado.Nombre;

                        comandoPersona.Parameters.Add("@apellido", SqlDbType.VarChar, 50).Value =
                            empleado.Apellido;

                        comandoPersona.Parameters.Add("@telefono", SqlDbType.VarChar, 20).Value =
                            empleado.Telefono;

                        comandoPersona.Parameters.Add("@correo", SqlDbType.VarChar, 100).Value =
                            string.IsNullOrWhiteSpace(empleado.Correo)
                            ? (object)DBNull.Value
                            : empleado.Correo;

                        comandoPersona.Parameters.Add("@cedula", SqlDbType.VarChar, 20).Value =
                            empleado.Cedula;

                        comandoPersona.Parameters.Add("@idUsuario", SqlDbType.Int).Value =
                            idUsuario;

                        comandoPersona.ExecuteNonQuery();
                    }

                    string actualizarEmpleado = @"
                        UPDATE Empleado
                        SET puesto = @puesto
                        WHERE id_empleado = @idEmpleado;";

                    using (SqlCommand comandoEmpleado =
                        new SqlCommand(actualizarEmpleado, conexion, transaccion))
                    {
                        comandoEmpleado.Parameters.Add("@idEmpleado", SqlDbType.Int).Value =
                            empleado.IdEmpleado;

                        comandoEmpleado.Parameters.Add("@puesto", SqlDbType.VarChar, 50).Value =
                            empleado.Puesto;

                        comandoEmpleado.ExecuteNonQuery();
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

        public void Eliminar(int idEmpleado)
        {
            string consulta = @"
                DELETE FROM Empleado
                WHERE id_empleado = @idEmpleado;";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@idEmpleado", SqlDbType.Int).Value =
                    idEmpleado;

                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        private int InsertarUsuario(
            EmpleadoDetalle empleado,
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
                    empleado.NombreUsuario;

                comando.Parameters.Add("@password", SqlDbType.VarChar, 255).Value =
                    empleado.Password;

                return (int)comando.ExecuteScalar();
            }
        }

        private void ActualizarUsuario(
            EmpleadoDetalle empleado,
            SqlConnection conexion,
            SqlTransaction transaccion)
        {
            string consulta;

            if (string.IsNullOrWhiteSpace(empleado.Password))
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
                    empleado.IdUsuario;

                comando.Parameters.Add("@usuario", SqlDbType.VarChar, 50).Value =
                    empleado.NombreUsuario;

                if (!string.IsNullOrWhiteSpace(empleado.Password))
                {
                    comando.Parameters.Add("@password", SqlDbType.VarChar, 255).Value =
                        empleado.Password;
                }

                comando.ExecuteNonQuery();
            }
        }

        private EmpleadoDetalle MapearEmpleado(SqlDataReader lector)
        {
            return new EmpleadoDetalle
            {
                IdEmpleado = (int)lector["id_empleado"],
                IdPersona = (int)lector["id_persona"],
                IdUsuario = (int)lector["id_usuario"],
                Nombre = lector["nombre"].ToString(),
                Apellido = lector["apellido"].ToString(),
                Telefono = lector["telefono"].ToString(),
                Correo = lector["correo"].ToString(),
                Cedula = lector["cedula"].ToString(),
                NombreUsuario = lector["usuario"].ToString(),
                Puesto = lector["puesto"].ToString()
            };
        }
    }
}
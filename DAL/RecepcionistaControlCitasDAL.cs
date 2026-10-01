using EDL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class RecepcionistaControlCitasDAL
    {
        private readonly string cadenaConexion =
            ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        public List<string> ListarEspecialidades(int idHospital)
        {
            List<string> especialidades = new List<string>();

            string consulta = @"
                SELECT DISTINCT especialidad
                FROM Medico
                WHERE @idHospital = 0
                   OR id_hospital = @idHospital
                ORDER BY especialidad;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add(
                    "@idHospital",
                    SqlDbType.Int).Value = idHospital;

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        especialidades.Add(
                            lector["especialidad"].ToString());
                    }
                }
            }

            return especialidades;
        }

        public List<CitaControlRecepcion> ListarCitasProximas(
            int idHospital,
            string especialidad,
            DateTime? fecha)
        {
            List<CitaControlRecepcion> citas =
                new List<CitaControlRecepcion>();

            string consulta = @"
                SELECT
                    c.id_cita,
                    c.fecha,
                    CONVERT(VARCHAR(5), c.hora, 108) AS hora,
                    c.estado,
                    ISNULL(c.diagnostico, '') AS diagnostico,

                    persona.nombre + ' ' + persona.apellido
                        AS paciente,
                    persona.cedula,
                    ISNULL(persona.telefono, '') AS telefono,
                    ISNULL(persona.correo, '') AS correo,

                    medicoPersona.nombre + ' ' +
                    medicoPersona.apellido AS medico,

                    medico.especialidad,
                    hospital.nombre AS hospital

                FROM Cita c
                INNER JOIN Paciente paciente
                    ON c.id_paciente = paciente.id_paciente
                INNER JOIN Persona persona
                    ON paciente.id_persona = persona.id_persona
                INNER JOIN Medico medico
                    ON c.id_medico = medico.id_medico
                INNER JOIN Persona medicoPersona
                    ON medico.id_persona =
                        medicoPersona.id_persona
                INNER JOIN Hospital hospital
                    ON medico.id_hospital =
                        hospital.id_hospital

                WHERE c.estado = 'En proceso'
                  AND (
                        c.fecha > CAST(GETDATE() AS DATE)
                        OR (
                            c.fecha = CAST(GETDATE() AS DATE)
                            AND c.hora >= CAST(GETDATE() AS TIME)
                        )
                  )
                  AND (
                        @idHospital = 0
                        OR medico.id_hospital = @idHospital
                  )
                  AND (
                        @especialidad IS NULL
                        OR medico.especialidad = @especialidad
                  )
                  AND (
                        @fecha IS NULL
                        OR c.fecha = @fecha
                  )

                ORDER BY c.fecha, c.hora, hospital.nombre,
                         medicoPersona.apellido,
                         medicoPersona.nombre;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add(
                    "@idHospital",
                    SqlDbType.Int).Value = idHospital;

                SqlParameter parametroEspecialidad =
                    comando.Parameters.Add(
                        "@especialidad",
                        SqlDbType.VarChar,
                        100);

                parametroEspecialidad.Value =
                    string.IsNullOrWhiteSpace(especialidad)
                    ? (object)DBNull.Value
                    : especialidad.Trim();

                SqlParameter parametroFecha =
                    comando.Parameters.Add(
                        "@fecha",
                        SqlDbType.Date);

                parametroFecha.Value = fecha.HasValue
                    ? (object)fecha.Value.Date
                    : DBNull.Value;

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        citas.Add(new CitaControlRecepcion
                        {
                            IdCita = Convert.ToInt32(
                                lector["id_cita"]),

                            Fecha = Convert.ToDateTime(
                                lector["fecha"]),

                            Hora = lector["hora"].ToString(),
                            Estado = lector["estado"].ToString(),

                            Diagnostico =
                                lector["diagnostico"].ToString(),

                            Paciente =
                                lector["paciente"].ToString(),

                            Cedula =
                                lector["cedula"].ToString(),

                            Telefono =
                                lector["telefono"].ToString(),

                            Correo =
                                lector["correo"].ToString(),

                            Medico =
                                lector["medico"].ToString(),

                            Especialidad =
                                lector["especialidad"].ToString(),

                            Hospital =
                                lector["hospital"].ToString()
                        });
                    }
                }
            }

            return citas;
        }

        public bool CancelarCita(int idCita)
        {
            string consulta = @"
                UPDATE Cita
                SET estado = 'Cancelada'
                WHERE id_cita = @idCita
                  AND estado = 'En proceso';";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add(
                    "@idCita",
                    SqlDbType.Int).Value = idCita;

                conexion.Open();

                return comando.ExecuteNonQuery() == 1;
            }
        }

        public void CancelarCitasVencidas()
        {
            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(
                    "sp_CancelarCitasVencidas",
                    conexion))
            {
                comando.CommandType =
                    CommandType.StoredProcedure;

                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }
    }
}
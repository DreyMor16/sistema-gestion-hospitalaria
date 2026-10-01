using EDL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class MedicoHistorialDAL
    {
        private readonly string cadenaConexion =
            ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        private class ContextoHistorialMedico
        {
            public int IdMedico { get; set; }
            public int IdHospital { get; set; }
        }

        public List<CitaHistorialMedico> ListarHistorial(
            int idUsuario,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            string filtro,
            string estado)
        {
            ContextoHistorialMedico contexto =
                ObtenerContextoMedico(idUsuario);

            List<CitaHistorialMedico> citas =
                new List<CitaHistorialMedico>();

            string consulta = @"
                SELECT
                    c.id_cita,
                    c.fecha,
                    c.hora,
                    c.Estado,
                    persona.nombre + ' ' + persona.apellido
                        AS nombre_paciente,
                    persona.cedula,
                    ISNULL(c.diagnostico, '') AS diagnostico
                FROM Cita c
                INNER JOIN Paciente paciente
                    ON paciente.id_paciente = c.id_paciente
                INNER JOIN Persona persona
                    ON persona.id_persona = paciente.id_persona
                INNER JOIN Medico medico
                    ON medico.id_medico = c.id_medico
                WHERE c.id_medico = @idMedico
                  AND medico.id_hospital = @idHospital
                  AND c.Estado IN ('Finalizada', 'Cancelada')
                  AND (@fechaInicio IS NULL OR c.fecha >= @fechaInicio)
                  AND (@fechaFin IS NULL OR c.fecha <= @fechaFin)
                  AND
                  (
                      @filtro = ''
                      OR persona.nombre LIKE '%' + @filtro + '%'
                      OR persona.apellido LIKE '%' + @filtro + '%'
                      OR persona.cedula LIKE '%' + @filtro + '%'
                      OR LTRIM(RTRIM(
                            persona.nombre + ' ' + persona.apellido
                         )) LIKE '%' + @filtro + '%'
                  )
                  AND (
                      @estado = 'Todos'
                      OR c.Estado = @estado
                  )
                ORDER BY c.fecha DESC, c.hora DESC;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add(
                    "@idMedico",
                    SqlDbType.Int).Value = contexto.IdMedico;

                comando.Parameters.Add(
                    "@idHospital",
                    SqlDbType.Int).Value = contexto.IdHospital;

                AgregarFiltros(
                    comando,
                    fechaInicio,
                    fechaFin,
                    filtro,
                    estado);

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        citas.Add(new CitaHistorialMedico
                        {
                            IdCita =
                                Convert.ToInt32(
                                    lector["id_cita"]),

                            Fecha =
                                Convert.ToDateTime(
                                    lector["fecha"]),

                            Hora =
                                ((TimeSpan)lector["hora"])
                                .ToString(@"hh\:mm"),

                            Estado =
                                lector["Estado"].ToString(),

                            NombrePaciente =
                                lector["nombre_paciente"].ToString(),

                            Cedula =
                                lector["cedula"].ToString(),

                            Diagnostico =
                                lector["diagnostico"].ToString()
                        });
                    }
                }
            }

            return citas;
        }

        public ResumenHistorialMedico ObtenerResumen(
            int idUsuario,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            string filtro,
            string estado)
        {
            ContextoHistorialMedico contexto =
                ObtenerContextoMedico(idUsuario);

            ResumenHistorialMedico resumen =
                new ResumenHistorialMedico();

            string consulta = @"
                SELECT
                    ISNULL(SUM(
                        CASE WHEN c.Estado = 'Finalizada'
                        THEN 1 ELSE 0 END
                    ), 0) AS total_finalizadas,

                    ISNULL(SUM(
                        CASE WHEN c.Estado = 'Cancelada'
                        THEN 1 ELSE 0 END
                    ), 0) AS total_canceladas
                FROM Cita c
                INNER JOIN Paciente paciente
                    ON paciente.id_paciente = c.id_paciente
                INNER JOIN Persona persona
                    ON persona.id_persona = paciente.id_persona
                INNER JOIN Medico medico
                    ON medico.id_medico = c.id_medico
                WHERE c.id_medico = @idMedico
                  AND medico.id_hospital = @idHospital
                  AND c.Estado IN ('Finalizada', 'Cancelada')
                  AND (@fechaInicio IS NULL OR c.fecha >= @fechaInicio)
                  AND (@fechaFin IS NULL OR c.fecha <= @fechaFin)
                  AND
                  (
                      @filtro = ''
                      OR persona.nombre LIKE '%' + @filtro + '%'
                      OR persona.apellido LIKE '%' + @filtro + '%'
                      OR persona.cedula LIKE '%' + @filtro + '%'
                      OR LTRIM(RTRIM(
                            persona.nombre + ' ' + persona.apellido
                         )) LIKE '%' + @filtro + '%'
                  )
                  AND (
                      @estado = 'Todos'
                      OR c.Estado = @estado
                  );";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add(
                    "@idMedico",
                    SqlDbType.Int).Value = contexto.IdMedico;

                comando.Parameters.Add(
                    "@idHospital",
                    SqlDbType.Int).Value = contexto.IdHospital;

                AgregarFiltros(
                    comando,
                    fechaInicio,
                    fechaFin,
                    filtro,
                    estado);

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        resumen.TotalFinalizadas =
                            Convert.ToInt32(
                                lector["total_finalizadas"]);

                        resumen.TotalCanceladas =
                            Convert.ToInt32(
                                lector["total_canceladas"]);
                    }
                }
            }

            return resumen;
        }

        private void AgregarFiltros(
            SqlCommand comando,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            string filtro,
            string estado)
        {
            comando.Parameters.Add(
                "@fechaInicio",
                SqlDbType.Date).Value = fechaInicio.HasValue
                ? (object)fechaInicio.Value.Date
                : DBNull.Value;

            comando.Parameters.Add(
                "@fechaFin",
                SqlDbType.Date).Value = fechaFin.HasValue
                ? (object)fechaFin.Value.Date
                : DBNull.Value;

            comando.Parameters.Add(
                "@filtro",
                SqlDbType.VarChar,
                100).Value = (filtro ?? "").Trim();

            comando.Parameters.Add(
                "@estado",
                SqlDbType.VarChar,
                20).Value = estado;
        }

        private ContextoHistorialMedico ObtenerContextoMedico(
            int idUsuario)
        {
            ContextoHistorialMedico contexto = null;

            string consulta = @"
                SELECT
                    medico.id_medico,
                    medico.id_hospital
                FROM Persona persona
                INNER JOIN Medico medico
                    ON medico.id_persona = persona.id_persona
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
                        contexto = new ContextoHistorialMedico
                        {
                            IdMedico =
                                Convert.ToInt32(
                                    lector["id_medico"]),

                            IdHospital =
                                Convert.ToInt32(
                                    lector["id_hospital"])
                        };
                    }
                }
            }

            if (contexto == null)
            {
                throw new InvalidOperationException(
                    "No se encontró un médico asociado a esta cuenta.");
            }

            return contexto;
        }
    }
}
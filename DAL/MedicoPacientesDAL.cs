using EDL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class MedicoPacientesDAL
    {
        private readonly string cadenaConexion =
            ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        public List<PacienteAtendidoMedico> ObtenerPacientesAtendidos(
            int idUsuario,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            string filtro)
        {
            ContextoMedico contexto =
                ObtenerContextoMedico(idUsuario);

            List<PacienteAtendidoMedico> pacientes =
                new List<PacienteAtendidoMedico>();

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(
                    "sp_PacientesAtendidosPorMedico",
                    conexion))
            {
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.Add(
                    "@IdMedico",
                    SqlDbType.Int).Value = contexto.IdMedico;

                comando.Parameters.Add(
                    "@IdHospital",
                    SqlDbType.Int).Value = contexto.IdHospital;

                comando.Parameters.Add(
                    "@FechaInicio",
                    SqlDbType.Date).Value = fechaInicio.HasValue
                    ? (object)fechaInicio.Value.Date
                    : DBNull.Value;

                comando.Parameters.Add(
                    "@FechaFin",
                    SqlDbType.Date).Value = fechaFin.HasValue
                    ? (object)fechaFin.Value.Date
                    : DBNull.Value;

                comando.Parameters.Add(
                    "@Filtro",
                    SqlDbType.VarChar,
                    100).Value = string.IsNullOrWhiteSpace(filtro)
                    ? (object)DBNull.Value
                    : filtro.Trim();

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        pacientes.Add(new PacienteAtendidoMedico
                        {
                            IdPaciente =
                                Convert.ToInt32(lector["id_paciente"]),

                            Cedula =
                                lector["cedula"].ToString(),

                            Nombre =
                                lector["nombre"].ToString(),

                            Apellido =
                                lector["apellido"].ToString(),

                            FechaUltimaAtencion =
                                Convert.ToDateTime(
                                    lector["fecha_ultima_atencion"])
                        });
                    }
                }
            }

            return pacientes;
        }

        public DetalleAtencionMedico ObtenerDetalleAtencion(
            int idUsuario,
            int idCita)
        {
            ContextoMedico contexto =
                ObtenerContextoMedico(idUsuario);

            DetalleAtencionMedico detalle = null;

            string consulta = @"
                SELECT
                    persona.nombre + ' ' + persona.apellido
                        AS nombre_paciente,
                    persona.cedula,
                    cita.fecha,
                    cita.hora,
                    cita.diagnostico
                FROM Cita cita
                INNER JOIN Paciente paciente
                    ON paciente.id_paciente = cita.id_paciente
                INNER JOIN Persona persona
                    ON persona.id_persona = paciente.id_persona
                INNER JOIN Medico medico
                    ON medico.id_medico = cita.id_medico
                WHERE cita.id_cita = @idCita
                  AND cita.id_medico = @idMedico
                  AND medico.id_hospital = @idHospital
                  AND cita.Estado = 'Finalizada';";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add(
                    "@idCita",
                    SqlDbType.Int).Value = idCita;

                comando.Parameters.Add(
                    "@idMedico",
                    SqlDbType.Int).Value = contexto.IdMedico;

                comando.Parameters.Add(
                    "@idHospital",
                    SqlDbType.Int).Value = contexto.IdHospital;

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        detalle = new DetalleAtencionMedico
                        {
                            NombrePaciente =
                                lector["nombre_paciente"].ToString(),

                            Cedula =
                                lector["cedula"].ToString(),

                            Fecha =
                                Convert.ToDateTime(lector["fecha"]),

                            Hora =
                                ((TimeSpan)lector["hora"])
                                .ToString(@"hh\:mm"),

                            Diagnostico =
                                lector["diagnostico"].ToString()
                        };
                    }
                }
            }

            return detalle;
        }

        public List<TratamientoPrescripcionMedico>
            ListarTratamientosPrescripciones(
                int idUsuario,
                int idCita)
        {
            ContextoMedico contexto =
                ObtenerContextoMedico(idUsuario);

            List<TratamientoPrescripcionMedico> tratamientos =
                new List<TratamientoPrescripcionMedico>();

            string consulta = @"
                SELECT
                    tratamiento.descripcion AS tratamiento,
                    tratamiento.costo AS costo_tratamiento,
                    ISNULL(
                        medicamento.nombre,
                        'Sin medicamento prescrito'
                    ) AS medicamento,
                    ISNULL(prescripcion.dosis, '—') AS dosis,
                    ISNULL(
                        CONVERT(VARCHAR(10), prescripcion.cantidad),
                        '—'
                    ) AS cantidad
                FROM Cita cita
                INNER JOIN Tratamiento tratamiento
                    ON tratamiento.id_cita = cita.id_cita
                LEFT JOIN Prescripcion prescripcion
                    ON prescripcion.id_tratamiento =
                        tratamiento.id_tratamiento
                LEFT JOIN Medicamento medicamento
                    ON medicamento.id_medicamento =
                        prescripcion.id_medicamento
                INNER JOIN Medico medico
                    ON medico.id_medico = cita.id_medico
                WHERE cita.id_cita = @idCita
                  AND cita.id_medico = @idMedico
                  AND medico.id_hospital = @idHospital
                ORDER BY tratamiento.descripcion, medicamento.nombre;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add(
                    "@idCita",
                    SqlDbType.Int).Value = idCita;

                comando.Parameters.Add(
                    "@idMedico",
                    SqlDbType.Int).Value = contexto.IdMedico;

                comando.Parameters.Add(
                    "@idHospital",
                    SqlDbType.Int).Value = contexto.IdHospital;

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        tratamientos.Add(
                            new TratamientoPrescripcionMedico
                            {
                                Tratamiento =
                                    lector["tratamiento"].ToString(),

                                CostoTratamiento =
                                    Convert.ToDecimal(
                                        lector["costo_tratamiento"]),

                                Medicamento =
                                    lector["medicamento"].ToString(),

                                Dosis =
                                    lector["dosis"].ToString(),

                                Cantidad =
                                    lector["cantidad"].ToString()
                            });
                    }
                }
            }

            return tratamientos;
        }

        private ContextoMedico ObtenerContextoMedico(
            int idUsuario)
        {
            ContextoMedico contexto = null;

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
                        contexto = new ContextoMedico
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
        public List<CitaAtendidaMedico>
    ListarCitasPacienteAtendido(
        int idUsuario,
        int idPaciente)
        {
            ContextoMedico contexto =
                ObtenerContextoMedico(idUsuario);

            List<CitaAtendidaMedico> citas =
                new List<CitaAtendidaMedico>();

            string consulta = @"
        SELECT
            cita.id_cita,
            persona.nombre + ' ' + persona.apellido
                AS nombre_paciente,
            persona.cedula,
            cita.fecha,
            cita.hora,
            cita.diagnostico
        FROM Cita cita
        INNER JOIN Paciente paciente
            ON paciente.id_paciente = cita.id_paciente
        INNER JOIN Persona persona
            ON persona.id_persona = paciente.id_persona
        INNER JOIN Medico medico
            ON medico.id_medico = cita.id_medico
        WHERE cita.id_paciente = @idPaciente
          AND cita.id_medico = @idMedico
          AND medico.id_hospital = @idHospital
          AND cita.Estado = 'Finalizada'
        ORDER BY cita.fecha DESC, cita.hora DESC;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add(
                    "@idPaciente",
                    SqlDbType.Int).Value = idPaciente;

                comando.Parameters.Add(
                    "@idMedico",
                    SqlDbType.Int).Value = contexto.IdMedico;

                comando.Parameters.Add(
                    "@idHospital",
                    SqlDbType.Int).Value = contexto.IdHospital;

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        citas.Add(new CitaAtendidaMedico
                        {
                            IdCita =
                                Convert.ToInt32(lector["id_cita"]),

                            NombrePaciente =
                                lector["nombre_paciente"].ToString(),

                            Cedula =
                                lector["cedula"].ToString(),

                            Fecha =
                                Convert.ToDateTime(lector["fecha"]),

                            Hora =
                                ((TimeSpan)lector["hora"])
                                .ToString(@"hh\:mm"),

                            Diagnostico =
                                lector["diagnostico"].ToString()
                        });
                    }
                }
            }

            return citas;
        }
    }
}
using EDL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class PacienteExpedienteDAL
    {
        private readonly string cadenaConexion =
            ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        public ResumenExpedientePaciente ObtenerResumen(int idUsuario)
        {
            ResumenExpedientePaciente resumen =
                new ResumenExpedientePaciente();

            string consulta = @"
                SELECT
                    COUNT(DISTINCT CASE
                        WHEN c.fecha <= CAST(GETDATE() AS DATE)
                         AND c.Estado IN ('Finalizada', 'Cancelada')
                        THEN c.id_cita
                    END) AS total_citas_pasadas,

                    COUNT(DISTINCT CASE
                        WHEN c.Estado = 'Finalizada'
                        THEN t.id_tratamiento
                    END) AS total_tratamientos,

                    COUNT(DISTINCT CASE
                        WHEN c.Estado = 'Finalizada'
                        THEN pr.id_medicamento
                    END) AS total_medicamentos

                FROM Persona personaPaciente
                INNER JOIN Paciente paciente
                    ON paciente.id_persona = personaPaciente.id_persona
                LEFT JOIN Cita c
                    ON c.id_paciente = paciente.id_paciente
                LEFT JOIN Tratamiento t
                    ON t.id_cita = c.id_cita
                LEFT JOIN Prescripcion pr
                    ON pr.id_tratamiento = t.id_tratamiento
                WHERE personaPaciente.id_usuario = @idUsuario;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@idUsuario", SqlDbType.Int).Value =
                    idUsuario;

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        resumen.TotalCitasPasadas =
                            Convert.ToInt32(
                                lector["total_citas_pasadas"]);

                        resumen.TotalTratamientos =
                            Convert.ToInt32(
                                lector["total_tratamientos"]);

                        resumen.TotalMedicamentos =
                            Convert.ToInt32(
                                lector["total_medicamentos"]);
                    }
                }
            }

            return resumen;
        }

        public List<CitaExpedientePaciente> ListarCitasPasadas(
            int idUsuario)
        {
            List<CitaExpedientePaciente> citas =
                new List<CitaExpedientePaciente>();

            string consulta = @"
                SELECT
                    c.id_cita,
                    c.fecha,
                    c.hora,
                    c.diagnostico,
                    c.Estado,
                    medicoPersona.nombre + ' ' + medicoPersona.apellido
                        AS medico,
                    medico.especialidad
                FROM Persona personaPaciente
                INNER JOIN Paciente paciente
                    ON paciente.id_persona = personaPaciente.id_persona
                INNER JOIN Cita c
                    ON c.id_paciente = paciente.id_paciente
                INNER JOIN Medico medico
                    ON medico.id_medico = c.id_medico
                INNER JOIN Persona medicoPersona
                    ON medicoPersona.id_persona = medico.id_persona
                WHERE personaPaciente.id_usuario = @idUsuario
                  AND c.fecha <= CAST(GETDATE() AS DATE)
                  AND c.Estado IN ('Finalizada', 'Cancelada')
                ORDER BY c.fecha DESC, c.hora DESC;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@idUsuario", SqlDbType.Int).Value =
                    idUsuario;

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        citas.Add(MapearCita(lector));
                    }
                }
            }

            return citas;
        }

        public List<TratamientoMedicamentoPaciente>
            ListarTratamientosYMedicamentos(int idUsuario)
        {
            List<TratamientoMedicamentoPaciente> tratamientos =
                new List<TratamientoMedicamentoPaciente>();

            string consulta = @"
                SELECT
                    c.fecha AS fecha_cita,
                    t.descripcion AS tratamiento,
                    t.costo AS costo_tratamiento,
                    ISNULL(m.nombre, 'Sin medicamento prescrito')
                        AS medicamento,
                    ISNULL(pr.dosis, '—') AS dosis,
                    ISNULL(CONVERT(VARCHAR(10), pr.cantidad), '—')
                        AS cantidad
                FROM Persona personaPaciente
                INNER JOIN Paciente paciente
                    ON paciente.id_persona = personaPaciente.id_persona
                INNER JOIN Cita c
                    ON c.id_paciente = paciente.id_paciente
                INNER JOIN Tratamiento t
                    ON t.id_cita = c.id_cita
                LEFT JOIN Prescripcion pr
                    ON pr.id_tratamiento = t.id_tratamiento
                LEFT JOIN Medicamento m
                    ON m.id_medicamento = pr.id_medicamento
                WHERE personaPaciente.id_usuario = @idUsuario
                  AND c.Estado = 'Finalizada'
                ORDER BY c.fecha DESC, t.descripcion, m.nombre;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@idUsuario", SqlDbType.Int).Value =
                    idUsuario;

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        tratamientos.Add(
                            MapearTratamiento(lector));
                    }
                }
            }

            return tratamientos;
        }

        public CitaExpedientePaciente ObtenerCita(
            int idUsuario,
            int idCita)
        {
            CitaExpedientePaciente cita = null;

            string consulta = @"
                SELECT
                    c.id_cita,
                    c.fecha,
                    c.hora,
                    c.diagnostico,
                    c.Estado,
                    medicoPersona.nombre + ' ' + medicoPersona.apellido
                        AS medico,
                    medico.especialidad
                FROM Persona personaPaciente
                INNER JOIN Paciente paciente
                    ON paciente.id_persona = personaPaciente.id_persona
                INNER JOIN Cita c
                    ON c.id_paciente = paciente.id_paciente
                INNER JOIN Medico medico
                    ON medico.id_medico = c.id_medico
                INNER JOIN Persona medicoPersona
                    ON medicoPersona.id_persona = medico.id_persona
                WHERE personaPaciente.id_usuario = @idUsuario
                  AND c.id_cita = @idCita;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@idUsuario", SqlDbType.Int).Value =
                    idUsuario;

                comando.Parameters.Add("@idCita", SqlDbType.Int).Value =
                    idCita;

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        cita = MapearCita(lector);
                    }
                }
            }

            return cita;
        }

        public List<TratamientoMedicamentoPaciente>
            ListarTratamientosDeCita(int idUsuario, int idCita)
        {
            List<TratamientoMedicamentoPaciente> tratamientos =
                new List<TratamientoMedicamentoPaciente>();

            string consulta = @"
                SELECT
                    c.fecha AS fecha_cita,
                    t.descripcion AS tratamiento,
                    t.costo AS costo_tratamiento,
                    ISNULL(m.nombre, 'Sin medicamento prescrito')
                        AS medicamento,
                    ISNULL(pr.dosis, '—') AS dosis,
                    ISNULL(CONVERT(VARCHAR(10), pr.cantidad), '—')
                        AS cantidad
                FROM Persona personaPaciente
                INNER JOIN Paciente paciente
                    ON paciente.id_persona = personaPaciente.id_persona
                INNER JOIN Cita c
                    ON c.id_paciente = paciente.id_paciente
                INNER JOIN Tratamiento t
                    ON t.id_cita = c.id_cita
                LEFT JOIN Prescripcion pr
                    ON pr.id_tratamiento = t.id_tratamiento
                LEFT JOIN Medicamento m
                    ON m.id_medicamento = pr.id_medicamento
                WHERE personaPaciente.id_usuario = @idUsuario
                  AND c.id_cita = @idCita
                ORDER BY t.descripcion, m.nombre;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@idUsuario", SqlDbType.Int).Value =
                    idUsuario;

                comando.Parameters.Add("@idCita", SqlDbType.Int).Value =
                    idCita;

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        tratamientos.Add(
                            MapearTratamiento(lector));
                    }
                }
            }

            return tratamientos;
        }

        private CitaExpedientePaciente MapearCita(
            SqlDataReader lector)
        {
            return new CitaExpedientePaciente
            {
                IdCita = Convert.ToInt32(lector["id_cita"]),
                Fecha = Convert.ToDateTime(lector["fecha"]),
                Hora = ((TimeSpan)lector["hora"])
                    .ToString(@"hh\:mm"),
                Estado = lector["Estado"].ToString(),
                Diagnostico = lector["diagnostico"].ToString(),
                Medico = lector["medico"].ToString(),
                Especialidad = lector["especialidad"].ToString()
            };
        }

        private TratamientoMedicamentoPaciente MapearTratamiento(
            SqlDataReader lector)
        {
            return new TratamientoMedicamentoPaciente
            {
                FechaCita = Convert.ToDateTime(lector["fecha_cita"]),
                Tratamiento = lector["tratamiento"].ToString(),
                CostoTratamiento =
                    Convert.ToDecimal(lector["costo_tratamiento"]),
                Medicamento = lector["medicamento"].ToString(),
                Dosis = lector["dosis"].ToString(),
                Cantidad = lector["cantidad"].ToString()
            };
        }
    }
}
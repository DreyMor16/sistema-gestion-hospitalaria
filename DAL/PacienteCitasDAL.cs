using EDL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class PacienteCitasDAL
    {
        private readonly string cadenaConexion =
            ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        public List<CitaProximaPaciente> ListarCitasProximas(
            int idUsuario)
        {
            List<CitaProximaPaciente> citas =
                new List<CitaProximaPaciente>();

            string consulta = @"
                SELECT
                    c.fecha,
                    c.hora,
                    c.Estado,
                    medicoPersona.nombre + ' ' + medicoPersona.apellido
                        AS medico,
                    medico.especialidad,
                    h.nombre AS hospital
                FROM Persona personaPaciente
                INNER JOIN Paciente paciente
                    ON paciente.id_persona = personaPaciente.id_persona
                INNER JOIN Cita c
                    ON c.id_paciente = paciente.id_paciente
                INNER JOIN Medico medico
                    ON medico.id_medico = c.id_medico
                INNER JOIN Persona medicoPersona
                    ON medicoPersona.id_persona = medico.id_persona
                INNER JOIN Hospital h
                    ON h.id_hospital = medico.id_hospital
                WHERE personaPaciente.id_usuario = @idUsuario
                 AND c.Estado <> 'Cancelada'
                 AND c.fecha >= CAST(GETDATE() AS DATE)
                ORDER BY c.fecha, c.hora;";

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
                        citas.Add(new CitaProximaPaciente
                        {
                            Fecha = Convert.ToDateTime(lector["fecha"]),

                            Hora = ((TimeSpan)lector["hora"])
                                .ToString(@"hh\:mm"),

                            Estado = lector["Estado"].ToString(),

                            Medico = lector["medico"].ToString(),

                            Especialidad =
                                lector["especialidad"].ToString(),

                            Hospital = lector["hospital"].ToString()
                        });
                    }
                }
            }

            return citas;
        }

        public List<HorarioDisponibleGeneral>
    ListarHorariosMedicinaGeneral(int idUsuario, DateTime fecha)
        {
            List<HorarioDisponibleGeneral> horarios =
                new List<HorarioDisponibleGeneral>();

            string consulta = @"
        WITH Horarios AS
        (
            SELECT CAST('07:00:00' AS TIME) AS hora
            UNION ALL SELECT CAST('08:00:00' AS TIME)
            UNION ALL SELECT CAST('09:00:00' AS TIME)
            UNION ALL SELECT CAST('10:00:00' AS TIME)
            UNION ALL SELECT CAST('11:00:00' AS TIME)
            UNION ALL SELECT CAST('12:00:00' AS TIME)
            UNION ALL SELECT CAST('13:00:00' AS TIME)
            UNION ALL SELECT CAST('14:00:00' AS TIME)
            UNION ALL SELECT CAST('15:00:00' AS TIME)
        )
        SELECT
            medico.id_medico,
            horarios.hora,
            personaMedico.nombre + ' ' + personaMedico.apellido
                AS medico,
            hospital.nombre AS hospital
        FROM Persona personaPaciente
        INNER JOIN Paciente paciente
            ON paciente.id_persona = personaPaciente.id_persona
        INNER JOIN Medico medico
            ON medico.id_hospital = paciente.id_hospital
        INNER JOIN Persona personaMedico
            ON personaMedico.id_persona = medico.id_persona
        INNER JOIN Hospital hospital
            ON hospital.id_hospital = medico.id_hospital
        CROSS JOIN Horarios horarios
        WHERE personaPaciente.id_usuario = @idUsuario
          AND LOWER(LTRIM(RTRIM(medico.especialidad)))
              COLLATE Modern_Spanish_CI_AI = 'medicina general'
          AND NOT EXISTS
            (
                SELECT 1
                FROM Cita cita
                WHERE cita.id_medico = medico.id_medico
                  AND cita.fecha = @fecha
                  AND cita.hora = horarios.hora
                  AND cita.Estado <> 'Cancelada'
            )
          AND (
                @fecha > CAST(GETDATE() AS DATE)
                OR (
                    @fecha = CAST(GETDATE() AS DATE)
                    AND horarios.hora >= CAST(GETDATE() AS TIME)
                )
            )
        ORDER BY horarios.hora, medico;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@idUsuario", SqlDbType.Int).Value =
                    idUsuario;

                comando.Parameters.Add("@fecha", SqlDbType.Date).Value =
                    fecha.Date;

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        horarios.Add(new HorarioDisponibleGeneral
                        {
                            IdMedico =
                                Convert.ToInt32(lector["id_medico"]),

                            Hora = ((TimeSpan)lector["hora"])
                                .ToString(@"hh\:mm"),

                            Medico = lector["medico"].ToString(),

                            Hospital = lector["hospital"].ToString()
                        });
                    }
                }
            }

            return horarios;
        }
        public void AgendarCita(
    int idUsuario,
    int idMedico,
    DateTime fecha,
    TimeSpan hora)
        {
            string consulta = @"
        INSERT INTO Cita
        (
            fecha,
            hora,
            diagnostico,
            Estado,
            id_paciente,
            id_medico
        )
        SELECT
            @fecha,
            @hora,
            NULL,
            'En proceso',
            paciente.id_paciente,
            medico.id_medico
        FROM Persona personaPaciente
        INNER JOIN Paciente paciente
            ON paciente.id_persona = personaPaciente.id_persona
        INNER JOIN Medico medico
            ON medico.id_medico = @idMedico
           AND medico.id_hospital = paciente.id_hospital
        WHERE personaPaciente.id_usuario = @idUsuario
          AND LOWER(LTRIM(RTRIM(medico.especialidad)))
              COLLATE Modern_Spanish_CI_AI = 'medicina general'
          AND NOT EXISTS
            (
                SELECT 1
                FROM Cita cita WITH (UPDLOCK, HOLDLOCK)
                WHERE cita.id_medico = medico.id_medico
                  AND cita.fecha = @fecha
                  AND cita.hora = @hora
                  AND cita.Estado <> 'Cancelada'
            )
          AND NOT EXISTS
          (
              SELECT 1
              FROM Cita citaPaciente WITH (UPDLOCK, HOLDLOCK)
              WHERE citaPaciente.id_paciente = paciente.id_paciente
                AND citaPaciente.fecha = @fecha
                AND citaPaciente.hora = @hora
                AND citaPaciente.Estado <> 'Cancelada'
          );

        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR(
                'El horario ya no está disponible o no pertenece a su hospital.',
                16,
                1
            );
        END";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@idUsuario", SqlDbType.Int).Value =
                    idUsuario;

                comando.Parameters.Add("@idMedico", SqlDbType.Int).Value =
                    idMedico;

                comando.Parameters.Add("@fecha", SqlDbType.Date).Value =
                    fecha.Date;

                comando.Parameters.Add("@hora", SqlDbType.Time).Value =
                    hora;

                conexion.Open();
                comando.ExecuteNonQuery();
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
                comando.CommandType = CommandType.StoredProcedure;

                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }
    }
}
using EDL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class RecepcionistaCitasDAL
    {
        private readonly string cadenaConexion =
            ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        public PacienteCitaRecepcion BuscarPacientePorCedula(
            string cedulaNormalizada)
        {
            PacienteCitaRecepcion paciente = null;

            string consulta = @"
                SELECT
                    paciente.id_paciente,
                    paciente.id_hospital,
                    persona.nombre + ' ' + persona.apellido
                        AS nombre_completo,
                    persona.cedula,
                    hospital.nombre AS hospital
                FROM Persona persona
                INNER JOIN Paciente paciente
                    ON paciente.id_persona = persona.id_persona
                INNER JOIN Hospital hospital
                    ON hospital.id_hospital = paciente.id_hospital
                WHERE REPLACE(REPLACE(persona.cedula, '-', ''), ' ', '')
                    = @cedula;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add(
                    "@cedula",
                    SqlDbType.VarChar,
                    20).Value = cedulaNormalizada;

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        paciente = new PacienteCitaRecepcion
                        {
                            IdPaciente =
                                Convert.ToInt32(
                                    lector["id_paciente"]),

                            IdHospital =
                                Convert.ToInt32(
                                    lector["id_hospital"]),

                            NombreCompleto =
                                lector["nombre_completo"].ToString(),

                            Cedula =
                                lector["cedula"].ToString(),

                            Hospital =
                                lector["hospital"].ToString()
                        };
                    }
                }
            }

            return paciente;
        }

        public List<string> ListarEspecialidades(
            int idHospital)
        {
            List<string> especialidades =
                new List<string>();

            string consulta = @"
                SELECT DISTINCT especialidad
                FROM Medico
                WHERE id_hospital = @idHospital
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

        public List<HorarioCitaRecepcion>
    ListarHorariosDisponibles(
        int idHospital,
        string especialidad,
        DateTime fecha)
        {
            List<HorarioCitaRecepcion> horarios =
                new List<HorarioCitaRecepcion>();

            Dictionary<string, HorarioCitaRecepcion>
                horariosPorHora =
                    new Dictionary<string, HorarioCitaRecepcion>();

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
            persona.nombre + ' ' + persona.apellido
                AS medico,
            medico.especialidad,
            hospital.nombre AS hospital,
            horarios.hora
        FROM Medico medico
        INNER JOIN Persona persona
            ON persona.id_persona = medico.id_persona
        INNER JOIN Hospital hospital
            ON hospital.id_hospital = medico.id_hospital
        CROSS JOIN Horarios horarios
        WHERE medico.id_hospital = @idHospital
          AND medico.especialidad = @especialidad
          AND NOT EXISTS
          (
              SELECT 1
              FROM Cita cita
              WHERE cita.id_medico = medico.id_medico
                AND cita.fecha = @fecha
                AND cita.hora = horarios.hora
                AND cita.Estado <> 'Cancelada'
          )
          AND
          (
              @fecha > CAST(GETDATE() AS DATE)
              OR
              (
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
                comando.Parameters.Add(
                    "@idHospital",
                    SqlDbType.Int).Value = idHospital;

                comando.Parameters.Add(
                    "@especialidad",
                    SqlDbType.VarChar,
                    100).Value = especialidad;

                comando.Parameters.Add(
                    "@fecha",
                    SqlDbType.Date).Value = fecha.Date;

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        string hora =
                            ((TimeSpan)lector["hora"])
                            .ToString(@"hh\:mm");

                        HorarioCitaRecepcion horario;

                        if (!horariosPorHora.TryGetValue(
                            hora,
                            out horario))
                        {
                            horario = new HorarioCitaRecepcion
                            {
                                Hora = hora,

                                Especialidad =
                                    lector["especialidad"].ToString(),

                                Hospital =
                                    lector["hospital"].ToString(),

                                MedicosDisponibles =
                                    new List<MedicoDisponibleCitaRecepcion>()
                            };

                            horariosPorHora.Add(hora, horario);
                            horarios.Add(horario);
                        }

                        horario.MedicosDisponibles.Add(
                            new MedicoDisponibleCitaRecepcion
                            {
                                IdMedico =
                                    Convert.ToInt32(
                                        lector["id_medico"]),

                                NombreCompleto =
                                    lector["medico"].ToString()
                            });
                    }
                }
            }

            return horarios;
        }
        public UltimaCitaEspecialidadRecepcion
    ObtenerUltimaCitaEspecialidad(
        int idPaciente,
        string especialidad)
        {
            UltimaCitaEspecialidadRecepcion cita = null;

            string consultaCita = @"
        SELECT TOP 1
            c.id_cita,
            c.fecha,
            c.hora,
            c.Estado,
            ISNULL(c.diagnostico, '') AS diagnostico,
            personaMedico.nombre + ' ' +
            personaMedico.apellido AS medico,
            hospital.nombre AS hospital
        FROM Cita c
        INNER JOIN Medico medico
            ON medico.id_medico = c.id_medico
        INNER JOIN Persona personaMedico
            ON personaMedico.id_persona = medico.id_persona
        INNER JOIN Hospital hospital
            ON hospital.id_hospital = medico.id_hospital
        WHERE c.id_paciente = @idPaciente
          AND medico.especialidad = @especialidad
          AND c.Estado = 'Finalizada'
        ORDER BY c.fecha DESC, c.hora DESC, c.id_cita DESC;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consultaCita, conexion))
            {
                comando.Parameters.Add(
                    "@idPaciente",
                    SqlDbType.Int).Value = idPaciente;

                comando.Parameters.Add(
                    "@especialidad",
                    SqlDbType.VarChar,
                    100).Value = especialidad;

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        cita = new UltimaCitaEspecialidadRecepcion
                        {
                            IdCita =
                                Convert.ToInt32(lector["id_cita"]),

                            Fecha =
                                Convert.ToDateTime(lector["fecha"]),

                            Hora =
                                ((TimeSpan)lector["hora"])
                                .ToString(@"hh\:mm"),

                            Estado =
                                lector["Estado"].ToString(),

                            Diagnostico =
                                lector["diagnostico"].ToString(),

                            Medico =
                                lector["medico"].ToString(),

                            Hospital =
                                lector["hospital"].ToString(),

                            Tratamientos =
                                new List<TratamientoUltimaCitaRecepcion>()
                        };
                    }
                }

                if (cita == null)
                {
                    return null;
                }

                string consultaTratamientos = @"
            SELECT
                tratamiento.descripcion,
                tratamiento.costo,

                ISNULL
                (
                    STUFF
                    (
                        (
                            SELECT
                                ' - ' + medicamento.nombre +
                                ' (' +
                                CAST(prescripcion.cantidad
                                    AS VARCHAR(10)) +
                                ' unidades, ' +
                                prescripcion.dosis +
                                ')'
                            FROM Prescripcion prescripcion
                            INNER JOIN Medicamento medicamento
                                ON medicamento.id_medicamento =
                                    prescripcion.id_medicamento
                            WHERE prescripcion.id_tratamiento =
                                tratamiento.id_tratamiento
                            ORDER BY medicamento.nombre
                            FOR XML PATH(''), TYPE
                        ).value('.', 'VARCHAR(MAX)'),
                        1,
                        3,
                        ''
                    ),
                    'Sin medicamentos prescritos'
                ) AS medicamentos

            FROM Tratamiento tratamiento
            WHERE tratamiento.id_cita = @idCita
            ORDER BY tratamiento.id_tratamiento;";

                using (SqlCommand comandoTratamientos =
                    new SqlCommand(
                        consultaTratamientos,
                        conexion))
                {
                    comandoTratamientos.Parameters.Add(
                        "@idCita",
                        SqlDbType.Int).Value = cita.IdCita;

                    using (SqlDataReader lectorTratamientos =
                        comandoTratamientos.ExecuteReader())
                    {
                        while (lectorTratamientos.Read())
                        {
                            cita.Tratamientos.Add(
                                new TratamientoUltimaCitaRecepcion
                                {
                                    Descripcion =
                                        lectorTratamientos["descripcion"]
                                        .ToString(),

                                    Costo =
                                        Convert.ToDecimal(
                                            lectorTratamientos["costo"]),

                                    Medicamentos =
                                        lectorTratamientos["medicamentos"]
                                        .ToString()
                                });
                        }
                    }
                }
            }

            return cita;
        }
        public void RegistrarCita(
            int idPaciente,
            int idMedico,
            DateTime fecha,
            TimeSpan hora)
        {
            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            {
                conexion.Open();

                SqlTransaction transaccion =
                    conexion.BeginTransaction();

                try
                {
                    string validarCita = @"
                        IF NOT EXISTS
                        (
                            SELECT 1
                            FROM Paciente paciente
                            INNER JOIN Medico medico
                                ON medico.id_hospital =
                                    paciente.id_hospital
                            WHERE paciente.id_paciente = @idPaciente
                              AND medico.id_medico = @idMedico
                        )
                        BEGIN
                            RAISERROR(
                                'El médico no pertenece al hospital del paciente.',
                                16,
                                1
                            );
                            RETURN;
                        END;

                        IF EXISTS
                        (
                            SELECT 1
                            FROM Cita cita WITH (UPDLOCK, HOLDLOCK)
                            WHERE cita.id_paciente = @idPaciente
                              AND cita.fecha = @fecha
                              AND cita.hora = @hora
                              AND cita.Estado <> 'Cancelada'
                        )
                        BEGIN
                            RAISERROR(
                                'El paciente ya tiene una cita activa en ese horario.',
                                16,
                                1
                            );
                            RETURN;
                        END;";

                    using (SqlCommand comandoValidar =
                        new SqlCommand(
                            validarCita,
                            conexion,
                            transaccion))
                    {
                        comandoValidar.Parameters.Add(
                            "@idPaciente",
                            SqlDbType.Int).Value = idPaciente;

                        comandoValidar.Parameters.Add(
                            "@idMedico",
                            SqlDbType.Int).Value = idMedico;

                        comandoValidar.Parameters.Add(
                            "@fecha",
                            SqlDbType.Date).Value = fecha.Date;

                        comandoValidar.Parameters.Add(
                            "@hora",
                            SqlDbType.Time).Value = hora;

                        comandoValidar.ExecuteNonQuery();
                    }

                    using (SqlCommand comando =
                        new SqlCommand(
                            "sp_RegistrarCita",
                            conexion,
                            transaccion))
                    {
                        comando.CommandType =
                            CommandType.StoredProcedure;

                        comando.Parameters.Add(
                            "@IdPaciente",
                            SqlDbType.Int).Value = idPaciente;

                        comando.Parameters.Add(
                            "@IdMedico",
                            SqlDbType.Int).Value = idMedico;

                        comando.Parameters.Add(
                            "@Fecha",
                            SqlDbType.Date).Value = fecha.Date;

                        comando.Parameters.Add(
                            "@Hora",
                            SqlDbType.Time).Value = hora;

                        comando.Parameters.Add(
                            "@Diagnostico",
                            SqlDbType.VarChar,
                            255).Value = DBNull.Value;

                        comando.Parameters.Add(
                            "@Estado",
                            SqlDbType.VarChar,
                            20).Value = "En proceso";

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
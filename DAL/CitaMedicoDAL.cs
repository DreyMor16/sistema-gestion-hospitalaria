using EDL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class CitaMedicoDAL
    {
        private readonly string cadenaConexion =
            ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        private class ContextoMedico
        {
            public int IdMedico { get; set; }
            public int IdHospital { get; set; }
        }

        public List<CitaProximaAtencion> ListarProximasCitas(
            int idUsuario)
        {
            ContextoMedico contexto =
                ObtenerContextoMedico(idUsuario);

            List<CitaProximaAtencion> citas =
                new List<CitaProximaAtencion>();

            string consulta = @"
                SELECT
                    cita.id_cita,
                    cita.fecha,
                    cita.hora,
                    persona.nombre + ' ' + persona.apellido
                        AS nombre_paciente,
                    persona.cedula
                FROM Cita cita
                INNER JOIN Paciente paciente
                    ON paciente.id_paciente = cita.id_paciente
                INNER JOIN Persona persona
                    ON persona.id_persona = paciente.id_persona
                INNER JOIN Medico medico
                    ON medico.id_medico = cita.id_medico
                WHERE cita.id_medico = @idMedico
                  AND medico.id_hospital = @idHospital
                  AND cita.Estado = 'En proceso'
                  AND cita.fecha >= CAST(GETDATE() AS DATE)
                ORDER BY cita.fecha, cita.hora;";

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

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        citas.Add(new CitaProximaAtencion
                        {
                            IdCita =
                                Convert.ToInt32(lector["id_cita"]),

                            Fecha =
                                Convert.ToDateTime(lector["fecha"]),

                            Hora =
                                ((TimeSpan)lector["hora"])
                                .ToString(@"hh\:mm"),

                            NombrePaciente =
                                lector["nombre_paciente"].ToString(),

                            Cedula =
                                lector["cedula"].ToString()
                        });
                    }
                }
            }

            return citas;
        }

        public DetalleCitaAtencion ObtenerCita(
            int idUsuario,
            int idCita)
        {
            ContextoMedico contexto =
                ObtenerContextoMedico(idUsuario);

            DetalleCitaAtencion cita = null;

            string consulta = @"
                SELECT
                    cita.id_cita,
                    persona.nombre + ' ' + persona.apellido
                        AS nombre_paciente,
                    persona.cedula,
                    hospital.nombre AS hospital,
                    cita.fecha,
                    cita.hora,
                    ISNULL(cita.diagnostico, '') AS diagnostico
                FROM Cita cita
                INNER JOIN Paciente paciente
                    ON paciente.id_paciente = cita.id_paciente
                INNER JOIN Persona persona
                    ON persona.id_persona = paciente.id_persona
                INNER JOIN Hospital hospital
                    ON hospital.id_hospital = paciente.id_hospital
                INNER JOIN Medico medico
                    ON medico.id_medico = cita.id_medico
                WHERE cita.id_cita = @idCita
                  AND cita.id_medico = @idMedico
                  AND medico.id_hospital = @idHospital
                  AND cita.Estado = 'En proceso';";

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
                        cita = new DetalleCitaAtencion
                        {
                            IdCita =
                                Convert.ToInt32(lector["id_cita"]),

                            NombrePaciente =
                                lector["nombre_paciente"].ToString(),

                            Cedula =
                                lector["cedula"].ToString(),

                            Hospital =
                                lector["hospital"].ToString(),

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

            return cita;
        }

        public void GuardarDiagnostico(
            int idUsuario,
            int idCita,
            string diagnostico)
        {
            ContextoMedico contexto =
                ObtenerContextoMedico(idUsuario);

            string consulta = @"
                UPDATE Cita
                SET diagnostico = @diagnostico
                WHERE id_cita = @idCita
                  AND id_medico = @idMedico
                  AND Estado = 'En proceso';";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add(
                    "@diagnostico",
                    SqlDbType.VarChar,
                    255).Value = diagnostico;

                comando.Parameters.Add(
                    "@idCita",
                    SqlDbType.Int).Value = idCita;

                comando.Parameters.Add(
                    "@idMedico",
                    SqlDbType.Int).Value = contexto.IdMedico;

                conexion.Open();

                if (comando.ExecuteNonQuery() != 1)
                {
                    throw new InvalidOperationException(
                        "No fue posible guardar el diagnóstico.");
                }
            }
        }

        public void AgregarTratamiento(
            int idUsuario,
            int idCita,
            string descripcion)
        {
            ContextoMedico contexto =
                ObtenerContextoMedico(idUsuario);

            string consulta = @"
                INSERT INTO Tratamiento
                (
                    descripcion,
                    costo,
                    id_cita
                )
                SELECT
                    @descripcion,
                    0,
                    cita.id_cita
                FROM Cita cita
                WHERE cita.id_cita = @idCita
                  AND cita.id_medico = @idMedico
                  AND cita.Estado = 'En proceso'
                  AND NULLIF(LTRIM(RTRIM(cita.diagnostico)), '') IS NOT NULL;

                IF @@ROWCOUNT = 0
                BEGIN
                    RAISERROR(
                        'Guarde un diagnóstico antes de agregar el tratamiento.',
                        16,
                        1
                    );
                END;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add(
                    "@descripcion",
                    SqlDbType.VarChar,
                    500).Value = descripcion;

                comando.Parameters.Add(
                    "@idCita",
                    SqlDbType.Int).Value = idCita;

                comando.Parameters.Add(
                    "@idMedico",
                    SqlDbType.Int).Value = contexto.IdMedico;

                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public List<TratamientoAtencion> ListarTratamientos(
            int idUsuario,
            int idCita)
        {
            ContextoMedico contexto =
                ObtenerContextoMedico(idUsuario);

            List<TratamientoAtencion> tratamientos =
                new List<TratamientoAtencion>();

            string consulta = @"
                SELECT
                    tratamiento.id_tratamiento,
                    tratamiento.descripcion,
                    tratamiento.costo
                FROM Tratamiento tratamiento
                INNER JOIN Cita cita
                    ON cita.id_cita = tratamiento.id_cita
                WHERE cita.id_cita = @idCita
                  AND cita.id_medico = @idMedico
                ORDER BY tratamiento.id_tratamiento DESC;";

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

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        tratamientos.Add(new TratamientoAtencion
                        {
                            IdTratamiento =
                                Convert.ToInt32(
                                    lector["id_tratamiento"]),

                            Descripcion =
                                lector["descripcion"].ToString(),

                            Costo =
                                Convert.ToDecimal(lector["costo"])
                        });
                    }
                }
            }

            return tratamientos;
        }

        public List<MedicamentoDisponibleAtencion>
            ListarMedicamentosDisponibles(
                int idUsuario,
                int idCita)
        {
            ContextoMedico contexto =
                ObtenerContextoMedico(idUsuario);

            List<MedicamentoDisponibleAtencion> medicamentos =
                new List<MedicamentoDisponibleAtencion>();

            string consulta = @"
                SELECT
                    medicamento.id_medicamento,
                    medicamento.nombre,
                    inventario.cantidad_stock,
                    medicamento.costo_unitario
                FROM Cita cita
                INNER JOIN Paciente paciente
                    ON paciente.id_paciente = cita.id_paciente
                INNER JOIN Medico medico
                    ON medico.id_medico = cita.id_medico
                INNER JOIN Inventario_Hospital inventario
                    ON inventario.id_hospital = paciente.id_hospital
                INNER JOIN Medicamento medicamento
                    ON medicamento.id_medicamento =
                        inventario.id_medicamento
                WHERE cita.id_cita = @idCita
                  AND cita.id_medico = @idMedico
                  AND medico.id_hospital = @idHospital
                  AND cita.Estado = 'En proceso'
                  AND inventario.cantidad_stock > 0
                ORDER BY medicamento.nombre;";

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
                        medicamentos.Add(
                            new MedicamentoDisponibleAtencion
                            {
                                IdMedicamento =
                                    Convert.ToInt32(
                                        lector["id_medicamento"]),

                                Nombre =
                                    lector["nombre"].ToString(),

                                StockDisponible =
                                    Convert.ToInt32(
                                        lector["cantidad_stock"]),

                                CostoUnitario =
                                    Convert.ToDecimal(
                                        lector["costo_unitario"])
                            });
                    }
                }
            }

            return medicamentos;
        }

        public void RegistrarPrescripcion(
            int idUsuario,
            int idCita,
            int idTratamiento,
            int idMedicamento,
            int cantidad,
            string dosis)
        {
            ContextoMedico contexto =
                ObtenerContextoMedico(idUsuario);

            string consulta = @"
                INSERT INTO Prescripcion
                (
                    id_tratamiento,
                    id_medicamento,
                    cantidad,
                    dosis
                )
                SELECT
                    tratamiento.id_tratamiento,
                    @idMedicamento,
                    @cantidad,
                    @dosis
                FROM Tratamiento tratamiento
                INNER JOIN Cita cita
                    ON cita.id_cita = tratamiento.id_cita
                INNER JOIN Paciente paciente
                    ON paciente.id_paciente = cita.id_paciente
                INNER JOIN Medico medico
                    ON medico.id_medico = cita.id_medico
                INNER JOIN Inventario_Hospital inventario
                    ON inventario.id_hospital = paciente.id_hospital
                   AND inventario.id_medicamento = @idMedicamento
                WHERE tratamiento.id_tratamiento = @idTratamiento
                  AND cita.id_cita = @idCita
                  AND cita.id_medico = @idMedico
                  AND medico.id_hospital = @idHospital
                  AND cita.Estado = 'En proceso'
                  AND NULLIF(LTRIM(RTRIM(cita.diagnostico)), '') IS NOT NULL
                  AND inventario.cantidad_stock > 0;

                IF @@ROWCOUNT = 0
                BEGIN
                    RAISERROR(
                        'El medicamento no tiene stock disponible para este paciente.',
                        16,
                        1
                    );
                END;";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add(
                    "@idTratamiento",
                    SqlDbType.Int).Value = idTratamiento;

                comando.Parameters.Add(
                    "@idMedicamento",
                    SqlDbType.Int).Value = idMedicamento;

                comando.Parameters.Add(
                    "@cantidad",
                    SqlDbType.Int).Value = cantidad;

                comando.Parameters.Add(
                    "@dosis",
                    SqlDbType.VarChar,
                    100).Value = dosis;

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
                comando.ExecuteNonQuery();
            }
        }

        public List<PrescripcionAtencion> ListarPrescripciones(
            int idUsuario,
            int idCita)
        {
            ContextoMedico contexto =
                ObtenerContextoMedico(idUsuario);

            List<PrescripcionAtencion> prescripciones =
                new List<PrescripcionAtencion>();

            string consulta = @"
                SELECT
                    tratamiento.descripcion AS tratamiento,
                    medicamento.nombre AS medicamento,
                    prescripcion.dosis,
                    prescripcion.cantidad,
                    medicamento.costo_unitario,
                    CAST(
                        prescripcion.cantidad * medicamento.costo_unitario
                        AS DECIMAL(10, 2)
                    ) AS subtotal
                FROM Prescripcion prescripcion
                INNER JOIN Tratamiento tratamiento
                    ON tratamiento.id_tratamiento =
                        prescripcion.id_tratamiento
                INNER JOIN Cita cita
                    ON cita.id_cita = tratamiento.id_cita
                INNER JOIN Medicamento medicamento
                    ON medicamento.id_medicamento =
                        prescripcion.id_medicamento
                WHERE cita.id_cita = @idCita
                  AND cita.id_medico = @idMedico
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

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        prescripciones.Add(
                            new PrescripcionAtencion
                            {
                                Tratamiento =
                                    lector["tratamiento"].ToString(),

                                Medicamento =
                                    lector["medicamento"].ToString(),

                                Dosis =
                                    lector["dosis"].ToString(),

                                Cantidad =
                                    Convert.ToInt32(
                                        lector["cantidad"]),

                                CostoUnitario =
                                    Convert.ToDecimal(
                                        lector["costo_unitario"]),

                                Subtotal =
                                    Convert.ToDecimal(
                                        lector["subtotal"])
                            });
                    }
                }
            }

            return prescripciones;
        }

        public void FinalizarCita(
            int idUsuario,
            int idCita,
            string diagnostico)
        {
            ContextoMedico contexto =
                ObtenerContextoMedico(idUsuario);

            string consulta = @"
                UPDATE Cita
                SET diagnostico = @diagnostico,
                    Estado = 'Finalizada'
                WHERE id_cita = @idCita
                  AND id_medico = @idMedico
                  AND Estado = 'En proceso';";

            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            using (SqlCommand comando =
                new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add(
                    "@diagnostico",
                    SqlDbType.VarChar,
                    255).Value = diagnostico;

                comando.Parameters.Add(
                    "@idCita",
                    SqlDbType.Int).Value = idCita;

                comando.Parameters.Add(
                    "@idMedico",
                    SqlDbType.Int).Value = contexto.IdMedico;

                conexion.Open();

                if (comando.ExecuteNonQuery() != 1)
                {
                    throw new InvalidOperationException(
                        "No fue posible finalizar la cita.");
                }
            }
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
                    "No se encontró un médico asociado a la cuenta.");
            }

            return contexto;
        }
    }
}

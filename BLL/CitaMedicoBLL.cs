using DAL;
using EDL;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class CitaMedicoBLL
    {
        private readonly CitaMedicoDAL citaDAL =
            new CitaMedicoDAL();

        public List<CitaProximaAtencion> ListarProximasCitas(
            int idUsuario)
        {
            ValidarUsuario(idUsuario);
            return citaDAL.ListarProximasCitas(idUsuario);
        }

        public DetalleCitaAtencion ObtenerCita(
            int idUsuario,
            int idCita)
        {
            ValidarCita(idUsuario, idCita);
            return citaDAL.ObtenerCita(idUsuario, idCita);
        }

        public void GuardarDiagnostico(
            int idUsuario,
            int idCita,
            string diagnostico)
        {
            ValidarCita(idUsuario, idCita);

            diagnostico = (diagnostico ?? "").Trim();

            if (diagnostico.Length > 255)
            {
                throw new ArgumentException(
                    "El diagnóstico no puede superar 255 caracteres.");
            }

            citaDAL.GuardarDiagnostico(
                idUsuario,
                idCita,
                diagnostico);
        }

        public void AgregarTratamiento(
            int idUsuario,
            int idCita,
            string descripcion)
        {
            ValidarCita(idUsuario, idCita);

            descripcion = (descripcion ?? "").Trim();

            if (string.IsNullOrWhiteSpace(descripcion))
            {
                throw new ArgumentException(
                    "La descripción del tratamiento es requerida.");
            }

            if (descripcion.Length > 500)
            {
                throw new ArgumentException(
                    "La descripción no puede superar 500 caracteres.");
            }

            citaDAL.AgregarTratamiento(
                idUsuario,
                idCita,
                descripcion);
        }

        public List<TratamientoAtencion> ListarTratamientos(
            int idUsuario,
            int idCita)
        {
            ValidarCita(idUsuario, idCita);
            return citaDAL.ListarTratamientos(idUsuario, idCita);
        }

        public List<MedicamentoDisponibleAtencion>
            ListarMedicamentosDisponibles(
                int idUsuario,
                int idCita)
        {
            ValidarCita(idUsuario, idCita);

            return citaDAL.ListarMedicamentosDisponibles(
                idUsuario,
                idCita);
        }

        public void RegistrarPrescripcion(
            int idUsuario,
            int idCita,
            int idTratamiento,
            int idMedicamento,
            int cantidad,
            string dosis)
        {
            ValidarCita(idUsuario, idCita);

            if (idTratamiento <= 0 || idMedicamento <= 0)
            {
                throw new ArgumentException(
                    "Debe seleccionar un tratamiento y medicamento.");
            }

            if (cantidad <= 0)
            {
                throw new ArgumentException(
                    "La cantidad debe ser mayor que cero.");
            }

            dosis = (dosis ?? "").Trim();

            if (string.IsNullOrWhiteSpace(dosis))
            {
                throw new ArgumentException(
                    "La dosis es requerida.");
            }

            if (dosis.Length > 100)
            {
                throw new ArgumentException(
                    "La dosis no puede superar 100 caracteres.");
            }

            citaDAL.RegistrarPrescripcion(
                idUsuario,
                idCita,
                idTratamiento,
                idMedicamento,
                cantidad,
                dosis);
        }

        public List<PrescripcionAtencion> ListarPrescripciones(
            int idUsuario,
            int idCita)
        {
            ValidarCita(idUsuario, idCita);
            return citaDAL.ListarPrescripciones(idUsuario, idCita);
        }

        public void FinalizarCita(
            int idUsuario,
            int idCita,
            string diagnostico)
        {
            ValidarCita(idUsuario, idCita);

            diagnostico = (diagnostico ?? "").Trim();

            if (string.IsNullOrWhiteSpace(diagnostico))
            {
                throw new ArgumentException(
                    "Debe registrar un diagnóstico antes de finalizar la cita.");
            }

            citaDAL.FinalizarCita(
                idUsuario,
                idCita,
                diagnostico);
        }

        private void ValidarUsuario(int idUsuario)
        {
            if (idUsuario <= 0)
            {
                throw new ArgumentException(
                    "El usuario no es válido.");
            }
        }

        private void ValidarCita(int idUsuario, int idCita)
        {
            ValidarUsuario(idUsuario);

            if (idCita <= 0)
            {
                throw new ArgumentException(
                    "La cita seleccionada no es válida.");
            }
        }
    }
}

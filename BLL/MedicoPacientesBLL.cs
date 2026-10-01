using DAL;
using EDL;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class MedicoPacientesBLL
    {
        private readonly MedicoPacientesDAL medicoDAL =
            new MedicoPacientesDAL();

        public List<PacienteAtendidoMedico>
         ObtenerPacientesAtendidos(
             int idUsuario,
             DateTime? fechaInicio,
             DateTime? fechaFin,
             string filtro)
            {
                ValidarUsuario(idUsuario);

                if (fechaInicio.HasValue &&
                    fechaFin.HasValue &&
                    fechaInicio.Value.Date > fechaFin.Value.Date)
                {
                    throw new ArgumentException(
                        "La fecha inicial no puede ser posterior a la fecha final.");
                }

                return medicoDAL.ObtenerPacientesAtendidos(
                    idUsuario,
                    fechaInicio,
                    fechaFin,
                    filtro);
        }

        public DetalleAtencionMedico ObtenerDetalleAtencion(
            int idUsuario,
            int idCita)
        {
            ValidarUsuario(idUsuario);

            if (idCita <= 0)
            {
                throw new ArgumentException(
                    "La cita seleccionada no es válida.");
            }

            return medicoDAL.ObtenerDetalleAtencion(
                idUsuario,
                idCita);
        }

        public List<TratamientoPrescripcionMedico>
            ListarTratamientosPrescripciones(
                int idUsuario,
                int idCita)
        {
            ValidarUsuario(idUsuario);

            if (idCita <= 0)
            {
                throw new ArgumentException(
                    "La cita seleccionada no es válida.");
            }

            return medicoDAL.ListarTratamientosPrescripciones(
                idUsuario,
                idCita);
        }

        private void ValidarUsuario(int idUsuario)
        {
            if (idUsuario <= 0)
            {
                throw new ArgumentException(
                    "El usuario no es válido.");
            }
        }

        private void ValidarPeriodo(
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            if (fechaInicio.Date > fechaFin.Date)
            {
                throw new ArgumentException(
                    "La fecha inicial no puede ser posterior a la fecha final.");
            }
        }
        public List<CitaAtendidaMedico>
    ListarCitasPacienteAtendido(
        int idUsuario,
        int idPaciente)
        {
            ValidarUsuario(idUsuario);

            if (idPaciente <= 0)
            {
                throw new ArgumentException(
                    "El paciente seleccionado no es válido.");
            }

            return medicoDAL.ListarCitasPacienteAtendido(
                idUsuario,
                idPaciente);
        }
    }
}
using DAL;
using EDL;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class PacienteExpedienteBLL
    {
        private readonly PacienteExpedienteDAL expedienteDAL =
            new PacienteExpedienteDAL();

        public ResumenExpedientePaciente ObtenerResumen(int idUsuario)
        {
            ValidarUsuario(idUsuario);
            return expedienteDAL.ObtenerResumen(idUsuario);
        }

        public List<CitaExpedientePaciente> ListarCitasPasadas(
            int idUsuario)
        {
            ValidarUsuario(idUsuario);
            return expedienteDAL.ListarCitasPasadas(idUsuario);
        }

        public List<TratamientoMedicamentoPaciente>
            ListarTratamientosYMedicamentos(int idUsuario)
        {
            ValidarUsuario(idUsuario);
            return expedienteDAL.ListarTratamientosYMedicamentos(
                idUsuario);
        }

        public CitaExpedientePaciente ObtenerCita(
            int idUsuario,
            int idCita)
        {
            ValidarUsuario(idUsuario);

            if (idCita <= 0)
            {
                throw new ArgumentException(
                    "La cita seleccionada no es válida.");
            }

            return expedienteDAL.ObtenerCita(idUsuario, idCita);
        }

        public List<TratamientoMedicamentoPaciente>
            ListarTratamientosDeCita(int idUsuario, int idCita)
        {
            ValidarUsuario(idUsuario);

            if (idCita <= 0)
            {
                throw new ArgumentException(
                    "La cita seleccionada no es válida.");
            }

            return expedienteDAL.ListarTratamientosDeCita(
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
    }
}
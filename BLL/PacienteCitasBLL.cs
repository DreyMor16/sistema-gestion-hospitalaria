using DAL;
using EDL;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class PacienteCitasBLL
    {
        private readonly PacienteCitasDAL citasDAL =
            new PacienteCitasDAL();

        public List<CitaProximaPaciente> ListarCitasProximas(
            int idUsuario)
        {
            if (idUsuario <= 0)
            {
                throw new ArgumentException(
                    "El usuario no es válido.");
            }

            return citasDAL.ListarCitasProximas(idUsuario);
        }

        public List<HorarioDisponibleGeneral>
    ListarHorariosMedicinaGeneral(
        int idUsuario,
        DateTime fecha)
        {
            if (idUsuario <= 0)
            {
                throw new ArgumentException(
                    "El usuario no es válido.");
            }

            if (fecha.Date < DateTime.Today)
            {
                throw new ArgumentException(
                    "No puede consultar horarios de una fecha anterior a hoy.");
            }

            return citasDAL.ListarHorariosMedicinaGeneral(
                idUsuario,
                fecha);
        }
        public void AgendarCita(
            int idUsuario,
            int idMedico,
            DateTime fecha,
            TimeSpan hora)
        {
            if (idUsuario <= 0 || idMedico <= 0)
            {
                throw new ArgumentException(
                    "La información de la cita no es válida.");
            }

            if (fecha.Date < DateTime.Today)
            {
                throw new ArgumentException(
                    "No puede agendar una cita en una fecha pasada.");
            }

            if (hora.Hours < 7 || hora.Hours > 15 ||
                hora.Minutes != 0 || hora.Seconds != 0)
            {
                throw new ArgumentException(
                    "La cita debe estar entre las 7:00 a. m. y las 4:00 p. m.");
            }

            citasDAL.AgendarCita(
                idUsuario,
                idMedico,
                fecha,
                hora);
        }
        public void CancelarCitasVencidas()
        {
            citasDAL.CancelarCitasVencidas();
        }
    }
}
using DAL;
using EDL;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class RecepcionistaControlCitasBLL
    {
        private readonly RecepcionistaControlCitasDAL citasDAL =
            new RecepcionistaControlCitasDAL();

        public List<string> ListarEspecialidades(int idHospital)
        {
            if (idHospital < 0)
            {
                throw new ArgumentException(
                    "El hospital seleccionado no es válido.");
            }

            return citasDAL.ListarEspecialidades(idHospital);
        }

        public List<CitaControlRecepcion> ListarCitasProximas(
            int idHospital,
            string especialidad,
            DateTime? fecha)
        {
            if (idHospital < 0)
            {
                throw new ArgumentException(
                    "El hospital seleccionado no es válido.");
            }

            if (fecha.HasValue &&
                fecha.Value.Date < DateTime.Today)
            {
                throw new ArgumentException(
                    "Solo puede consultar citas de hoy o futuras.");
            }

            return citasDAL.ListarCitasProximas(
                idHospital,
                especialidad,
                fecha);
        }

        public void CancelarCita(int idCita)
        {
            if (idCita <= 0)
            {
                throw new ArgumentException(
                    "La cita seleccionada no es válida.");
            }

            bool cancelada = citasDAL.CancelarCita(idCita);

            if (!cancelada)
            {
                throw new InvalidOperationException(
                    "La cita ya fue atendida o cancelada.");
            }
        }

        public void CancelarCitasVencidas()
        {
            citasDAL.CancelarCitasVencidas();
        }
    }
}
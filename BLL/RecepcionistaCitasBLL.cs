using DAL;
using EDL;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL
{
    public class RecepcionistaCitasBLL
    {
        private readonly RecepcionistaCitasDAL citasDAL =
            new RecepcionistaCitasDAL();

        public PacienteCitaRecepcion BuscarPaciente(
            string cedula)
        {
            string cedulaNormalizada =
                NormalizarCedula(cedula);

            if (cedulaNormalizada.Length != 9)
            {
                throw new ArgumentException(
                    "La cédula debe contener 9 dígitos.");
            }

            return citasDAL.BuscarPacientePorCedula(
                cedulaNormalizada);
        }

        public List<string> ListarEspecialidades(
            int idHospital)
        {
            ValidarHospital(idHospital);

            return citasDAL.ListarEspecialidades(idHospital);
        }

        public List<HorarioCitaRecepcion>
            ListarHorariosDisponibles(
                int idHospital,
                string especialidad,
                DateTime fecha)
        {
            ValidarHospital(idHospital);
            ValidarFecha(fecha);

            if (string.IsNullOrWhiteSpace(especialidad))
            {
                throw new ArgumentException(
                    "Debe seleccionar una especialidad.");
            }

            return citasDAL.ListarHorariosDisponibles(
                idHospital,
                especialidad.Trim(),
                fecha);
        }
        public UltimaCitaEspecialidadRecepcion
        ObtenerUltimaCitaEspecialidad(
            int idPaciente,
            string especialidad)
            {
                if (idPaciente <= 0)
                {
                    throw new ArgumentException(
                        "El paciente no es válido.");
                }

                if (string.IsNullOrWhiteSpace(especialidad))
                {
                    throw new ArgumentException(
                        "Debe seleccionar una especialidad.");
                }

                return citasDAL.ObtenerUltimaCitaEspecialidad(
                    idPaciente,
                    especialidad.Trim());
            }
        public void RegistrarCita(
            int idPaciente,
            int idMedico,
            DateTime fecha,
            TimeSpan hora)
        {
            if (idPaciente <= 0 || idMedico <= 0)
            {
                throw new ArgumentException(
                    "La información de la cita no es válida.");
            }

            ValidarFecha(fecha);

            if (hora.Hours < 7 || hora.Hours > 15 ||
                hora.Minutes != 0 || hora.Seconds != 0)
            {
                throw new ArgumentException(
                    "La cita debe iniciar entre las 7:00 a. m. y las 3:00 p. m.");
            }

            citasDAL.RegistrarCita(
                idPaciente,
                idMedico,
                fecha,
                hora);
        }

        public void CancelarCitasVencidas()
        {
            citasDAL.CancelarCitasVencidas();
        }

        private string NormalizarCedula(string cedula)
        {
            StringBuilder resultado = new StringBuilder();

            foreach (char caracter in cedula ?? "")
            {
                if (char.IsDigit(caracter))
                {
                    resultado.Append(caracter);
                }
            }

            return resultado.ToString();
        }

        private void ValidarHospital(int idHospital)
        {
            if (idHospital <= 0)
            {
                throw new ArgumentException(
                    "El hospital no es válido.");
            }
        }

        private void ValidarFecha(DateTime fecha)
        {
            if (fecha.Date < DateTime.Today)
            {
                throw new ArgumentException(
                    "No puede agendar una cita en una fecha pasada.");
            }
        }
    }
}
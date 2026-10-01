using DAL;
using EDL;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL
{
    public class RecepcionistaPagoBLL
    {
        private readonly RecepcionistaPagoDAL pagoDAL =
            new RecepcionistaPagoDAL();

        public PacientePagoRecepcion BuscarPaciente(string cedula)
        {
            string cedulaNormalizada =
                NormalizarCedula(cedula);

            if (cedulaNormalizada.Length != 9)
            {
                throw new ArgumentException(
                    "La cédula debe contener 9 dígitos.");
            }

            return pagoDAL.BuscarPacientePorCedula(
                cedulaNormalizada);
        }

        public List<CitaPendientePagoRecepcion>
            ObtenerPendientes(int idPaciente)
        {
            ValidarPaciente(idPaciente);

            return pagoDAL.ObtenerPendientes(idPaciente);
        }

        public CitaPendientePagoRecepcion ObtenerPendiente(
            int idPaciente,
            int idCita)
        {
            ValidarPaciente(idPaciente);
            ValidarCita(idCita);

            return pagoDAL.ObtenerPendiente(
                idPaciente,
                idCita);
        }

        public void RegistrarPago(
            int idPaciente,
            int idCita,
            string metodoPago)
        {
            ValidarPaciente(idPaciente);
            ValidarCita(idCita);

            if (metodoPago != "Efectivo" &&
                metodoPago != "Tarjeta" &&
                metodoPago != "Sinpe")
            {
                throw new ArgumentException(
                    "Debe seleccionar un método de pago válido.");
            }

            pagoDAL.RegistrarPago(
                idPaciente,
                idCita,
                metodoPago);
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

        private void ValidarPaciente(int idPaciente)
        {
            if (idPaciente <= 0)
            {
                throw new ArgumentException(
                    "El paciente seleccionado no es válido.");
            }
        }

        private void ValidarCita(int idCita)
        {
            if (idCita <= 0)
            {
                throw new ArgumentException(
                    "La cita seleccionada no es válida.");
            }
        }
    }
}
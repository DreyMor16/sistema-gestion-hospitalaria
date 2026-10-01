using DAL;
using EDL;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class PagoBLL
    {
        private readonly PagoDAL pagoDAL = new PagoDAL();

        public List<PagoPendientePaciente> ObtenerPendientes(
            int idUsuario)
        {
            ValidarUsuario(idUsuario);
            return pagoDAL.ObtenerPendientes(idUsuario);
        }

        public PagoPendientePaciente ObtenerPendiente(
            int idUsuario,
            int idCita)
        {
            ValidarUsuario(idUsuario);

            if (idCita <= 0)
            {
                throw new ArgumentException(
                    "La cita seleccionada no es válida.");
            }

            return pagoDAL.ObtenerPendiente(
                idUsuario,
                idCita);
        }

        public void RegistrarPago(
            int idUsuario,
            int idCita,
            string metodoPago)
        {
            ValidarUsuario(idUsuario);

            if (idCita <= 0)
            {
                throw new ArgumentException(
                    "La cita seleccionada no es válida.");
            }

            if (metodoPago != "Efectivo" &&
                metodoPago != "Tarjeta" &&
                metodoPago != "Sinpe")
            {
                throw new ArgumentException(
                    "Debe seleccionar un método de pago válido.");
            }

            pagoDAL.RegistrarPago(
                idUsuario,
                idCita,
                metodoPago);
        }

        public List<HistorialPagoPaciente> ListarHistorial(
            int idUsuario)
        {
            ValidarUsuario(idUsuario);
            return pagoDAL.ListarHistorial(idUsuario);
        }

        public decimal ObtenerTotalPagado(
            int idUsuario,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            ValidarUsuario(idUsuario);

            if (fechaInicio.Date > fechaFin.Date)
            {
                throw new ArgumentException(
                    "La fecha inicial no puede ser posterior a la fecha final.");
            }

            return pagoDAL.ObtenerTotalPagado(
                idUsuario,
                fechaInicio,
                fechaFin);
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

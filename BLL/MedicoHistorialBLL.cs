using DAL;
using EDL;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class MedicoHistorialBLL
    {
        private readonly MedicoHistorialDAL historialDAL =
            new MedicoHistorialDAL();

        public List<CitaHistorialMedico> ListarHistorial(
            int idUsuario,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            string filtro,
            string estado)
        {
            ValidarDatos(
                idUsuario,
                fechaInicio,
                fechaFin,
                estado);

            return historialDAL.ListarHistorial(
                idUsuario,
                fechaInicio,
                fechaFin,
                filtro,
                estado);
        }

        public ResumenHistorialMedico ObtenerResumen(
            int idUsuario,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            string filtro,
            string estado)
        {
            ValidarDatos(
                idUsuario,
                fechaInicio,
                fechaFin,
                estado);

            return historialDAL.ObtenerResumen(
                idUsuario,
                fechaInicio,
                fechaFin,
                filtro,
                estado);
        }

        private void ValidarDatos(
            int idUsuario,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            string estado)
        {
            if (idUsuario <= 0)
            {
                throw new ArgumentException(
                    "El usuario no es válido.");
            }

            if (fechaInicio.HasValue &&
                fechaFin.HasValue &&
                fechaInicio.Value.Date > fechaFin.Value.Date)
            {
                throw new ArgumentException(
                    "La fecha inicial no puede ser posterior a la fecha final.");
            }

            if (estado != "Todos" &&
                estado != "Finalizada" &&
                estado != "Cancelada")
            {
                throw new ArgumentException(
                    "El estado seleccionado no es válido.");
            }
        }
    }
}
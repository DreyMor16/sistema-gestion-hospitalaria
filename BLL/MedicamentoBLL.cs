using DAL;
using EDL;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class MedicamentoBLL
    {
        private readonly MedicamentoDAL medicamentoDAL =
            new MedicamentoDAL();

        public List<Medicamento> Listar()
        {
            return medicamentoDAL.Listar();
        }

        public List<Medicamento> Buscar(string filtro)
        {
            if (string.IsNullOrWhiteSpace(filtro))
            {
                return medicamentoDAL.Listar();
            }

            return medicamentoDAL.Buscar(filtro);
        }

        public Medicamento ObtenerPorId(int idMedicamento)
        {
            if (idMedicamento <= 0)
            {
                throw new ArgumentException(
                    "El medicamento seleccionado no es válido.");
            }

            return medicamentoDAL.ObtenerPorId(idMedicamento);
        }

        public void Registrar(Medicamento medicamento)
        {
            ValidarMedicamento(medicamento);
            medicamentoDAL.Insertar(medicamento);
        }

        public void Actualizar(Medicamento medicamento)
        {
            if (medicamento.IdMedicamento <= 0)
            {
                throw new ArgumentException(
                    "El medicamento seleccionado no es válido.");
            }

            ValidarMedicamento(medicamento);
            medicamentoDAL.Actualizar(medicamento);
        }

        public void Eliminar(int idMedicamento)
        {
            if (idMedicamento <= 0)
            {
                throw new ArgumentException(
                    "El medicamento seleccionado no es válido.");
            }

            medicamentoDAL.Eliminar(idMedicamento);
        }

        public void AgregarStock(int idHospital, int idMedicamento, int cantidad)
        {
            if (idHospital <= 0)
                throw new Exception("Debe seleccionar un hospital.");

            if (idMedicamento <= 0)
                throw new Exception("Debe seleccionar un medicamento.");

            if (cantidad <= 0)
                throw new Exception("La cantidad a agregar debe ser mayor que cero.");

            medicamentoDAL.AgregarStock(idHospital, idMedicamento, cantidad);
        }

        public List<InventarioMedicamentoDetalle>
            ObtenerInventarioUltimos30Dias(int idHospital)
        {
            if (idHospital <= 0)
            {
                throw new ArgumentException(
                    "Debe seleccionar un hospital.");
            }

            return medicamentoDAL.ObtenerInventarioUltimos30Dias(idHospital);
        }

        private void ValidarMedicamento(Medicamento medicamento)
        {
            if (medicamento == null)
            {
                throw new ArgumentException(
                    "Los datos del medicamento son requeridos.");
            }

            medicamento.Nombre = medicamento.Nombre == null
                ? ""
                : medicamento.Nombre.Trim();

            medicamento.Descripcion = medicamento.Descripcion == null
                ? ""
                : medicamento.Descripcion.Trim();

            if (string.IsNullOrWhiteSpace(medicamento.Nombre))
            {
                throw new ArgumentException(
                    "El nombre del medicamento es requerido.");
            }

            if (medicamento.Nombre.Length > 100)
            {
                throw new ArgumentException(
                    "El nombre no puede superar los 100 caracteres.");
            }

            if (medicamento.Descripcion.Length > 300)
            {
                throw new ArgumentException(
                    "La descripción no puede superar los 300 caracteres.");
            }

            if (medicamento.CostoUnitario <= 0)
            {
                throw new ArgumentException(
                    "El costo unitario debe ser mayor que cero.");
            }

            if (medicamento.CostoUnitario > 99999999.99m)
            {
                throw new ArgumentException(
                    "El costo unitario es demasiado alto.");
            }
        }
        public List<MedicamentoHospitalDetalle> BuscarPorHospital(
        int idHospital,
        string filtro)
            {
                if (idHospital < 0)
                {
                    throw new ArgumentException("El hospital seleccionado no es válido.");
                }

                return medicamentoDAL.BuscarPorHospital(idHospital, filtro);
            }
    }
}
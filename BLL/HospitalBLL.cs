using DAL;
using EDL;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class HospitalBLL
    {
        private readonly HospitalDAL hospitalDAL = new HospitalDAL();

        public List<Hospital> Listar()
        {
            return hospitalDAL.Listar();
        }

        public Hospital ObtenerPorId(int idHospital)
        {
            if (idHospital <= 0)
            {
                throw new ArgumentException("El hospital seleccionado no es válido.");
            }

            return hospitalDAL.ObtenerPorId(idHospital);
        }

        public int Registrar(Hospital hospital)
        {
            ValidarHospital(hospital);
            return hospitalDAL.Insertar(hospital);
        }

        public void Actualizar(Hospital hospital)
        {
            if (hospital.IdHospital <= 0)
            {
                throw new ArgumentException("El hospital seleccionado no es válido.");
            }

            ValidarHospital(hospital);
            hospitalDAL.Actualizar(hospital);
        }

        public void Eliminar(int idHospital)
        {
            if (idHospital <= 0)
            {
                throw new ArgumentException("El hospital seleccionado no es válido.");
            }

            hospitalDAL.Eliminar(idHospital);
        }

        private void ValidarHospital(Hospital hospital)
        {
            if (hospital == null)
            {
                throw new ArgumentException("Los datos del hospital son requeridos.");
            }

            hospital.Nombre = hospital.Nombre == null ? "" : hospital.Nombre.Trim();
            hospital.Direccion = hospital.Direccion == null ? "" : hospital.Direccion.Trim();
            hospital.Telefono = hospital.Telefono == null ? "" : hospital.Telefono.Trim();

            if (string.IsNullOrWhiteSpace(hospital.Nombre))
            {
                throw new ArgumentException("El nombre del hospital es requerido.");
            }

            if (string.IsNullOrWhiteSpace(hospital.Direccion))
            {
                throw new ArgumentException("La dirección es requerida.");
            }

            if (string.IsNullOrWhiteSpace(hospital.Telefono))
            {
                throw new ArgumentException("El teléfono es requerido.");
            }

            if (hospital.Nombre.Length > 100)
            {
                throw new ArgumentException("El nombre no puede superar los 100 caracteres.");
            }

            if (hospital.Direccion.Length > 200)
            {
                throw new ArgumentException("La dirección no puede superar los 200 caracteres.");
            }

            if (hospital.Telefono.Length > 20)
            {
                throw new ArgumentException("El teléfono no puede superar los 20 caracteres.");
            }
        }
        public List<Hospital> Buscar(string filtro)
        {
            if (string.IsNullOrWhiteSpace(filtro))
            {
                return hospitalDAL.Listar();
            }

            return hospitalDAL.Buscar(filtro);
        }
    }
}
using DAL;
using EDL;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BLL
{
    public class PacienteBLL
    {
        private readonly PacienteDAL pacienteDAL = new PacienteDAL();

        public List<PacienteDetalle> Buscar(string filtro, int idHospital)
        {
            return pacienteDAL.Buscar(filtro, idHospital);
        }

        public PacienteDetalle ObtenerPorId(int idPaciente)
        {
            if (idPaciente <= 0)
            {
                throw new ArgumentException("El paciente seleccionado no es válido.");
            }

            return pacienteDAL.ObtenerPorId(idPaciente);
        }

        public void Registrar(PacienteDetalle paciente)
        {
            ValidarPaciente(paciente);
            pacienteDAL.Insertar(paciente);
        }

        public void Actualizar(PacienteDetalle paciente)
        {
            if (paciente.IdPaciente <= 0 || paciente.IdPersona <= 0)
            {
                throw new ArgumentException("El paciente seleccionado no es válido.");
            }

            ValidarPaciente(paciente);
            pacienteDAL.Actualizar(paciente);
        }

        private void ValidarPaciente(PacienteDetalle paciente)
        {
            if (paciente == null)
            {
                throw new ArgumentException("Los datos del paciente son requeridos.");
            }

            paciente.Nombre = paciente.Nombre == null ? "" : paciente.Nombre.Trim();
            paciente.Apellido = paciente.Apellido == null ? "" : paciente.Apellido.Trim();
            paciente.Telefono = paciente.Telefono == null ? "" : paciente.Telefono.Trim();
            paciente.Correo = paciente.Correo == null ? "" : paciente.Correo.Trim();
            paciente.Cedula = paciente.Cedula == null ? "" : paciente.Cedula.Trim();
            paciente.Direccion = paciente.Direccion == null ? "" : paciente.Direccion.Trim();

            if (string.IsNullOrWhiteSpace(paciente.Nombre))
            {
                throw new ArgumentException("El nombre es requerido.");
            }

            if (string.IsNullOrWhiteSpace(paciente.Apellido))
            {
                throw new ArgumentException("El apellido es requerido.");
            }

            if (string.IsNullOrWhiteSpace(paciente.Cedula))
            {
                throw new ArgumentException("La cédula es requerida.");
            }

            if (!Regex.IsMatch(paciente.Cedula, @"^\d{9}$"))
            {
                throw new ArgumentException(
                    "La cédula debe contener exactamente 9 dígitos numéricos.");
            }

            if (string.IsNullOrWhiteSpace(paciente.Telefono))
            {
                throw new ArgumentException("El teléfono es requerido.");
            }

            if (!Regex.IsMatch(paciente.Telefono, @"^\d{8}$"))
            {
                throw new ArgumentException(
                    "El teléfono debe contener exactamente 8 dígitos numéricos.");
            }

            if (paciente.IdHospital <= 0)
            {
                throw new ArgumentException("Debe seleccionar un hospital.");
            }

            if (paciente.FechaNacimiento == DateTime.MinValue ||
                paciente.FechaNacimiento.Date > DateTime.Today)
            {
                throw new ArgumentException("La fecha de nacimiento no es válida.");
            }

            if (paciente.Genero != "Masculino" &&
                paciente.Genero != "Femenino" &&
                paciente.Genero != "Otro")
            {
                throw new ArgumentException("Debe seleccionar un género válido.");
            }

            if (!string.IsNullOrWhiteSpace(paciente.Correo) &&
                !paciente.Correo.Contains("@"))
            {
                throw new ArgumentException("El correo no tiene un formato válido.");
            }
        }
        public string ObtenerNombreHospitalPorUsuario(int idUsuario)
        {
            if (idUsuario <= 0)
            {
                throw new ArgumentException("El usuario no es válido.");
            }

            return pacienteDAL.ObtenerNombreHospitalPorUsuario(idUsuario);
        }
    }
}

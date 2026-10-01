using DAL;
using EDL;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BLL
{
    public class MedicoBLL
    {
        private readonly MedicoDAL medicoDAL = new MedicoDAL();

        public List<MedicoDetalle> Buscar(string filtro, int idHospital)
        {
            return medicoDAL.Buscar(filtro, idHospital);
        }

        public MedicoDetalle ObtenerPorId(int idMedico)
        {
            if (idMedico <= 0)
            {
                throw new ArgumentException("El médico seleccionado no es válido.");
            }

            return medicoDAL.ObtenerPorId(idMedico);
        }

        public void Registrar(MedicoDetalle medico)
        {
            ValidarMedico(medico, true);
            medico.Password = PasswordHasher.Hash(medico.Password);
            medicoDAL.Insertar(medico);
        }

        public void Actualizar(MedicoDetalle medico)
        {
            if (medico.IdMedico <= 0 || medico.IdPersona <= 0)
            {
                throw new ArgumentException("El médico seleccionado no es válido.");
            }

            ValidarMedico(medico, false);

            if (!string.IsNullOrWhiteSpace(medico.Password))
            {
                medico.Password = PasswordHasher.Hash(medico.Password);
            }

            medicoDAL.Actualizar(medico);
        }

        private void ValidarMedico(MedicoDetalle medico, bool passwordRequerida)
        {
            if (medico == null)
            {
                throw new ArgumentException("Los datos del médico son requeridos.");
            }

            medico.Nombre = medico.Nombre == null ? "" : medico.Nombre.Trim();
            medico.Apellido = medico.Apellido == null ? "" : medico.Apellido.Trim();
            medico.Telefono = medico.Telefono == null ? "" : medico.Telefono.Trim();
            medico.Correo = medico.Correo == null ? "" : medico.Correo.Trim();
            medico.Cedula = medico.Cedula == null ? "" : medico.Cedula.Trim();
            medico.Especialidad = medico.Especialidad == null ? "" : medico.Especialidad.Trim();
            medico.NombreUsuario = medico.NombreUsuario == null
                ? ""
                : medico.NombreUsuario.Trim();

            medico.Password = medico.Password ?? "";

            if (string.IsNullOrWhiteSpace(medico.Nombre))
            {
                throw new ArgumentException("El nombre es requerido.");
            }

            if (string.IsNullOrWhiteSpace(medico.Apellido))
            {
                throw new ArgumentException("El apellido es requerido.");
            }

            if (!Regex.IsMatch(medico.Cedula, @"^\d{9}$"))
            {
                throw new ArgumentException(
                    "La cédula debe contener exactamente 9 dígitos numéricos.");
            }

            if (!Regex.IsMatch(medico.Telefono, @"^\d{8}$"))
            {
                throw new ArgumentException(
                    "El teléfono debe contener exactamente 8 dígitos numéricos.");
            }

            if (string.IsNullOrWhiteSpace(medico.NombreUsuario))
            {
                throw new ArgumentException("El usuario es requerido.");
            }

            if (!Regex.IsMatch(medico.NombreUsuario, @"^[a-zA-Z0-9._-]{3,50}$"))
            {
                throw new ArgumentException(
                    "El usuario debe tener entre 3 y 50 caracteres; use letras, números, punto, guion o guion bajo.");
            }

            if (passwordRequerida && string.IsNullOrWhiteSpace(medico.Password))
            {
                throw new ArgumentException("La contraseña es requerida.");
            }

            if (!string.IsNullOrWhiteSpace(medico.Password) &&
                medico.Password.Length < 6)
            {
                throw new ArgumentException(
                    "La contraseña debe contener al menos 6 caracteres.");
            }

            if (string.IsNullOrWhiteSpace(medico.Especialidad))
            {
                throw new ArgumentException("La especialidad es requerida.");
            }

            if (medico.IdHospital <= 0)
            {
                throw new ArgumentException("Debe seleccionar un hospital.");
            }

            if (!string.IsNullOrWhiteSpace(medico.Correo) &&
                !medico.Correo.Contains("@"))
            {
                throw new ArgumentException("El correo no tiene un formato válido.");
            }
        }
        public void Eliminar(int idMedico)
        {
            if (idMedico <= 0)
            {
                throw new ArgumentException("El médico seleccionado no es válido.");
            }

            medicoDAL.Eliminar(idMedico);
        }
    }
}

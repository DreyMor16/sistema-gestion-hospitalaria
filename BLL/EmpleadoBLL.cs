using DAL;
using EDL;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BLL
{
    public class EmpleadoBLL
    {
        private readonly EmpleadoDAL empleadoDAL = new EmpleadoDAL();

        public List<EmpleadoDetalle> Buscar(string filtro, string puesto)
        {
            return empleadoDAL.Buscar(filtro, puesto);
        }

        public EmpleadoDetalle ObtenerPorId(int idEmpleado)
        {
            if (idEmpleado <= 0)
            {
                throw new ArgumentException("El empleado seleccionado no es válido.");
            }

            return empleadoDAL.ObtenerPorId(idEmpleado);
        }

        public void Registrar(EmpleadoDetalle empleado)
        {
            ValidarEmpleado(empleado, true);
            empleado.Password = PasswordHasher.Hash(empleado.Password);
            empleadoDAL.Insertar(empleado);
        }

        public void Actualizar(EmpleadoDetalle empleado)
        {
            if (empleado.IdEmpleado <= 0 || empleado.IdPersona <= 0)
            {
                throw new ArgumentException("El empleado seleccionado no es válido.");
            }

            ValidarEmpleado(empleado, false);

            if (!string.IsNullOrWhiteSpace(empleado.Password))
            {
                empleado.Password = PasswordHasher.Hash(empleado.Password);
            }

            empleadoDAL.Actualizar(empleado);
        }

        public void Eliminar(int idEmpleado)
        {
            if (idEmpleado <= 0)
            {
                throw new ArgumentException("El empleado seleccionado no es válido.");
            }

            empleadoDAL.Eliminar(idEmpleado);
        }

        private void ValidarEmpleado(
            EmpleadoDetalle empleado,
            bool passwordRequerida)
        {
            if (empleado == null)
            {
                throw new ArgumentException("Los datos del empleado son requeridos.");
            }

            empleado.Nombre = empleado.Nombre == null ? "" : empleado.Nombre.Trim();
            empleado.Apellido = empleado.Apellido == null ? "" : empleado.Apellido.Trim();
            empleado.Telefono = empleado.Telefono == null ? "" : empleado.Telefono.Trim();
            empleado.Correo = empleado.Correo == null ? "" : empleado.Correo.Trim();
            empleado.Cedula = empleado.Cedula == null ? "" : empleado.Cedula.Trim();
            empleado.NombreUsuario = empleado.NombreUsuario == null
                ? ""
                : empleado.NombreUsuario.Trim();

            empleado.Password = empleado.Password ?? "";

            if (string.IsNullOrWhiteSpace(empleado.Nombre))
            {
                throw new ArgumentException("El nombre es requerido.");
            }

            if (string.IsNullOrWhiteSpace(empleado.Apellido))
            {
                throw new ArgumentException("El apellido es requerido.");
            }

            if (!Regex.IsMatch(empleado.Cedula, @"^\d{9}$"))
            {
                throw new ArgumentException(
                    "La cédula debe contener exactamente 9 dígitos numéricos.");
            }

            if (!Regex.IsMatch(empleado.Telefono, @"^\d{8}$"))
            {
                throw new ArgumentException(
                    "El teléfono debe contener exactamente 8 dígitos numéricos.");
            }

            if (string.IsNullOrWhiteSpace(empleado.NombreUsuario))
            {
                throw new ArgumentException("El usuario es requerido.");
            }

            if (!Regex.IsMatch(
                empleado.NombreUsuario,
                @"^[a-zA-Z0-9._-]{3,50}$"))
            {
                throw new ArgumentException(
                    "El usuario debe tener entre 3 y 50 caracteres; use letras, números, punto, guion o guion bajo.");
            }

            if (passwordRequerida &&
                string.IsNullOrWhiteSpace(empleado.Password))
            {
                throw new ArgumentException("La contraseña es requerida.");
            }

            if (!string.IsNullOrWhiteSpace(empleado.Password) &&
                empleado.Password.Length < 6)
            {
                throw new ArgumentException(
                    "La contraseña debe contener al menos 6 caracteres.");
            }

            if (empleado.Puesto != "Administrador" &&
                empleado.Puesto != "Recepcionista")
            {
                throw new ArgumentException(
                    "Debe seleccionar Administrador o Recepcionista.");
            }

            if (!string.IsNullOrWhiteSpace(empleado.Correo) &&
                !empleado.Correo.Contains("@"))
            {
                throw new ArgumentException("El correo no tiene un formato válido.");
            }
        }
    }
}

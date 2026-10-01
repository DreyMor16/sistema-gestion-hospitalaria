using System;
using EDL;
using DAL;
using System.Text.RegularExpressions;

namespace BLL
{
    public class UsuarioBLL
    {
        private UsuarioDAL _usuarioDAL = new UsuarioDAL();

        public Usuario ValidarLogin(string nombreUsuario, string password)
        {
            // Regla de negocio básica: Validar que no envíen campos vacíos
            if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("El usuario y la contraseña son requeridos.");
            }

            Usuario usuario =
                _usuarioDAL.ObtenerPorNombreUsuario(nombreUsuario.Trim());

            if (usuario == null ||
                !PasswordHasher.Verify(password, usuario.Password))
            {
                return null;
            }

            // La credencial nunca se conserva en la sesión web.
            usuario.Password = null;
            return usuario;
        }
        public EstadoRegistroPaciente VerificarCedulaPaciente(
    string cedula)
        {
            cedula = (cedula ?? "").Trim();

            if (!Regex.IsMatch(cedula, @"^\d{9}$"))
            {
                throw new ArgumentException(
                    "La cédula debe contener exactamente 9 dígitos.");
            }

            return _usuarioDAL.VerificarCedulaPaciente(cedula);
        }

        public void CrearUsuarioParaPaciente(
            RegistroPaciente registro)
        {
            if (registro == null)
            {
                throw new ArgumentException(
                    "Los datos de registro son requeridos.");
            }

            registro.Cedula = (registro.Cedula ?? "").Trim();
            registro.NombreUsuario =
                (registro.NombreUsuario ?? "").Trim();

            registro.Password = registro.Password ?? "";

            if (!Regex.IsMatch(registro.Cedula, @"^\d{9}$"))
            {
                throw new ArgumentException(
                    "La cédula debe contener exactamente 9 dígitos.");
            }

            if (!Regex.IsMatch(
                registro.NombreUsuario,
                @"^[a-zA-Z0-9._-]{3,50}$"))
            {
                throw new ArgumentException(
                    "El usuario debe tener entre 3 y 50 caracteres y no puede tener espacios.");
            }

            if (registro.Password.Length < 6)
            {
                throw new ArgumentException(
                    "La contraseña debe tener al menos 6 caracteres.");
            }

            EstadoRegistroPaciente estado =
                _usuarioDAL.VerificarCedulaPaciente(registro.Cedula);

            if (!estado.PacienteRegistrado)
            {
                throw new ArgumentException(
                    "No existe un paciente registrado con esa cédula.");
            }

            if (estado.YaTieneUsuario)
            {
                throw new ArgumentException(
                    "Esta cédula ya tiene una cuenta asociada.");
            }

            registro.Password = PasswordHasher.Hash(registro.Password);
            _usuarioDAL.CrearUsuarioParaPaciente(registro);
        }
        public PerfilUsuario ObtenerPerfil(int idUsuario)
        {
            if (idUsuario <= 0)
            {
                throw new ArgumentException("El usuario no es válido.");
            }

            return _usuarioDAL.ObtenerPerfil(idUsuario);
        }

        public void ActualizarPerfil(PerfilUsuario perfil)
        {
            if (perfil == null || perfil.IdUsuario <= 0)
            {
                throw new ArgumentException("Los datos del perfil no son válidos.");
            }

            perfil.NombreUsuario = (perfil.NombreUsuario ?? "").Trim();
            perfil.Telefono = (perfil.Telefono ?? "").Trim();
            perfil.Correo = (perfil.Correo ?? "").Trim();
            perfil.NuevaPassword = (perfil.NuevaPassword ?? "").Trim();

            if (!Regex.IsMatch(
                perfil.NombreUsuario,
                @"^[a-zA-Z0-9._-]{3,50}$"))
            {
                throw new ArgumentException(
                    "El usuario debe tener entre 3 y 50 caracteres, sin espacios.");
            }

            if (!string.IsNullOrWhiteSpace(perfil.Telefono) &&
                !Regex.IsMatch(perfil.Telefono, @"^\d{8}$"))
            {
                throw new ArgumentException(
                    "El teléfono debe contener exactamente 8 dígitos.");
            }

            if (!string.IsNullOrWhiteSpace(perfil.Correo) &&
                !Regex.IsMatch(
                    perfil.Correo,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new ArgumentException(
                    "Ingrese un correo electrónico válido.");
            }

            if (!string.IsNullOrWhiteSpace(perfil.NuevaPassword) &&
                perfil.NuevaPassword.Length < 6)
            {
                throw new ArgumentException(
                    "La nueva contraseña debe tener al menos 6 caracteres.");
            }

            if (!string.IsNullOrWhiteSpace(perfil.NuevaPassword))
            {
                perfil.NuevaPassword =
                    PasswordHasher.Hash(perfil.NuevaPassword);
            }

            _usuarioDAL.ActualizarPerfil(perfil);
        }
    }
}

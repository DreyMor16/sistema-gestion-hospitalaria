using System;

namespace EDL
{
    public class PacienteDetalle
    {
        public int IdPaciente { get; set; }
        public int IdPersona { get; set; }

        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Cedula { get; set; }
        public string NombreUsuario { get; set; }

        public DateTime FechaNacimiento { get; set; }
        public string Genero { get; set; }
        public string Direccion { get; set; }

        public int IdHospital { get; set; }
        public string NombreHospital { get; set; }
    }
}
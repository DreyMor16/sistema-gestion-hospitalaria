using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDL
{
    public class MedicoDetalle
    {
        public int IdMedico { get; set; }
        public int IdPersona { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Cedula { get; set; }
        public string NombreUsuario { get; set; }
        public string Password { get; set; }
        public string Especialidad { get; set; }
        public int IdHospital { get; set; }
        public string NombreHospital { get; set; }
    }
}

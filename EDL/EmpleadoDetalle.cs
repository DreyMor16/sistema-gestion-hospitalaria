using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDL
{
    public class EmpleadoDetalle
    {
        public int IdEmpleado { get; set; }
        public int IdPersona { get; set; }
        public int IdUsuario { get; set; }

        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Cedula { get; set; }

        public string NombreUsuario { get; set; }
        public string Password { get; set; }

        public string Puesto { get; set; }
    }
}

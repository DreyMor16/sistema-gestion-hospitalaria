using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDL
{
    public class PerfilUsuario
    {
        public int IdUsuario { get; set; } // Solo interno.

        public string NombreUsuario { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }

        // Se usa solo si el usuario desea cambiar su contraseña.
        public string NuevaPassword { get; set; }
    }
}

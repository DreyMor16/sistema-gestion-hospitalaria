using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDL
{
    public class RegistroPaciente
    {
        public string Cedula { get; set; }
        public string NombreUsuario { get; set; }
        public string Password { get; set; }
    }

    public class EstadoRegistroPaciente
    {
        public bool PacienteRegistrado { get; set; }
        public bool YaTieneUsuario { get; set; }
    }
}
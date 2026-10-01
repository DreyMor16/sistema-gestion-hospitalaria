using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDL
{
    public class Paciente
    {
        public int IdPaciente { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Genero { get; set; }
        public string Direccion { get; set; }
        public int IdPersona { get; set; }
        public int IdHospital { get; set; }
    }
}

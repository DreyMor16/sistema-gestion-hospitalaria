using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDL
{
    public class Cita
    {
        public int IdCita { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public string Diagnostico { get; set; }
        public string Estado { get; set; }
        public int IdPaciente { get; set; }
        public int IdMedico { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDL
{
    public class Tratamiento
    {
        public int IdTratamiento { get; set; }
        public string Descripcion { get; set; }
        public decimal Costo { get; set; }
        public int IdCita { get; set; }
    }
}

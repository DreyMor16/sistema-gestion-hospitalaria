using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDL
{
    public class Prescripcion
    {
        public int IdPrescripcion { get; set; }
        public int IdTratamiento { get; set; }
        public int IdMedicamento { get; set; }
        public int Cantidad { get; set; }
        public string Dosis { get; set; }
    }
}

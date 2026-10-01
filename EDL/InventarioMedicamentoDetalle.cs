using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDL
{
    public class InventarioMedicamentoDetalle
    {
        public int IdMedicamento { get; set; }
        public string Medicamento { get; set; }
        public int StockActual { get; set; }
        public int TotalPrescrito30Dias { get; set; }
    }
}
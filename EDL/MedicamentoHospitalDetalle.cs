using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDL
{
    public class MedicamentoHospitalDetalle
    {
        public int IdMedicamento { get; set; } // Interno, no se muestra.

        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal CostoUnitario { get; set; }

        public string NombreHospital { get; set; }
        public int CantidadStock { get; set; }
    }
}
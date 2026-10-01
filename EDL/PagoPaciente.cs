using System;

namespace EDL
{
    public class PagoPendientePaciente
    {
        public int IdCita { get; set; } // Interno, no se muestra.

        public string DetalleTratamiento { get; set; }
        public decimal MontoPendiente { get; set; }
        public DateTime FechaCita { get; set; }
    }

    public class HistorialPagoPaciente
    {
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; }
        public string DetalleTratamiento { get; set; }
        public DateTime FechaCita { get; set; }
    }
}

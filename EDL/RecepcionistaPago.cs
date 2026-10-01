using System;

namespace EDL
{
    public class PacientePagoRecepcion
    {
        public int IdPaciente { get; set; } // Interno.

        public string NombreCompleto { get; set; }
        public string Cedula { get; set; }
        public string Hospital { get; set; }
    }

    public class CitaPendientePagoRecepcion
    {
        public int IdCita { get; set; } // Interno.

        public DateTime FechaCita { get; set; }
        public string DetalleTratamientos { get; set; }
        public decimal MontoPendiente { get; set; }
    }
}
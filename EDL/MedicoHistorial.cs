using System;

namespace EDL
{
    public class CitaHistorialMedico
    {
        public int IdCita { get; set; } // Interno.

        public DateTime Fecha { get; set; }
        public string Hora { get; set; }
        public string Estado { get; set; }

        public string NombrePaciente { get; set; }
        public string Cedula { get; set; }
        public string Diagnostico { get; set; }
    }

    public class ResumenHistorialMedico
    {
        public int TotalFinalizadas { get; set; }
        public int TotalCanceladas { get; set; }
    }
}
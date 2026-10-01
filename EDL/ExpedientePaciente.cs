using System;
using System.Collections.Generic;

namespace EDL
{
    public class ResumenExpedientePaciente
    {
        public int TotalCitasPasadas { get; set; }
        public int TotalTratamientos { get; set; }
        public int TotalMedicamentos { get; set; }
    }

    public class CitaExpedientePaciente
    {
        public int IdCita { get; set; } // Uso interno, nunca se muestra.

        public DateTime Fecha { get; set; }
        public string Hora { get; set; }
        public string Estado { get; set; }
        public string Diagnostico { get; set; }
        public string Medico { get; set; }
        public string Especialidad { get; set; }
    }

    public class TratamientoMedicamentoPaciente
    {
        public DateTime FechaCita { get; set; }
        public string Tratamiento { get; set; }
        public decimal CostoTratamiento { get; set; }
        public string Medicamento { get; set; }
        public string Dosis { get; set; }
        public string Cantidad { get; set; }
    }
}
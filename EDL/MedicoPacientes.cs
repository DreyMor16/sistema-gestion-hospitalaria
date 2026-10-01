using System;

namespace EDL
{
    public class ContextoMedico
    {
        public int IdMedico { get; set; }
        public int IdHospital { get; set; }
    }

    public class PacienteAtendidoMedico
    {
        public int IdPaciente { get; set; } // Interno.

        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }

        public DateTime FechaUltimaAtencion { get; set; }
    }

    public class DetalleAtencionMedico
    {
        public string NombrePaciente { get; set; }
        public string Cedula { get; set; }

        public DateTime Fecha { get; set; }
        public string Hora { get; set; }
        public string Diagnostico { get; set; }
    }

    public class TratamientoPrescripcionMedico
    {
        public string Tratamiento { get; set; }
        public decimal CostoTratamiento { get; set; }

        public string Medicamento { get; set; }
        public string Dosis { get; set; }
        public string Cantidad { get; set; }
    }
    public class CitaAtendidaMedico
    {
        public int IdCita { get; set; } // Interno.

        public string NombrePaciente { get; set; }
        public string Cedula { get; set; }

        public DateTime Fecha { get; set; }
        public string Hora { get; set; }
        public string Diagnostico { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace EDL
{
    public class PacienteCitaRecepcion
    {
        public int IdPaciente { get; set; }
        public int IdHospital { get; set; }

        public string NombreCompleto { get; set; }
        public string Cedula { get; set; }
        public string Hospital { get; set; }
    }

    public class MedicoDisponibleCitaRecepcion
    {
        public int IdMedico { get; set; }
        public string NombreCompleto { get; set; }
    }

    public class HorarioCitaRecepcion
    {
        public string Especialidad { get; set; }
        public string Hospital { get; set; }
        public string Hora { get; set; }

        public List<MedicoDisponibleCitaRecepcion>
            MedicosDisponibles
        { get; set; }
    }

    public class UltimaCitaEspecialidadRecepcion
    {
        public int IdCita { get; set; }

        public DateTime Fecha { get; set; }
        public string Hora { get; set; }
        public string Estado { get; set; }
        public string Diagnostico { get; set; }
        public string Medico { get; set; }
        public string Hospital { get; set; }

        public List<TratamientoUltimaCitaRecepcion>
            Tratamientos
        { get; set; }
    }

    public class TratamientoUltimaCitaRecepcion
    {
        public string Descripcion { get; set; }
        public decimal Costo { get; set; }
        public string Medicamentos { get; set; }
    }
}
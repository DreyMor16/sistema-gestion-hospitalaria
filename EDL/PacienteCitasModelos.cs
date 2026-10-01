using System;

namespace EDL
{
    public class CitaProximaPaciente
    {
        public DateTime Fecha { get; set; }
        public string Hora { get; set; }
        public string Estado { get; set; }
        public string Medico { get; set; }
        public string Especialidad { get; set; }
        public string Hospital { get; set; }
    }

    public class HorarioDisponibleGeneral
    {
        public int IdMedico { get; set; }
        public string Hora { get; set; }
        public string Medico { get; set; }
        public string Hospital { get; set; }
    }
}
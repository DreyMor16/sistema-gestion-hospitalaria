using System;

namespace EDL
{
    public class CitaControlRecepcion
    {
        public int IdCita { get; set; }

        public DateTime Fecha { get; set; }
        public string Hora { get; set; }
        public string Estado { get; set; }
        public string Diagnostico { get; set; }

        public string Paciente { get; set; }
        public string Cedula { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }

        public string Medico { get; set; }
        public string Especialidad { get; set; }
        public string Hospital { get; set; }
    }
}
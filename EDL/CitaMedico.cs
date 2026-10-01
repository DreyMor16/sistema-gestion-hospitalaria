using System;

namespace EDL
{
    public class CitaProximaAtencion
    {
        public int IdCita { get; set; } // Interno.

        public DateTime Fecha { get; set; }
        public string Hora { get; set; }

        public string NombrePaciente { get; set; }
        public string Cedula { get; set; }
    }

    public class DetalleCitaAtencion
    {
        public int IdCita { get; set; } // Interno.

        public string NombrePaciente { get; set; }
        public string Cedula { get; set; }
        public string Hospital { get; set; }

        public DateTime Fecha { get; set; }
        public string Hora { get; set; }
        public string Diagnostico { get; set; }
    }

    public class TratamientoAtencion
    {
        public int IdTratamiento { get; set; } // Interno.

        public string Descripcion { get; set; }
        public decimal Costo { get; set; }
    }

    public class MedicamentoDisponibleAtencion
    {
        public int IdMedicamento { get; set; } // Interno.

        public string Nombre { get; set; }
        public int StockDisponible { get; set; }
        public decimal CostoUnitario { get; set; }

        public string Detalle
        {
            get
            {
                return Nombre + " · " + StockDisponible +
                    " unidades · ₡ " + CostoUnitario.ToString("N2");
            }
        }
    }

    public class PrescripcionAtencion
    {
        public string Tratamiento { get; set; }
        public string Medicamento { get; set; }
        public string Dosis { get; set; }
        public int Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}

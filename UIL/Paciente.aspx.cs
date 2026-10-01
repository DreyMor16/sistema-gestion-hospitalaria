using BLL;
using EDL;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace UIL
{
    public partial class Paciente : System.Web.UI.Page
    {
        private readonly PacienteCitasBLL citasBLL =
            new PacienteCitasBLL();

        private readonly PagoBLL pagoBLL =
            new PagoBLL();

        private readonly PacienteBLL pacienteBLL =
            new PacienteBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            Usuario usuario = Session["UsuarioActivo"] as Usuario;

            if (usuario == null || usuario.Rol != "Paciente")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarResumenPanel(usuario.IdUsuario);
            }
        }

        private void CargarResumenPanel(int idUsuario)
        {
            CargarProximaCita(idUsuario);
            CargarPagosPendientes(idUsuario);
            CargarHospital(idUsuario);

            lblExpediente.Text = "Activo";
        }

        private void CargarProximaCita(int idUsuario)
        {
            try
            {
                List<CitaProximaPaciente> citas =
                    citasBLL.ListarCitasProximas(idUsuario);

                if (citas.Count > 0)
                {
                    CitaProximaPaciente proxima = citas[0];

                    lblProximaCita.Text =
                        proxima.Fecha.ToString("dd/MM/yyyy");

                    lblDetalleProxima.Text =
                        proxima.Hora + " · " + proxima.Medico;
                }
                else
                {
                    lblProximaCita.Text = "Sin citas";
                    lblDetalleProxima.Text =
                        "Puede consultar horarios disponibles";
                }
            }
            catch
            {
                lblProximaCita.Text = "—";
                lblDetalleProxima.Text =
                    "No fue posible consultar las citas";
            }
        }

        private void CargarPagosPendientes(int idUsuario)
        {
            try
            {
                List<PagoPendientePaciente> pendientes =
                    pagoBLL.ObtenerPendientes(idUsuario);

                decimal montoPendiente = 0;

                foreach (PagoPendientePaciente pendiente in pendientes)
                {
                    montoPendiente += pendiente.MontoPendiente;
                }

                lblPagosPendientes.Text =
                    pendientes.Count.ToString();

                lblDetallePagos.Text = pendientes.Count == 0
                    ? "No tiene pagos pendientes"
                    : "₡ " + montoPendiente.ToString(
                        "N2",
                        new CultureInfo("es-CR"));
            }
            catch
            {
                lblPagosPendientes.Text = "—";
                lblDetallePagos.Text =
                    "No fue posible consultar los pagos";
            }
        }

        private void CargarHospital(int idUsuario)
        {
            try
            {
                string nombreHospital =
                    pacienteBLL.ObtenerNombreHospitalPorUsuario(
                        idUsuario);

                lblHospital.Text =
                    string.IsNullOrWhiteSpace(nombreHospital)
                    ? "No asignado"
                    : nombreHospital;
            }
            catch
            {
                lblHospital.Text = "—";
            }
        }
    }
}
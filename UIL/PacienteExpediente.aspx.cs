using BLL;
using EDL;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace UIL
{
    public partial class PacienteExpediente : System.Web.UI.Page
    {
        private readonly PacienteExpedienteBLL expedienteBLL =
            new PacienteExpedienteBLL();

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
                CargarExpediente(usuario.IdUsuario);
            }
        }

        protected void gvCitas_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (e.CommandName != "VerDetalle")
            {
                return;
            }

            int indice;

            if (!int.TryParse(e.CommandArgument.ToString(), out indice))
            {
                return;
            }

            int idCita =
                Convert.ToInt32(gvCitas.DataKeys[indice].Value);

            Usuario usuario = Session["UsuarioActivo"] as Usuario;

            if (usuario != null)
            {
                CargarDetalleCita(usuario.IdUsuario, idCita);
            }
        }

        protected void btnCerrarDetalle_Click(
            object sender,
            EventArgs e)
        {
            pnlDetalleCita.Visible = false;
        }

        private void CargarExpediente(int idUsuario)
        {
            try
            {
                ResumenExpedientePaciente resumen =
                    expedienteBLL.ObtenerResumen(idUsuario);

                lblTotalCitas.Text =
                    resumen.TotalCitasPasadas.ToString();

                lblTotalTratamientos.Text =
                    resumen.TotalTratamientos.ToString();

                lblTotalMedicamentos.Text =
                    resumen.TotalMedicamentos.ToString();

                gvCitas.DataSource =
                    expedienteBLL.ListarCitasPasadas(idUsuario);

                gvCitas.DataBind();

                gvMisTratamientos.DataSource =
                    expedienteBLL.ListarTratamientosYMedicamentos(
                        idUsuario);

                gvMisTratamientos.DataBind();
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible cargar su expediente personal.",
                    true);
            }
        }

        private void CargarDetalleCita(
            int idUsuario,
            int idCita)
        {
            try
            {
                CitaExpedientePaciente cita =
                    expedienteBLL.ObtenerCita(idUsuario, idCita);

                if (cita == null)
                {
                    MostrarMensaje(
                        "No se encontró la cita seleccionada.",
                        true);

                    pnlDetalleCita.Visible = false;
                    return;
                }

                lblDetalleFecha.Text =
                    cita.Fecha.ToString("dd/MM/yyyy");

                lblDetalleHora.Text = cita.Hora;

                lblDetalleMedico.Text =
                    cita.Medico + " · " + cita.Especialidad;

                lblDetalleEstado.Text = cita.Estado;
                lblDetalleEstado.CssClass =
                    ClaseEstado(cita.Estado);

                litDiagnostico.Text =
                    Server.HtmlEncode(
                        string.IsNullOrWhiteSpace(cita.Diagnostico)
                        ? "No se registró un diagnóstico para esta cita."
                        : cita.Diagnostico);

                List<TratamientoMedicamentoPaciente> tratamientos =
                    expedienteBLL.ListarTratamientosDeCita(
                        idUsuario,
                        idCita);

                gvDetalleTratamientos.DataSource = tratamientos;
                gvDetalleTratamientos.DataBind();

                pnlDetalleCita.Visible = true;
                pnlMensaje.Visible = false;
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible cargar el detalle de la cita.",
                    true);
            }
        }

        protected string ClaseEstado(string estado)
        {
            if (estado == "Finalizada")
            {
                return "status-finalizada";
            }

            if (estado == "Cancelada")
            {
                return "status-cancelada";
            }

            return "status-proceso";
        }

        private void MostrarMensaje(string mensaje, bool esError)
        {
            pnlMensaje.Visible = true;
            lblMensaje.Text = mensaje;

            pnlMensaje.CssClass = esError
                ? "portal-alert portal-alert-error"
                : "portal-alert portal-alert-success";

            pnlMensaje.Attributes["role"] = "alert";
        }
    }
}

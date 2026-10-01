using BLL;
using EDL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;

namespace UIL
{
    public partial class MedicoHistorial : System.Web.UI.Page
    {
        private readonly MedicoHistorialBLL historialBLL =
            new MedicoHistorialBLL();

        private readonly MedicoPacientesBLL pacientesBLL =
            new MedicoPacientesBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            Usuario usuario = Session["UsuarioActivo"] as Usuario;

            if (usuario == null || usuario.Rol != "Medico")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarHistorial(usuario.IdUsuario);
            }
        }

        protected void btnConsultar_Click(
            object sender,
            EventArgs e)
        {
            Usuario usuario = Session["UsuarioActivo"] as Usuario;

            if (usuario != null)
            {
                CargarHistorial(usuario.IdUsuario);
            }
        }

        protected void gvHistorial_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (e.CommandName != "VerAtencion")
            {
                return;
            }

            int indice;

            if (!int.TryParse(e.CommandArgument.ToString(), out indice))
            {
                return;
            }

            int idCita =
                Convert.ToInt32(
                    gvHistorial.DataKeys[indice].Value);

            Usuario usuario = Session["UsuarioActivo"] as Usuario;

            if (usuario != null)
            {
                CargarDetalleAtencion(
                    usuario.IdUsuario,
                    idCita);
            }
        }

        protected void btnCerrarDetalle_Click(
            object sender,
            EventArgs e)
        {
            pnlDetalleCita.Visible = false;
        }

        private void CargarHistorial(int idUsuario)
        {
            try
            {
                DateTime? fechaInicio =
                    ObtenerFechaOpcional(
                        txtFechaInicio.Text,
                        "La fecha inicial no es válida.");

                DateTime? fechaFin =
                    ObtenerFechaOpcional(
                        txtFechaFin.Text,
                        "La fecha final no es válida.");

                string estado = ddlEstado.SelectedValue;

                List<CitaHistorialMedico> historial =
                    historialBLL.ListarHistorial(
                        idUsuario,
                        fechaInicio,
                        fechaFin,
                        txtFiltro.Text,
                        estado);

                ResumenHistorialMedico resumen =
                    historialBLL.ObtenerResumen(
                        idUsuario,
                        fechaInicio,
                        fechaFin,
                        txtFiltro.Text,
                        estado);

                gvHistorial.DataSource = historial;
                gvHistorial.DataBind();

                lblFinalizadas.Text =
                    resumen.TotalFinalizadas.ToString();

                lblCanceladas.Text =
                    resumen.TotalCanceladas.ToString();

                pnlDetalleCita.Visible = false;
                pnlMensaje.Visible = false;
            }
            catch (ArgumentException ex)
            {
                gvHistorial.DataSource = null;
                gvHistorial.DataBind();

                MostrarMensaje(ex.Message, true);
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible consultar el historial de citas.",
                    true);
            }
        }

        private void CargarDetalleAtencion(
            int idUsuario,
            int idCita)
        {
            try
            {
                DetalleAtencionMedico detalle =
                    pacientesBLL.ObtenerDetalleAtencion(
                        idUsuario,
                        idCita);

                if (detalle == null)
                {
                    MostrarMensaje(
                        "No se encontró el detalle de la cita.",
                        true);

                    return;
                }

                lblPacienteDetalle.Text =
                    detalle.NombrePaciente +
                    " · Cédula: " +
                    detalle.Cedula;

                lblFechaDetalle.Text =
                    detalle.Fecha.ToString("dd/MM/yyyy");

                lblHoraDetalle.Text = detalle.Hora;

                litDiagnostico.Text =
                    Server.HtmlEncode(
                        String.IsNullOrWhiteSpace(
                            detalle.Diagnostico)
                        ? "No se registró un diagnóstico."
                        : detalle.Diagnostico);

                List<TratamientoPrescripcionMedico> tratamientos =
                    pacientesBLL.ListarTratamientosPrescripciones(
                        idUsuario,
                        idCita);

                gvTratamientos.DataSource = tratamientos;
                gvTratamientos.DataBind();

                pnlDetalleCita.Visible = true;
                pnlMensaje.Visible = false;
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible cargar el detalle clínico.",
                    true);
            }
        }

        protected string ClaseEstado(string estado)
        {
            if (estado == "Finalizada")
            {
                return "status-finalizada";
            }

            return "status-cancelada";
        }

        private DateTime? ObtenerFechaOpcional(
            string fechaTexto,
            string mensajeError)
        {
            if (String.IsNullOrWhiteSpace(fechaTexto))
            {
                return null;
            }

            DateTime fecha;

            if (!DateTime.TryParseExact(
                fechaTexto,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out fecha))
            {
                throw new ArgumentException(mensajeError);
            }

            return fecha;
        }

        private void MostrarMensaje(
            string mensaje,
            bool esError)
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

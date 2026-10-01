using BLL;
using EDL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls;

namespace UIL
{
    public partial class PacientePagos : System.Web.UI.Page
    {
        private readonly PagoBLL pagoBLL = new PagoBLL();

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
                txtFechaInicio.Text =
                    new DateTime(
                        DateTime.Today.Year,
                        DateTime.Today.Month,
                        1).ToString("yyyy-MM-dd");

                txtFechaFin.Text =
                    DateTime.Today.ToString("yyyy-MM-dd");

                CargarInformacion(usuario.IdUsuario);
            }
        }

        protected void btnCalcularTotal_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                Usuario usuario = Session["UsuarioActivo"] as Usuario;

                CargarTotalPagado(usuario.IdUsuario);
                pnlMensaje.Visible = false;
            }
            catch (ArgumentException ex)
            {
                MostrarMensaje(ex.Message, true);
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible calcular el total pagado.",
                    true);
            }
        }

        protected void gvPendientes_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (e.CommandName != "SeleccionarPago")
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
                    gvPendientes.DataKeys[indice].Value);

            Usuario usuario = Session["UsuarioActivo"] as Usuario;

            CargarTratamientoParaPago(
                usuario.IdUsuario,
                idCita);
        }

        protected void btnConfirmarPago_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                int idCita;

                if (!int.TryParse(
                    hdnIdCita.Value,
                    out idCita))
                {
                    throw new ArgumentException(
                        "La cita seleccionada no es válida.");
                }

                Usuario usuario = Session["UsuarioActivo"] as Usuario;

                pagoBLL.RegistrarPago(
                    usuario.IdUsuario,
                    idCita,
                    ddlMetodoPago.SelectedValue);

                pnlConfirmarPago.Visible = false;

                MostrarMensaje(
                    "El pago completo de la cita fue registrado correctamente.",
                    false);

                CargarInformacion(usuario.IdUsuario);
            }
            catch (ArgumentException ex)
            {
                MostrarMensaje(ex.Message, true);
            }
            catch (SqlException)
            {
                MostrarMensaje(
                    "La cita ya fue pagada o no está disponible.",
                    true);
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible registrar el pago.",
                    true);
            }
        }

        protected void btnCancelarPago_Click(
            object sender,
            EventArgs e)
        {
            pnlConfirmarPago.Visible = false;
            hdnIdCita.Value = "";
            ddlMetodoPago.SelectedValue = "0";
        }

        private void CargarInformacion(int idUsuario)
        {
            CargarPendientes(idUsuario);
            CargarHistorial(idUsuario);
            CargarTotalPagado(idUsuario);
        }

        private void CargarPendientes(int idUsuario)
        {
            List<PagoPendientePaciente> pendientes =
                pagoBLL.ObtenerPendientes(idUsuario);

            gvPendientes.DataSource = pendientes;
            gvPendientes.DataBind();
        }

        private void CargarHistorial(int idUsuario)
        {
            List<HistorialPagoPaciente> historial =
                pagoBLL.ListarHistorial(idUsuario);

            gvHistorialPagos.DataSource = historial;
            gvHistorialPagos.DataBind();
        }

        private void CargarTotalPagado(int idUsuario)
        {
            DateTime fechaInicio;
            DateTime fechaFin;

            if (!DateTime.TryParseExact(
                txtFechaInicio.Text,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out fechaInicio))
            {
                throw new ArgumentException(
                    "La fecha inicial no es válida.");
            }

            if (!DateTime.TryParseExact(
                txtFechaFin.Text,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out fechaFin))
            {
                throw new ArgumentException(
                    "La fecha final no es válida.");
            }

            decimal total = pagoBLL.ObtenerTotalPagado(
                idUsuario,
                fechaInicio,
                fechaFin);

            lblTotalPagado.Text =
                "₡ " + total.ToString(
                    "N2",
                    new CultureInfo("es-CR"));
        }

        private void CargarTratamientoParaPago(
            int idUsuario,
            int idCita)
        {
            try
            {
                PagoPendientePaciente pendiente =
                    pagoBLL.ObtenerPendiente(
                        idUsuario,
                        idCita);

                if (pendiente == null)
                {
                    MostrarMensaje(
                        "La cita ya fue pagada o no está disponible.",
                        true);

                    return;
                }

                hdnIdCita.Value = pendiente.IdCita.ToString();

                lblTratamientoSeleccionado.Text =
                    "Cita del " +
                    pendiente.FechaCita.ToString("dd/MM/yyyy") +
                    ": " + pendiente.DetalleTratamiento;

                lblMontoSeleccionado.Text =
                    "₡ " + pendiente.MontoPendiente.ToString(
                        "N2",
                        new CultureInfo("es-CR"));

                ddlMetodoPago.SelectedValue = "0";
                pnlConfirmarPago.Visible = true;
                pnlMensaje.Visible = false;
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible cargar la cita seleccionada.",
                    true);
            }
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

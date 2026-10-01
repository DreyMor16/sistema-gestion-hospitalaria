using BLL;
using EDL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls;

namespace UIL
{
    public partial class RecepcionistaPago : System.Web.UI.Page
    {
        private readonly RecepcionistaPagoBLL pagoBLL =
            new RecepcionistaPagoBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            Usuario usuario = Session["UsuarioActivo"] as Usuario;

            if (usuario == null || usuario.Rol != "Recepcionista")
            {
                Response.Redirect("Login.aspx");
                return;
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                LimpiarBusquedaAnterior();

                PacientePagoRecepcion paciente =
                    pagoBLL.BuscarPaciente(txtCedula.Text);

                if (paciente == null)
                {
                    MostrarMensaje(
                        "No se encontró un paciente registrado con esa cédula.",
                        true);

                    return;
                }

                hdnIdPaciente.Value =
                    paciente.IdPaciente.ToString();

                lblPaciente.Text =
                    paciente.NombreCompleto;

                lblCedulaPaciente.Text =
                    paciente.Cedula;

                lblHospitalPaciente.Text =
                    paciente.Hospital;

                CargarPendientes(paciente.IdPaciente);

                pnlPaciente.Visible = true;
                pnlMensaje.Visible = false;
            }
            catch (ArgumentException ex)
            {
                MostrarMensaje(ex.Message, true);
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible buscar la información del paciente.",
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

            if (!int.TryParse(
                e.CommandArgument.ToString(),
                out indice))
            {
                return;
            }

            int idCita =
                Convert.ToInt32(
                    gvPendientes.DataKeys[indice].Value);

            CargarCitaParaPago(
                ObtenerIdPaciente(),
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

                int idPaciente = ObtenerIdPaciente();

                pagoBLL.RegistrarPago(
                    idPaciente,
                    idCita,
                    ddlMetodoPago.SelectedValue);

                pnlConfirmarPago.Visible = false;
                hdnIdCita.Value = "";

                CargarPendientes(idPaciente);

                MostrarMensaje(
                    "El pago de la cita fue registrado correctamente.",
                    false);
            }
            catch (ArgumentException ex)
            {
                MostrarMensaje(ex.Message, true);
            }
            catch (SqlException)
            {
                MostrarMensaje(
                    "La cita ya fue pagada o no tiene tratamientos pendientes.",
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

        private void CargarPendientes(int idPaciente)
        {
            List<CitaPendientePagoRecepcion> pendientes =
                pagoBLL.ObtenerPendientes(idPaciente);

            gvPendientes.DataSource = pendientes;
            gvPendientes.DataBind();
        }

        private void CargarCitaParaPago(
            int idPaciente,
            int idCita)
        {
            try
            {
                CitaPendientePagoRecepcion pendiente =
                    pagoBLL.ObtenerPendiente(
                        idPaciente,
                        idCita);

                if (pendiente == null)
                {
                    MostrarMensaje(
                        "La cita ya fue pagada o no tiene tratamientos pendientes.",
                        true);

                    return;
                }

                hdnIdCita.Value =
                    pendiente.IdCita.ToString();

                lblCitaSeleccionada.Text =
                    "Cita del " +
                    pendiente.FechaCita.ToString("dd/MM/yyyy") +
                    ": " +
                    pendiente.DetalleTratamientos;

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

        private int ObtenerIdPaciente()
        {
            int idPaciente;

            if (!int.TryParse(
                hdnIdPaciente.Value,
                out idPaciente) || idPaciente <= 0)
            {
                throw new ArgumentException(
                    "Debe buscar un paciente antes de registrar un pago.");
            }

            return idPaciente;
        }

        private void LimpiarBusquedaAnterior()
        {
            pnlPaciente.Visible = false;
            pnlConfirmarPago.Visible = false;

            hdnIdPaciente.Value = "";
            hdnIdCita.Value = "";
            ddlMetodoPago.SelectedValue = "0";
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

using BLL;
using EDL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;

namespace UIL
{
    public partial class RecepcionistaControlCitas :
        System.Web.UI.Page
    {
        private readonly RecepcionistaControlCitasBLL citasBLL =
            new RecepcionistaControlCitasBLL();

        private readonly HospitalBLL hospitalBLL =
            new HospitalBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            Usuario usuario = Session["UsuarioActivo"] as Usuario;

            if (usuario == null || usuario.Rol != "Recepcionista")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            citasBLL.CancelarCitasVencidas();

            txtFecha.Attributes["min"] =
                DateTime.Today.ToString("yyyy-MM-dd");

            if (!IsPostBack)
            {
                CargarHospitales();
                CargarEspecialidades(0);
                CargarCitas();
            }
        }

        protected void ddlHospital_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            try
            {
                int idHospital = ObtenerIdHospital();

                CargarEspecialidades(idHospital);
                CargarCitas();

                pnlMensaje.Visible = false;
            }
            catch (ArgumentException ex)
            {
                MostrarMensaje(ex.Message, true);
            }
        }

        protected void btnConsultar_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                CargarCitas();
                pnlMensaje.Visible = false;
            }
            catch (ArgumentException ex)
            {
                MostrarMensaje(ex.Message, true);
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible consultar las citas.",
                    true);
            }
        }

        protected void btnLimpiar_Click(
            object sender,
            EventArgs e)
        {
            ddlHospital.SelectedValue = "0";
            txtFecha.Text = "";

            CargarEspecialidades(0);
            CargarCitas();

            pnlMensaje.Visible = false;
        }

        protected void gvCitas_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Cancelar")
            {
                return;
            }

            try
            {
                int idCita;

                if (!int.TryParse(
                    e.CommandArgument.ToString(),
                    out idCita))
                {
                    throw new ArgumentException(
                        "La cita seleccionada no es válida.");
                }

                citasBLL.CancelarCita(idCita);

                CargarCitas();

                MostrarMensaje(
                    "La cita fue cancelada y el horario del médico quedó disponible.",
                    false);
            }
            catch (ArgumentException ex)
            {
                MostrarMensaje(ex.Message, true);
            }
            catch (InvalidOperationException ex)
            {
                MostrarMensaje(ex.Message, true);
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible cancelar la cita.",
                    true);
            }
        }

        private void CargarHospitales()
        {
            List<Hospital> hospitales = hospitalBLL.Listar();

            ddlHospital.DataSource = hospitales;
            ddlHospital.DataTextField = "Nombre";
            ddlHospital.DataValueField = "IdHospital";
            ddlHospital.DataBind();

            ddlHospital.Items.Insert(
                0,
                new ListItem("Todos los hospitales", "0"));
        }

        private void CargarEspecialidades(int idHospital)
        {
            List<string> especialidades =
                citasBLL.ListarEspecialidades(idHospital);

            ddlEspecialidad.DataSource = especialidades;
            ddlEspecialidad.DataBind();

            ddlEspecialidad.Items.Insert(
                0,
                new ListItem(
                    "Todas las especialidades",
                    "0"));
        }

        private void CargarCitas()
        {
            int idHospital = ObtenerIdHospital();

            string especialidad =
                ddlEspecialidad.SelectedValue == "0"
                ? null
                : ddlEspecialidad.SelectedValue;

            DateTime? fecha = ObtenerFechaOpcional();

            List<CitaControlRecepcion> citas =
                citasBLL.ListarCitasProximas(
                    idHospital,
                    especialidad,
                    fecha);

            gvCitas.DataSource = citas;
            gvCitas.DataBind();
        }

        private int ObtenerIdHospital()
        {
            int idHospital;

            if (!int.TryParse(
                ddlHospital.SelectedValue,
                out idHospital) || idHospital < 0)
            {
                throw new ArgumentException(
                    "El hospital seleccionado no es válido.");
            }

            return idHospital;
        }

        private DateTime? ObtenerFechaOpcional()
        {
            if (string.IsNullOrWhiteSpace(txtFecha.Text))
            {
                return null;
            }

            DateTime fecha;

            if (!DateTime.TryParseExact(
                txtFecha.Text,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out fecha))
            {
                throw new ArgumentException(
                    "La fecha seleccionada no es válida.");
            }

            if (fecha.Date < DateTime.Today)
            {
                throw new ArgumentException(
                    "Solo puede consultar citas de hoy o futuras.");
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

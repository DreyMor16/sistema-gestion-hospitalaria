using BLL;
using EDL;
using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace UIL
{
    public partial class RecepcionistaPaciente : System.Web.UI.Page
    {
        private readonly PacienteBLL pacienteBLL =
            new PacienteBLL();

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

            txtFechaNacimiento.Attributes["max"] =
                DateTime.Today.ToString("yyyy-MM-dd");

            if (!IsPostBack)
            {
                CargarHospitales();
            }
        }

        protected void btnRegistrar_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                PacienteDetalle paciente =
                    CrearPacienteDesdeFormulario();

                pacienteBLL.Registrar(paciente);

                LimpiarFormulario();

                MostrarMensaje(
                    "El paciente fue registrado correctamente. " +
                    "Podrá crear su usuario desde el login.",
                    false);
            }
            catch (ArgumentException ex)
            {
                MostrarMensaje(ex.Message, true);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    if (ex.Message.ToLower().Contains("cedula"))
                    {
                        MostrarMensaje(
                            "La cédula ya está registrada.",
                            true);
                    }
                    else if (ex.Message.ToLower().Contains("correo"))
                    {
                        MostrarMensaje(
                            "El correo ya está registrado.",
                            true);
                    }
                    else
                    {
                        MostrarMensaje(
                            "Ya existe un registro con datos únicos iguales.",
                            true);
                    }
                }
                else
                {
                    MostrarMensaje(
                        "No fue posible registrar el paciente.",
                        true);
                }
            }
            catch
            {
                MostrarMensaje(
                    "Ocurrió un error inesperado al registrar el paciente.",
                    true);
            }
        }

        protected void btnLimpiar_Click(
            object sender,
            EventArgs e)
        {
            LimpiarFormulario();
            pnlMensaje.Visible = false;
        }

        private void CargarHospitales()
        {
            ddlHospital.DataSource = hospitalBLL.Listar();
            ddlHospital.DataTextField = "Nombre";
            ddlHospital.DataValueField = "IdHospital";
            ddlHospital.DataBind();

            ddlHospital.Items.Insert(
                0,
                new ListItem(
                    "Seleccione un hospital",
                    "0"));
        }

        private PacienteDetalle CrearPacienteDesdeFormulario()
        {
            int idHospital;
            DateTime fechaNacimiento;

            int.TryParse(
                ddlHospital.SelectedValue,
                out idHospital);

            DateTime.TryParse(
                txtFechaNacimiento.Text,
                out fechaNacimiento);

            return new PacienteDetalle
            {
                Nombre = txtNombre.Text,
                Apellido = txtApellido.Text,
                Cedula = txtCedula.Text,
                Telefono = txtTelefono.Text,
                Correo = txtCorreo.Text,
                FechaNacimiento = fechaNacimiento,
                Genero = ddlGenero.SelectedValue,
                Direccion = txtDireccion.Text,
                IdHospital = idHospital
            };
        }

        private void LimpiarFormulario()
        {
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtCedula.Text = "";
            txtTelefono.Text = "";
            txtCorreo.Text = "";
            txtFechaNacimiento.Text = "";
            txtDireccion.Text = "";

            ddlGenero.SelectedValue = "0";
            ddlHospital.SelectedValue = "0";
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

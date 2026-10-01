using BLL;
using EDL;
using System;
using System.Data.SqlClient;

namespace UIL
{
    public partial class RegistroPaciente : System.Web.UI.Page
    {
        private readonly UsuarioBLL usuarioBLL = new UsuarioBLL();

        private string CedulaVerificada
        {
            get
            {
                return ViewState["CedulaVerificada"] == null
                    ? ""
                    : ViewState["CedulaVerificada"].ToString();
            }

            set
            {
                ViewState["CedulaVerificada"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lnkIrLogin.Visible = true;
            }
        }

        protected void btnVerificarCedula_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                EstadoRegistroPaciente estado =
                    usuarioBLL.VerificarCedulaPaciente(
                        txtCedula.Text);

                if (!estado.PacienteRegistrado)
                {
                    MostrarMensaje(
                        "No encontramos un paciente registrado con esa cédula. Debe acudir al centro hospitalario para completar su información.",
                        true);

                    return;
                }

                if (estado.YaTieneUsuario)
                {
                    MostrarMensaje(
                        "Esta cédula ya tiene una cuenta activa. Utilice el inicio de sesión.",
                        true);

                    return;
                }

                CedulaVerificada = txtCedula.Text.Trim();

                txtCedula.Enabled = false;
                btnVerificarCedula.Visible = false;

                pnlCredenciales.Visible = true;

                MostrarMensaje(
                    "Paciente encontrado. Ahora puede crear su usuario y contraseña.",
                    false);
            }
            catch (ArgumentException ex)
            {
                MostrarMensaje(ex.Message, true);
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible verificar la cédula.",
                    true);
            }
        }

        protected void btnCrearCuenta_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CedulaVerificada))
                {
                    throw new ArgumentException(
                        "Primero debe verificar su cédula.");
                }

                if (txtNuevaPassword.Text !=
                    txtConfirmarPassword.Text)
                {
                    throw new ArgumentException(
                        "La confirmación de contraseña no coincide.");
                }

                EDL.RegistroPaciente registro = new EDL.RegistroPaciente
                {
                    Cedula = CedulaVerificada,
                    NombreUsuario = txtNuevoUsuario.Text,
                    Password = txtNuevaPassword.Text
                };

                usuarioBLL.CrearUsuarioParaPaciente(registro);

                pnlCedula.Visible = false;
                pnlCredenciales.Visible = false;

                MostrarMensaje(
                    "Su cuenta fue activada correctamente. Ya puede iniciar sesión.",
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
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    MostrarMensaje(
                        "El nombre de usuario ya está en uso. Seleccione otro.",
                        true);
                }
                else
                {
                    MostrarMensaje(
                        "No fue posible crear la cuenta.",
                        true);
                }
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible crear la cuenta.",
                    true);
            }
        }

        private void MostrarMensaje(string mensaje, bool esError)
        {
            pnlMensaje.Visible = true;
            lblMensaje.Text = mensaje;

            pnlMensaje.CssClass = esError
                ? "register-alert register-alert-error"
                : "register-alert";

            pnlMensaje.Attributes["role"] = "alert";
        }
    }
}

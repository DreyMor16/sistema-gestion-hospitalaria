using BLL;
using EDL;
using System;

namespace UIL
{
    public partial class Login : System.Web.UI.Page
    {
        private UsuarioBLL _usuarioBLL = new UsuarioBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlError.Visible = false;
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsuario.Text.Trim();
            string password = txtPassword.Text.Trim();

            try
            {
                Usuario usuarioValidado =
                    _usuarioBLL.ValidarLogin(username, password);

                if (usuarioValidado == null)
                {
                    MostrarMensajeError("Usuario o contraseña incorrectos.");
                    return;
                }

                Session["UsuarioActivo"] = usuarioValidado;

                switch (usuarioValidado.Rol)
                {
                    case "Paciente":
                        Response.Redirect("Paciente.aspx");
                        break;

                    case "Medico":
                        Response.Redirect("Medico.aspx");
                        break;

                    case "Administrador":
                        Response.Redirect("Administrador.aspx");
                        break;

                    case "Recepcionista":
                        Response.Redirect("Recepcionista.aspx");
                        break;

                    default:
                        Session.Clear();
                        MostrarMensajeError(
                            "Este usuario no tiene un rol válido asignado.");
                        break;
                }
            }
            catch (ArgumentException ex)
            {
                MostrarMensajeError("Error: " + ex.Message);
            }
            catch (Exception)
            {
                MostrarMensajeError(
                    "No fue posible iniciar sesión. Inténtelo nuevamente.");
            }
        }

        private void MostrarMensajeError(string mensaje)
        {
            pnlError.Visible = true;
            lblError.Text = mensaje;
            pnlError.CssClass =
                "login-alert d-flex align-items-center gap-2";
            pnlError.Attributes["role"] = "alert";
        }
    }
}

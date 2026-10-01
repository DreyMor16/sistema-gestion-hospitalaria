using BLL;
using EDL;
using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace UIL
{
    public partial class MiInformacion : System.Web.UI.Page
    {
        private readonly UsuarioBLL usuarioBLL = new UsuarioBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            Usuario usuarioActivo = Session["UsuarioActivo"] as Usuario;

            if (usuarioActivo == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarPerfil(usuarioActivo.IdUsuario);
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                Usuario usuarioActivo = Session["UsuarioActivo"] as Usuario;

                if (usuarioActivo == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                if (txtNuevaPassword.Text != txtConfirmarPassword.Text)
                {
                    throw new ArgumentException(
                        "La confirmación de la contraseña no coincide.");
                }

                PerfilUsuario perfil = new PerfilUsuario
                {
                    IdUsuario = usuarioActivo.IdUsuario,
                    NombreUsuario = txtUsuario.Text,
                    Telefono = txtTelefono.Text,
                    Correo = txtCorreo.Text,
                    NuevaPassword = txtNuevaPassword.Text
                };

                usuarioBLL.ActualizarPerfil(perfil);

                // Actualiza el usuario guardado en la sesión.
                usuarioActivo.NombreUsuario = perfil.NombreUsuario;

                usuarioActivo.Password = null;

                Session["UsuarioActivo"] = usuarioActivo;

                txtNuevaPassword.Text = "";
                txtConfirmarPassword.Text = "";

                MostrarMensaje(
                    "Su información fue actualizada correctamente.",
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
                    MostrarMensaje(
                        "El usuario o correo indicado ya pertenece a otra cuenta.",
                        true);
                }
                else
                {
                    MostrarMensaje(
                        "No fue posible actualizar su información.",
                        true);
                }
            }
            catch (Exception)
            {
                MostrarMensaje(
                    "No fue posible actualizar su información.",
                    true);
            }
        }

        private void CargarPerfil(int idUsuario)
        {
            PerfilUsuario perfil = usuarioBLL.ObtenerPerfil(idUsuario);

            if (perfil == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            txtUsuario.Text = perfil.NombreUsuario;
            txtTelefono.Text = perfil.Telefono ?? "";
            txtCorreo.Text = perfil.Correo ?? "";
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

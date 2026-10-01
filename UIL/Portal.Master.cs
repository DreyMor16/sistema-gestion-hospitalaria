using EDL;
using System;

namespace UIL
{
    public partial class PortalMaster : System.Web.UI.MasterPage
    {
        public Usuario UsuarioActual { get; private set; }

        public string InicialUsuario
        {
            get
            {
                if (UsuarioActual == null)
                {
                    return "U";
                }

                string nombre = UsuarioActual.Nombre;

                if (string.IsNullOrWhiteSpace(nombre))
                {
                    nombre = UsuarioActual.NombreUsuario;
                }

                if (string.IsNullOrWhiteSpace(nombre))
                {
                    return "U";
                }

                return nombre.Substring(0, 1).ToUpper();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            UsuarioActual = Session["UsuarioActivo"] as Usuario;

            if (UsuarioActual == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            pnlPaciente.Visible = false;
            pnlMedico.Visible = false;
            pnlRecepcionista.Visible = false;
            pnlAdministrador.Visible = false;

            switch (UsuarioActual.Rol)
            {
                case "Paciente":
                    pnlPaciente.Visible = true;
                    litSubtitulo.Text = "Portal del paciente";
                    litMensajeSuperior.Text = "Su salud, siempre cerca";
                    break;

                case "Medico":
                    pnlMedico.Visible = true;
                    litSubtitulo.Text = "Portal médico";
                    litMensajeSuperior.Text = "Información clínica protegida";
                    break;

                case "Recepcionista":
                    pnlRecepcionista.Visible = true;
                    litSubtitulo.Text = "Centro de recepción";
                    litMensajeSuperior.Text = "Atención humana y eficiente";
                    break;

                case "Administrador":
                    pnlAdministrador.Visible = true;
                    litSubtitulo.Text = "Panel administrativo";
                    litMensajeSuperior.Text = "Administración segura del hospital";
                    break;

                default:
                    Session.Clear();
                    Session.Abandon();
                    Response.Redirect("~/Login.aspx");
                    break;
            }
        }

        protected void btnSalir_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}
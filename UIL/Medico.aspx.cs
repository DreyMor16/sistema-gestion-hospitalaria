using BLL;
using EDL;
using System;

namespace UIL
{
    public partial class Medico : System.Web.UI.Page
    {
        private readonly ResumenJornadaMedicoBLL resumenBLL =
            new ResumenJornadaMedicoBLL();

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
                CargarResumen(usuario.IdUsuario);
            }
        }

        private void CargarResumen(int idUsuario)
        {
            try
            {
                ResumenJornadaMedico resumen =
                    resumenBLL.ObtenerResumen(idUsuario);

                lblCitasHoy.Text =
                    resumen.CitasHoy.ToString();

                lblPacientesHoy.Text =
                    resumen.PacientesHoy.ToString();

                lblEnProceso.Text =
                    resumen.EnProcesoHoy.ToString();

                lblFinalizadas.Text =
                    resumen.FinalizadasHoy.ToString();
            }
            catch
            {
                lblCitasHoy.Text = "—";
                lblPacientesHoy.Text = "—";
                lblEnProceso.Text = "—";
                lblFinalizadas.Text = "—";
            }
        }
    }
}
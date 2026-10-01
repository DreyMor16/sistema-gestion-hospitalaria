using EDL;
using System;
using BLL;

namespace UIL
{
    public partial class Recepcionista : System.Web.UI.Page
    {
        private readonly ResumenCitasRecepcionBLL resumenBLL =
            new ResumenCitasRecepcionBLL();
        protected void Page_Load(object sender, EventArgs e)
        {
            Usuario usuario = Session["UsuarioActivo"] as Usuario;

            if (usuario == null || usuario.Rol != "Recepcionista")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarCitasHoy();
            }
        }
        private void CargarCitasHoy()
        {
            ResumenCitasRecepcion resumen =
                resumenBLL.ObtenerResumen();

            lblCitasHoy.Text =
                resumen.CitasHoy.ToString();
        }
    }
}
using EDL;
using System;
using BLL;
namespace UIL
{
    public partial class Administrador : System.Web.UI.Page
    {
        private readonly DashboardBLL dashboardBLL = new DashboardBLL();    
        protected void Page_Load(object sender, EventArgs e)
        {
            Usuario usuario = Session["UsuarioActivo"] as Usuario;

            if (usuario == null || usuario.Rol != "Administrador")
            {
                Response.Redirect("Login.aspx");
            }
            if (!IsPostBack)
            {
                CargarResumen();
            }

        }
        private void CargarResumen()
        {
            try
            {
                ResumenAdministrador resumen =
                    dashboardBLL.ObtenerResumen();

                lblTotalPacientes.Text =
                    resumen.TotalPacientes.ToString();

                lblTotalMedicos.Text =
                    resumen.TotalMedicos.ToString();

                lblTotalMedicamentos.Text =
                    resumen.TotalMedicamentos.ToString();
            }
            catch
            {
                lblTotalPacientes.Text = "—";
                lblTotalMedicos.Text = "—";
                lblTotalMedicamentos.Text = "—";
            }
        }
    }
}
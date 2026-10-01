using BLL;
using EDL;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace UIL
{
    public partial class MantenimientoHospital : System.Web.UI.Page
    {
        private readonly HospitalBLL hospitalBLL = new HospitalBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            Usuario usuario = Session["UsuarioActivo"] as Usuario;

            if (usuario == null || usuario.Rol != "Administrador")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarHospitales();
                LimpiarFormulario();
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                Hospital hospital = new Hospital
                {
                    Nombre = txtNombre.Text,
                    Direccion = txtDireccion.Text,
                    Telefono = txtTelefono.Text
                };

                int idHospital;

                if (int.TryParse(hdnIdHospital.Value, out idHospital))
                {
                    hospital.IdHospital = idHospital;
                    hospitalBLL.Actualizar(hospital);

                    MostrarMensaje("Hospital actualizado correctamente.", false);
                }
                else
                {
                    hospitalBLL.Registrar(hospital);

                    MostrarMensaje("Hospital registrado correctamente.", false);
                }

                LimpiarFormulario();
                CargarHospitales();
            }
            catch (Exception ex)
            {
                MostrarMensaje(ex.Message, true);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            pnlMensaje.Visible = false;
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarHospitales();
        }

        protected void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            CargarHospitales();
        }

        protected void btnLimpiarFiltro_Click(object sender, EventArgs e)
        {
            txtFiltro.Text = "";
            CargarHospitales();
        }

        protected void gvHospitales_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int indice;
            int idHospital;

            if (!int.TryParse(e.CommandArgument.ToString(), out indice))
            {
                return;
            }

            idHospital = Convert.ToInt32(gvHospitales.DataKeys[indice].Value);

            if (e.CommandName == "Editar")
            {
                CargarHospitalEnFormulario(idHospital);
            }
            else if (e.CommandName == "Eliminar")
            {
                EliminarHospital(idHospital);
            }
        }

        private void CargarHospitales()
        {
            List<Hospital> hospitales =
                hospitalBLL.Buscar(txtFiltro.Text);

            gvHospitales.DataSource = hospitales;
            gvHospitales.DataBind();

            lblResultados.Text = hospitales.Count == 1
                ? "1 hospital"
                : hospitales.Count + " hospitales";
        }

        private void CargarHospitalEnFormulario(int idHospital)
        {
            Hospital hospital = hospitalBLL.ObtenerPorId(idHospital);

            if (hospital == null)
            {
                MostrarMensaje("No se encontró el hospital seleccionado.", true);
                return;
            }

            hdnIdHospital.Value = hospital.IdHospital.ToString();
            txtNombre.Text = hospital.Nombre;
            txtDireccion.Text = hospital.Direccion;
            txtTelefono.Text = hospital.Telefono;

            tituloFormulario.InnerText = "Modificar hospital";
            btnGuardar.Text = "Guardar cambios";
            btnCancelar.Visible = true;
            pnlMensaje.Visible = false;
        }

        private void EliminarHospital(int idHospital)
        {
            try
            {
                hospitalBLL.Eliminar(idHospital);

                MostrarMensaje("Hospital eliminado correctamente.", false);
                LimpiarFormulario();
                CargarHospitales();
            }
            catch (Exception)
            {
                MostrarMensaje(
                    "No se puede eliminar el hospital porque tiene pacientes, médicos o inventario relacionados.",
                    true);
            }
        }

        private void LimpiarFormulario()
        {
            hdnIdHospital.Value = "";
            txtNombre.Text = "";
            txtDireccion.Text = "";
            txtTelefono.Text = "";

            tituloFormulario.InnerText = "Registrar hospital";
            btnGuardar.Text = "Guardar hospital";
            btnCancelar.Visible = false;
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

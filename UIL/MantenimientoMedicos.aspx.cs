using BLL;
using EDL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace UIL
{
    public partial class MantenimientoMedicos : System.Web.UI.Page
    {
        private readonly MedicoBLL medicoBLL = new MedicoBLL();
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
                CargarHospitalesFormulario();
                CargarHospitalesFiltro();
                LimpiarFormulario();
                CargarMedicos();
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                int idMedico = 0;
                int idPersona = 0;

                bool esEdicion =
                    int.TryParse(hdnIdMedico.Value, out idMedico) &&
                    int.TryParse(hdnIdPersona.Value, out idPersona);

                MedicoDetalle medico =
                    CrearMedicoDesdeFormulario(esEdicion);

                if (esEdicion)
                {
                    medico.IdMedico = idMedico;
                    medico.IdPersona = idPersona;

                    medicoBLL.Actualizar(medico);

                    MostrarMensaje("Médico actualizado correctamente.", false);
                }
                else
                {
                    medicoBLL.Registrar(medico);

                    MostrarMensaje("Médico registrado correctamente.", false);
                }

                LimpiarFormulario();
                CargarMedicos();
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
                            "La cédula ya está registrada para otra persona.",
                            true);
                    }
                    else if (ex.Message.ToLower().Contains("correo"))
                    {
                        MostrarMensaje(
                            "El correo ya está registrado para otra persona.",
                            true);
                    }
                    else if (ex.Message.ToLower().Contains("usuario"))
                    {
                        MostrarMensaje(
                            "El nombre de usuario ya está registrado. Escriba otro.",
                            true);
                    }
                    else
                    {
                        MostrarMensaje(
                            "Ya existe un registro con esos datos únicos.",
                            true);
                    }
                }
                else
                {
                    MostrarMensaje(
                        "No fue posible guardar el médico. Intente nuevamente.",
                        true);
                }
            }
            catch (Exception)
            {
                MostrarMensaje(
                    "Ocurrió un error inesperado al guardar el médico.",
                    true);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            pnlMensaje.Visible = false;
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarMedicos();
        }

        protected void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            CargarMedicos();
        }

        protected void ddlFiltroHospital_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarMedicos();
        }

        protected void btnLimpiarFiltro_Click(object sender, EventArgs e)
        {
            txtFiltro.Text = "";
            ddlFiltroHospital.SelectedValue = "0";
            CargarMedicos();
        }

        protected void gvMedicos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int indice;

            if (!int.TryParse(e.CommandArgument.ToString(), out indice))
            {
                return;
            }

            int idMedico =
                Convert.ToInt32(gvMedicos.DataKeys[indice].Value);

            if (e.CommandName == "Editar")
            {
                CargarMedicoEnFormulario(idMedico);
            }
            else if (e.CommandName == "Eliminar")
            {
                EliminarMedico(idMedico);
            }
        }

        private void CargarHospitalesFormulario()
        {
            ddlHospital.DataSource = hospitalBLL.Listar();
            ddlHospital.DataTextField = "Nombre";
            ddlHospital.DataValueField = "IdHospital";
            ddlHospital.DataBind();

            ddlHospital.Items.Insert(
                0,
                new ListItem("Seleccione un hospital", "0"));
        }

        private void CargarHospitalesFiltro()
        {
            ddlFiltroHospital.DataSource = hospitalBLL.Listar();
            ddlFiltroHospital.DataTextField = "Nombre";
            ddlFiltroHospital.DataValueField = "IdHospital";
            ddlFiltroHospital.DataBind();

            ddlFiltroHospital.Items.Insert(
                0,
                new ListItem("Todos los hospitales", "0"));
        }

        private void CargarMedicos()
        {
            int idHospital;

            if (!int.TryParse(ddlFiltroHospital.SelectedValue, out idHospital))
            {
                idHospital = 0;
            }

            List<MedicoDetalle> medicos =
                medicoBLL.Buscar(txtFiltro.Text, idHospital);

            gvMedicos.DataSource = medicos;
            gvMedicos.DataBind();

            lblResultados.Text = medicos.Count == 1
                ? "1 médico"
                : medicos.Count + " médicos";
        }

        private MedicoDetalle CrearMedicoDesdeFormulario(
            bool esEdicion)
        {
            if (!esEdicion &&
                txtPassword.Text != txtConfirmarPassword.Text)
            {
                throw new ArgumentException("Las contraseñas no coinciden.");
            }

            int idHospital;

            int.TryParse(ddlHospital.SelectedValue, out idHospital);

            return new MedicoDetalle
            {
                Nombre = txtNombre.Text,
                Apellido = txtApellido.Text,
                NombreUsuario = txtUsuario.Text,
                Password = esEdicion ? "" : txtPassword.Text,
                Cedula = txtCedula.Text,
                Telefono = txtTelefono.Text,
                Correo = txtCorreo.Text,
                Especialidad = txtEspecialidad.Text,
                IdHospital = idHospital
            };
        }

        private void CargarMedicoEnFormulario(int idMedico)
        {
            MedicoDetalle medico = medicoBLL.ObtenerPorId(idMedico);

            if (medico == null)
            {
                MostrarMensaje("No se encontró el médico seleccionado.", true);
                return;
            }

            hdnIdMedico.Value = medico.IdMedico.ToString();
            hdnIdPersona.Value = medico.IdPersona.ToString();

            txtNombre.Text = medico.Nombre;
            txtApellido.Text = medico.Apellido;
            txtUsuario.Text = medico.NombreUsuario;
            txtPassword.Text = "";
            txtConfirmarPassword.Text = "";
            pnlPassword.Visible = false;
            pnlConfirmarPassword.Visible = false;
            txtCedula.Text = medico.Cedula;
            txtTelefono.Text = medico.Telefono;
            txtCorreo.Text = medico.Correo;
            txtEspecialidad.Text = medico.Especialidad;
            ddlHospital.SelectedValue = medico.IdHospital.ToString();

            tituloFormulario.InnerText = "Modificar médico";
            btnGuardar.Text = "Guardar cambios";
            btnCancelar.Visible = true;
            pnlMensaje.Visible = false;
        }

        private void LimpiarFormulario()
        {
            hdnIdMedico.Value = "";
            hdnIdPersona.Value = "";

            txtNombre.Text = "";
            txtApellido.Text = "";
            txtUsuario.Text = "";
            txtPassword.Text = "";
            txtConfirmarPassword.Text = "";
            pnlPassword.Visible = true;
            pnlConfirmarPassword.Visible = true;
            txtCedula.Text = "";
            txtTelefono.Text = "";
            txtCorreo.Text = "";
            txtEspecialidad.Text = "";

            ddlHospital.SelectedValue = "0";

            tituloFormulario.InnerText = "Registrar médico";
            btnGuardar.Text = "Guardar médico";
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
        private void EliminarMedico(int idMedico)
        {
            try
            {
                medicoBLL.Eliminar(idMedico);

                MostrarMensaje("Médico eliminado correctamente.", false);

                LimpiarFormulario();
                CargarMedicos();
            }
            catch (SqlException)
            {
                MostrarMensaje(
                    "No se puede eliminar el médico porque tiene citas relacionadas.",
                    true);
            }
            catch (Exception)
            {
                MostrarMensaje(
                    "No fue posible eliminar el médico. Intente nuevamente.",
                    true);
            }
        }
    }
}

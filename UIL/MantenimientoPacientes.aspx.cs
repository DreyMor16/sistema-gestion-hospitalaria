using BLL;
using EDL;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace UIL
{
    public partial class MantenimientoPacientes : System.Web.UI.Page
    {
        private readonly PacienteBLL pacienteBLL = new PacienteBLL();
        private readonly HospitalBLL hospitalBLL = new HospitalBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            Usuario usuario = Session["UsuarioActivo"] as Usuario;

            if (usuario == null || usuario.Rol != "Administrador")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            txtFechaNacimiento.Attributes["max"] =
                DateTime.Today.ToString("yyyy-MM-dd");

            if (!IsPostBack)
            {
                CargarHospitalesFormulario();
                CargarHospitalesFiltro();
                LimpiarFormulario();
                CargarPacientes();
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                PacienteDetalle paciente = CrearPacienteDesdeFormulario();

                int idPaciente;
                int idPersona;

                if (int.TryParse(hdnIdPaciente.Value, out idPaciente) &&
                    int.TryParse(hdnIdPersona.Value, out idPersona))
                {
                    paciente.IdPaciente = idPaciente;
                    paciente.IdPersona = idPersona;

                    pacienteBLL.Actualizar(paciente);

                    MostrarMensaje("Paciente actualizado correctamente.", false);
                }
                else
                {
                    pacienteBLL.Registrar(paciente);

                    MostrarMensaje("Paciente registrado correctamente.", false);
                }

                LimpiarFormulario();
                CargarPacientes();
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
                        MostrarMensaje("La cédula ya está registrada para otro paciente.", true);
                    }
                    else if (ex.Message.ToLower().Contains("correo"))
                    {
                        MostrarMensaje("El correo ya está registrado para otra persona.", true);
                    }
                    else
                    {
                        MostrarMensaje("Ya existe un registro con esos datos únicos.", true);
                    }
                }
                else
                {
                    MostrarMensaje(
                        "No fue posible guardar el paciente. Intente nuevamente.",
                        true);
                }
            }
            catch (Exception)
            {
                MostrarMensaje(
                    "Ocurrió un error inesperado al guardar el paciente.",
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
            CargarPacientes();
        }

        protected void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            CargarPacientes();
        }

        protected void ddlFiltroHospital_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarPacientes();
        }

        protected void btnLimpiarFiltro_Click(object sender, EventArgs e)
        {
            txtFiltro.Text = "";
            ddlFiltroHospital.SelectedValue = "0";
            CargarPacientes();
        }

        protected void gvPacientes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Editar")
            {
                return;
            }

            int indice;

            if (!int.TryParse(e.CommandArgument.ToString(), out indice))
            {
                return;
            }

            int idPaciente =
                Convert.ToInt32(gvPacientes.DataKeys[indice].Value);

            CargarPacienteEnFormulario(idPaciente);
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

        private void CargarPacientes()
        {
            int idHospital;

            if (!int.TryParse(ddlFiltroHospital.SelectedValue, out idHospital))
            {
                idHospital = 0;
            }

            List<PacienteDetalle> pacientes =
                pacienteBLL.Buscar(txtFiltro.Text, idHospital);

            gvPacientes.DataSource = pacientes;
            gvPacientes.DataBind();

            lblResultados.Text = pacientes.Count == 1
                ? "1 paciente"
                : pacientes.Count + " pacientes";
        }

        private PacienteDetalle CrearPacienteDesdeFormulario()
        {
            int idHospital;

            int.TryParse(ddlHospital.SelectedValue, out idHospital);

            DateTime fechaNacimiento;

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

        private void CargarPacienteEnFormulario(int idPaciente)
        {
            PacienteDetalle paciente =
                pacienteBLL.ObtenerPorId(idPaciente);

            if (paciente == null)
            {
                MostrarMensaje("No se encontró el paciente seleccionado.", true);
                return;
            }

            hdnIdPaciente.Value = paciente.IdPaciente.ToString();
            hdnIdPersona.Value = paciente.IdPersona.ToString();

            txtNombre.Text = paciente.Nombre;
            txtApellido.Text = paciente.Apellido;
            txtCedula.Text = paciente.Cedula;
            txtTelefono.Text = paciente.Telefono;
            txtCorreo.Text = paciente.Correo;

            txtFechaNacimiento.Text =
                paciente.FechaNacimiento.ToString("yyyy-MM-dd");

            ddlGenero.SelectedValue = paciente.Genero;
            ddlHospital.SelectedValue = paciente.IdHospital.ToString();
            txtDireccion.Text = paciente.Direccion;

            tituloFormulario.InnerText = "Modificar paciente";
            btnGuardar.Text = "Guardar cambios";
            btnCancelar.Visible = true;
            pnlMensaje.Visible = false;
        }

        private void LimpiarFormulario()
        {
            hdnIdPaciente.Value = "";
            hdnIdPersona.Value = "";

            txtNombre.Text = "";
            txtApellido.Text = "";
            txtCedula.Text = "";
            txtTelefono.Text = "";
            txtCorreo.Text = "";
            txtFechaNacimiento.Text = "";
            txtDireccion.Text = "";

            ddlGenero.SelectedIndex = 0;
            ddlHospital.SelectedValue = "0";

            tituloFormulario.InnerText = "Registrar paciente";
            btnGuardar.Text = "Guardar paciente";
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

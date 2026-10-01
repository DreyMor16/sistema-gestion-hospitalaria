using BLL;
using EDL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace UIL
{
    public partial class MantenimientoEmpleados : System.Web.UI.Page
    {
        private readonly EmpleadoBLL empleadoBLL = new EmpleadoBLL();

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
                LimpiarFormulario();
                CargarEmpleados();
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtPassword.Text != txtConfirmarPassword.Text)
                {
                    throw new ArgumentException("Las contraseñas no coinciden.");
                }

                EmpleadoDetalle empleado = CrearEmpleadoDesdeFormulario();

                int idEmpleado;
                int idPersona;
                int idUsuario;

                if (int.TryParse(hdnIdEmpleado.Value, out idEmpleado) &&
                    int.TryParse(hdnIdPersona.Value, out idPersona) &&
                    int.TryParse(hdnIdUsuario.Value, out idUsuario))
                {
                    empleado.IdEmpleado = idEmpleado;
                    empleado.IdPersona = idPersona;
                    empleado.IdUsuario = idUsuario;

                    empleadoBLL.Actualizar(empleado);

                    MostrarMensaje("Empleado actualizado correctamente.", false);
                }
                else
                {
                    empleadoBLL.Registrar(empleado);

                    MostrarMensaje("Empleado registrado correctamente.", false);
                }

                LimpiarFormulario();
                CargarEmpleados();
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
                        "No fue posible guardar el empleado. Intente nuevamente.",
                        true);
                }
            }
            catch (Exception)
            {
                MostrarMensaje(
                    "Ocurrió un error inesperado al guardar el empleado.",
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
            CargarEmpleados();
        }

        protected void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            CargarEmpleados();
        }

        protected void ddlFiltroPuesto_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            CargarEmpleados();
        }

        protected void btnLimpiarFiltro_Click(object sender, EventArgs e)
        {
            txtFiltro.Text = "";
            ddlFiltroPuesto.SelectedValue = "";
            CargarEmpleados();
        }

        protected void gvEmpleados_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            int indice;

            if (!int.TryParse(e.CommandArgument.ToString(), out indice))
            {
                return;
            }

            int idEmpleado =
                Convert.ToInt32(gvEmpleados.DataKeys[indice].Value);

            if (e.CommandName == "Editar")
            {
                CargarEmpleadoEnFormulario(idEmpleado);
            }
            else if (e.CommandName == "Eliminar")
            {
                EliminarEmpleado(idEmpleado);
            }
        }

        private void CargarEmpleados()
        {
            List<EmpleadoDetalle> empleados =
                empleadoBLL.Buscar(
                    txtFiltro.Text,
                    ddlFiltroPuesto.SelectedValue);

            gvEmpleados.DataSource = empleados;
            gvEmpleados.DataBind();

            lblResultados.Text = empleados.Count == 1
                ? "1 empleado"
                : empleados.Count + " empleados";
        }

        private EmpleadoDetalle CrearEmpleadoDesdeFormulario()
        {
            return new EmpleadoDetalle
            {
                Nombre = txtNombre.Text,
                Apellido = txtApellido.Text,
                Cedula = txtCedula.Text,
                Telefono = txtTelefono.Text,
                Correo = txtCorreo.Text,
                Puesto = ddlPuesto.SelectedValue,
                NombreUsuario = txtUsuario.Text,
                Password = txtPassword.Text
            };
        }

        private void CargarEmpleadoEnFormulario(int idEmpleado)
        {
            EmpleadoDetalle empleado =
                empleadoBLL.ObtenerPorId(idEmpleado);

            if (empleado == null)
            {
                MostrarMensaje("No se encontró el empleado seleccionado.", true);
                return;
            }

            hdnIdEmpleado.Value = empleado.IdEmpleado.ToString();
            hdnIdPersona.Value = empleado.IdPersona.ToString();
            hdnIdUsuario.Value = empleado.IdUsuario.ToString();

            txtNombre.Text = empleado.Nombre;
            txtApellido.Text = empleado.Apellido;
            txtCedula.Text = empleado.Cedula;
            txtTelefono.Text = empleado.Telefono;
            txtCorreo.Text = empleado.Correo;
            ddlPuesto.SelectedValue = empleado.Puesto;
            txtUsuario.Text = empleado.NombreUsuario;

            txtPassword.Text = "";
            txtConfirmarPassword.Text = "";

            tituloFormulario.InnerText = "Modificar empleado";
            btnGuardar.Text = "Guardar cambios";
            btnCancelar.Visible = true;
            pnlMensaje.Visible = false;
        }

        private void EliminarEmpleado(int idEmpleado)
        {
            try
            {
                empleadoBLL.Eliminar(idEmpleado);

                MostrarMensaje("Empleado eliminado correctamente.", false);

                LimpiarFormulario();
                CargarEmpleados();
            }
            catch (Exception)
            {
                MostrarMensaje(
                    "No fue posible eliminar el empleado. Intente nuevamente.",
                    true);
            }
        }

        private void LimpiarFormulario()
        {
            hdnIdEmpleado.Value = "";
            hdnIdPersona.Value = "";
            hdnIdUsuario.Value = "";

            txtNombre.Text = "";
            txtApellido.Text = "";
            txtCedula.Text = "";
            txtTelefono.Text = "";
            txtCorreo.Text = "";
            txtUsuario.Text = "";
            txtPassword.Text = "";
            txtConfirmarPassword.Text = "";

            ddlPuesto.SelectedIndex = 0;

            tituloFormulario.InnerText = "Registrar empleado";
            btnGuardar.Text = "Guardar empleado";
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

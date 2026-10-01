using BLL;
using EDL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace UIL
{
    public partial class MedicoCita : System.Web.UI.Page
    {
        private readonly CitaMedicoBLL citaBLL =
            new CitaMedicoBLL();

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
                CargarProximasCitas(usuario.IdUsuario);
            }
        }

        protected void gvProximasCitas_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Atender")
            {
                return;
            }

            int indice;

            if (!int.TryParse(e.CommandArgument.ToString(), out indice))
            {
                return;
            }

            int idCita =
                Convert.ToInt32(
                    gvProximasCitas.DataKeys[indice].Value);

            Usuario usuario = Session["UsuarioActivo"] as Usuario;

            if (usuario != null)
            {
                CargarAtencion(usuario.IdUsuario, idCita);
            }
        }

        protected void btnGuardarDiagnostico_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                Usuario usuario = Session["UsuarioActivo"] as Usuario;

                citaBLL.GuardarDiagnostico(
                    usuario.IdUsuario,
                    ObtenerIdCita(),
                    txtDiagnostico.Text);

                MostrarMensaje(
                    "Diagnóstico guardado correctamente.",
                    false);
            }
            catch (ArgumentException ex)
            {
                MostrarMensaje(ex.Message, true);
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible guardar el diagnóstico.",
                    true);
            }
        }

        protected void btnAgregarTratamiento_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                Usuario usuario = Session["UsuarioActivo"] as Usuario;

                citaBLL.AgregarTratamiento(
                    usuario.IdUsuario,
                    ObtenerIdCita(),
                    txtDescripcionTratamiento.Text);

                txtDescripcionTratamiento.Text = "";

                CargarDatosClinicos(
                    usuario.IdUsuario,
                    ObtenerIdCita());

                MostrarMensaje(
                    "Tratamiento agregado correctamente.",
                    false);
            }
            catch (ArgumentException ex)
            {
                MostrarMensaje(ex.Message, true);
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible agregar el tratamiento.",
                    true);
            }
        }

        protected void gvTratamientos_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Prescribir")
            {
                return;
            }

            int indice;

            if (!int.TryParse(e.CommandArgument.ToString(), out indice))
            {
                return;
            }

            int idTratamiento =
                Convert.ToInt32(
                    gvTratamientos.DataKeys[indice].Value);

            Usuario usuario = Session["UsuarioActivo"] as Usuario;

            hdnIdTratamiento.Value = idTratamiento.ToString();

            CargarMedicamentosDisponibles(
                usuario.IdUsuario,
                ObtenerIdCita());

            txtCantidad.Text = "";
            txtDosis.Text = "";
            pnlPrescripcion.Visible = true;
        }

        protected void btnRegistrarPrescripcion_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                Usuario usuario = Session["UsuarioActivo"] as Usuario;

                int idMedicamento;
                int idTratamiento;
                int cantidad;

                if (!int.TryParse(
                    ddlMedicamento.SelectedValue,
                    out idMedicamento) || idMedicamento <= 0)
                {
                    throw new ArgumentException(
                        "Debe seleccionar un medicamento.");
                }

                if (!int.TryParse(
                    hdnIdTratamiento.Value,
                    out idTratamiento) || idTratamiento <= 0)
                {
                    throw new ArgumentException(
                        "Debe seleccionar un tratamiento.");
                }

                if (!int.TryParse(
                    txtCantidad.Text,
                    out cantidad))
                {
                    throw new ArgumentException(
                        "La cantidad debe ser un número entero.");
                }

                citaBLL.RegistrarPrescripcion(
                    usuario.IdUsuario,
                    ObtenerIdCita(),
                    idTratamiento,
                    idMedicamento,
                    cantidad,
                    txtDosis.Text);

                pnlPrescripcion.Visible = false;

                CargarDatosClinicos(
                    usuario.IdUsuario,
                    ObtenerIdCita());

                MostrarMensaje(
                    "Prescripción registrada y stock actualizado.",
                    false);
            }
            catch (ArgumentException ex)
            {
                MostrarMensaje(ex.Message, true);
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Stock insuficiente"))
                {
                    MostrarMensaje(
                        "No hay stock suficiente en el hospital del paciente.",
                        true);
                }
                else
                {
                    MostrarMensaje(
                        "No fue posible registrar la prescripción.",
                        true);
                }
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible registrar la prescripción.",
                    true);
            }
        }

        protected void btnFinalizarCita_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                Usuario usuario = Session["UsuarioActivo"] as Usuario;

                citaBLL.FinalizarCita(
                    usuario.IdUsuario,
                    ObtenerIdCita(),
                    txtDiagnostico.Text);

                pnlAtencion.Visible = false;

                CargarProximasCitas(usuario.IdUsuario);

                MostrarMensaje(
                    "La cita fue finalizada correctamente.",
                    false);
            }
            catch (ArgumentException ex)
            {
                MostrarMensaje(ex.Message, true);
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible finalizar la cita.",
                    true);
            }
        }

        private void CargarProximasCitas(int idUsuario)
        {
            try
            {
                List<CitaProximaAtencion> citas =
                    citaBLL.ListarProximasCitas(idUsuario);

                gvProximasCitas.DataSource = citas;
                gvProximasCitas.DataBind();
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible cargar las próximas citas.",
                    true);
            }
        }

        private void CargarAtencion(
            int idUsuario,
            int idCita)
        {
            try
            {
                DetalleCitaAtencion cita =
                    citaBLL.ObtenerCita(idUsuario, idCita);

                if (cita == null)
                {
                    MostrarMensaje(
                        "La cita seleccionada ya no está disponible.",
                        true);

                    return;
                }

                hdnIdCita.Value = cita.IdCita.ToString();

                lblPaciente.Text =
                    cita.NombrePaciente +
                    " · Cédula: " +
                    cita.Cedula;

                lblDatosCita.Text =
                    cita.Fecha.ToString("dd/MM/yyyy") +
                    " · " +
                    cita.Hora;

                lblHospitalPaciente.Text = cita.Hospital;

                txtDiagnostico.Text = cita.Diagnostico;

                pnlPrescripcion.Visible = false;
                hdnIdTratamiento.Value = "";

                CargarDatosClinicos(idUsuario, idCita);

                pnlAtencion.Visible = true;
                pnlMensaje.Visible = false;
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible cargar la atención de la cita.",
                    true);
            }
        }

        private void CargarDatosClinicos(
            int idUsuario,
            int idCita)
        {
            gvTratamientos.DataSource =
                citaBLL.ListarTratamientos(idUsuario, idCita);

            gvTratamientos.DataBind();

            gvPrescripciones.DataSource =
                citaBLL.ListarPrescripciones(idUsuario, idCita);

            gvPrescripciones.DataBind();
        }

        private void CargarMedicamentosDisponibles(
            int idUsuario,
            int idCita)
        {
            List<MedicamentoDisponibleAtencion> medicamentos =
                citaBLL.ListarMedicamentosDisponibles(
                    idUsuario,
                    idCita);

            ddlMedicamento.DataSource = medicamentos;
            ddlMedicamento.DataTextField = "Detalle";
            ddlMedicamento.DataValueField = "IdMedicamento";
            ddlMedicamento.DataBind();

            ddlMedicamento.Items.Insert(
                0,
                new ListItem(
                    "Seleccione un medicamento disponible",
                    "0"));
        }

        private int ObtenerIdCita()
        {
            int idCita;

            if (!int.TryParse(hdnIdCita.Value, out idCita) ||
                idCita <= 0)
            {
                throw new ArgumentException(
                    "La cita seleccionada no es válida.");
            }

            return idCita;
        }

        private void MostrarMensaje(
            string mensaje,
            bool esError)
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

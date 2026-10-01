using BLL;
using EDL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls;

namespace UIL
{
    public partial class RecepcionistaCitas : System.Web.UI.Page
    {
        private readonly RecepcionistaCitasBLL citasBLL =
            new RecepcionistaCitasBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            Usuario usuario = Session["UsuarioActivo"] as Usuario;

            if (usuario == null || usuario.Rol != "Recepcionista")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            citasBLL.CancelarCitasVencidas();

            txtFechaCita.Attributes["min"] =
                DateTime.Today.ToString("yyyy-MM-dd");

            if (!IsPostBack)
            {
                txtFechaCita.Text =
                    DateTime.Today.ToString("yyyy-MM-dd");
            }
        }

        protected void btnBuscarPaciente_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                pnlAgenda.Visible = false;
                pnlUltimaCita.Visible = false;
                gvHorarios.DataSource = null;
                gvHorarios.DataBind();

                PacienteCitaRecepcion paciente =
                    citasBLL.BuscarPaciente(txtCedula.Text);

                if (paciente == null)
                {
                    MostrarMensaje(
                        "No se encontró un paciente con esa cédula. " +
                        "Debe registrarlo primero.",
                        true);

                    return;
                }

                hdnIdPaciente.Value =
                    paciente.IdPaciente.ToString();

                hdnIdHospital.Value =
                    paciente.IdHospital.ToString();

                lblPaciente.Text =
                    paciente.NombreCompleto;

                lblCedulaPaciente.Text =
                    paciente.Cedula;

                lblHospitalPaciente.Text =
                    paciente.Hospital;

                CargarEspecialidades(paciente.IdHospital);

                pnlAgenda.Visible = true;
                pnlMensaje.Visible = false;
            }
            catch (ArgumentException ex)
            {
                MostrarMensaje(ex.Message, true);
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible buscar al paciente.",
                    true);
            }
        }

        protected void btnConsultarDisponibilidad_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                DateTime fecha = ObtenerFecha();
                int idHospital = ObtenerIdHospital();

                if (ddlEspecialidad.SelectedValue == "0")
                {
                    throw new ArgumentException(
                        "Debe seleccionar una especialidad.");
                }

                CargarHorarios(
                    idHospital,
                    ddlEspecialidad.SelectedValue,
                    fecha);
                CargarUltimaCita(
                    ObtenerIdPaciente(),
                    ddlEspecialidad.SelectedValue);

                pnlMensaje.Visible = false;
            }
            catch (ArgumentException ex)
            {
                MostrarMensaje(ex.Message, true);
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible consultar la disponibilidad.",
                    true);
            }
        }

        protected void gvHorarios_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Agendar")
            {
                return;
            }

            try
            {
                int indice;

                if (!int.TryParse(
                    e.CommandArgument.ToString(),
                    out indice))
                {
                    return;
                }

                DropDownList ddlMedico =
                    gvHorarios.Rows[indice]
                    .FindControl("ddlMedicoDisponible")
                    as DropDownList;

                                if (ddlMedico == null ||
                                    ddlMedico.SelectedValue == "0")
                                {
                                    throw new ArgumentException(
                                        "Debe seleccionar un médico disponible.");
                                }

                                int idMedico;

                                if (!int.TryParse(
                                    ddlMedico.SelectedValue,
                                    out idMedico))
                                {
                                    throw new ArgumentException(
                                        "El médico seleccionado no es válido.");
                                }

                string horaTexto =
                    gvHorarios.DataKeys[indice].Value.ToString();

                TimeSpan hora;

                if (!TimeSpan.TryParse(horaTexto, out hora))
                {
                    throw new ArgumentException(
                        "La hora seleccionada no es válida.");
                }

                DateTime fecha = ObtenerFecha();
                int idPaciente = ObtenerIdPaciente();

                citasBLL.RegistrarCita(
                    idPaciente,
                    idMedico,
                    fecha,
                    hora);

                MostrarMensaje(
                    "La cita fue creada correctamente.",
                    false);

                CargarHorarios(
                    ObtenerIdHospital(),
                    ddlEspecialidad.SelectedValue,
                    fecha);
            }
            catch (ArgumentException ex)
            {
                MostrarMensaje(ex.Message, true);
            }
            catch (SqlException ex)
            {
                MostrarMensaje(ex.Message, true);
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible crear la cita.",
                    true);
            }
        }

        private void CargarEspecialidades(int idHospital)
        {
            List<string> especialidades =
                citasBLL.ListarEspecialidades(idHospital);

            ddlEspecialidad.DataSource = especialidades;
            ddlEspecialidad.DataBind();

            ddlEspecialidad.Items.Insert(
                0,
                new ListItem(
                    "Seleccione una especialidad",
                    "0"));
        }

        private void CargarHorarios(
            int idHospital,
            string especialidad,
            DateTime fecha)
        {
            List<HorarioCitaRecepcion> horarios =
                citasBLL.ListarHorariosDisponibles(
                    idHospital,
                    especialidad,
                    fecha);

            gvHorarios.DataSource = horarios;
            gvHorarios.DataBind();
        }
        private void CargarUltimaCita(
    int idPaciente,
    string especialidad)
        {
            UltimaCitaEspecialidadRecepcion cita =
                citasBLL.ObtenerUltimaCitaEspecialidad(
                    idPaciente,
                    especialidad);

            if (cita == null)
            {
                pnlUltimaCita.Visible = false;
                return;
            }

            lblFechaUltimaCita.Text =
                cita.Fecha.ToString("dd/MM/yyyy");

            lblHoraUltimaCita.Text =
                cita.Hora;

            lblMedicoUltimaCita.Text =
                cita.Medico;

            lblHospitalUltimaCita.Text =
                cita.Hospital;

            lblEstadoUltimaCita.Text =
                cita.Estado;

            lblDiagnosticoUltimaCita.Text =
                string.IsNullOrWhiteSpace(cita.Diagnostico)
                ? "No se registró diagnóstico."
                : cita.Diagnostico;

            gvTratamientosPrevios.DataSource =
                cita.Tratamientos;

            gvTratamientosPrevios.DataBind();

            pnlUltimaCita.Visible = true;
        }
        private DateTime ObtenerFecha()
        {
            DateTime fecha;

            if (!DateTime.TryParseExact(
                txtFechaCita.Text,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out fecha))
            {
                throw new ArgumentException(
                    "Debe seleccionar una fecha válida.");
            }

            return fecha;
        }

        private int ObtenerIdPaciente()
        {
            int idPaciente;

            if (!int.TryParse(
                hdnIdPaciente.Value,
                out idPaciente) || idPaciente <= 0)
            {
                throw new ArgumentException(
                    "Debe buscar un paciente antes de crear la cita.");
            }

            return idPaciente;
        }

        private int ObtenerIdHospital()
        {
            int idHospital;

            if (!int.TryParse(
                hdnIdHospital.Value,
                out idHospital) || idHospital <= 0)
            {
                throw new ArgumentException(
                    "No se encontró el hospital del paciente.");
            }

            return idHospital;
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
        protected void gvHorarios_RowDataBound(
        object sender,
        GridViewRowEventArgs e)
            {
                if (e.Row.RowType != DataControlRowType.DataRow)
                {
                    return;
                }

                HorarioCitaRecepcion horario =
                    e.Row.DataItem as HorarioCitaRecepcion;

                DropDownList ddlMedico =
                    e.Row.FindControl("ddlMedicoDisponible")
                    as DropDownList;

                if (horario == null || ddlMedico == null)
                {
                    return;
                }

                ddlMedico.DataSource =
                    horario.MedicosDisponibles;

                ddlMedico.DataTextField =
                    "NombreCompleto";

                ddlMedico.DataValueField =
                    "IdMedico";

                ddlMedico.DataBind();

                ddlMedico.Items.Insert(
                    0,
                    new ListItem(
                        "Seleccione un médico",
                        "0"));
            }
    }
}

using BLL;
using EDL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Data.SqlClient;

namespace UIL
{
    public partial class PacienteCitas : System.Web.UI.Page
    {
        private readonly PacienteCitasBLL citasBLL =
            new PacienteCitasBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            Usuario usuario = Session["UsuarioActivo"] as Usuario;

            if (usuario == null || usuario.Rol != "Paciente")
            {
                Response.Redirect("Login.aspx");
                return;
            }
            citasBLL.CancelarCitasVencidas();
            if (!IsPostBack)
            {
                txtFechaDisponibilidad.Text =
                    DateTime.Today.ToString("yyyy-MM-dd");

                txtFechaDisponibilidad.Attributes["min"] =
                    DateTime.Today.ToString("yyyy-MM-dd");

                CargarCitasProximas(usuario.IdUsuario);
                CargarHorarios(usuario.IdUsuario, DateTime.Today);
            }
        }

        protected void btnConsultarHorarios_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                DateTime fecha;

                if (!DateTime.TryParseExact(
                    txtFechaDisponibilidad.Text,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out fecha))
                {
                    throw new ArgumentException(
                        "Debe seleccionar una fecha válida.");
                }
                Usuario usuario = Session["UsuarioActivo"] as Usuario;

                if (usuario == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }
                CargarHorarios(usuario.IdUsuario, fecha);
                pnlMensaje.Visible = false;
            }
            catch (ArgumentException ex)
            {
                MostrarMensaje(ex.Message, true);
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible consultar los horarios disponibles.",
                    true);
            }
        }

        private void CargarCitasProximas(int idUsuario)
        {
            List<CitaProximaPaciente> citas =
                citasBLL.ListarCitasProximas(idUsuario);

            gvCitasProximas.DataSource = citas;
            gvCitasProximas.DataBind();
        }

        private void CargarHorarios(int idUsuario, DateTime fecha)
        {
            List<HorarioDisponibleGeneral> horarios =
                citasBLL.ListarHorariosMedicinaGeneral(
                    idUsuario,
                    fecha);

            gvHorariosDisponibles.DataSource = horarios;
            gvHorariosDisponibles.DataBind();

            lblFechaConsultada.Text =
                fecha.ToString(
                    "dddd dd 'de' MMMM 'de' yyyy",
                    new CultureInfo("es-CR"));
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
        protected void gvHorariosDisponibles_RowCommand(
    object sender,
    System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Agendar")
            {
                return;
            }

            try
            {
                int indice;

                if (!int.TryParse(e.CommandArgument.ToString(), out indice))
                {
                    return;
                }

                int idMedico = Convert.ToInt32(
                    gvHorariosDisponibles.DataKeys[indice]
                    .Values["IdMedico"]);

                string horaTexto = gvHorariosDisponibles.DataKeys[indice]
                    .Values["Hora"].ToString();

                TimeSpan hora;

                if (!TimeSpan.TryParse(horaTexto, out hora))
                {
                    throw new ArgumentException(
                        "La hora seleccionada no es válida.");
                }

                DateTime fecha;

                if (!DateTime.TryParseExact(
                    txtFechaDisponibilidad.Text,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out fecha))
                {
                    throw new ArgumentException(
                        "La fecha seleccionada no es válida.");
                }

                Usuario usuario = Session["UsuarioActivo"] as Usuario;

                if (usuario == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                citasBLL.AgendarCita(
                    usuario.IdUsuario,
                    idMedico,
                    fecha,
                    hora);

                MostrarMensaje(
                    "La cita fue agendada correctamente.",
                    false);

                CargarCitasProximas(usuario.IdUsuario);
                CargarHorarios(usuario.IdUsuario, fecha);
            }
            catch (ArgumentException ex)
            {
                MostrarMensaje(ex.Message, true);
            }
            catch (SqlException)
            {
                MostrarMensaje(
                    "Ese horario acaba de ser ocupado. Consulte los horarios nuevamente.",
                    true);
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible agendar la cita.",
                    true);
            }
        }
    }
}

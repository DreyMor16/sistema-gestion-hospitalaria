using BLL;
using EDL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;

namespace UIL
{
    public partial class MedicoPacientes : System.Web.UI.Page
    {
        private readonly MedicoPacientesBLL medicoBLL =
            new MedicoPacientesBLL();

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
                CargarPacientes(usuario.IdUsuario);
            }
        }

        protected void btnConsultar_Click(
            object sender,
            EventArgs e)
        {
            Usuario usuario = Session["UsuarioActivo"] as Usuario;

            if (usuario != null)
            {
                CargarPacientes(usuario.IdUsuario);
            }
        }

        protected void gvPacientesAtendidos_RowCommand(
    object sender,
    GridViewCommandEventArgs e)
        {
            if (e.CommandName != "VerCitas")
            {
                return;
            }

            int indice;

            if (!int.TryParse(e.CommandArgument.ToString(), out indice))
            {
                return;
            }

            int idPaciente =
                Convert.ToInt32(
                    gvPacientesAtendidos.DataKeys[indice].Value);

            Usuario usuario = Session["UsuarioActivo"] as Usuario;

            if (usuario != null)
            {
                CargarCitasPaciente(
                    usuario.IdUsuario,
                    idPaciente);
            }
        }

        protected void btnCerrarDetalle_Click(
            object sender,
            EventArgs e)
        {
            pnlDetalleAtencion.Visible = false;
        }

        private void CargarPacientes(int idUsuario)
        {
            try
            {
                DateTime? fechaInicio =
                    ObtenerFechaOpcional(
                        txtFechaInicio.Text,
                        "La fecha inicial no es válida.");

                DateTime? fechaFin =
                    ObtenerFechaOpcional(
                        txtFechaFin.Text,
                        "La fecha final no es válida.");

                List<PacienteAtendidoMedico> pacientes =
                    medicoBLL.ObtenerPacientesAtendidos(
                        idUsuario,
                        fechaInicio,
                        fechaFin,
                        txtFiltro.Text);

                gvPacientesAtendidos.DataSource = pacientes;
                gvPacientesAtendidos.DataBind();

                pnlDetalleAtencion.Visible = false;
                pnlTratamientosCita.Visible = false;
                pnlMensaje.Visible = false;
            }
            catch (ArgumentException ex)
            {
                gvPacientesAtendidos.DataSource = null;
                gvPacientesAtendidos.DataBind();

                MostrarMensaje(ex.Message, true);
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible consultar los pacientes atendidos.",
                    true);
            }
        }

        private void CargarDetalleAtencion(
            int idUsuario,
            int idCita)
        {
            try
            {
                DetalleAtencionMedico detalle =
                    medicoBLL.ObtenerDetalleAtencion(
                        idUsuario,
                        idCita);

                if (detalle == null)
                {
                    MostrarMensaje(
                        "No se encontró la atención seleccionada.",
                        true);

                    return;
                }

                lblPacienteDetalle.Text =
                    detalle.NombrePaciente +
                    " · Cédula: " +
                    detalle.Cedula;

                lblFechaDetalle.Text =
                    detalle.Fecha.ToString("dd/MM/yyyy");

                lblHoraDetalle.Text = detalle.Hora;

                litDiagnostico.Text =
                    Server.HtmlEncode(
                        string.IsNullOrWhiteSpace(
                            detalle.Diagnostico)
                        ? "No se registró un diagnóstico."
                        : detalle.Diagnostico);

                List<TratamientoPrescripcionMedico> tratamientos =
                    medicoBLL.ListarTratamientosPrescripciones(
                        idUsuario,
                        idCita);

                gvTratamientosPrescripciones.DataSource =
                    tratamientos;

                gvTratamientosPrescripciones.DataBind();

                pnlTratamientosCita.Visible = true;
                pnlDetalleAtencion.Visible = true;
                pnlMensaje.Visible = false;
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible cargar el detalle de la atención.",
                    true);
            }
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
        private DateTime? ObtenerFechaOpcional(
    string fechaTexto,
    string mensajeError)
        {
            if (string.IsNullOrWhiteSpace(fechaTexto))
            {
                return null;
            }

            DateTime fecha;

            if (!DateTime.TryParseExact(
                fechaTexto,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out fecha))
            {
                throw new ArgumentException(mensajeError);
            }

            return fecha;
        }
        private void CargarCitasPaciente(
    int idUsuario,
    int idPaciente)
        {
            try
            {
                List<CitaAtendidaMedico> citas =
                    medicoBLL.ListarCitasPacienteAtendido(
                        idUsuario,
                        idPaciente);

                if (citas.Count == 0)
                {
                    MostrarMensaje(
                        "No se encontraron citas finalizadas para este paciente.",
                        true);

                    return;
                }

                lblPacienteDetalle.Text =
                    citas[0].NombrePaciente +
                    " · Cédula: " +
                    citas[0].Cedula;

                gvCitasPaciente.DataSource = citas;
                gvCitasPaciente.DataBind();

                pnlTratamientosCita.Visible = false;
                pnlDetalleAtencion.Visible = true;
                pnlMensaje.Visible = false;
            }
            catch
            {
                MostrarMensaje(
                    "No fue posible cargar las citas del paciente.",
                    true);
            }
        }
        protected void gvCitasPaciente_RowCommand(
    object sender,
    GridViewCommandEventArgs e)
        {
            if (e.CommandName != "VerDetalleCita")
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
                    gvCitasPaciente.DataKeys[indice].Value);

            Usuario usuario = Session["UsuarioActivo"] as Usuario;

            if (usuario != null)
            {
                CargarDetalleAtencion(
                    usuario.IdUsuario,
                    idCita);
            }
        }
    }
}

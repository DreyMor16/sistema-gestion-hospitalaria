using BLL;
using EDL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls;

namespace UIL
{
    public partial class MantenimientoMedicamentos : System.Web.UI.Page
    {
        private readonly MedicamentoBLL medicamentoBLL =
            new MedicamentoBLL();

        private readonly HospitalBLL hospitalBLL =
            new HospitalBLL();

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
                CargarMedicamentosCombo();
                CargarMedicamentos();
                LimpiarFormularioMedicamento();
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                Medicamento medicamento = new Medicamento
                {
                    Nombre = txtNombre.Text,
                    Descripcion = txtDescripcion.Text,
                    CostoUnitario = ObtenerCostoUnitario()
                };

                int idMedicamento;

                if (int.TryParse(hdnIdMedicamento.Value, out idMedicamento))
                {
                    medicamento.IdMedicamento = idMedicamento;

                    medicamentoBLL.Actualizar(medicamento);

                    MostrarMensaje(
                        pnlMensaje,
                        lblMensaje,
                        "Medicamento actualizado correctamente.",
                        false);
                }
                else
                {
                    medicamentoBLL.Registrar(medicamento);

                    MostrarMensaje(
                        pnlMensaje,
                        lblMensaje,
                        "Medicamento registrado correctamente.",
                        false);
                }

                LimpiarFormularioMedicamento();
                CargarMedicamentos();
                CargarMedicamentosCombo();
            }
            catch (ArgumentException ex)
            {
                MostrarMensaje(
                    pnlMensaje,
                    lblMensaje,
                    ex.Message,
                    true);
            }
            catch (Exception)
            {
                MostrarMensaje(
                    pnlMensaje,
                    lblMensaje,
                    "No fue posible guardar el medicamento.",
                    true);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarFormularioMedicamento();
            pnlMensaje.Visible = false;
        }

        protected void btnGuardarStock_Click(object sender, EventArgs e)
        {
            try
            {
                int idHospital;
                int idMedicamento;
                int cantidadStock;

                int.TryParse(
                    ddlHospitalStock.SelectedValue,
                    out idHospital);

                int.TryParse(
                    ddlMedicamentoStock.SelectedValue,
                    out idMedicamento);

                if (!int.TryParse(
                    txtCantidadStock.Text,
                    out cantidadStock))
                {
                    throw new ArgumentException(
                        "La cantidad de stock debe ser un número entero.");
                }

                InventarioHospital inventario = new InventarioHospital
                {
                    IdHospital = idHospital,
                    IdMedicamento = idMedicamento,
                    CantidadStock = cantidadStock
                };

                medicamentoBLL.AgregarStock(
                    Convert.ToInt32(ddlHospitalStock.SelectedValue),
                    Convert.ToInt32(ddlMedicamentoStock.SelectedValue),
                    Convert.ToInt32(txtCantidadStock.Text)
                );

                MostrarMensaje(
                    pnlMensajeStock,
                    lblMensajeStock,
                    "Stock actualizado correctamente.",
                    false);
                CargarMedicamentos();
                txtCantidadStock.Text = "";
            }
            catch (ArgumentException ex)
            {
                MostrarMensaje(
                    pnlMensajeStock,
                    lblMensajeStock,
                    ex.Message,
                    true);
            }
            catch (Exception)
            {
                MostrarMensaje(
                    pnlMensajeStock,
                    lblMensajeStock,
                    "No fue posible actualizar el stock.",
                    true);
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarMedicamentos();
        }

        protected void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            CargarMedicamentos();
        }

        protected void btnLimpiarFiltro_Click(object sender, EventArgs e)
        {
            txtFiltro.Text = "";
            CargarMedicamentos();
        }

        protected void gvMedicamentos_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            int indice;

            if (!int.TryParse(e.CommandArgument.ToString(), out indice))
            {
                return;
            }

            int idMedicamento =
                Convert.ToInt32(
                    gvMedicamentos.DataKeys[indice].Value);

            if (e.CommandName == "Editar")
            {
                CargarMedicamentoEnFormulario(idMedicamento);
            }
            else if (e.CommandName == "Eliminar")
            {
                EliminarMedicamento(idMedicamento);
            }
        }

        protected void btnConsultarInventario_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                int idHospital;

                int.TryParse(
                    ddlHospitalReporte.SelectedValue,
                    out idHospital);

                List<InventarioMedicamentoDetalle> inventario =
                    medicamentoBLL.ObtenerInventarioUltimos30Dias(
                        idHospital);

                gvInventarioReporte.DataSource = inventario;
                gvInventarioReporte.DataBind();

                pnlMensajeReporte.Visible = false;
            }
            catch (ArgumentException ex)
            {
                gvInventarioReporte.DataSource = null;
                gvInventarioReporte.DataBind();

                MostrarMensaje(
                    pnlMensajeReporte,
                    lblMensajeReporte,
                    ex.Message,
                    true);
            }
            catch (Exception)
            {
                MostrarMensaje(
                    pnlMensajeReporte,
                    lblMensajeReporte,
                    "No fue posible consultar el inventario.",
                    true);
            }
        }

        private void CargarHospitales()
        {
            List<Hospital> hospitales = hospitalBLL.Listar();

            ddlHospitalStock.DataSource = hospitales;
            ddlHospitalStock.DataTextField = "Nombre";
            ddlHospitalStock.DataValueField = "IdHospital";
            ddlHospitalStock.DataBind();

            ddlHospitalStock.Items.Insert(
                0,
                new ListItem("Seleccione un hospital", "0"));

            ddlHospitalReporte.DataSource = hospitales;
            ddlHospitalReporte.DataTextField = "Nombre";
            ddlHospitalReporte.DataValueField = "IdHospital";
            ddlHospitalReporte.DataBind();

            ddlHospitalReporte.Items.Insert(
                0,
                new ListItem("Seleccione un hospital", "0"));
            ddlHospitalCatalogo.DataSource = hospitales;
            ddlHospitalCatalogo.DataTextField = "Nombre";
            ddlHospitalCatalogo.DataValueField = "IdHospital";
            ddlHospitalCatalogo.DataBind();

            ddlHospitalCatalogo.Items.Insert(
                0,
                new ListItem("Todos los hospitales", "0"));
        }

        private void CargarMedicamentosCombo()
        {
            ddlMedicamentoStock.DataSource =
                medicamentoBLL.Listar();

            ddlMedicamentoStock.DataTextField = "Nombre";
            ddlMedicamentoStock.DataValueField = "IdMedicamento";
            ddlMedicamentoStock.DataBind();

            ddlMedicamentoStock.Items.Insert(
                0,
                new ListItem("Seleccione un medicamento", "0"));
        }

        private void CargarMedicamentos()
        {
            int idHospital;

            int.TryParse(
                ddlHospitalCatalogo.SelectedValue,
                out idHospital);

            List<MedicamentoHospitalDetalle> medicamentos =
                medicamentoBLL.BuscarPorHospital(
                    idHospital,
                    txtFiltro.Text);

            gvMedicamentos.DataSource = medicamentos;
            gvMedicamentos.DataBind();

            lblResultados.Text = medicamentos.Count == 1
                ? "1 medicamento"
                : medicamentos.Count + " medicamentos";
        }

        private decimal ObtenerCostoUnitario()
        {
            string costoTexto = txtCostoUnitario.Text
                .Trim()
                .Replace(",", ".");

            decimal costo;

            if (!decimal.TryParse(
                costoTexto,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out costo))
            {
                throw new ArgumentException(
                    "El costo unitario debe ser un número válido.");
            }

            return costo;
        }

        private void CargarMedicamentoEnFormulario(int idMedicamento)
        {
            Medicamento medicamento =
                medicamentoBLL.ObtenerPorId(idMedicamento);

            if (medicamento == null)
            {
                MostrarMensaje(
                    pnlMensaje,
                    lblMensaje,
                    "No se encontró el medicamento seleccionado.",
                    true);

                return;
            }

            hdnIdMedicamento.Value =
                medicamento.IdMedicamento.ToString();

            txtNombre.Text = medicamento.Nombre;
            txtDescripcion.Text = medicamento.Descripcion;

            txtCostoUnitario.Text =
                medicamento.CostoUnitario.ToString(
                    "0.00",
                    CultureInfo.InvariantCulture);

            tituloFormulario.InnerText = "Modificar medicamento";
            btnGuardar.Text = "Guardar cambios";
            btnCancelar.Visible = true;
            pnlMensaje.Visible = false;
        }

        private void EliminarMedicamento(int idMedicamento)
        {
            try
            {
                medicamentoBLL.Eliminar(idMedicamento);

                MostrarMensaje(
                    pnlMensaje,
                    lblMensaje,
                    "Medicamento eliminado correctamente.",
                    false);

                LimpiarFormularioMedicamento();
                CargarMedicamentos();
                CargarMedicamentosCombo();
            }
            catch (SqlException)
            {
                MostrarMensaje(
                    pnlMensaje,
                    lblMensaje,
                    "No se puede eliminar el medicamento porque tiene inventario o prescripciones relacionadas.",
                    true);
            }
            catch (Exception)
            {
                MostrarMensaje(
                    pnlMensaje,
                    lblMensaje,
                    "No fue posible eliminar el medicamento.",
                    true);
            }
        }

        private void LimpiarFormularioMedicamento()
        {
            hdnIdMedicamento.Value = "";
            txtNombre.Text = "";
            txtDescripcion.Text = "";
            txtCostoUnitario.Text = "";

            tituloFormulario.InnerText = "Registrar medicamento";
            btnGuardar.Text = "Guardar medicamento";
            btnCancelar.Visible = false;
        }

        private void MostrarMensaje(
            Panel panel,
            Label etiqueta,
            string mensaje,
            bool esError)
        {
            panel.Visible = true;
            etiqueta.Text = mensaje;

            panel.CssClass = esError
                ? "portal-alert portal-alert-error"
                : "portal-alert portal-alert-success";

            panel.Attributes["role"] = "alert";
        }
        protected void ddlHospitalCatalogo_SelectedIndexChanged(
        object sender,
        EventArgs e)
            {
                CargarMedicamentos();
            }
    }
}

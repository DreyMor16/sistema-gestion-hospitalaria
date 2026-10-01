<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Portal.Master"
    CodeBehind="MantenimientoHospital.aspx.cs"
    Inherits="UIL.MantenimientoHospital" 
    MaintainScrollPositionOnPostBack="true"%>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="eyebrow">Administrador / Mantenimientos</div>
    <h1 class="page-title">Mantenimiento de hospitales</h1>

    <section class="hero">
        <span class="hero-badge">
            <i class="bi bi-hospital"></i> Gestión institucional
        </span>

        <h2>Administre la información de cada hospital.</h2>

        <p>
            Cree, consulte, modifique o elimine hospitales del sistema.
        </p>
    </section>

    <div class="content-grid">
        <section class="panel">
            <div class="panel-heading">
                <div>
                    <h2 id="tituloFormulario" runat="server">Registrar hospital</h2>
                    <p>Complete los datos institucionales requeridos.</p>
                </div>
            </div>

            <asp:Panel ID="pnlMensaje" runat="server" Visible="false"
                CssClass="empty-state">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <asp:HiddenField ID="hdnIdHospital" runat="server" />

            <div class="form-grid">
                <div class="full">
                    <label class="portal-label">Nombre del hospital</label>

                    <asp:TextBox ID="txtNombre" runat="server"
                        CssClass="portal-input"
                        MaxLength="100"
                        placeholder="Ejemplo: Hospital Central">
                    </asp:TextBox>
                </div>

                <div class="full">
                    <label class="portal-label">Dirección</label>

                    <asp:TextBox ID="txtDireccion" runat="server"
                        CssClass="portal-input"
                        MaxLength="200"
                        placeholder="Provincia, cantón y dirección exacta">
                    </asp:TextBox>
                </div>

                <div class="full">
                    <label class="portal-label">Teléfono</label>

                    <asp:TextBox ID="txtTelefono" runat="server"
                        CssClass="portal-input"
                        MaxLength="20"
                        placeholder="Ejemplo: 2222-1111">
                    </asp:TextBox>
                </div>

                <div class="full">
                    <asp:Button ID="btnGuardar" runat="server"
                        Text="Guardar hospital"
                        CssClass="btn-primary-portal"
                        OnClick="btnGuardar_Click" />

                    <asp:Button ID="btnCancelar" runat="server"
                        Text="Cancelar edición"
                        CssClass="btn-outline-portal"
                        CausesValidation="false"
                        Visible="false"
                        OnClick="btnCancelar_Click" />
                </div>
            </div>
        </section>

        <section class="panel">
            <div class="panel-heading">
                <div>
                    <h2>Guía del módulo</h2>
                    <p>Acciones disponibles en el mantenimiento.</p>
                </div>
            </div>

            <div class="info-list">
                <div class="info-row">
                    <span><i class="bi bi-plus-circle"></i> Crear</span>
                    <strong>Registra un nuevo hospital</strong>
                </div>

                <div class="info-row">
                    <span><i class="bi bi-search"></i> Consultar</span>
                    <strong>Lista todos los hospitales</strong>
                </div>

                <div class="info-row">
                    <span><i class="bi bi-pencil-square"></i> Modificar</span>
                    <strong>Seleccione “Editar” en la tabla</strong>
                </div>

                <div class="info-row">
                    <span><i class="bi bi-trash"></i> Eliminar</span>
                    <strong>Elimina el registro seleccionado</strong>
                </div>
            </div>
        </section>
    </div>

   <section class="panel hospital-table-panel" style="margin-top: 22px;">
    <div class="panel-heading">
        <div>
            <h2>Hospitales registrados</h2>
            <p>Busque y administre los hospitales del sistema.</p>
        </div>

        <span class="badge-soft">
            <asp:Label ID="lblResultados" runat="server"></asp:Label>
        </span>
    </div>

    <div class="hospital-search">
        <div class="hospital-search-field">
            <i class="bi bi-search"></i>

            <asp:TextBox ID="txtFiltro" runat="server"
                CssClass="portal-input"
                placeholder="Buscar por nombre, dirección o teléfono"
                AutoPostBack="true"
                OnTextChanged="txtFiltro_TextChanged">
            </asp:TextBox>
        </div>

        <asp:Button ID="btnBuscar" runat="server"
            Text="Buscar"
            CssClass="btn-primary-portal"
            OnClick="btnBuscar_Click" />

        <asp:Button ID="btnLimpiarFiltro" runat="server"
            Text="Limpiar"
            CssClass="btn-outline-portal"
            CausesValidation="false"
            OnClick="btnLimpiarFiltro_Click" />
    </div>

    <div class="table-responsive">
        <asp:GridView ID="gvHospitales" runat="server"
            AutoGenerateColumns="False"
            DataKeyNames="IdHospital"
            CssClass="portal-table hospital-grid"
            OnRowCommand="gvHospitales_RowCommand"
            EmptyDataText="No se encontraron hospitales con esa búsqueda.">

            <Columns>
                <asp:TemplateField HeaderText="Hospital">
                    <ItemTemplate>
                        <div class="hospital-name">
                            <span class="hospital-icon">
                                <i class="bi bi-hospital"></i>
                            </span>

                            <strong><%# Eval("Nombre") %></strong>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Dirección">
                    <ItemTemplate>
                        <span class="hospital-detail">
                            <i class="bi bi-geo-alt"></i>
                            <%# Eval("Direccion") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Teléfono">
                    <ItemTemplate>
                        <span class="hospital-detail">
                            <i class="bi bi-telephone"></i>
                            <%# Eval("Telefono") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <div class="hospital-actions">
                            <asp:LinkButton ID="btnEditar" runat="server"
                                CommandName="Editar"
                                CommandArgument="<%# Container.DataItemIndex %>"
                                CssClass="btn-outline-portal">

                                <i class="bi bi-pencil-square"></i> Editar
                            </asp:LinkButton>

                            <asp:LinkButton ID="btnEliminar" runat="server"
                                CommandName="Eliminar"
                                CommandArgument="<%# Container.DataItemIndex %>"
                                CssClass="btn-delete-portal"
                                OnClientClick="return confirmarAccion(this, '¿Desea eliminar este hospital?');">

                                <i class="bi bi-trash"></i> Eliminar
                            </asp:LinkButton>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</section>

</asp:Content>

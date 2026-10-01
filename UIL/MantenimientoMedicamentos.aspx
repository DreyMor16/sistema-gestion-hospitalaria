<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Portal.Master"
    CodeBehind="MantenimientoMedicamentos.aspx.cs"
    Inherits="UIL.MantenimientoMedicamentos" 
    MaintainScrollPositionOnPostBack="true"%>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="eyebrow">Administrador / Mantenimientos</div>
    <h1 class="page-title">Medicamentos e inventario</h1>

    <section class="hero">
        <span class="hero-badge">
            <i class="bi bi-capsule-pill"></i> Gestión farmacéutica
        </span>

        <h2>Control preciso para una atención sin interrupciones.</h2>

        <p>
            Administre el catálogo de medicamentos, asigne stock
            por hospital y consulte el consumo de los últimos 30 días.
        </p>
    </section>

    <div class="content-grid">
        <section class="panel">
            <div class="panel-heading">
                <div>
                    <h2 id="tituloFormulario" runat="server">
                        Registrar medicamento
                    </h2>
                    <p>El precio unitario se registra en colones costarricenses.</p>
                </div>
            </div>

            <asp:Panel ID="pnlMensaje" runat="server"
                Visible="false"
                CssClass="empty-state">

                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <asp:HiddenField ID="hdnIdMedicamento" runat="server" />

            <div class="form-grid">
                <div class="full">
                    <label class="portal-label">Nombre del medicamento</label>

                    <asp:TextBox ID="txtNombre" runat="server"
                        CssClass="portal-input"
                        MaxLength="100"
                        placeholder="Ejemplo: Paracetamol">
                    </asp:TextBox>
                </div>

                <div class="full">
                    <label class="portal-label">Descripción</label>

                    <asp:TextBox ID="txtDescripcion" runat="server"
                        CssClass="portal-input"
                        TextMode="MultiLine"
                        Rows="3"
                        MaxLength="300"
                        placeholder="Descripción o indicaciones generales">
                    </asp:TextBox>
                </div>

                <div class="full">
                    <label class="portal-label">Costo unitario (₡)</label>

                    <asp:TextBox ID="txtCostoUnitario" runat="server"
                        CssClass="portal-input"
                        TextMode="Number"
                        step="0.01"
                        min="0.01"
                        placeholder="Ejemplo: 500.00">
                    </asp:TextBox>
                </div>

                <div class="full">
                    <asp:Button ID="btnGuardar" runat="server"
                        Text="Guardar medicamento"
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
                    <h2>Asignar stock</h2>
                    <p>Defina la cantidad actual por hospital.</p>
                </div>
            </div>

            <asp:Panel ID="pnlMensajeStock" runat="server"
                Visible="false"
                CssClass="empty-state">

                <asp:Label ID="lblMensajeStock" runat="server"></asp:Label>
            </asp:Panel>

            <div class="form-grid">
                <div class="full">
                    <label class="portal-label">Hospital</label>

                    <asp:DropDownList ID="ddlHospitalStock" runat="server"
                        CssClass="portal-select">
                    </asp:DropDownList>
                </div>

                <div class="full">
                    <label class="portal-label">Medicamento</label>

                    <asp:DropDownList ID="ddlMedicamentoStock" runat="server"
                        CssClass="portal-select">
                    </asp:DropDownList>
                </div>

                <div class="full">
                    <label class="portal-label">Unidades agregadas</label>

                    <asp:TextBox ID="txtCantidadStock" runat="server"
                        CssClass="portal-input"
                        TextMode="Number"
                        min="0"
                        placeholder="Ejemplo: 100"
                        inputmode="numeric"
                        oninput="this.value=this.value.replace(/[^0-9]/g, '');">
                    </asp:TextBox>
                </div>

                <div class="full">
                    <asp:Button ID="btnGuardarStock" runat="server"
                        Text="Guardar stock"
                        CssClass="btn-primary-portal"
                        OnClick="btnGuardarStock_Click" />
                </div>
            </div>

            <div class="stock-card" style="margin-top: 18px;">
                <strong><i class="bi bi-info-circle"></i> Importante</strong>
                <span>
                    Si el medicamento ya existe en el inventario del hospital,
                    las unidades ingresadas se suman al stock actual.
                </span>
            </div>
        </section>
    </div>

    <section class="panel hospital-table-panel" style="margin-top: 22px;">
        <div class="panel-heading">
            <div>
                <h2>Catálogo de medicamentos</h2>
                <p>Consulte, edite o elimine medicamentos registrados.</p>
            </div>

            <span class="badge-soft">
                <asp:Label ID="lblResultados" runat="server"></asp:Label>
            </span>
        </div>
        <div class="medication-filter">
            <div class="portal-field">
                <label class="portal-label">
                    <i class="bi bi-hospital"></i> Filtrar medicamentos por hospital
                </label>

                <asp:DropDownList ID="ddlHospitalCatalogo" runat="server"
                    CssClass="portal-select"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="ddlHospitalCatalogo_SelectedIndexChanged">
                </asp:DropDownList>
            </div>
        </div>
        <div class="hospital-search">
            <div class="hospital-search-field">
                <i class="bi bi-search"></i>

                <asp:TextBox ID="txtFiltro" runat="server"
                    CssClass="portal-input"
                    placeholder="Buscar por nombre o descripción"
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
            <asp:GridView ID="gvMedicamentos" runat="server"
                AutoGenerateColumns="False"
                DataKeyNames="IdMedicamento"
                CssClass="portal-table hospital-grid"
                OnRowCommand="gvMedicamentos_RowCommand"
                EmptyDataText="No se encontraron medicamentos.">

                <Columns>
                    <asp:TemplateField HeaderText="Medicamento">
                        <ItemTemplate>
                            <div class="hospital-name">
                                <span class="hospital-icon">
                                    <i class="bi bi-capsule-pill"></i>
                                </span>

                                <strong><%# Eval("Nombre") %></strong>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="Descripcion"
                        HeaderText="Descripción" />

                    <asp:TemplateField HeaderText="Costo unitario">
                        <ItemTemplate>
                            <span class="medication-price">
                                <i class="bi bi-currency-exchange"></i>

                                <%# System.String.Format(
                                    new System.Globalization.CultureInfo("es-CR"),
                                    "₡ {0:N2}",
                                    Eval("CostoUnitario")) %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="NombreHospital"
                        HeaderText="Hospital" />

                    <asp:TemplateField HeaderText="Stock disponible">
                        <ItemTemplate>
                            <span class="badge-soft">
                                <i class="bi bi-box-seam"></i>
                                <%# Eval("CantidadStock") %> unidades
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
                                    OnClientClick="return confirmarAccion(this, '¿Desea eliminar este medicamento?');">

                                    <i class="bi bi-trash"></i> Eliminar
                                </asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </section>

    <section class="panel" style="margin-top: 22px;">
        <div class="panel-heading">
            <div>
                <h2>Inventario y prescripciones: últimos 30 días</h2>
                <p>
                    Información obtenida mediante
                    <code>sp_InventarioYPrescripcionesUltimos30Dias</code>.
                </p>
            </div>
        </div>

        <div class="hospital-search">
            <asp:DropDownList ID="ddlHospitalReporte" runat="server"
                CssClass="portal-select">
            </asp:DropDownList>

            <asp:Button ID="btnConsultarInventario" runat="server"
                Text="Consultar inventario"
                CssClass="btn-primary-portal"
                OnClick="btnConsultarInventario_Click" />
        </div>

        <asp:Panel ID="pnlMensajeReporte" runat="server"
            Visible="false"
            CssClass="empty-state">

            <asp:Label ID="lblMensajeReporte" runat="server"></asp:Label>
        </asp:Panel>

        <div class="table-responsive">
            <asp:GridView ID="gvInventarioReporte" runat="server"
                AutoGenerateColumns="False"
                CssClass="portal-table hospital-grid"
                EmptyDataText="No hay inventario para el hospital seleccionado.">

                <Columns>
                    <asp:TemplateField HeaderText="Medicamento">
                        <ItemTemplate>
                            <div class="hospital-name">
                                <span class="hospital-icon">
                                    <i class="bi bi-capsule"></i>
                                </span>

                                <strong><%# Eval("Medicamento") %></strong>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Stock actual">
                        <ItemTemplate>
                            <span class="badge-soft">
                                <%# Eval("StockActual") %> unidades
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Prescrito en 30 días">
                        <ItemTemplate>
                            <span class="medication-price">
                                <i class="bi bi-graph-up-arrow"></i>
                                <%# Eval("TotalPrescrito30Dias") %> unidades
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </section>

</asp:Content>

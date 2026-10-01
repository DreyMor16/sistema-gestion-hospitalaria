<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Portal.Master"
    CodeBehind="PacientePagos.aspx.cs"
    Inherits="UIL.PacientePagos" 
    MaintainScrollPositionOnPostBack="true"%>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="eyebrow">Paciente / Pagos</div>
    <h1 class="page-title">Pagos de tratamientos</h1>

    <section class="payment-hero">
        <div>
            <span class="badge-soft">
                <i class="bi bi-shield-check"></i>
                Pagos seguros
            </span>

            <h2>Administre sus tratamientos pendientes</h2>

            <p>
                Consulte sus saldos, pague todos los tratamientos de una
                misma cita y revise su historial de pagos.
            </p>
        </div>

        <div class="payment-hero-icon">
            <i class="bi bi-credit-card-2-front"></i>
        </div>
    </section>

    <asp:Panel ID="pnlMensaje" runat="server"
        Visible="false"
        CssClass="empty-state"
        style="margin-top: 20px;">

        <asp:Label ID="lblMensaje" runat="server"></asp:Label>
    </asp:Panel>

    <section class="panel" style="margin-top: 22px;">
        <div class="panel-heading">
            <div>
                <h2>Total pagado por período</h2>
                <p>Información obtenida mediante el procedimiento almacenado.</p>
            </div>
        </div>

        <div class="form-grid">
            <div>
                <label class="portal-label">Fecha inicial</label>

                <asp:TextBox ID="txtFechaInicio" runat="server"
                    CssClass="portal-input"
                    TextMode="Date">
                </asp:TextBox>
            </div>

            <div>
                <label class="portal-label">Fecha final</label>

                <asp:TextBox ID="txtFechaFin" runat="server"
                    CssClass="portal-input"
                    TextMode="Date">
                </asp:TextBox>
            </div>

            <div class="full">
                <asp:Button ID="btnCalcularTotal" runat="server"
                    Text="Calcular total pagado"
                    CssClass="btn-outline-portal"
                    OnClick="btnCalcularTotal_Click" />
            </div>
        </div>

        <div class="payment-summary">
            <i class="bi bi-wallet2"></i>

            <div>
                <span>Total pagado en el período seleccionado</span>
                <strong>
                    <asp:Label ID="lblTotalPagado" runat="server"
                        Text="₡ 0,00">
                    </asp:Label>
                </strong>
            </div>
        </div>
    </section>

    <section class="panel hospital-table-panel" style="margin-top: 22px;">
        <div class="panel-heading">
            <div>
                <h2>Citas pendientes de pago</h2>
                <p>Cada pago incluye todos los tratamientos pendientes de esa cita.</p>
            </div>
        </div>

        <div class="table-responsive">
            <asp:GridView ID="gvPendientes" runat="server"
                AutoGenerateColumns="False"
                DataKeyNames="IdCita"
                CssClass="portal-table hospital-grid"
                OnRowCommand="gvPendientes_RowCommand"
                EmptyDataText="No tiene tratamientos pendientes de pago.">

                <Columns>
                    <asp:BoundField DataField="FechaCita"
                        HeaderText="Fecha de cita"
                        DataFormatString="{0:dd/MM/yyyy}" />

                    <asp:TemplateField HeaderText="Tratamientos de la cita">
                        <ItemTemplate>
                            <span class="payment-treatment-list">
                                <i class="bi bi-list-check"></i>
                                <%# Eval("DetalleTratamiento") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Monto pendiente">
                        <ItemTemplate>
                            <span class="payment-balance">
                                <i class="bi bi-cash-stack"></i>
                                <%# System.String.Format(
                                    new System.Globalization.CultureInfo("es-CR"),
                                    "₡ {0:N2}",
                                    Eval("MontoPendiente")) %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Acción">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnPagar" runat="server"
                                CommandName="SeleccionarPago"
                                CommandArgument="<%# Container.DataItemIndex %>"
                                CssClass="btn-primary-portal">

                                <i class="bi bi-credit-card"></i>
                                Pagar
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </section>

    <asp:Panel ID="pnlConfirmarPago" runat="server"
        Visible="false"
        CssClass="panel"
        style="margin-top: 22px;">

        <div class="panel-heading">
            <div>
                <h2>Confirmar pago</h2>
                <p>El monto incluye todos los tratamientos pendientes de esta cita.</p>
            </div>
        </div>

        <asp:HiddenField ID="hdnIdCita" runat="server" />

        <div class="payment-summary">
            <i class="bi bi-receipt"></i>

            <div>
                <span>
                    <asp:Label ID="lblTratamientoSeleccionado"
                        runat="server">
                    </asp:Label>
                </span>

                <strong>
                    <asp:Label ID="lblMontoSeleccionado"
                        runat="server">
                    </asp:Label>
                </strong>
            </div>
        </div>

        <div class="form-grid" style="margin-top: 20px;">
            <div class="full">
                <label class="portal-label">Método de pago</label>

                <asp:DropDownList ID="ddlMetodoPago" runat="server"
                    CssClass="portal-select">

                    <asp:ListItem Text="Seleccione un método de pago"
                        Value="0"></asp:ListItem>

                    <asp:ListItem Text="Efectivo"
                        Value="Efectivo"></asp:ListItem>

                    <asp:ListItem Text="Tarjeta"
                        Value="Tarjeta"></asp:ListItem>

                    <asp:ListItem Text="Sinpe"
                        Value="Sinpe"></asp:ListItem>
                </asp:DropDownList>
            </div>

            <div class="full">
                <asp:Button ID="btnConfirmarPago" runat="server"
                    Text="Confirmar pago"
                    CssClass="btn-primary-portal"
                    OnClick="btnConfirmarPago_Click"
                    OnClientClick="return confirmarAccion(this, '¿Desea confirmar el pago completo de esta cita?');" />

                <asp:Button ID="btnCancelarPago" runat="server"
                    Text="Cancelar"
                    CssClass="btn-outline-portal"
                    CausesValidation="false"
                    OnClick="btnCancelarPago_Click" />
            </div>
        </div>
    </asp:Panel>

    <section class="panel hospital-table-panel" style="margin-top: 22px;">
        <div class="panel-heading">
            <div>
                <h2>Historial de pagos</h2>
                <p>Pagos registrados en su cuenta hospitalaria.</p>
            </div>
        </div>

        <div class="table-responsive">
            <asp:GridView ID="gvHistorialPagos" runat="server"
                AutoGenerateColumns="False"
                CssClass="portal-table hospital-grid"
                EmptyDataText="Aún no tiene pagos registrados.">

                <Columns>
                    <asp:BoundField DataField="FechaPago"
                        HeaderText="Fecha de pago"
                        DataFormatString="{0:dd/MM/yyyy}" />

                    <asp:TemplateField HeaderText="Tratamientos pagados">
                        <ItemTemplate>
                            <span class="payment-treatment-list">
                                <i class="bi bi-list-check"></i>
                                <%# Eval("DetalleTratamiento") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="FechaCita"
                        HeaderText="Fecha de cita"
                        DataFormatString="{0:dd/MM/yyyy}" />

                    <asp:TemplateField HeaderText="Método">
                        <ItemTemplate>
                            <span class="badge-soft">
                                <i class="bi bi-credit-card"></i>
                                <%# Eval("MetodoPago") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Monto pagado">
                        <ItemTemplate>
                            <span class="payment-paid">
                                <%# System.String.Format(
                                    new System.Globalization.CultureInfo("es-CR"),
                                    "₡ {0:N2}",
                                    Eval("Monto")) %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </section>

</asp:Content>

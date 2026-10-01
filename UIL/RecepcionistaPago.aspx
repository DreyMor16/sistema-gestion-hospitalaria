<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Portal.Master"
    CodeBehind="RecepcionistaPago.aspx.cs"
    Inherits="UIL.RecepcionistaPago"
    MaintainScrollPositionOnPostBack="true" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="eyebrow">Recepción / Pagos</div>
    <h1 class="page-title">Pago de tratamientos</h1>

    <section class="reception-payment-hero">
        <div>
            <span class="badge-soft">
                <i class="bi bi-credit-card-2-front"></i>
                Cobro hospitalario
            </span>

            <h2>Busque al paciente por cédula</h2>

            <p>
                Consulte las citas con tratamientos pendientes
                y registre el pago completo de la atención.
            </p>
        </div>

        <div class="reception-payment-hero-icon">
            <i class="bi bi-cash-coin"></i>
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
                <h2>Buscar paciente</h2>
                <p>Digite los nueve dígitos de la cédula.</p>
            </div>
        </div>

        <div class="cedula-lookup">
            <div style="flex: 1;">
                <label class="portal-label">Cédula del paciente</label>

                <asp:TextBox ID="txtCedula" runat="server"
                    CssClass="portal-input"
                    MaxLength="20"
                    inputmode="numeric"
                    placeholder="Ejemplo: 102220002"
                    oninput="this.value=this.value.replace(/[^0-9]/g, '');">
                </asp:TextBox>
            </div>

            <asp:Button ID="btnBuscar" runat="server"
                Text="Buscar tratamientos"
                CssClass="btn-primary-portal"
                OnClick="btnBuscar_Click" />
        </div>
    </section>

    <asp:Panel ID="pnlPaciente" runat="server"
        Visible="false">

        <asp:HiddenField ID="hdnIdPaciente" runat="server" />

        <section class="reception-patient-card">
            <i class="bi bi-person-vcard"></i>

            <div>
                <strong>
                    <asp:Label ID="lblPaciente"
                        runat="server">
                    </asp:Label>
                </strong>

                <span>
                    Cédula:
                    <asp:Label ID="lblCedulaPaciente"
                        runat="server">
                    </asp:Label>
                </span>

                <span>
                    Hospital:
                    <asp:Label ID="lblHospitalPaciente"
                        runat="server">
                    </asp:Label>
                </span>
            </div>
        </section>

        <section class="panel hospital-table-panel">
            <div class="panel-heading">
                <div>
                    <h2>Tratamientos pendientes</h2>
                    <p>El pago cubre todos los tratamientos pendientes de una cita.</p>
                </div>
            </div>

            <div class="table-responsive">
                <asp:GridView ID="gvPendientes" runat="server"
                    AutoGenerateColumns="False"
                    DataKeyNames="IdCita"
                    CssClass="portal-table hospital-grid"
                    OnRowCommand="gvPendientes_RowCommand"
                    EmptyDataText="Este paciente no tiene tratamientos pendientes de pago.">

                    <Columns>
                        <asp:BoundField DataField="FechaCita"
                            HeaderText="Fecha de cita"
                            DataFormatString="{0:dd/MM/yyyy}" />

                        <asp:TemplateField HeaderText="Tratamientos">
                            <ItemTemplate>
                                <span class="payment-treatment-list">
                                    <i class="bi bi-list-check"></i>
                                    <%# Eval("DetalleTratamientos") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Monto pendiente">
                            <ItemTemplate>
                                <span class="reception-payment-total">
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
                                <asp:LinkButton ID="btnPagar"
                                    runat="server"
                                    CommandName="SeleccionarPago"
                                    CommandArgument="<%# Container.DataItemIndex %>"
                                    CssClass="btn-primary-portal">

                                    <i class="bi bi-credit-card"></i>
                                    Cobrar
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </section>
    </asp:Panel>

    <asp:Panel ID="pnlConfirmarPago" runat="server"
        Visible="false"
        CssClass="panel"
        style="margin-top: 22px;">

        <div class="panel-heading">
            <div>
                <h2>Confirmar pago</h2>
                <p>Revise el monto antes de registrar el cobro.</p>
            </div>
        </div>

        <asp:HiddenField ID="hdnIdCita" runat="server" />

        <div class="payment-summary">
            <i class="bi bi-receipt"></i>

            <div>
                <span>
                    <asp:Label ID="lblCitaSeleccionada"
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
                    OnClientClick="return confirmarAccion(this, '¿Desea confirmar este pago?');" />

                <asp:Button ID="btnCancelarPago" runat="server"
                    Text="Cancelar"
                    CssClass="btn-outline-portal"
                    CausesValidation="false"
                    OnClick="btnCancelarPago_Click" />
            </div>
        </div>
    </asp:Panel>

</asp:Content>

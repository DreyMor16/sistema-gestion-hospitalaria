<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Portal.Master"
    CodeBehind="RecepcionistaControlCitas.aspx.cs"
    Inherits="UIL.RecepcionistaControlCitas"
    MaintainScrollPositionOnPostBack="true" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="eyebrow">Recepción / Agenda</div>
    <h1 class="page-title">Control de citas</h1>

    <section class="reception-control-hero">
        <span class="reception-control-icon">
            <i class="bi bi-calendar2-check"></i>
        </span>

        <div>
            <h1>Próximas atenciones</h1>
            <p>
                Consulte la agenda por hospital, especialidad o fecha
                y cancele una cita cuando corresponda.
            </p>
        </div>
    </section>

    <asp:Panel ID="pnlMensaje" runat="server"
        Visible="false"
        CssClass="empty-state"
        style="margin-bottom: 20px;">

        <asp:Label ID="lblMensaje" runat="server"></asp:Label>
    </asp:Panel>

    <section class="panel">
        <div class="panel-heading">
            <div>
                <h2>Filtros de agenda</h2>
                <p>
                    Si no selecciona filtros, se mostrarán todas las
                    citas pendientes desde este momento.
                </p>
            </div>
        </div>

        <div class="appointment-control-filters">
            <div class="portal-field">
                <label class="portal-label">Hospital</label>

                <asp:DropDownList ID="ddlHospital" runat="server"
                    CssClass="portal-select"
                    AutoPostBack="true"
                    OnSelectedIndexChanged=
                        "ddlHospital_SelectedIndexChanged">
                </asp:DropDownList>
            </div>

            <div class="portal-field">
                <label class="portal-label">Especialidad</label>

                <asp:DropDownList ID="ddlEspecialidad" runat="server"
                    CssClass="portal-select">
                </asp:DropDownList>
            </div>

            <div class="portal-field">
                <label class="portal-label">Fecha</label>

                <asp:TextBox ID="txtFecha" runat="server"
                    CssClass="portal-input"
                    TextMode="Date">
                </asp:TextBox>
            </div>

            <div>
                <asp:Button ID="btnConsultar" runat="server"
                    Text="Consultar"
                    CssClass="btn-primary-portal"
                    OnClick="btnConsultar_Click" />

                <asp:Button ID="btnLimpiar" runat="server"
                    Text="Limpiar"
                    CssClass="btn-outline-portal"
                    CausesValidation="false"
                    OnClick="btnLimpiar_Click" />
            </div>
        </div>

        <div class="table-responsive">
            <asp:GridView ID="gvCitas" runat="server"
                AutoGenerateColumns="False"
                CssClass="portal-table hospital-grid"
                OnRowCommand="gvCitas_RowCommand"
                EmptyDataText=
                    "No hay citas próximas con los filtros seleccionados.">

                <Columns>
                    <asp:TemplateField HeaderText="Fecha y hora">
                        <ItemTemplate>
                            <strong>
                                <%# Eval("Fecha", "{0:dd/MM/yyyy}") %>
                            </strong>

                            <br />

                            <span class="appointment-time-badge">
                                <i class="bi bi-clock"></i>
                                <%# Eval("Hora") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Paciente">
                        <ItemTemplate>
                            <div class="appointment-patient-name">
                                <i class="bi bi-person-circle"></i>
                                <%# Eval("Paciente") %>
                            </div>

                            <div class="appointment-contact">
                                <span>
                                    <i class="bi bi-person-vcard"></i>
                                    <%# Eval("Cedula") %>
                                </span>

                                <span>
                                    <i class="bi bi-telephone"></i>
                                    <%# Eval("Telefono") %>
                                </span>

                                <span>
                                    <i class="bi bi-envelope"></i>
                                    <%# Eval("Correo") %>
                                </span>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Atención">
                        <ItemTemplate>
                            <strong><%# Eval("Medico") %></strong>

                            <br />

                            <span class="appointment-specialty-badge">
                                <i class="bi bi-heart-pulse"></i>
                                <%# Eval("Especialidad") %>
                            </span>

                            <br /><br />

                            <small><%# Eval("Hospital") %></small>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Información de cita">
                        <ItemTemplate>
                            <span class="appointment-state-badge">
                                <%# Eval("Estado") %>
                            </span>

                            <br /><br />

                            <%# string.IsNullOrWhiteSpace(
                                    Eval("Diagnostico").ToString())
                                ? "Sin diagnóstico registrado."
                                : Eval("Diagnostico") %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Acción">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnCancelar"
                                runat="server"
                                CommandName="Cancelar"
                                CommandArgument='<%# Eval("IdCita") %>'
                                CssClass="btn-outline-portal appointment-cancel-button"
                                OnClientClick="return confirmarAccion(this, '¿Desea cancelar esta cita? Esta acción liberará el horario del médico.');">

                                <i class="bi bi-calendar-x"></i>
                                Cancelar
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>

        
    </section>

</asp:Content>

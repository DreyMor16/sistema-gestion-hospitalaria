<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Portal.Master"
    CodeBehind="PacienteCitas.aspx.cs"
    Inherits="UIL.PacienteCitas" 
    MaintainScrollPositionOnPostBack="true"%>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="eyebrow">Paciente / Citas</div>
    <h1 class="page-title">Mis citas</h1>

    <section class="appointment-hero">
        <div>
            <span class="badge-soft">
                <i class="bi bi-calendar2-heart"></i>
                Agenda médica personal
            </span>

            <h2>Próximas atenciones y horarios disponibles</h2>

            <p>
                Consulte las citas que tiene programadas y explore
                los horarios disponibles de Medicina General.
            </p>
        </div>

        <div class="appointment-hero-icon">
            <i class="bi bi-calendar2-week"></i>
        </div>
    </section>

    <asp:Panel ID="pnlMensaje" runat="server"
        Visible="false"
        CssClass="empty-state"
        style="margin-top: 20px;">

        <asp:Label ID="lblMensaje" runat="server"></asp:Label>
    </asp:Panel>

    <section class="panel hospital-table-panel" style="margin-top: 22px;">
        <div class="panel-heading">
            <div>
                <h2>Mis próximas citas</h2>
                <p>Estar 20 minutos antes de la cita, sino se cancelará.</p>
            </div>
        </div>

        <div class="table-responsive">
            <asp:GridView ID="gvCitasProximas" runat="server"
                AutoGenerateColumns="False"
                CssClass="portal-table hospital-grid"
                EmptyDataText="No tiene citas próximas programadas.">

                <Columns>
                    <asp:BoundField DataField="Fecha"
                        HeaderText="Fecha"
                        DataFormatString="{0:dd/MM/yyyy}" />

                    <asp:TemplateField HeaderText="Hora">
                        <ItemTemplate>
                            <span class="appointment-time">
                                <i class="bi bi-clock"></i>
                                <%# Eval("Hora") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Médico">
                        <ItemTemplate>
                            <strong><%# Eval("Medico") %></strong>
                            <br />
                            <small><%# Eval("Especialidad") %></small>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="Hospital"
                        HeaderText="Hospital" />

                    <asp:TemplateField HeaderText="Estado">
                        <ItemTemplate>
                            <span class="status-proceso">
                                <%# Eval("Estado") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </section>

    <section class="panel hospital-table-panel" style="margin-top: 22px;">
        <div class="panel-heading">
            <div>
                <h2>Horarios disponibles: Medicina General</h2>
                <p>
                    La jornada disponible es de 7:00 a. m. a 4:00 p. m.
                </p>
            </div>
        </div>

        <div class="schedule-filter">
            <div class="portal-field">
                <label class="portal-label">
                    <i class="bi bi-calendar-date"></i>
                    Fecha para consultar
                </label>

                <asp:TextBox ID="txtFechaDisponibilidad" runat="server"
                    CssClass="portal-input"
                    TextMode="Date">
                </asp:TextBox>
            </div>

            <asp:Button ID="btnConsultarHorarios" runat="server"
                Text="Ver horarios disponibles"
                CssClass="btn-primary-portal"
                OnClick="btnConsultarHorarios_Click" />
        </div>

        <div class="panel-heading">
            <div>
                <p>
                    Disponibilidad para:
                    <strong>
                        <asp:Label ID="lblFechaConsultada"
                            runat="server">
                        </asp:Label>
                    </strong>
                </p>
            </div>
        </div>

        <div class="table-responsive">
           <asp:GridView ID="gvHorariosDisponibles" runat="server"
                AutoGenerateColumns="False"
                DataKeyNames="IdMedico,Hora"
                CssClass="portal-table hospital-grid"
                OnRowCommand="gvHorariosDisponibles_RowCommand"
                EmptyDataText="No hay horarios disponibles de Medicina General para esta fecha.">

                <Columns>
                    <asp:TemplateField HeaderText="Hora disponible">
                        <ItemTemplate>
                            <span class="appointment-time">
                                <i class="bi bi-clock-history"></i>
                                <%# Eval("Hora") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Médico">
                        <ItemTemplate>
                            <strong><%# Eval("Medico") %></strong>
                            <br />
                            <small>Medicina General</small>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="Hospital"
                        HeaderText="Hospital" />
                    <asp:TemplateField HeaderText="Acción">

                    <ItemTemplate>
                        <asp:LinkButton ID="btnAgendar" runat="server"
                            CommandName="Agendar"
                            CommandArgument="<%# Container.DataItemIndex %>"
                            CssClass="btn-primary-portal"
                            OnClientClick="return confirmarAccion(this, '¿Desea agendar esta cita?');">

                            <i class="bi bi-calendar2-plus"></i>
                            Agendar cita
                        </asp:LinkButton>
                    </ItemTemplate>
</asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </section>

</asp:Content>

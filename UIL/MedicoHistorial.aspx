<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Portal.Master"
    CodeBehind="MedicoHistorial.aspx.cs"
    Inherits="UIL.MedicoHistorial" 
    MaintainScrollPositionOnPostBack="true"%>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="eyebrow">Médico / Historial</div>
    <h1 class="page-title">Historial de citas</h1>

    <section class="history-hero">
        <div>
            <span class="badge-soft">
                <i class="bi bi-clock-history"></i>
                Bitácora clínica
            </span>

            <h2>Actividad de atención médica</h2>

            <p>
                Consulte citas finalizadas y canceladas, organizadas
                por fecha, paciente y estado.
            </p>
        </div>

        <div class="history-hero-icon">
            <i class="bi bi-journal-medical"></i>
        </div>
    </section>

    <asp:Panel ID="pnlMensaje" runat="server"
        Visible="false"
        CssClass="empty-state"
        style="margin-top: 20px;">

        <asp:Label ID="lblMensaje" runat="server"></asp:Label>
    </asp:Panel>

    <section class="history-stat-grid">
        <article class="history-stat">
            <i class="bi bi-check2-circle"></i>

            <div>
                <span>Citas finalizadas</span>
                <strong>
                    <asp:Label ID="lblFinalizadas" runat="server"
                        Text="0">
                    </asp:Label>
                </strong>
            </div>
        </article>

        <article class="history-stat">
            <i class="bi bi-x-circle"></i>

            <div>
                <span>Citas canceladas</span>
                <strong>
                    <asp:Label ID="lblCanceladas" runat="server"
                        Text="0">
                    </asp:Label>
                </strong>
            </div>
        </article>
    </section>

    <section class="panel hospital-table-panel">
        <div class="panel-heading">
            <div>
                <h2>Consultar historial</h2>
                <p>
                    Deje los filtros vacíos para ver todo su historial.
                </p>
            </div>
        </div>

        <div class="doctor-filter">
            <div class="portal-field doctor-filter-search">
                <label class="portal-label">Paciente</label>

                <asp:TextBox ID="txtFiltro" runat="server"
                    CssClass="portal-input"
                    MaxLength="100"
                    placeholder="Nombre, apellido o cédula">
                </asp:TextBox>
            </div>

            <div class="portal-field">
                <label class="portal-label">Estado</label>

                <asp:DropDownList ID="ddlEstado" runat="server"
                    CssClass="portal-select">

                    <asp:ListItem Text="Todos los estados"
                        Value="Todos"></asp:ListItem>

                    <asp:ListItem Text="Finalizada"
                        Value="Finalizada"></asp:ListItem>

                    <asp:ListItem Text="Cancelada"
                        Value="Cancelada"></asp:ListItem>
                </asp:DropDownList>
            </div>

            <div class="portal-field">
                <label class="portal-label">Fecha inicial</label>

                <asp:TextBox ID="txtFechaInicio" runat="server"
                    CssClass="portal-input"
                    TextMode="Date">
                </asp:TextBox>
            </div>

            <div class="portal-field">
                <label class="portal-label">Fecha final</label>

                <asp:TextBox ID="txtFechaFin" runat="server"
                    CssClass="portal-input"
                    TextMode="Date">
                </asp:TextBox>
            </div>

            <asp:Button ID="btnConsultar" runat="server"
                Text="Consultar"
                CssClass="btn-primary-portal"
                OnClick="btnConsultar_Click" />
        </div>

        <div class="table-responsive">
            <asp:GridView ID="gvHistorial" runat="server"
                AutoGenerateColumns="False"
                DataKeyNames="IdCita"
                CssClass="portal-table hospital-grid"
                OnRowCommand="gvHistorial_RowCommand"
                EmptyDataText="No hay citas en el historial con los filtros seleccionados.">

                <Columns>
                    <asp:BoundField DataField="Fecha"
                        HeaderText="Fecha"
                        DataFormatString="{0:dd/MM/yyyy}" />

                    <asp:BoundField DataField="Hora"
                        HeaderText="Hora" />

                    <asp:TemplateField HeaderText="Paciente">
                        <ItemTemplate>
                            <strong><%# Eval("NombrePaciente") %></strong>
                            <br />
                            <small><%# Eval("Cedula") %></small>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Estado">
                        <ItemTemplate>
                            <span class="<%# ClaseEstado(Eval("Estado").ToString()) %>">
                                <%# Eval("Estado") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Diagnóstico">
                        <ItemTemplate>
                            <span class="history-diagnosis">
                                <%# String.IsNullOrWhiteSpace(
                                    Eval("Diagnostico").ToString())
                                    ? "Sin diagnóstico registrado"
                                    : Eval("Diagnostico") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Detalle">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnVerAtencion"
                                runat="server"
                                CommandName="VerAtencion"
                                CommandArgument="<%# Container.DataItemIndex %>"
                                Visible='<%# Eval("Estado").ToString() == "Finalizada" %>'
                                CssClass="btn-outline-portal">

                                <i class="bi bi-file-earmark-medical"></i>
                                Ver atención
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </section>

    <asp:Panel ID="pnlDetalleCita" runat="server"
        Visible="false"
        CssClass="panel"
        style="margin-top: 22px;">

        <div class="panel-heading">
            <div>
                <h2>Detalle clínico de la cita</h2>
                <p>Tratamientos, medicamentos y prescripciones registrados.</p>
            </div>

            <asp:Button ID="btnCerrarDetalle" runat="server"
                Text="Cerrar"
                CssClass="btn-outline-portal"
                CausesValidation="false"
                OnClick="btnCerrarDetalle_Click" />
        </div>

        <div class="clinical-meta">
            <span>
                <i class="bi bi-person-vcard"></i>
                <asp:Label ID="lblPacienteDetalle" runat="server"></asp:Label>
            </span>

            <span>
                <i class="bi bi-calendar3"></i>
                <asp:Label ID="lblFechaDetalle" runat="server"></asp:Label>
            </span>

            <span>
                <i class="bi bi-clock"></i>
                <asp:Label ID="lblHoraDetalle" runat="server"></asp:Label>
            </span>
        </div>

        <div class="clinical-diagnosis">
            <strong>Diagnóstico</strong>
            <asp:Literal ID="litDiagnostico" runat="server"></asp:Literal>
        </div>

        <div class="table-responsive">
            <asp:GridView ID="gvTratamientos" runat="server"
                AutoGenerateColumns="False"
                CssClass="portal-table hospital-grid"
                EmptyDataText="No hay tratamientos registrados para esta cita.">

                <Columns>
                    <asp:BoundField DataField="Tratamiento"
                        HeaderText="Tratamiento" />

                    <asp:TemplateField HeaderText="Costo">
                        <ItemTemplate>
                            <%# String.Format(
                                new System.Globalization.CultureInfo("es-CR"),
                                "₡ {0:N2}",
                                Eval("CostoTratamiento")) %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="Medicamento"
                        HeaderText="Medicamento" />

                    <asp:BoundField DataField="Dosis"
                        HeaderText="Dosis" />

                    <asp:BoundField DataField="Cantidad"
                        HeaderText="Cantidad" />
                </Columns>
            </asp:GridView>
        </div>
    </asp:Panel>

</asp:Content>
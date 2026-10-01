<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Portal.Master"
    CodeBehind="PacienteExpediente.aspx.cs"
    Inherits="UIL.PacienteExpediente" 
    MaintainScrollPositionOnPostBack="true"%>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="eyebrow">Paciente / Expediente</div>
    <h1 class="page-title">Mi expediente personal</h1>

    <section class="record-hero">
        <span class="record-chip">
            <i class="bi bi-shield-check"></i>
            Información clínica personal
        </span>

        <h2>Historial de atención médica</h2>

        <p>
            Consulte sus citas pasadas, tratamientos, medicamentos
            y prescripciones registrados por el hospital.
        </p>
    </section>

    <section class="record-stat-grid">
        <article class="record-stat">
            <i class="bi bi-calendar2-check"></i>
            <span>Citas pasadas</span>
            <strong>
                <asp:Label ID="lblTotalCitas" runat="server"
                    Text="0"></asp:Label>
            </strong>
        </article>

        <article class="record-stat">
            <i class="bi bi-clipboard2-pulse"></i>
            <span>Tratamientos</span>
            <strong>
                <asp:Label ID="lblTotalTratamientos" runat="server"
                    Text="0"></asp:Label>
            </strong>
        </article>

        <article class="record-stat">
            <i class="bi bi-capsule-pill"></i>
            <span>Medicamentos indicados</span>
            <strong>
                <asp:Label ID="lblTotalMedicamentos" runat="server"
                    Text="0"></asp:Label>
            </strong>
        </article>
    </section>

    <asp:Panel ID="pnlMensaje" runat="server"
        Visible="false"
        CssClass="empty-state">

        <asp:Label ID="lblMensaje" runat="server"></asp:Label>
    </asp:Panel>

    <section class="panel hospital-table-panel">
        <div class="panel-heading">
            <div>
                <h2>Historial de citas pasadas</h2>
                <p>Seleccione una cita para ver su información clínica.</p>
            </div>
        </div>

        <div class="table-responsive">
            <asp:GridView ID="gvCitas" runat="server"
                AutoGenerateColumns="False"
                DataKeyNames="IdCita"
                CssClass="portal-table hospital-grid"
                OnRowCommand="gvCitas_RowCommand"
                EmptyDataText="Aún no tiene citas finalizadas o canceladas.">

                <Columns>
                    <asp:BoundField DataField="Fecha"
                        HeaderText="Fecha"
                        DataFormatString="{0:dd/MM/yyyy}" />

                    <asp:BoundField DataField="Hora"
                        HeaderText="Hora" />

                    <asp:TemplateField HeaderText="Médico">
                        <ItemTemplate>
                            <strong><%# Eval("Medico") %></strong>
                            <br />
                            <small><%# Eval("Especialidad") %></small>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Estado">
                        <ItemTemplate>
                            <span class="<%# ClaseEstado(Eval("Estado").ToString()) %>">
                                <%# Eval("Estado") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Expediente">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnVerDetalle"
                                runat="server"
                                CommandName="VerDetalle"
                                CommandArgument="<%# Container.DataItemIndex %>"
                                CssClass="btn-outline-portal">

                                <i class="bi bi-file-earmark-medical"></i>
                                Ver expediente
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </section>

    <asp:Panel ID="pnlDetalleCita" runat="server"
        Visible="false"
        CssClass="panel record-detail"
        style="margin-top: 22px;">

        <div class="panel-heading">
            <div>
                <h2>Detalle de la cita</h2>
                <p>Información clínica relacionada con la cita seleccionada.</p>
            </div>

            <asp:Button ID="btnCerrarDetalle" runat="server"
                Text="Cerrar detalle"
                CssClass="btn-outline-portal"
                CausesValidation="false"
                OnClick="btnCerrarDetalle_Click" />
        </div>

        <div class="record-meta">
            <span>
                <i class="bi bi-calendar3"></i>
                <asp:Label ID="lblDetalleFecha" runat="server"></asp:Label>
            </span>

            <span>
                <i class="bi bi-clock"></i>
                <asp:Label ID="lblDetalleHora" runat="server"></asp:Label>
            </span>

            <span>
                <i class="bi bi-person-badge"></i>
                <asp:Label ID="lblDetalleMedico" runat="server"></asp:Label>
            </span>

            <asp:Label ID="lblDetalleEstado" runat="server"></asp:Label>
        </div>

        <div class="record-diagnosis">
            <strong><i class="bi bi-clipboard2-pulse"></i> Diagnóstico</strong>
            <asp:Literal ID="litDiagnostico" runat="server"></asp:Literal>
        </div>

        <h3 class="section-subtitle">Tratamientos, medicamentos y prescripciones</h3>

        <div class="table-responsive">
            <asp:GridView ID="gvDetalleTratamientos" runat="server"
                AutoGenerateColumns="False"
                CssClass="portal-table hospital-grid"
                EmptyDataText="No hay tratamientos registrados en esta cita.">

                <Columns>
                    <asp:BoundField DataField="Tratamiento"
                        HeaderText="Tratamiento" />

                    <asp:TemplateField HeaderText="Costo">
                        <ItemTemplate>
                            <span class="medication-price">
                                <i class="bi bi-currency-exchange"></i>
                                <%# System.String.Format(
                                    new System.Globalization.CultureInfo("es-CR"),
                                    "₡ {0:N2}",
                                    Eval("CostoTratamiento")) %>
                            </span>
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

    <section class="panel hospital-table-panel" style="margin-top: 22px;">
        <div class="panel-heading">
            <div>
                <h2>Mis tratamientos y medicamentos</h2>
                <p>
                    Consulte los tratamientos de sus citas finalizadas
                    y las prescripciones relacionadas.
                </p>
            </div>
        </div>

        <div class="table-responsive">
            <asp:GridView ID="gvMisTratamientos" runat="server"
                AutoGenerateColumns="False"
                CssClass="portal-table hospital-grid"
                EmptyDataText="Aún no tiene tratamientos o medicamentos registrados.">

                <Columns>
                    <asp:BoundField DataField="FechaCita"
                        HeaderText="Fecha de cita"
                        DataFormatString="{0:dd/MM/yyyy}" />

                    <asp:BoundField DataField="Tratamiento"
                        HeaderText="Tratamiento" />

                    <asp:BoundField DataField="Medicamento"
                        HeaderText="Medicamento" />

                    <asp:BoundField DataField="Dosis"
                        HeaderText="Dosis" />

                    <asp:BoundField DataField="Cantidad"
                        HeaderText="Cantidad" />

                    <asp:TemplateField HeaderText="Costo del tratamiento">
                        <ItemTemplate>
                            <span class="medication-price">
                                <%# System.String.Format(
                                    new System.Globalization.CultureInfo("es-CR"),
                                    "₡ {0:N2}",
                                    Eval("CostoTratamiento")) %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </section>

</asp:Content>
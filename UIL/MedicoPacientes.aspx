<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Portal.Master"
    CodeBehind="MedicoPacientes.aspx.cs"
    Inherits="UIL.MedicoPacientes" 
    MaintainScrollPositionOnPostBack="true"%>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="eyebrow">Médico / Pacientes</div>
    <h1 class="page-title">Pacientes atendidos</h1>

    <section class="doctor-record-hero">
        <div>
            <span class="badge-soft">
                <i class="bi bi-people-fill"></i>
                Historial clínico
            </span>

            <h2>Consulte las atenciones registradas</h2>

            <p>
                Seleccione un período para revisar pacientes atendidos,
                diagnósticos, tratamientos y prescripciones.
            </p>
        </div>

        <div class="doctor-record-icon">
            <i class="bi bi-clipboard2-pulse"></i>
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
                <h2>Período de consulta</h2>
                <p>
                    Se muestran únicamente pacientes atendidos
                    por su cuenta médica.
                </p>
            </div>
        </div>
<div class="doctor-filter">

    <div class="portal-field doctor-filter-search">
        <label class="portal-label">
            Buscar paciente
        </label>

        <asp:TextBox ID="txtFiltro" runat="server"
            CssClass="portal-input"
            MaxLength="100"
            placeholder="Nombre, apellido o cédula">
        </asp:TextBox>
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
        Text="Buscar pacientes"
        CssClass="btn-primary-portal"
        OnClick="btnConsultar_Click" />
</div>

        <div class="table-responsive">
            <asp:GridView ID="gvPacientesAtendidos" runat="server"
                AutoGenerateColumns="False"
                DataKeyNames="IdPaciente"
                CssClass="portal-table hospital-grid"
                OnRowCommand="gvPacientesAtendidos_RowCommand"
                EmptyDataText="No hay pacientes atendidos en el período seleccionado.">

                <Columns>
                    <asp:BoundField DataField="FechaUltimaAtencion"
                        HeaderText="Última atención"
                        DataFormatString="{0:dd/MM/yyyy}" />

                    <asp:TemplateField HeaderText="Paciente">
                        <ItemTemplate>
                            <strong>
                                <%# Eval("Nombre") %>
                                <%# Eval("Apellido") %>
                            </strong>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="Cedula"
                        HeaderText="Cédula" />

                    <asp:TemplateField HeaderText="Expediente">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnVerAtencion"
                                runat="server"
                                CommandName="VerCitas"
                                CommandArgument="<%# Container.DataItemIndex %>"
                                CssClass="btn-outline-portal">

                                <i class="bi bi-file-earmark-medical"></i>
                                Ver citas
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </section>

    <asp:Panel ID="pnlDetalleAtencion" runat="server"
    Visible="false"
    CssClass="panel"
    style="margin-top: 22px;">

    <div class="panel-heading">
        <div>
            <h2>Historial de citas del paciente</h2>
            <p>
                Seleccione una cita para consultar su diagnóstico,
                tratamientos y prescripciones.
            </p>
        </div>

        <asp:Button ID="btnCerrarDetalle" runat="server"
            Text="Cerrar"
            CssClass="btn-outline-portal"
            CausesValidation="false"
            OnClick="btnCerrarDetalle_Click" />
    </div>

    <div class="doctor-patient-heading">
        <i class="bi bi-person-vcard"></i>

        <div>
            <strong>
                <asp:Label ID="lblPacienteDetalle" runat="server"></asp:Label>
            </strong>

            <span>
                Citas atendidas por este médico
            </span>
        </div>
    </div>

    <div class="table-responsive">
        <asp:GridView ID="gvCitasPaciente" runat="server"
            AutoGenerateColumns="False"
            DataKeyNames="IdCita"
            CssClass="portal-table hospital-grid"
            OnRowCommand="gvCitasPaciente_RowCommand"
            EmptyDataText="No hay citas finalizadas para este paciente.">

            <Columns>
                <asp:BoundField DataField="Fecha"
                    HeaderText="Fecha"
                    DataFormatString="{0:dd/MM/yyyy}" />

                <asp:BoundField DataField="Hora"
                    HeaderText="Hora" />

                <asp:BoundField DataField="Diagnostico"
                    HeaderText="Diagnóstico" />

                <asp:TemplateField HeaderText="Detalle clínico">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnVerDetalleCita"
                            runat="server"
                            CommandName="VerDetalleCita"
                            CommandArgument="<%# Container.DataItemIndex %>"
                            CssClass="btn-outline-portal">

                            <i class="bi bi-clipboard2-pulse"></i>
                            Ver tratamiento
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <asp:Panel ID="pnlTratamientosCita" runat="server"
        Visible="false"
        style="margin-top: 22px;">

        <div class="clinical-meta">
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
            <strong>
                <i class="bi bi-clipboard2-pulse"></i>
                Diagnóstico
            </strong>

            <asp:Literal ID="litDiagnostico" runat="server"></asp:Literal>
        </div>

        <div class="table-responsive">
            <asp:GridView ID="gvTratamientosPrescripciones"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="portal-table hospital-grid"
                EmptyDataText="No hay tratamientos registrados para esta cita.">

                <Columns>
                    <asp:BoundField DataField="Tratamiento"
                        HeaderText="Tratamiento" />

                    <asp:TemplateField HeaderText="Costo">
                        <ItemTemplate>
                            <%# System.String.Format(
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

</asp:Panel>
</asp:Content>
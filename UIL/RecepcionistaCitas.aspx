<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Portal.Master"
    CodeBehind="RecepcionistaCitas.aspx.cs"
    Inherits="UIL.RecepcionistaCitas"
    MaintainScrollPositionOnPostBack="true" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="eyebrow">Recepción / Citas</div>
    <h1 class="page-title">Crear cita médica</h1>

    <section class="reception-appointment-hero">
        <div>
            <span class="badge-soft">
                <i class="bi bi-calendar2-plus"></i>
                Agenda hospitalaria
            </span>

            <h2>Disponibilidad por especialidad</h2>

            <p>
                Busque al paciente, seleccione una especialidad
                y asigne un horario disponible con el médico.
            </p>
        </div>

        <div class="reception-appointment-hero-icon">
            <i class="bi bi-calendar2-week"></i>
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

        <div class="appointment-search-layout">
            <div>
                <label class="portal-label">Cédula</label>

                <asp:TextBox ID="txtCedula" runat="server"
                    CssClass="portal-input"
                    MaxLength="9"
                    inputmode="numeric"
                    placeholder="Ejemplo: 102220002"
                    oninput="this.value=this.value.replace(/[^0-9]/g, '');">
                </asp:TextBox>
            </div>

            <div></div>

            <asp:Button ID="btnBuscarPaciente" runat="server"
                Text="Buscar paciente"
                CssClass="btn-primary-portal"
                OnClick="btnBuscarPaciente_Click" />
        </div>
    </section>

    <asp:Panel ID="pnlAgenda" runat="server"
        Visible="false">

        <asp:HiddenField ID="hdnIdPaciente" runat="server" />
        <asp:HiddenField ID="hdnIdHospital" runat="server" />

        <section class="appointment-patient-banner">
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
            <asp:Panel ID="pnlUltimaCita" runat="server"
                    Visible="false">

                    <section class="appointment-history-card">
                        <div class="appointment-history-head">
                            <i class="bi bi-clock-history"></i>

                            <div>
                                <strong>Última cita en esta especialidad</strong>

                                <span>
                                    Se muestra la última atención finalizada del paciente.
                                </span>
                            </div>
                        </div>

                        <div class="appointment-history-body">
                            <div class="appointment-history-item">
                                <span>Fecha</span>

                                <strong>
                                    <asp:Label ID="lblFechaUltimaCita"
                                        runat="server">
                                    </asp:Label>
                                </strong>
                            </div>

                            <div class="appointment-history-item">
                                <span>Hora</span>

                                <strong>
                                    <asp:Label ID="lblHoraUltimaCita"
                                        runat="server">
                                    </asp:Label>
                                </strong>
                            </div>

                            <div class="appointment-history-item">
                                <span>Médico</span>

                                <strong>
                                    <asp:Label ID="lblMedicoUltimaCita"
                                        runat="server">
                                    </asp:Label>
                                </strong>
                            </div>

                            <div class="appointment-history-item">
                                <span>Hospital</span>

                                <strong>
                                    <asp:Label ID="lblHospitalUltimaCita"
                                        runat="server">
                                    </asp:Label>
                                </strong>
                            </div>

                            <div class="appointment-history-item">
                                <span>Estado</span>

                                <strong>
                                    <asp:Label ID="lblEstadoUltimaCita"
                                        runat="server">
                                    </asp:Label>
                                </strong>
                            </div>

                            <div class="appointment-history-item full">
                                <span>Diagnóstico</span>

                                <strong>
                                    <asp:Label ID="lblDiagnosticoUltimaCita"
                                        runat="server">
                                    </asp:Label>
                                </strong>
                            </div>
                        </div>

                        <div class="table-responsive">
                            <asp:GridView ID="gvTratamientosPrevios"
                                runat="server"
                                AutoGenerateColumns="False"
                                CssClass="portal-table hospital-grid"
                                EmptyDataText="La cita no tiene tratamientos registrados.">

                                <Columns>
                                    <asp:BoundField DataField="Descripcion"
                                        HeaderText="Tratamiento" />

                                    <asp:TemplateField HeaderText="Costo">
                                        <ItemTemplate>
                                            <%# System.String.Format(
                                                new System.Globalization.CultureInfo("es-CR"),
                                                "₡ {0:N2}",
                                                Eval("Costo")) %>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="Medicamentos"
                                        HeaderText="Medicamentos y prescripción" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </section>
                </asp:Panel>
            <div class="panel-heading">
                <div>
                    <h2>Consultar disponibilidad</h2>
                    <p>La última cita inicia a las 3:00 p. m.</p>
                </div>
            </div>

            <div class="appointment-search-layout">
                <div>
                    <label class="portal-label">Fecha de cita</label>

                    <asp:TextBox ID="txtFechaCita" runat="server"
                        CssClass="portal-input"
                        TextMode="Date">
                    </asp:TextBox>
                </div>

                <div>
                    <label class="portal-label">Especialidad</label>

                    <asp:DropDownList ID="ddlEspecialidad"
                        runat="server"
                        CssClass="portal-select">
                    </asp:DropDownList>
                </div>

                <asp:Button ID="btnConsultarDisponibilidad"
                    runat="server"
                    Text="Ver horarios"
                    CssClass="btn-primary-portal"
                    OnClick="btnConsultarDisponibilidad_Click" />
            </div>

            <div class="table-responsive" style="margin-top: 22px;">
                <asp:GridView ID="gvHorarios" runat="server"
                    AutoGenerateColumns="False"
                    DataKeyNames="Hora"
                    OnRowDataBound="gvHorarios_RowDataBound"
                    CssClass="portal-table hospital-grid"
                    OnRowCommand="gvHorarios_RowCommand"
                    EmptyDataText="No hay horarios disponibles para esa especialidad y fecha.">

                    <Columns>
                        <asp:TemplateField HeaderText="Hora">
                            <ItemTemplate>
                                <span class="appointment-time">
                                    <i class="bi bi-clock"></i>
                                    <%# Eval("Hora") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Médico disponible">
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlMedicoDisponible"
                                    runat="server"
                                    CssClass="doctor-choice">
                                </asp:DropDownList>

                                <br />

                                <span class="doctor-specialty">
                                    <i class="bi bi-heart-pulse"></i>
                                    <%# Eval("Especialidad") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="Hospital"
                            HeaderText="Hospital" />

                        <asp:TemplateField HeaderText="Acción">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnAgendar"
                                    runat="server"
                                    CommandName="Agendar"
                                    CommandArgument="<%# Container.DataItemIndex %>"
                                    CssClass="btn-primary-portal"
                                    OnClientClick="return confirmarAccion(this, '¿Desea crear esta cita?');">

                                    <i class="bi bi-calendar2-check"></i>
                                    Agendar
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </section>
    </asp:Panel>

</asp:Content>

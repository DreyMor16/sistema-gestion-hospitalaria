<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Portal.Master"
    CodeBehind="MedicoCita.aspx.cs"
    Inherits="UIL.MedicoCita"
    MaintainScrollPositionOnPostBack="true"%>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="eyebrow">Médico / Atención</div>
    <h1 class="page-title">Próximas citas</h1>

    <section class="consultation-hero">
        <div>
            <span class="badge-soft">
                <i class="bi bi-clipboard2-pulse"></i>
                Atención clínica
            </span>

            <h2>Registre tratamientos y prescripciones</h2>

            <p>
                Atienda las citas pendientes, agregue diagnóstico,
                tratamientos y medicamentos disponibles para el paciente.
            </p>
        </div>

        <div class="consultation-hero-icon">
            <i class="bi bi-heart-pulse"></i>
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
                <h2>Citas pendientes de atención</h2>
                <p>Seleccione una cita para iniciar la atención clínica.</p>
            </div>
        </div>

        <div class="table-responsive">
            <asp:GridView ID="gvProximasCitas" runat="server"
                AutoGenerateColumns="False"
                DataKeyNames="IdCita"
                CssClass="portal-table hospital-grid"
                OnRowCommand="gvProximasCitas_RowCommand"
                EmptyDataText="No tiene citas pendientes de atención.">

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

                    <asp:TemplateField HeaderText="Atención">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnAtender" runat="server"
                                CommandName="Atender"
                                CommandArgument="<%# Container.DataItemIndex %>"
                                CssClass="btn-primary-portal">

                                <i class="bi bi-clipboard2-check"></i>
                                Atender
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </section>

    <asp:Panel ID="pnlAtencion" runat="server"
        Visible="false"
        CssClass="panel"
        style="margin-top: 22px;">

        <div class="panel-heading">
            <div>
                <h2>Atención de la cita</h2>
                <p>Los medicamentos se validan contra el stock del hospital del paciente.</p>
            </div>
        </div>

        <asp:HiddenField ID="hdnIdCita" runat="server" />
        <asp:HiddenField ID="hdnIdTratamiento" runat="server" />

        <div class="consultation-patient-card">
            <i class="bi bi-person-vcard"></i>

            <div>
                <strong>
                    <asp:Label ID="lblPaciente" runat="server"></asp:Label>
                </strong>

                <span>
                    <asp:Label ID="lblDatosCita" runat="server"></asp:Label>
                </span>

                <br />

                <small>
                    Hospital:
                    <asp:Label ID="lblHospitalPaciente"
                        runat="server">
                    </asp:Label>
                </small>
            </div>
        </div>

        <div class="form-grid">
            <div class="full">
                <label class="portal-label">Diagnóstico</label>

                <asp:TextBox ID="txtDiagnostico" runat="server"
                    CssClass="portal-input"
                    TextMode="MultiLine"
                    Rows="3"
                    MaxLength="255"
                    placeholder="Diagnóstico clínico de la atención">
                </asp:TextBox>
            </div>

            <div class="full">
                <asp:Button ID="btnGuardarDiagnostico" runat="server"
                    Text="Guardar diagnóstico"
                    CssClass="btn-outline-portal"
                    OnClick="btnGuardarDiagnostico_Click" />
            </div>
        </div>

        <hr />

        <h3 class="section-subtitle">Agregar tratamiento</h3>

        <p class="consultation-step-note">
            Guarde primero el diagnóstico. El costo se calcula automáticamente
            con las prescripciones que agregue al tratamiento.
        </p>

        <div class="form-grid">
            <div>
                <label class="portal-label">Descripción</label>

                <asp:TextBox ID="txtDescripcionTratamiento"
                    runat="server"
                    CssClass="portal-input"
                    MaxLength="500"
                    placeholder="Ejemplo: Terapia respiratoria">
                </asp:TextBox>
            </div>

            <div class="full">
                <asp:Button ID="btnAgregarTratamiento" runat="server"
                    Text="Agregar tratamiento"
                    CssClass="btn-primary-portal"
                    OnClick="btnAgregarTratamiento_Click" />
            </div>
        </div>

        <div class="table-responsive" style="margin-top: 18px;">
            <asp:GridView ID="gvTratamientos" runat="server"
                AutoGenerateColumns="False"
                DataKeyNames="IdTratamiento"
                CssClass="portal-table hospital-grid"
                OnRowCommand="gvTratamientos_RowCommand"
                EmptyDataText="Aún no hay tratamientos registrados.">

                <Columns>
                    <asp:BoundField DataField="Descripcion"
                        HeaderText="Tratamiento" />

                    <asp:TemplateField HeaderText="Costo">
                        <ItemTemplate>
                            <%# String.Format(
                                new System.Globalization.CultureInfo("es-CR"),
                                "₡ {0:N2}",
                                Eval("Costo")) %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Prescripción">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnPrescribir" runat="server"
                                CommandName="Prescribir"
                                CommandArgument="<%# Container.DataItemIndex %>"
                                CssClass="btn-outline-portal">

                                <i class="bi bi-capsule-pill"></i>
                                Prescribir medicamento
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>

        <asp:Panel ID="pnlPrescripcion" runat="server"
            Visible="false"
            CssClass="panel"
            style="margin-top: 22px;">

            <div class="panel-heading">
                <div>
                    <h2>Registrar prescripción</h2>
                    <p>Solo se muestran medicamentos disponibles para este paciente.</p>
                </div>
            </div>

            <div class="form-grid">
                <div class="full">
                    <label class="portal-label">Medicamento disponible</label>

                    <asp:DropDownList ID="ddlMedicamento" runat="server"
                        CssClass="portal-select">
                    </asp:DropDownList>
                </div>

                <div>
                    <label class="portal-label">Cantidad</label>

                    <asp:TextBox ID="txtCantidad" runat="server"
                        CssClass="portal-input"
                        TextMode="Number"
                        min="1"
                        inputmode="numeric"
                        oninput="this.value=this.value.replace(/[^0-9]/g, '');">
                    </asp:TextBox>
                </div>

                <div>
                    <label class="portal-label">Dosis</label>

                    <asp:TextBox ID="txtDosis" runat="server"
                        CssClass="portal-input"
                        MaxLength="100"
                        placeholder="Ejemplo: 1 tableta cada 8 horas">
                    </asp:TextBox>
                </div>

                <div class="full">
                    <asp:Button ID="btnRegistrarPrescripcion"
                        runat="server"
                        Text="Registrar prescripción"
                        CssClass="btn-primary-portal"
                        OnClick="btnRegistrarPrescripcion_Click" />
                </div>
            </div>
        </asp:Panel>

        <h3 class="section-subtitle" style="margin-top: 24px;">
            Prescripciones registradas
        </h3>

        <div class="table-responsive">
            <asp:GridView ID="gvPrescripciones" runat="server"
                AutoGenerateColumns="False"
                CssClass="portal-table hospital-grid"
                EmptyDataText="Aún no hay prescripciones registradas.">

                <Columns>
                    <asp:BoundField DataField="Tratamiento"
                        HeaderText="Tratamiento" />

                    <asp:BoundField DataField="Medicamento"
                        HeaderText="Medicamento" />

                    <asp:BoundField DataField="Dosis"
                        HeaderText="Dosis" />

                    <asp:BoundField DataField="Cantidad"
                        HeaderText="Cantidad" />

                    <asp:TemplateField HeaderText="Costo unitario">
                        <ItemTemplate>
                            <%# String.Format(
                                new System.Globalization.CultureInfo("es-CR"),
                                "₡ {0:N2}",
                                Eval("CostoUnitario")) %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Subtotal">
                        <ItemTemplate>
                            <%# String.Format(
                                new System.Globalization.CultureInfo("es-CR"),
                                "₡ {0:N2}",
                                Eval("Subtotal")) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>

        <div class="consultation-finish">
            <div>
                <strong>Finalizar atención</strong>
                <span>Revise el diagnóstico, tratamientos y prescripciones antes de cerrar la cita.</span>
            </div>

            <asp:Button ID="btnFinalizarCita" runat="server"
                Text="Finalizar cita"
                CssClass="btn-primary-portal"
                OnClick="btnFinalizarCita_Click"
                OnClientClick="return confirmarAccion(this, '¿Desea finalizar esta cita?');" />
        </div>
    </asp:Panel>

</asp:Content>

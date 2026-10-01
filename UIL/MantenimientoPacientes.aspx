<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Portal.Master"
    CodeBehind="MantenimientoPacientes.aspx.cs"
    Inherits="UIL.MantenimientoPacientes" 
    MaintainScrollPositionOnPostBack="true"%>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="eyebrow">Administrador / Mantenimientos</div>
    <h1 class="page-title">Mantenimiento de pacientes</h1>

    <section class="hero">
        <span class="hero-badge">
            <i class="bi bi-person-vcard"></i> Registro clínico
        </span>

        <h2>Información humana para una atención segura.</h2>

        <p>
            Registre, consulte y modifique los datos personales
            y clínicos de cada paciente.
        </p>
    </section>

    <div class="content-grid">
        <section class="panel">
            <div class="panel-heading">
                <div>
                    <h2 id="tituloFormulario" runat="server">Registrar paciente</h2>
                    <p>Los campos marcados son necesarios para crear el expediente.</p>
                </div>
            </div>

            <asp:Panel ID="pnlMensaje" runat="server"
                Visible="false"
                CssClass="empty-state">

                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <asp:HiddenField ID="hdnIdPaciente" runat="server" />
            <asp:HiddenField ID="hdnIdPersona" runat="server" />

            <div class="form-grid">
                <div>
                    <label class="portal-label">Nombre</label>
                    <asp:TextBox ID="txtNombre" runat="server"
                        CssClass="portal-input"
                        MaxLength="50"
                        placeholder="Nombre">
                    </asp:TextBox>
                </div>

                <div>
                    <label class="portal-label">Apellido</label>
                    <asp:TextBox ID="txtApellido" runat="server"
                        CssClass="portal-input"
                        MaxLength="50"
                        placeholder="Apellido">
                    </asp:TextBox>
                </div>

                <div>
                    <label class="portal-label">Cédula</label>
                    <asp:TextBox ID="txtCedula" runat="server"
                        CssClass="portal-input"
                        MaxLength="9"
                        placeholder="9 dígitos"
                        inputmode="numeric"
                        oninput="this.value=this.value.replace(/[^0-9]/g, '').slice(0, 9);">
                    </asp:TextBox>
                </div>

                <div>
                    <label class="portal-label">Teléfono</label>
                    <asp:TextBox ID="txtTelefono" runat="server"
                        CssClass="portal-input"
                        MaxLength="8"
                        placeholder="8 dígitos"
                        inputmode="numeric"
                        oninput="this.value=this.value.replace(/[^0-9]/g, '').slice(0, 8);">
                    </asp:TextBox>
                </div>

                <div>
                    <label class="portal-label">Correo</label>
                    <asp:TextBox ID="txtCorreo" runat="server"
                        CssClass="portal-input"
                        MaxLength="100"
                        TextMode="Email"
                        placeholder="correo@ejemplo.com">
                    </asp:TextBox>
                </div>

                <div>
                    <label class="portal-label">Fecha de nacimiento</label>
                    <asp:TextBox ID="txtFechaNacimiento" runat="server"
                        CssClass="portal-input"
                        TextMode="Date">
                    </asp:TextBox>
                </div>

                <div>
                    <label class="portal-label">Género</label>
                    <asp:DropDownList ID="ddlGenero" runat="server"
                        CssClass="portal-select">

                        <asp:ListItem Text="Seleccione un género" Value="" />
                        <asp:ListItem Text="Masculino" Value="Masculino" />
                        <asp:ListItem Text="Femenino" Value="Femenino" />
                        <asp:ListItem Text="Otro" Value="Otro" />
                    </asp:DropDownList>
                </div>

                <div>
                    <label class="portal-label">Hospital</label>
                    <asp:DropDownList ID="ddlHospital" runat="server"
                        CssClass="portal-select">
                    </asp:DropDownList>
                </div>

                <div class="full">
                    <label class="portal-label">Dirección</label>
                    <asp:TextBox ID="txtDireccion" runat="server"
                        CssClass="portal-input"
                        MaxLength="200"
                        TextMode="MultiLine"
                        Rows="2"
                        placeholder="Dirección del paciente">
                    </asp:TextBox>
                </div>

                <div class="full">
                    <asp:Button ID="btnGuardar" runat="server"
                        Text="Guardar paciente"
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
                    <h2>Acciones disponibles</h2>
                    <p>Este módulo conserva el expediente del paciente.</p>
                </div>
            </div>

            <div class="info-list">
                <div class="info-row">
                    <span><i class="bi bi-person-plus"></i> Crear</span>
                    <strong>Registra Persona y Paciente</strong>
                </div>

                <div class="info-row">
                    <span><i class="bi bi-search"></i> Consultar</span>
                    <strong>Busca por datos personales</strong>
                </div>

                <div class="info-row">
                    <span><i class="bi bi-pencil-square"></i> Modificar</span>
                    <strong>Actualiza el expediente</strong>
                </div>

                <div class="info-row">
                    <span><i class="bi bi-shield-check"></i> Protección</span>
                    <strong>No se permite eliminar pacientes</strong>
                </div>
            </div>
        </section>
    </div>

    <section class="panel hospital-table-panel" style="margin-top: 22px;">
        <div class="panel-heading">
            <div>
                <h2>Pacientes registrados</h2>
                <p>Filtre por hospital o busque por información personal.</p>
            </div>

            <span class="badge-soft">
                <asp:Label ID="lblResultados" runat="server"></asp:Label>
            </span>
        </div>

        <div class="hospital-search">
            <div class="hospital-search-field">
                <i class="bi bi-search"></i>

                <asp:TextBox ID="txtFiltro" runat="server"
                    CssClass="portal-input"
                    placeholder="Nombre, apellido, correo, cédula, teléfono o usuario"
                    AutoPostBack="true"
                    OnTextChanged="txtFiltro_TextChanged">
                </asp:TextBox>
            </div>

            <asp:DropDownList ID="ddlFiltroHospital" runat="server"
                CssClass="portal-select"
                AutoPostBack="true"
                OnSelectedIndexChanged="ddlFiltroHospital_SelectedIndexChanged">
            </asp:DropDownList>

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
            <asp:GridView ID="gvPacientes" runat="server"
                AutoGenerateColumns="False"
                DataKeyNames="IdPaciente"
                CssClass="portal-table hospital-grid"
                OnRowCommand="gvPacientes_RowCommand"
                EmptyDataText="No se encontraron pacientes con esos filtros.">

                <Columns>
                    <asp:TemplateField HeaderText="Paciente">
                        <ItemTemplate>
                            <div class="hospital-name">
                                <span class="hospital-icon">
                                    <i class="bi bi-person-heart"></i>
                                </span>

                                <div>
                                    <strong>
                                        <%# Eval("Nombre") %> <%# Eval("Apellido") %>
                                    </strong>

                                    <small class="patient-user">
                                        <%# Eval("NombreUsuario") %>
                                    </small>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="NombreHospital"
                        HeaderText="Hospital" />

                    <asp:TemplateField HeaderText="Identificación">
                        <ItemTemplate>
                            <span class="hospital-detail">
                                <i class="bi bi-person-vcard"></i>
                                <%# Eval("Cedula") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Contacto">
                        <ItemTemplate>
                            <div class="patient-contact">
                                <span><i class="bi bi-envelope"></i> <%# Eval("Correo") %></span>
                                <span><i class="bi bi-telephone"></i> <%# Eval("Telefono") %></span>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Información clínica">
                        <ItemTemplate>
                            <div class="patient-contact">
                                <span><i class="bi bi-calendar3"></i> <%# Eval("FechaNacimiento", "{0:dd/MM/yyyy}") %></span>
                                <span><i class="bi bi-gender-ambiguous"></i> <%# Eval("Genero") %></span>
                                <span><i class="bi bi-geo-alt"></i> <%# Eval("Direccion") %></span>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Acción">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnEditar" runat="server"
                                CommandName="Editar"
                                CommandArgument="<%# Container.DataItemIndex %>"
                                CssClass="btn-outline-portal">

                                <i class="bi bi-pencil-square"></i> Editar
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </section>

</asp:Content>
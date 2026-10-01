<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Portal.Master"
    CodeBehind="MantenimientoMedicos.aspx.cs"
    Inherits="UIL.MantenimientoMedicos" 
    MaintainScrollPositionOnPostBack="true"%>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="eyebrow">Administrador / Mantenimientos</div>
    <h1 class="page-title">Mantenimiento de médicos</h1>

    <section class="hero">
        <span class="hero-badge">
            <i class="bi bi-clipboard2-pulse"></i> Equipo clínico
        </span>

        <h2>Profesionales preparados para cuidar cada vida.</h2>

        <p>
            Registre, consulte y actualice la información
            personal y profesional de los médicos.
        </p>
    </section>

    <div class="content-grid">
        <section class="panel">
            <div class="panel-heading">
                <div>
                    <h2 id="tituloFormulario" runat="server">Registrar médico</h2>
                    <p>Complete los datos personales y profesionales.</p>
                </div>
            </div>

            <asp:Panel ID="pnlMensaje" runat="server"
                Visible="false"
                CssClass="empty-state">

                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <asp:HiddenField ID="hdnIdMedico" runat="server" />
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
    <label class="portal-label">Usuario</label>

    <asp:TextBox ID="txtUsuario" runat="server"
        CssClass="portal-input"
        MaxLength="50"
        placeholder="Ejemplo: dr.lopez"
        autocomplete="username">
    </asp:TextBox>
</div>

<asp:Panel ID="pnlPassword" runat="server">
    <label class="portal-label">Contraseña</label>

    <div class="password-field">
        <asp:TextBox ID="txtPassword" runat="server"
            CssClass="portal-input"
            TextMode="Password"
            placeholder="Mínimo 6 caracteres"
            autocomplete="new-password">
        </asp:TextBox>

        <button type="button"
            class="btn-toggle-password"
            onclick="togglePassword('<%= txtPassword.ClientID %>', this)"
            aria-label="Mostrar contraseña">

            <i class="bi bi-eye"></i>
        </button>
    </div>

</asp:Panel>

<asp:Panel ID="pnlConfirmarPassword" runat="server"
    CssClass="full">
    <label class="portal-label">Confirmar contraseña</label>

    <div class="password-field">
        <asp:TextBox ID="txtConfirmarPassword" runat="server"
            CssClass="portal-input"
            TextMode="Password"
            placeholder="Repita la contraseña"
            autocomplete="new-password">
        </asp:TextBox>

        <button type="button"
            class="btn-toggle-password"
            onclick="togglePassword('<%= txtConfirmarPassword.ClientID %>', this)"
            aria-label="Mostrar contraseña">

            <i class="bi bi-eye"></i>
        </button>
    </div>

    <small class="specialty-helper">
        La contraseña se define únicamente al registrar al médico.
    </small>
</asp:Panel>
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
    <label class="portal-label">Especialidad</label>

    <div class="specialty-field">
        <asp:TextBox ID="txtEspecialidad" runat="server"
            CssClass="portal-input"
            MaxLength="100"
            list="listaEspecialidades"
            placeholder="Escriba o seleccione una especialidad">
        </asp:TextBox>
    </div>

    <small class="specialty-helper">
        Escriba para filtrar la lista de especialidades.
    </small>

    <datalist id="listaEspecialidades">
        <option value="Medicina General"></option>
        <option value="Medicina Interna"></option>
        <option value="Medicina Familiar"></option>
        <option value="Cardiología"></option>
        <option value="Pediatría"></option>
        <option value="Ginecología y Obstetricia"></option>
        <option value="Cirugía General"></option>
        <option value="Dermatología"></option>
        <option value="Endocrinología"></option>
        <option value="Gastroenterología"></option>
        <option value="Geriatría"></option>
        <option value="Hematología"></option>
        <option value="Infectología"></option>
        <option value="Nefrología"></option>
        <option value="Neumología"></option>
        <option value="Neurología"></option>
        <option value="Nutrición"></option>
        <option value="Oftalmología"></option>
        <option value="Oncología"></option>
        <option value="Ortopedia y Traumatología"></option>
        <option value="Otorrinolaringología"></option>
        <option value="Psiquiatría"></option>
        <option value="Radiología"></option>
        <option value="Reumatología"></option>
        <option value="Urología"></option>
        <option value="Anestesiología"></option>
        <option value="Emergenciología"></option>
        <option value="Medicina Física y Rehabilitación"></option>
    </datalist>
</div>

                <div class="full">
                    <label class="portal-label">Hospital asignado</label>
                    <asp:DropDownList ID="ddlHospital" runat="server"
                        CssClass="portal-select">
                    </asp:DropDownList>
                </div>

                <div class="full">
                    <asp:Button ID="btnGuardar" runat="server"
                        Text="Guardar médico"
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
                    <p>Gestione el equipo médico del hospital.</p>
                </div>
            </div>

            <div class="info-list">
                <div class="info-row">
                    <span><i class="bi bi-person-plus"></i> Crear</span>
                    <strong>Registra Persona y Médico</strong>
                </div>

                <div class="info-row">
                    <span><i class="bi bi-search"></i> Consultar</span>
                    <strong>Filtra por datos u hospital</strong>
                </div>

                <div class="info-row">
                    <span><i class="bi bi-pencil-square"></i> Modificar</span>
                    <strong>Actualiza información profesional</strong>
                </div>

                <div class="info-row">
                    <span><i class="bi bi-shield-check"></i> Protección</span>
                    <strong>No se permite eliminar médicos</strong>
                </div>
            </div>
        </section>
    </div>

    <section class="panel hospital-table-panel" style="margin-top: 22px;">
        <div class="panel-heading">
            <div>
                <h2>Médicos registrados</h2>
                <p>Busque por hospital, nombre, correo, cédula o especialidad.</p>
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
                    placeholder="Nombre, apellido, correo, cédula, teléfono o especialidad"
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
            <asp:GridView ID="gvMedicos" runat="server"
                AutoGenerateColumns="False"
                DataKeyNames="IdMedico"
                CssClass="portal-table hospital-grid"
                OnRowCommand="gvMedicos_RowCommand"
                EmptyDataText="No se encontraron médicos con esos filtros.">

                <Columns>
                    <asp:TemplateField HeaderText="Médico">
                        <ItemTemplate>
                            <div class="hospital-name">
                                <span class="hospital-icon">
                                    <i class="bi bi-person-badge"></i>
                                </span>

                                <div>
                                    <strong>
                                        Dr(a). <%# Eval("Nombre") %> <%# Eval("Apellido") %>
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

                    <asp:TemplateField HeaderText="Especialidad">
                        <ItemTemplate>
                            <span class="doctor-specialty">
                                <i class="bi bi-clipboard2-pulse"></i>
                                <%# Eval("Especialidad") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Cédula">
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

                    <asp:TemplateField HeaderText="Acciones">
                        <ItemTemplate>
                            <div class="hospital-actions">
                                <asp:LinkButton ID="btnEditar" runat="server"
                                    CommandName="Editar"
                                    CommandArgument="<%# Container.DataItemIndex %>"
                                    CssClass="btn-outline-portal">

                                    <i class="bi bi-pencil-square"></i> Editar
                                </asp:LinkButton>

                                <asp:LinkButton ID="btnEliminar" runat="server"
                                    CommandName="Eliminar"
                                    CommandArgument="<%# Container.DataItemIndex %>"
                                    CssClass="btn-delete-portal"
                                    OnClientClick="return confirmarAccion(this, '¿Desea eliminar este médico?');">

                                    <i class="bi bi-trash"></i> Eliminar
                                </asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </section>
     <script>
         function togglePassword(controlId, button) {
             var input = document.getElementById(controlId);
             var icon = button.querySelector("i");

             if (input.type === "password") {
                 input.type = "text";
                 icon.classList.remove("bi-eye");
                 icon.classList.add("bi-eye-slash");
                 button.setAttribute("aria-label", "Ocultar contraseña");
             } else {
                 input.type = "password";
                 icon.classList.remove("bi-eye-slash");
                 icon.classList.add("bi-eye");
                 button.setAttribute("aria-label", "Mostrar contraseña");
             }
         }
     </script>
</asp:Content>

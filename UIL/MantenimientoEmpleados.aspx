<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Portal.Master"
    CodeBehind="MantenimientoEmpleados.aspx.cs"
    Inherits="UIL.MantenimientoEmpleados" 
    MaintainScrollPositionOnPostBack="true"%>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="eyebrow">Administrador / Mantenimientos</div>
    <h1 class="page-title">Mantenimiento de empleados</h1>

    <section class="hero">
        <span class="hero-badge">
            <i class="bi bi-person-workspace"></i> Equipo administrativo
        </span>

        <h2>El talento que hace posible una atención excelente.</h2>

        <p>
            Registre, consulte, modifique o elimine empleados
            de Administración y Recepción.
        </p>
    </section>

    <div class="content-grid">
        <section class="panel">
            <div class="panel-heading">
                <div>
                    <h2 id="tituloFormulario" runat="server">Registrar empleado</h2>
                    <p>Complete los datos personales, de acceso y puesto.</p>
                </div>
            </div>

            <asp:Panel ID="pnlMensaje" runat="server"
                Visible="false"
                CssClass="empty-state">

                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <asp:HiddenField ID="hdnIdEmpleado" runat="server" />
            <asp:HiddenField ID="hdnIdPersona" runat="server" />
            <asp:HiddenField ID="hdnIdUsuario" runat="server" />

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
                    <label class="portal-label">Puesto</label>
                    <asp:DropDownList ID="ddlPuesto" runat="server"
                        CssClass="portal-select">

                        <asp:ListItem Text="Seleccione un puesto" Value="" />
                        <asp:ListItem Text="Administrador" Value="Administrador" />
                        <asp:ListItem Text="Recepcionista" Value="Recepcionista" />
                    </asp:DropDownList>
                </div>

                <div>
                    <label class="portal-label">Usuario</label>
                    <asp:TextBox ID="txtUsuario" runat="server"
                        CssClass="portal-input"
                        MaxLength="50"
                        placeholder="Ejemplo: recepcion.maria"
                        autocomplete="username">
                    </asp:TextBox>
                </div>

                <div>
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
                </div>

                <div class="full">
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
                        Al editar, deje ambas contraseñas vacías si no desea cambiarla.
                    </small>
                </div>

                <div class="full">
                    <asp:Button ID="btnGuardar" runat="server"
                        Text="Guardar empleado"
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
                    <h2>Roles permitidos</h2>
                    <p>El puesto define el apartado que verá al iniciar sesión.</p>
                </div>
            </div>

            <div class="info-list">
                <div class="info-row">
                    <span><i class="bi bi-shield-lock"></i> Administrador</span>
                    <strong>Accede a mantenimientos</strong>
                </div>

                <div class="info-row">
                    <span><i class="bi bi-person-heart"></i> Recepcionista</span>
                    <strong>Gestiona pacientes, citas y pagos</strong>
                </div>

                <div class="info-row">
                    <span><i class="bi bi-pencil-square"></i> Editar</span>
                    <strong>Actualiza datos y puesto</strong>
                </div>

                <div class="info-row">
                    <span><i class="bi bi-trash"></i> Eliminar</span>
                    <strong>Retira el empleado del puesto</strong>
                </div>
            </div>
        </section>
    </div>

    <section class="panel hospital-table-panel" style="margin-top: 22px;">
        <div class="panel-heading">
            <div>
                <h2>Empleados registrados</h2>
                <p>Busque empleados por datos personales o puesto.</p>
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

            <asp:DropDownList ID="ddlFiltroPuesto" runat="server"
                CssClass="portal-select"
                AutoPostBack="true"
                OnSelectedIndexChanged="ddlFiltroPuesto_SelectedIndexChanged">

                <asp:ListItem Text="Todos los puestos" Value="" />
                <asp:ListItem Text="Administrador" Value="Administrador" />
                <asp:ListItem Text="Recepcionista" Value="Recepcionista" />
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
            <asp:GridView ID="gvEmpleados" runat="server"
                AutoGenerateColumns="False"
                DataKeyNames="IdEmpleado"
                CssClass="portal-table hospital-grid"
                OnRowCommand="gvEmpleados_RowCommand"
                EmptyDataText="No se encontraron empleados con esos filtros.">

                <Columns>
                    <asp:TemplateField HeaderText="Empleado">
                        <ItemTemplate>
                            <div class="hospital-name">
                                <span class="hospital-icon">
                                    <i class="bi bi-person-workspace"></i>
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

                    <asp:TemplateField HeaderText="Puesto">
                        <ItemTemplate>
                            <span class="employee-position">
                                <i class="bi bi-briefcase"></i>
                                <%# Eval("Puesto") %>
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
                                    OnClientClick="return confirmarAccion(this, '¿Desea eliminar este empleado?');">

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
    } else {
        input.type = "password";
        icon.classList.remove("bi-eye-slash");
        icon.classList.add("bi-eye");
    }
        }
    </script>

</asp:Content>

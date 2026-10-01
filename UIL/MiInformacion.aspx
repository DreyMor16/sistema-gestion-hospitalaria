<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Portal.Master"
    CodeBehind="MiInformacion.aspx.cs"
    Inherits="UIL.MiInformacion" 
    MaintainScrollPositionOnPostBack="true"%>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="eyebrow">Perfil de usuario</div>
    <h1 class="page-title">Mi información</h1>

    <section class="panel account-profile-card">
        <div class="account-summary">
            <div class="account-avatar">
                <i class="bi bi-person-gear"></i>
            </div>

            <div>
                <h2>Administre los datos de su cuenta</h2>
                <p>
                    Puede actualizar su usuario, correo, teléfono
                    y contraseña cuando lo necesite.
                </p>
            </div>
        </div>

        <asp:Panel ID="pnlMensaje" runat="server"
            Visible="false"
            CssClass="empty-state">

            <asp:Label ID="lblMensaje" runat="server"></asp:Label>
        </asp:Panel>

        <div class="account-divider">
            <i class="bi bi-person-badge"></i> Datos de acceso
        </div>

        <div class="form-grid">
            <div>
                <label class="portal-label">Usuario</label>

                <asp:TextBox ID="txtUsuario" runat="server"
                    CssClass="portal-input"
                    MaxLength="50"
                    autocomplete="username">
                </asp:TextBox>
            </div>

            <div>
                <label class="portal-label">Teléfono</label>

                <asp:TextBox ID="txtTelefono" runat="server"
                    CssClass="portal-input"
                    MaxLength="8"
                    inputmode="numeric"
                    oninput="this.value=this.value.replace(/[^0-9]/g, '');"
                    placeholder="88887777">
                </asp:TextBox>
            </div>

            <div class="full">
                <label class="portal-label">Correo electrónico</label>

                <asp:TextBox ID="txtCorreo" runat="server"
                    CssClass="portal-input"
                    MaxLength="100"
                    TextMode="Email"
                    placeholder="correo@ejemplo.com">
                </asp:TextBox>
            </div>
        </div>

        <div class="account-divider">
            <i class="bi bi-shield-lock"></i> Cambiar contraseña
        </div>

        <p class="panel-description">
            Deje estos dos campos vacíos si desea conservar su contraseña actual.
        </p>

        <div class="form-grid">
            <div>
                <label class="portal-label">Nueva contraseña</label>

                <div class="password-field">
                    <asp:TextBox ID="txtNuevaPassword" runat="server"
                        CssClass="portal-input"
                        TextMode="Password"
                        MaxLength="255">
                    </asp:TextBox>

                    <button type="button" class="btn-toggle-password"
                        onclick="togglePassword('<%= txtNuevaPassword.ClientID %>', this)">
                        <i class="bi bi-eye"></i>
                    </button>
                </div>
            </div>

            <div>
                <label class="portal-label">Confirmar contraseña</label>

                <div class="password-field">
                    <asp:TextBox ID="txtConfirmarPassword" runat="server"
                        CssClass="portal-input"
                        TextMode="Password"
                        MaxLength="255">
                    </asp:TextBox>

                    <button type="button" class="btn-toggle-password"
                        onclick="togglePassword('<%= txtConfirmarPassword.ClientID %>', this)">
                        <i class="bi bi-eye"></i>
                    </button>
                </div>
            </div>

            <div class="full">
                <asp:Button ID="btnGuardar" runat="server"
                    Text="Guardar cambios"
                    CssClass="btn-primary-portal"
                    OnClick="btnGuardar_Click" />
            </div>
        </div>
    </section>

    <script>
function togglePassword(idCampo, boton) {
    var campo = document.getElementById(idCampo);
    var icono = boton.querySelector("i");

    if (campo.type === "password") {
        campo.type = "text";
        icono.className = "bi bi-eye-slash";
    } else {
        campo.type = "password";
        icono.className = "bi bi-eye";
    }
        }
    </script>

</asp:Content>
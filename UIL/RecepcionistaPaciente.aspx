<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Portal.Master"
    CodeBehind="RecepcionistaPaciente.aspx.cs"
    Inherits="UIL.RecepcionistaPaciente"
    MaintainScrollPositionOnPostBack="true" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="eyebrow">Recepción / Pacientes</div>
    <h1 class="page-title">Registrar nuevo paciente</h1>

    <section class="reception-registration-hero">
        <div>
            <span class="badge-soft">
                <i class="bi bi-person-plus"></i>
                Admisión hospitalaria
            </span>

            <h2>Información personal y clínica</h2>

            <p>
                Registre los datos necesarios para crear el expediente
                del paciente en el hospital seleccionado.
            </p>
        </div>

        <div class="reception-registration-hero-icon">
            <i class="bi bi-clipboard2-pulse"></i>
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
                <h2>Datos del paciente</h2>
                <p>Los campos con información obligatoria deben completarse.</p>
            </div>
        </div>

        <h3 class="registration-section-title">
            <i class="bi bi-person-vcard"></i>
            Información personal
        </h3>

        <div class="form-grid">
            <div>
                <label class="portal-label">Nombre</label>

                <asp:TextBox ID="txtNombre" runat="server"
                    CssClass="portal-input"
                    MaxLength="50"
                    placeholder="Nombre del paciente">
                </asp:TextBox>
            </div>

            <div>
                <label class="portal-label">Apellido</label>

                <asp:TextBox ID="txtApellido" runat="server"
                    CssClass="portal-input"
                    MaxLength="50"
                    placeholder="Apellido del paciente">
                </asp:TextBox>
            </div>

            <div>
                <label class="portal-label">Cédula</label>

                <asp:TextBox ID="txtCedula" runat="server"
                    CssClass="portal-input"
                    MaxLength="9"
                    inputmode="numeric"
                    placeholder="9 dígitos"
                    oninput="this.value=this.value.replace(/[^0-9]/g, '');">
                </asp:TextBox>
            </div>

            <div>
                <label class="portal-label">Teléfono</label>

                <asp:TextBox ID="txtTelefono" runat="server"
                    CssClass="portal-input"
                    MaxLength="8"
                    inputmode="numeric"
                    placeholder="8 dígitos"
                    oninput="this.value=this.value.replace(/[^0-9]/g, '');">
                </asp:TextBox>
            </div>

            <div class="full">
                <label class="portal-label">Correo electrónico</label>

                <asp:TextBox ID="txtCorreo" runat="server"
                    CssClass="portal-input"
                    MaxLength="100"
                    placeholder="correo@ejemplo.com">
                </asp:TextBox>
            </div>
        </div>

        <h3 class="registration-section-title">
            <i class="bi bi-heart-pulse"></i>
            Información clínica
        </h3>

        <div class="form-grid">
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

                    <asp:ListItem Text="Seleccione un género"
                        Value="0"></asp:ListItem>

                    <asp:ListItem Text="Masculino"
                        Value="Masculino"></asp:ListItem>

                    <asp:ListItem Text="Femenino"
                        Value="Femenino"></asp:ListItem>

                    <asp:ListItem Text="Otro"
                        Value="Otro"></asp:ListItem>
                </asp:DropDownList>
            </div>

            <div class="full">
                <label class="portal-label">Hospital</label>

                <asp:DropDownList ID="ddlHospital" runat="server"
                    CssClass="portal-select">
                </asp:DropDownList>
            </div>

            <div class="full">
                <label class="portal-label">Dirección</label>

                <asp:TextBox ID="txtDireccion" runat="server"
                    CssClass="portal-input"
                    TextMode="MultiLine"
                    Rows="3"
                    MaxLength="200"
                    placeholder="Dirección de residencia">
                </asp:TextBox>
            </div>

            <div class="full">
                <asp:Button ID="btnRegistrar" runat="server"
                    Text="Registrar paciente"
                    CssClass="btn-primary-portal"
                    OnClick="btnRegistrar_Click" />

                <asp:Button ID="btnLimpiar" runat="server"
                    Text="Limpiar formulario"
                    CssClass="btn-outline-portal"
                    CausesValidation="false"
                    OnClick="btnLimpiar_Click" />
            </div>
        </div>

        <div class="registration-note">
            <i class="bi bi-info-circle"></i>

            <span>
                El paciente se registra sin usuario ni contraseña.
                Podrá crear su acceso posteriormente desde el login.
            </span>
        </div>
    </section>

</asp:Content>
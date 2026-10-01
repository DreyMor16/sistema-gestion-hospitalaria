<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="RegistroPaciente.aspx.cs"
    Inherits="UIL.RegistroPaciente" 
    MaintainScrollPositionOnPostBack="true"%>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Hospital General | Activar cuenta</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />

    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css"
        rel="stylesheet" />

    <style>
        body {
            min-height: 100vh;
            margin: 0;
            font-family: "Segoe UI", system-ui, sans-serif;
            background:
                radial-gradient(circle at 80% 15%, rgba(72,220,225,.35), transparent 22rem),
                linear-gradient(135deg, #06223e, #075a87, #08365d);
        }

        .register-page {
            min-height: 100vh;
            display: grid;
            place-items: center;
            padding: 24px;
        }

        .register-card {
            width: min(560px, 100%);
            padding: clamp(28px, 6vw, 48px);
            border: 1px solid rgba(255,255,255,.4);
            border-radius: 26px;
            background: rgba(255,255,255,.97);
            box-shadow: 0 25px 70px rgba(1,20,38,.38);
        }

        .register-icon {
            display: grid;
            width: 58px;
            height: 58px;
            place-items: center;
            margin-bottom: 23px;
            border-radius: 17px;
            color: #087eaa;
            background: #dff8fa;
            font-size: 27px;
        }

        .eyebrow {
            color: #1686a5;
            font-size: .78rem;
            font-weight: 700;
            letter-spacing: .13em;
            text-transform: uppercase;
        }

        h1 {
            margin: 8px 0 10px;
            color: #123b59;
            font-size: 2rem;
            font-weight: 700;
        }

        .subtitle {
            margin-bottom: 28px;
            color: #718092;
            line-height: 1.55;
        }

        .form-label {
            color: #36516a;
            font-size: .9rem;
            font-weight: 600;
        }

        .form-control {
            height: 52px;
            border: 1px solid #d8e4eb;
            border-radius: 12px;
            background: #f9fbfc;
        }

        .form-control:focus {
            border-color: #23aac0;
            box-shadow: 0 0 0 .24rem rgba(28,201,216,.14);
        }

        .btn-main {
            min-height: 52px;
            border: 0;
            border-radius: 12px;
            background: linear-gradient(100deg, #087eaa, #13b4bf);
            color: white;
            font-weight: 700;
        }

        .password-wrap {
            position: relative;
        }

        .password-wrap .form-control {
            padding-right: 48px;
        }

        .btn-eye {
            position: absolute;
            top: 50%;
            right: 5px;
            border: 0;
            color: #61839b;
            background: transparent;
            transform: translateY(-50%);
        }

        .back-link {
            display: block;
            margin-top: 22px;
            color: #087eaa;
            font-weight: 700;
            text-align: center;
            text-decoration: none;
        }

        .register-alert {
            display: flex;
            align-items: flex-start;
            gap: 10px;
            margin: 0 0 20px;
            padding: 13px 15px;
            border: 1px solid #c3e4da;
            border-left: 5px solid #178d6c;
            border-radius: 14px;
            color: #17604e;
            background: #effcf7;
            box-shadow: 0 8px 18px rgba(20, 104, 78, .07);
            font-size: .9rem;
        }

        .register-alert-error {
            border-color: #f0c1c4;
            border-left-color: #cf4e57;
            color: #942f38;
            background: #fff6f6;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">
        <main class="register-page">
            <section class="register-card">
                <div class="register-icon">
                    <i class="bi bi-person-plus-fill"></i>
                </div>

                <div class="eyebrow">Hospital General</div>
                <h1>Activar cuenta de paciente</h1>

                <p class="subtitle">
                    Primero confirme su cédula. El hospital debe tener
                    su información registrada antes de activar el acceso.
                </p>

                <asp:Panel ID="pnlMensaje" runat="server"
                    Visible="false"
                    CssClass="register-alert">

                    <asp:Label ID="lblMensaje" runat="server"></asp:Label>
                </asp:Panel>

                <asp:Panel ID="pnlCedula" runat="server">
                    <div class="mb-3">
                        <label class="form-label">Cédula</label>

                        <asp:TextBox ID="txtCedula" runat="server"
                            CssClass="form-control"
                            MaxLength="9"
                            inputmode="numeric"
                            placeholder="Ingrese su cédula de 9 dígitos"
                            oninput="this.value=this.value.replace(/[^0-9]/g, '');">
                        </asp:TextBox>
                    </div>

                    <asp:Button ID="btnVerificarCedula" runat="server"
                        Text="Verificar cédula"
                        CssClass="btn btn-main w-100"
                        OnClick="btnVerificarCedula_Click" />
                </asp:Panel>

                <asp:Panel ID="pnlCredenciales" runat="server"
                    Visible="false">

                    <div class="alert alert-info">
                        <i class="bi bi-check-circle-fill"></i>
                        Paciente encontrado. Cree sus credenciales de acceso.
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Usuario</label>

                        <asp:TextBox ID="txtNuevoUsuario" runat="server"
                            CssClass="form-control"
                            MaxLength="50"
                            autocomplete="username"
                            placeholder="Ejemplo: juan.perez">
                        </asp:TextBox>
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Contraseña</label>

                        <div class="password-wrap">
                            <asp:TextBox ID="txtNuevaPassword" runat="server"
                                CssClass="form-control"
                                TextMode="Password"
                                MaxLength="255"
                                autocomplete="new-password">
                            </asp:TextBox>

                            <button type="button" class="btn-eye"
                                onclick="togglePassword('<%= txtNuevaPassword.ClientID %>', this)">
                                <i class="bi bi-eye"></i>
                            </button>
                        </div>
                    </div>

                    <div class="mb-4">
                        <label class="form-label">Confirmar contraseña</label>

                        <div class="password-wrap">
                            <asp:TextBox ID="txtConfirmarPassword" runat="server"
                                CssClass="form-control"
                                TextMode="Password"
                                MaxLength="255"
                                autocomplete="new-password">
                            </asp:TextBox>

                            <button type="button" class="btn-eye"
                                onclick="togglePassword('<%= txtConfirmarPassword.ClientID %>', this)">
                                <i class="bi bi-eye"></i>
                            </button>
                        </div>
                    </div>

                    <asp:Button ID="btnCrearCuenta" runat="server"
                        Text="Crear mi cuenta"
                        CssClass="btn btn-main w-100"
                        OnClick="btnCrearCuenta_Click" />
                </asp:Panel>

                <asp:HyperLink ID="lnkIrLogin" runat="server"
                    NavigateUrl="~/Login.aspx"
                    CssClass="back-link">

                    <i class="bi bi-arrow-left"></i>
                    Volver al inicio de sesión
                </asp:HyperLink>
            </section>
        </main>
    </form>

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
</body>
</html>

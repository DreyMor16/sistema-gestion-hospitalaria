<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="UIL.Login" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Hospital General | Inicio de sesión</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet" />

    <style>
        :root {
            --navy: #082a4b;
            --blue: #087eaa;
            --cyan: #1cc9d8;
            --ink: #16324b;
        }

        * { box-sizing: border-box; }

        body {
            min-height: 100vh;
            margin: 0;
            overflow-x: hidden;
            font-family: "Segoe UI", system-ui, sans-serif;
            color: var(--ink);
            background:
                radial-gradient(circle at 80% 15%, rgba(72, 220, 225, .38), transparent 22rem),
                radial-gradient(circle at 8% 88%, rgba(19, 135, 195, .55), transparent 30rem),
                linear-gradient(135deg, #06223e 0%, #075a87 52%, #08365d 100%);
        }

        body::before,
        body::after {
            content: "";
            position: fixed;
            pointer-events: none;
            z-index: 0;
        }

        body::before {
            width: 38rem;
            height: 38rem;
            right: -12rem;
            bottom: -18rem;
            border: 1px solid rgba(255,255,255,.16);
            border-radius: 50%;
            box-shadow:
                0 0 0 4rem rgba(255,255,255,.035),
                0 0 0 8rem rgba(255,255,255,.025);
        }

        body::after {
            inset: 0;
            opacity: .16;
            background-image:
                linear-gradient(rgba(255,255,255,.35) 1px, transparent 1px),
                linear-gradient(90deg, rgba(255,255,255,.35) 1px, transparent 1px);
            background-size: 38px 38px;
            mask-image: linear-gradient(to bottom, black, transparent 78%);
        }

        .login-page {
            position: relative;
            z-index: 1;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 32px 20px;
        }

        .login-card {
            width: min(1040px, 100%);
            min-height: 610px;
            overflow: hidden;
            border: 1px solid rgba(255,255,255,.38);
            border-radius: 28px;
            background: rgba(255,255,255,.96);
            box-shadow: 0 25px 70px rgba(1, 20, 38, .38);
        }

        .welcome-side {
            position: relative;
            isolation: isolate;
            padding: clamp(38px, 6vw, 70px);
            color: white;
            overflow: hidden;
            background: linear-gradient(145deg, #082f55, #087a9e);
        }

        .welcome-side::before {
            content: "+";
            position: absolute;
            z-index: -1;
            top: 46%;
            left: 49%;
            color: rgba(255,255,255,.09);
            font: 700 25rem/.4 Arial, sans-serif;
            transform: translate(-50%, -50%) rotate(-10deg);
        }

        .welcome-side::after {
            content: "";
            position: absolute;
            z-index: -1;
            width: 20rem;
            height: 20rem;
            right: -8rem;
            bottom: -7rem;
            border-radius: 50%;
            background: rgba(35, 213, 218, .32);
            box-shadow: -12rem -4rem 0 -4rem rgba(255,255,255,.09);
        }

        .brand-mark {
            display: grid;
            place-items: center;
            width: 54px;
            height: 54px;
            margin-bottom: 38px;
            border: 1px solid rgba(255,255,255,.35);
            border-radius: 16px;
            background: rgba(255,255,255,.14);
            font-size: 27px;
        }

        .welcome-side h1 {
            font-size: clamp(2rem, 3.4vw, 3rem);
            font-weight: 700;
            line-height: 1.12;
        }

        .welcome-side p {
            max-width: 390px;
            color: rgba(255,255,255,.82);
            font-size: 1.04rem;
            line-height: 1.7;
        }

        .care-note {
            position: absolute;
            right: clamp(28px, 5vw, 58px);
            bottom: clamp(32px, 5vw, 58px);
            color: rgba(255,255,255,.82);
            font-size: .86rem;
            letter-spacing: .03em;
        }

        .pulse {
            color: #79f6ef;
            font-size: 1.4rem;
            vertical-align: middle;
        }

        .form-side {
            display: flex;
            align-items: center;
            padding: clamp(35px, 6vw, 72px);
        }

        .form-content {
            width: 100%;
            max-width: 370px;
            margin: auto;
        }

        .eyebrow {
            margin-bottom: 10px;
            color: #1686a5;
            font-size: .78rem;
            font-weight: 700;
            letter-spacing: .13em;
            text-transform: uppercase;
        }

        .form-content h2 {
            margin-bottom: 10px;
            color: #123b59;
            font-size: 2rem;
            font-weight: 700;
        }

        .subtitle {
            margin-bottom: 32px;
            color: #718092;
            line-height: 1.55;
        }

        .form-label {
            margin-bottom: 8px;
            color: #36516a;
            font-size: .9rem;
            font-weight: 600;
        }

        .field-wrap { position: relative; }

        .field-icon {
            position: absolute;
            z-index: 2;
            top: 50%;
            left: 15px;
            color: #7192a7;
            transform: translateY(-50%);
        }

        .form-control {
            height: 52px;
            padding-left: 44px;
            border: 1px solid #d8e4eb;
            border-radius: 12px !important;
            background: #f9fbfc;
            color: #173d59;
        }

        .form-control:focus {
            border-color: #23aac0;
            background: white;
            box-shadow: 0 0 0 .24rem rgba(28, 201, 216, .14);
        }

        .password-field .form-control { padding-right: 48px; }

        .btn-eye {
            position: absolute;
            z-index: 3;
            top: 50%;
            right: 5px;
            padding: 7px 10px;
            color: #61839b;
            transform: translateY(-50%);
        }

        .btn-eye:hover { color: #087eaa; }

        .btn-login {
            height: 52px;
            border: 0;
            border-radius: 12px;
            background: linear-gradient(100deg, #087eaa, #13b4bf);
            box-shadow: 0 10px 20px rgba(8, 126, 170, .23);
            font-weight: 700;
            transition: .2s;
        }

        .btn-login:hover {
            background: linear-gradient(100deg, #076f98, #0ea3ad);
            box-shadow: 0 13px 25px rgba(8, 126, 170, .3);
            transform: translateY(-2px);
        }

        .system-footer {
            margin-top: 30px;
            color: #8595a4;
            font-size: .82rem;
        }

        .system-footer i { color: #19a7b5; }

        .account-activation {
            margin-top: 20px;
            padding-top: 18px;
            border-top: 1px solid #e5edf1;
            color: #718092;
            font-size: .88rem;
            text-align: center;
        }

        .account-activation a {
            color: #087eaa;
            font-weight: 700;
            text-decoration: none;
        }

        .account-activation a:hover {
            color: #066d94;
            text-decoration: underline;
        }

        .activation-note {
            margin-top: 14px;
            color: #8b9aaa;
            font-size: .76rem;
            line-height: 1.45;
            text-align: center;
        }

        .login-alert {
            margin: 0 0 20px;
            padding: 13px 15px;
            border: 1px solid #f0bfc4;
            border-left: 5px solid #d14f58;
            border-radius: 14px;
            color: #912e38;
            background: #fff5f6;
            box-shadow: 0 8px 18px rgba(135, 32, 45, .08);
            font-size: .9rem;
        }

        @media (max-width: 767.98px) {
            .login-page {
                align-items: flex-start;
                padding: 18px;
            }

            .login-card { min-height: 0; }
            .welcome-side { min-height: 255px; }
            .brand-mark { margin-bottom: 22px; }
            .welcome-side h1 { font-size: 2rem; }
            .care-note { bottom: 25px; }
            .form-side { padding: 38px 28px; }
        }
    </style>
</head>

<body>
    <form id="form1" runat="server" class="login-page">
        <main class="login-card row g-0">

            <section class="welcome-side col-md-6 d-flex flex-column">
                <div class="brand-mark">
                    <i class="bi bi-heart-pulse-fill"></i>
                </div>

                <h1>Salud <br /> y Vida</h1>

                <p class="mt-3 mb-0">
                    Un espacio seguro para coordinar el cuidado de cada paciente
                    con cercanía, precisión y confianza.
                </p>

                <div class="care-note">
                    <span class="pulse"><i class="bi bi-activity"></i></span>
                    CUIDADO 
                </div>
            </section>

            <section class="form-side col-md-6">
                <div class="form-content">

                    <div class="eyebrow">Hospital General</div>

                    <h2>Inicio de sesión</h2>

                    <p class="subtitle">
                        Ingrese sus credenciales para acceder al sistema hospitalario.
                    </p>

                    <asp:Panel ID="pnlError" runat="server" Visible="false"
                        CssClass="login-alert d-flex align-items-center gap-2">
                        <i class="bi bi-exclamation-circle-fill"></i>
                        <asp:Label ID="lblError" runat="server"></asp:Label>
                    </asp:Panel>

                    <div class="mb-3">
                        <label for="<%= txtUsuario.ClientID %>" class="form-label">Usuario</label>

                        <div class="field-wrap">
                            <i class="bi bi-person field-icon"></i>

                            <asp:TextBox ID="txtUsuario" runat="server"
                                CssClass="form-control"
                                placeholder="Ingrese su usuario"
                                autocomplete="username">
                            </asp:TextBox>
                        </div>
                    </div>

                    <div class="mb-4">
                        <label for="<%= txtPassword.ClientID %>" class="form-label">Contraseña</label>

                        <div class="field-wrap password-field">
                            <i class="bi bi-shield-lock field-icon"></i>

                            <asp:TextBox ID="txtPassword" runat="server"
                                CssClass="form-control"
                                TextMode="Password"
                                placeholder="Ingrese su contraseña"
                                autocomplete="current-password">
                            </asp:TextBox>

                            <button type="button" class="btn btn-eye border-0"
                                onclick="togglePassword()"
                                aria-label="Mostrar u ocultar contraseña">
                                <i id="eyeIcon" class="bi bi-eye"></i>
                            </button>
                        </div>
                    </div>

                    <asp:Button ID="btnLogin" runat="server"
                        Text="Iniciar sesión"
                        CssClass="btn btn-login w-100 text-white"
                        OnClick="btnLogin_Click" />
                    <div class="account-activation">
                        ¿Es paciente y aún no tiene cuenta?

                        <br />

                        <a href="RegistroPaciente.aspx">
                            <i class="bi bi-person-plus-fill"></i>
                            Activar mi acceso como paciente
                        </a>

                        <div class="activation-note">
                            Solo disponible para pacientes registrados previamente
                            por el centro hospitalario.
                        </div>
                    </div>
                    <div class="system-footer text-center">
                        <i class="bi bi-shield-check me-1"></i>
                        Acceso seguro al Sistema Hospitalario
                    </div>

                </div>
            </section>

        </main>
    </form>

    <script>
        function togglePassword() {
            var txtPassword = document.getElementById('<%= txtPassword.ClientID %>');
            var eyeIcon = document.getElementById('eyeIcon');
            var isPassword = txtPassword.type === 'password';

            txtPassword.type = isPassword ? 'text' : 'password';
            eyeIcon.classList.toggle('bi-eye', !isPassword);
            eyeIcon.classList.toggle('bi-eye-slash', isPassword);
        }
    </script>
</body>
</html>

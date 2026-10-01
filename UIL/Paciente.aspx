<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Portal.Master"
    CodeBehind="Paciente.aspx.cs"
    Inherits="UIL.Paciente" 
    MaintainScrollPositionOnPostBack="true"%>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="eyebrow">Portal del paciente</div>
    <h1 class="page-title">Mi espacio de salud</h1>

    <section class="hero">
        <span class="hero-badge">
            <i class="bi bi-heart-pulse"></i> Atención personalizada
        </span>

        <h2>Su salud, siempre cerca.</h2>

        <p>
            Consulte su información personal, expediente, citas médicas
            y tratamientos pendientes de pago.
        </p>
    </section>

    <section class="stat-grid">

    <article class="stat-card">
        <span class="stat-icon">
            <i class="bi bi-calendar2-check"></i>
        </span>

        <div class="stat-label">Próxima cita</div>

        <div class="stat-value">
            <asp:Label ID="lblProximaCita" runat="server"
                Text="—">
            </asp:Label>
        </div>

        <asp:Label ID="lblDetalleProxima" runat="server"
            CssClass="stat-note">
        </asp:Label>
    </article>

    <article class="stat-card">
        <span class="stat-icon">
            <i class="bi bi-file-earmark-medical"></i>
        </span>

        <div class="stat-label">Expediente</div>

        <div class="stat-value">
            <asp:Label ID="lblExpediente" runat="server"
                Text="Activo">
            </asp:Label>
        </div>

        <span class="stat-note">Acceso clínico protegido</span>
    </article>

    <article class="stat-card">
        <span class="stat-icon">
            <i class="bi bi-receipt"></i>
        </span>

        <div class="stat-label">Pagos pendientes</div>

        <div class="stat-value">
            <asp:Label ID="lblPagosPendientes" runat="server"
                Text="—">
            </asp:Label>
        </div>

        <asp:Label ID="lblDetallePagos" runat="server"
            CssClass="stat-note">
        </asp:Label>
    </article>

    <article class="stat-card">
        <span class="stat-icon">
            <i class="bi bi-hospital"></i>
        </span>

        <div class="stat-label">Hospital</div>

        <div class="stat-value stat-value-compact">
            <asp:Label ID="lblHospital" runat="server"
                Text="—">
            </asp:Label>
        </div>

        <span class="stat-note">Centro hospitalario asociado</span>
    </article>

</section>

    <div class="content-grid">
        <section class="panel">
            <div class="panel-heading">
                <div>
                    <h2>Mi información</h2>
                    <p>Datos asociados a su cuenta hospitalaria.</p>
                </div>

                <a class="btn-outline-portal" href="MiInformacion.aspx">
                    <i class="bi bi-pencil-square"></i> Editar
                </a>
            </div>

            <div class="info-list">
                <div class="info-row">
                    <span>Nombre</span>
                    <strong><%: ((UIL.PortalMaster)Master).UsuarioActual.Nombre %></strong>
                </div>

                <div class="info-row">
                    <span>Apellido</span>
                    <strong><%: ((UIL.PortalMaster)Master).UsuarioActual.Apellido %></strong>
                </div>

                <div class="info-row">
                    <span>Usuario</span>
                    <strong><%: ((UIL.PortalMaster)Master).UsuarioActual.NombreUsuario %></strong>
                </div>
            </div>
        </section>

        <section class="panel">
            <div class="panel-heading">
                <div>
                    <h2>Accesos rápidos</h2>
                    <p>Consulte la información clínica importante.</p>
                </div>
            </div>

            <div class="action-grid">
                <a class="action-card" href="PacienteExpediente.aspx">
                    <i class="bi bi-file-earmark-medical"></i>
                    <h3>Expediente</h3>
                    <p>Historial de citas, diagnósticos y tratamientos.</p>
                </a>

                <a class="action-card" href="PacienteCitas.aspx">
                    <i class="bi bi-calendar2-check"></i>
                    <h3>Mis citas</h3>
                    <p>Consulte sus próximas y anteriores consultas.</p>
                </a>

                <a class="action-card" href="PacientePagos.aspx">
                    <i class="bi bi-credit-card"></i>
                    <h3>Pagar</h3>
                    <p>Revise tratamientos y pagos pendientes.</p>
                </a>
            </div>
        </section>
    </div>

</asp:Content>

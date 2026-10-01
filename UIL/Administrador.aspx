<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Portal.Master"
    CodeBehind="Administrador.aspx.cs"
    Inherits="UIL.Administrador" 
    MaintainScrollPositionOnPostBack="true"%>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="eyebrow">Centro de control</div>
    <h1 class="page-title">Gestión hospitalaria</h1>

    <section class="hero">
        <span class="hero-badge">
            <i class="bi bi-stars"></i> Panel administrativo
        </span>

        <h2>Todo el hospital, en un solo lugar.</h2>

        <p>
            Administre la información institucional, pacientes,
            personal médico, empleados y medicamentos.
        </p>
    </section>

    <section class="stat-grid">

    <article class="stat-card">
        <span class="stat-icon">
            <i class="bi bi-person-vcard"></i>
        </span>

        <div class="stat-label">Pacientes</div>

        <div class="stat-value">
            <asp:Label ID="lblTotalPacientes" runat="server"
                Text="0">
            </asp:Label>
        </div>
    </article>

    <article class="stat-card">
        <span class="stat-icon">
            <i class="bi bi-clipboard2-pulse"></i>
        </span>

        <div class="stat-label">Médicos</div>

        <div class="stat-value">
            <asp:Label ID="lblTotalMedicos" runat="server"
                Text="0">
            </asp:Label>
        </div>
    </article>

    <article class="stat-card">
        <span class="stat-icon">
            <i class="bi bi-capsule-pill"></i>
        </span>

        <div class="stat-label">Medicamentos</div>

        <div class="stat-value">
            <asp:Label ID="lblTotalMedicamentos" runat="server"
                Text="0">
            </asp:Label>
        </div>
    </article>

</section>

    <section class="panel">
        <div class="panel-heading">
            <div>
                <h2>Mantenimientos del sistema</h2>
                <p>Seleccione el módulo que desea administrar.</p>
            </div>
        </div>

        <div class="action-grid">
            <a class="action-card" href="MantenimientoHospital.aspx">
                <i class="bi bi-hospital"></i>
                <h3>Hospital</h3>
                <p>Nombre, dirección, teléfono y datos institucionales.</p>
            </a>

            <a class="action-card" href="MantenimientoPacientes.aspx">
                <i class="bi bi-person-vcard"></i>
                <h3>Pacientes</h3>
                <p>Datos personales, fecha de nacimiento y hospital.</p>
            </a>

            <a class="action-card" href="MantenimientoMedicos.aspx">
                <i class="bi bi-clipboard2-pulse"></i>
                <h3>Médicos</h3>
                <p>Especialidad, datos personales y hospital asignado.</p>
            </a>

            <a class="action-card" href="MantenimientoEmpleados.aspx">
                <i class="bi bi-person-workspace"></i>
                <h3>Empleados</h3>
                <p>Puestos de administrador, recepción y otros cargos.</p>
            </a>

            <a class="action-card" href="MantenimientoMedicamentos.aspx">
                <i class="bi bi-capsule-pill"></i>
                <h3>Medicamentos</h3>
                <p>Catálogo, costos e inventario de medicamentos.</p>
            </a>

            <a class="action-card" href="MiInformacion.aspx">
                <i class="bi bi-person-circle"></i>
                <h3>Mi información</h3>
                <p>Consulte o modifique los datos de su cuenta.</p>
            </a>
        </div>
    </section>

</asp:Content>
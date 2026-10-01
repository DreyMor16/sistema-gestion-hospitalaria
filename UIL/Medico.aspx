<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Portal.Master"
    CodeBehind="Medico.aspx.cs"
    Inherits="UIL.Medico" 
    MaintainScrollPositionOnPostBack="true"%>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="eyebrow">Portal médico</div>
    <h1 class="page-title">Jornada clínica</h1>

    <section class="hero">
        <span class="hero-badge">
            <i class="bi bi-clipboard2-pulse"></i> Atención clínica
        </span>

        <h2>Una consulta clara comienza con un buen historial.</h2>

        <p>
            Visualice pacientes, consulte el historial de citas,
            registre diagnósticos y administre su información profesional.
        </p>
    </section>

    <section class="stat-grid">

    <article class="stat-card">
        <span class="stat-icon">
            <i class="bi bi-calendar-day"></i>
        </span>

        <div class="stat-label">Citas de hoy</div>

        <asp:Label ID="lblCitasHoy"
            runat="server"
            CssClass="stat-value"
            Text="0">
        </asp:Label>
    </article>

    <article class="stat-card">
        <span class="stat-icon">
            <i class="bi bi-people"></i>
        </span>

        <div class="stat-label">Pacientes de hoy</div>

        <asp:Label ID="lblPacientesHoy"
            runat="server"
            CssClass="stat-value"
            Text="0">
        </asp:Label>
    </article>

    <article class="stat-card">
        <span class="stat-icon">
            <i class="bi bi-hourglass-split"></i>
        </span>

        <div class="stat-label">En proceso</div>

        <asp:Label ID="lblEnProceso"
            runat="server"
            CssClass="stat-value"
            Text="0">
        </asp:Label>
    </article>

    <article class="stat-card">
        <span class="stat-icon">
            <i class="bi bi-check2-circle"></i>
        </span>

        <div class="stat-label">Finalizadas hoy</div>

        <asp:Label ID="lblFinalizadas"
            runat="server"
            CssClass="stat-value"
            Text="0">
        </asp:Label>
    </article>

</section>

    <section class="panel">
        <div class="panel-heading">
            <div>
                <h2>Herramientas clínicas</h2>
                <p>Acceda a la información necesaria para sus consultas.</p>
            </div>
        </div>

        <div class="action-grid">
            <a class="action-card" href="MedicoPacientes.aspx">
                <i class="bi bi-people"></i>
                <h3>Ver pacientes</h3>
                <p>Consulte los pacientes con citas asignadas.</p>
            </a>

            <a class="action-card" href="MedicoHistorial.aspx">
                <i class="bi bi-clock-history"></i>
                <h3>Historial de citas</h3>
                <p>Revise diagnósticos, fechas y estado de citas.</p>
            </a>

            <a class="action-card" href="MedicoCita.aspx">
                <i class="bi bi-clipboard2-plus"></i>
                <h3>Atender cita</h3>
                <p>Registre diagnóstico y cambie el estado de la consulta.</p>
            </a>

            <a class="action-card" href="MiInformacion.aspx">
                <i class="bi bi-person-circle"></i>
                <h3>Mi información</h3>
                <p>Consulte o modifique sus datos profesionales.</p>
            </a>
        </div>
    </section>

</asp:Content>
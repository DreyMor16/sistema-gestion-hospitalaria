<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Portal.Master"
    CodeBehind="Recepcionista.aspx.cs"
    Inherits="UIL.Recepcionista" 
    MaintainScrollPositionOnPostBack="true"%>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="eyebrow">Centro de recepción</div>
    <h1 class="page-title">Atención al paciente</h1>
    <section class="reception-home-layout">

    <section class="hero">
        <span class="hero-badge">
            <i class="bi bi-person-heart"></i>
            Gestión de atención
        </span>

        <h2>Cada registro es el inicio de una buena atención.</h2>

        <p>
            Registre pacientes, cree citas y gestione los pagos
            de tratamientos de forma rápida y organizada.
        </p>
    </section>

    <a class="today-appointments-card"
        href="RecepcionistaControlCitas.aspx">

        <div class="today-appointments-top">
            <span class="today-appointments-icon">
                <i class="bi bi-calendar-day"></i>
            </span>

            <small>Agenda hospitalaria</small>
        </div>

        <div>
            <div class="today-appointments-value">
                <asp:Label ID="lblCitasHoy" runat="server"
                    Text="0"></asp:Label>
            </div>

            <div class="today-appointments-label">
                Citas activas para hoy
            </div>
        </div>

        <span class="today-appointments-link">
            Ver y controlar citas
            <i class="bi bi-arrow-right"></i>
        </span>
    </a>

</section>

   

    <section class="panel">
        <div class="panel-heading">
            <div>
                <h2>Acciones de recepción</h2>
                <p>Seleccione la operación que desea realizar.</p>
            </div>
        </div>

        <div class="action-grid">
            <a class="action-card" href="RecepcionistaPago.aspx">
                <i class="bi bi-credit-card"></i>
                <h3>Pagar tratamiento</h3>
                <p>Registre pagos por efectivo, tarjeta o SINPE.</p>
            </a>

            <a class="action-card" href="RecepcionistaCitas.aspx">
                <i class="bi bi-calendar2-plus"></i>
                <h3>Crear cita</h3>
                <p>Asigne paciente, médico, fecha y hora de atención.</p>
            </a>

            <a class="action-card" href="RecepcionistaPaciente.aspx">
                <i class="bi bi-person-plus"></i>
                <h3>Nuevo paciente</h3>
                <p>Registre datos personales y clínicos del paciente.</p>
            </a>
            <a class="action-card" href="MiInformacion.aspx">
                <i class="bi bi-person-circle"></i>
                <h3>Mi información</h3>
                <p>Consulte o modifique los datos de su cuenta.</p>
            </a>
        </div>
    </section>

</asp:Content>
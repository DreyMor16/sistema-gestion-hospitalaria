# Sistema de Gestión Hospitalaria

Aplicación web multiusuario para administrar la operación de una red de
hospitales. Permite gestionar pacientes, personal médico, citas, expedientes,
tratamientos, prescripciones, inventario de medicamentos y pagos.

Este proyecto fue desarrollado como una aplicación académica de base de datos,
con una arquitectura por capas y reglas de negocio separadas del acceso a datos.

## Funcionalidades

- Autenticación y navegación según el rol del usuario.
- Administración de hospitales, médicos, empleados, pacientes y medicamentos.
- Registro, consulta y cancelación de citas.
- Atención médica con diagnósticos, tratamientos y prescripciones.
- Expediente e historial de citas por paciente.
- Control de existencias de medicamentos por hospital.
- Registro e historial de pagos.
- Paneles de resumen para administración, recepción y personal médico.

## Tecnologías

- C#
- ASP.NET Web Forms
- .NET Framework 4.8
- SQL Server y ADO.NET
- Bootstrap 5 y jQuery
- Visual Studio

## Arquitectura

```text
UIL  -> interfaz web y manejo de sesión
BLL  -> validaciones y reglas de negocio
DAL  -> consultas parametrizadas y acceso a SQL Server
EDL  -> entidades y modelos de transferencia
```

La base de datos incluye tablas relacionadas, procedimientos almacenados y un
trigger que valida y descuenta existencias cuando se registra una prescripción.

## Requisitos

- Windows
- Visual Studio 2022 con la carga de trabajo **Desarrollo de ASP.NET y web**
- .NET Framework 4.8 Developer Pack
- SQL Server 2019 o posterior
- SQL Server Management Studio, recomendado

## Instalación local

1. Clona este repositorio.
2. Ejecuta, en orden, `database/01_HospitalDB.sql`,
   `database/02_actualizar_costos_tratamientos.sql` y
   `database/03_agrupar_pagos_por_cita.sql` en SQL Server Management Studio.
3. Ajusta la conexión `cnn` de `UIL/Web.config` si tu instancia no utiliza
   `Data Source=localhost`.
4. Abre `BD_Hospital.slnx` en Visual Studio.
5. Restaura los paquetes NuGet de la solución.
6. Establece `UIL` como proyecto de inicio y ejecuta la aplicación.

## Usuarios de demostración

El script incluye información ficticia para ejecutar el proyecto localmente.
Algunos usuarios disponibles son:

| Rol | Usuario | Contraseña |
| --- | --- | --- |
| Administrador | `admin1` | `123456` |
| Recepcionista | `recepcion1` | `123456` |
| Médico | `drmartinez` | `123456` |
| Paciente | `paciente1` | `123456` |

Estas credenciales son exclusivamente para una base de datos local de
demostración. No deben usarse en un despliegue público.

## Seguridad

- Las contraseñas se almacenan mediante PBKDF2-HMAC-SHA256 con salt.
- Las consultas de autenticación utilizan parámetros SQL.
- La cadena de conexión de ejemplo usa autenticación integrada de Windows y no
  contiene credenciales.
- Los errores internos no se muestran en la pantalla de inicio de sesión.

## Estructura del repositorio

```text
BLL/       Reglas de negocio
DAL/       Acceso a datos
EDL/       Entidades
UIL/       Aplicación ASP.NET Web Forms
database/  Creación y mejoras de la base de datos
docs/      Material visual del proyecto
```

## Validación funcional completa

Se comprobó el recorrido de extremo a extremo con los cuatro perfiles del
sistema y datos sintéticos. La evidencia incluye **60 capturas** de las
operaciones de creación, consulta, búsqueda, actualización, cancelación, pago y
eliminación que permite cada módulo.

| Área | Flujo validado |
| --- | --- |
| Administración | Hospitales, médicos, empleados, pacientes, medicamentos e inventario. |
| Recepción | Registro de pacientes, agenda, cancelación de citas y cobros. |
| Médico | Diagnóstico, tratamiento, prescripción, cierre de cita e historial. |
| Paciente | Activación, expediente, citas, pagos, totales y perfil. |

**[Ver las 60 capturas del recorrido funcional](docs/OPERACIONES.md)**

Todas las imágenes muestran información ficticia. Después de las pruebas se
eliminaron los registros temporales y se restauró la base de datos de
demostración.

### Galería destacada

<table>
  <tr>
    <td width="50%"><strong>Acceso unificado</strong><br><img src="docs/screenshots/operations/01-login-unificado.jpg" alt="Inicio de sesión unificado" width="100%"></td>
    <td width="50%"><strong>Panel administrativo</strong><br><img src="docs/screenshots/operations/02-admin-dashboard.jpg" alt="Panel administrativo" width="100%"></td>
  </tr>
  <tr>
    <td width="50%"><strong>Creación de citas</strong><br><img src="docs/screenshots/operations/25-recepcion-cita-creada.jpg" alt="Cita creada desde recepción" width="100%"></td>
    <td width="50%"><strong>Atención y prescripción</strong><br><img src="docs/screenshots/operations/34-medico-prescripcion-registrada.jpg" alt="Prescripción médica registrada" width="100%"></td>
  </tr>
  <tr>
    <td colspan="2"><strong>Pago desde el portal del paciente</strong><br><img src="docs/screenshots/operations/55-paciente-pago-confirmado.jpg" alt="Pago confirmado por el paciente" width="100%"></td>
  </tr>
</table>


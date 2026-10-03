# Recorrido funcional completo

Esta galería documenta un ciclo de pruebas de extremo a extremo del Sistema de
Gestión Hospitalaria. Las 60 capturas se realizaron con información sintética y
abarcan los cuatro perfiles disponibles: administrador, recepción, médico y
paciente.

> La prueba se ejecutó en un entorno local controlado. Al finalizar se eliminaron
> todos los registros temporales y la base de datos quedó restaurada con sus datos
> de demostración originales.

[Volver al README](../README.md)

## Cobertura verificada

| Perfil | Operaciones comprobadas |
| --- | --- |
| Administrador | Panel, hospitales, médicos, empleados, pacientes, medicamentos, inventario, búsqueda, creación, edición, actualización y eliminación. |
| Recepción | Panel, registro de pacientes, búsqueda, consulta de horarios, creación y cancelación de citas, perfil y cobro de tratamientos. |
| Médico | Panel, próximas citas, atención, diagnóstico, tratamiento, prescripción, cierre de cita, historial y consulta de pacientes. |
| Paciente | Activación de cuenta, panel, expediente, citas, disponibilidad, pagos, totales, historial y perfil. |

## 1. Acceso y panel administrativo

<table>
  <tr>
    <td width="50%"><strong>Inicio de sesión unificado</strong><br><img src="screenshots/operations/01-login-unificado.jpg" alt="Inicio de sesión unificado" width="100%"></td>
    <td width="50%"><strong>Panel de administración</strong><br><img src="screenshots/operations/02-admin-dashboard.jpg" alt="Panel de administración" width="100%"></td>
  </tr>
</table>

## 2. Administración de hospitales

<table>
  <tr>
    <td width="50%"><strong>Hospital registrado</strong><br><img src="screenshots/operations/03-admin-hospital-creado.jpg" alt="Hospital registrado" width="100%"></td>
    <td width="50%"><strong>Formulario de edición</strong><br><img src="screenshots/operations/04-admin-hospital-edicion.jpg" alt="Edición de hospital" width="100%"></td>
  </tr>
  <tr>
    <td width="50%"><strong>Hospital actualizado</strong><br><img src="screenshots/operations/05-admin-hospital-actualizado.jpg" alt="Hospital actualizado" width="100%"></td>
    <td width="50%"><strong>Búsqueda de hospitales</strong><br><img src="screenshots/operations/06-admin-hospital-busqueda.jpg" alt="Búsqueda de hospitales" width="100%"></td>
  </tr>
  <tr>
    <td colspan="2"><strong>Eliminación confirmada</strong><br><img src="screenshots/operations/60-admin-hospital-eliminado.jpg" alt="Hospital eliminado" width="100%"></td>
  </tr>
</table>

## 3. Medicamentos e inventario

<table>
  <tr>
    <td width="50%"><strong>Catálogo de medicamentos</strong><br><img src="screenshots/operations/07-admin-medicamentos-listado.jpg" alt="Listado de medicamentos" width="100%"></td>
    <td width="50%"><strong>Consulta de inventario</strong><br><img src="screenshots/operations/08-admin-inventario-consulta.jpg" alt="Consulta de inventario" width="100%"></td>
  </tr>
  <tr>
    <td width="50%"><strong>Medicamento registrado</strong><br><img src="screenshots/operations/09-admin-medicamento-creado.jpg" alt="Medicamento registrado" width="100%"></td>
    <td width="50%"><strong>Edición de medicamento</strong><br><img src="screenshots/operations/10-admin-medicamento-edicion.jpg" alt="Edición de medicamento" width="100%"></td>
  </tr>
  <tr>
    <td width="50%"><strong>Existencias asignadas</strong><br><img src="screenshots/operations/11-admin-medicamento-stock.jpg" alt="Actualización de existencias" width="100%"></td>
    <td width="50%"><strong>Medicamento eliminado</strong><br><img src="screenshots/operations/59-admin-medicamento-eliminado.jpg" alt="Medicamento eliminado" width="100%"></td>
  </tr>
</table>

## 4. Gestión de médicos

<table>
  <tr>
    <td width="50%"><strong>Médico registrado</strong><br><img src="screenshots/operations/12-admin-medico-creado.jpg" alt="Médico registrado" width="100%"></td>
    <td width="50%"><strong>Formulario de edición</strong><br><img src="screenshots/operations/13-admin-medico-edicion.jpg" alt="Edición de médico" width="100%"></td>
  </tr>
  <tr>
    <td width="50%"><strong>Médico actualizado</strong><br><img src="screenshots/operations/14-admin-medico-actualizado.jpg" alt="Médico actualizado" width="100%"></td>
    <td width="50%"><strong>Médico eliminado</strong><br><img src="screenshots/operations/58-admin-medico-eliminado.jpg" alt="Médico eliminado" width="100%"></td>
  </tr>
</table>

## 5. Gestión de empleados

<table>
  <tr>
    <td width="50%"><strong>Empleado registrado</strong><br><img src="screenshots/operations/15-admin-empleado-creado.jpg" alt="Empleado registrado" width="100%"></td>
    <td width="50%"><strong>Formulario de edición</strong><br><img src="screenshots/operations/16-admin-empleado-edicion.jpg" alt="Edición de empleado" width="100%"></td>
  </tr>
  <tr>
    <td width="50%"><strong>Empleado actualizado</strong><br><img src="screenshots/operations/17-admin-empleado-actualizado.jpg" alt="Empleado actualizado" width="100%"></td>
    <td width="50%"><strong>Empleado eliminado</strong><br><img src="screenshots/operations/57-admin-empleado-eliminado.jpg" alt="Empleado eliminado" width="100%"></td>
  </tr>
</table>

## 6. Gestión administrativa de pacientes

<table>
  <tr>
    <td width="50%"><strong>Paciente registrado</strong><br><img src="screenshots/operations/18-admin-paciente-creado.jpg" alt="Paciente registrado desde administración" width="100%"></td>
    <td width="50%"><strong>Formulario de edición</strong><br><img src="screenshots/operations/19-admin-paciente-edicion.jpg" alt="Edición de paciente" width="100%"></td>
  </tr>
  <tr>
    <td colspan="2"><strong>Paciente actualizado</strong><br><img src="screenshots/operations/20-admin-paciente-actualizado.jpg" alt="Paciente actualizado" width="100%"></td>
  </tr>
</table>

## 7. Flujo de recepción y citas

<table>
  <tr>
    <td width="50%"><strong>Panel de recepción</strong><br><img src="screenshots/operations/21-recepcion-dashboard.jpg" alt="Panel de recepción" width="100%"></td>
    <td width="50%"><strong>Paciente registrado</strong><br><img src="screenshots/operations/22-recepcion-paciente-registrado.jpg" alt="Paciente registrado en recepción" width="100%"></td>
  </tr>
  <tr>
    <td width="50%"><strong>Búsqueda del paciente</strong><br><img src="screenshots/operations/23-recepcion-buscar-paciente-cita.jpg" alt="Búsqueda de paciente para cita" width="100%"></td>
    <td width="50%"><strong>Horarios disponibles</strong><br><img src="screenshots/operations/24-recepcion-horarios-disponibles.jpg" alt="Horarios disponibles en recepción" width="100%"></td>
  </tr>
  <tr>
    <td width="50%"><strong>Cita creada</strong><br><img src="screenshots/operations/25-recepcion-cita-creada.jpg" alt="Cita creada desde recepción" width="100%"></td>
    <td width="50%"><strong>Control de citas</strong><br><img src="screenshots/operations/26-recepcion-control-citas.jpg" alt="Control de citas" width="100%"></td>
  </tr>
  <tr>
    <td width="50%"><strong>Cita cancelada</strong><br><img src="screenshots/operations/27-recepcion-cita-cancelada.jpg" alt="Cita cancelada" width="100%"></td>
    <td width="50%"><strong>Perfil actualizado</strong><br><img src="screenshots/operations/28-perfil-actualizado.jpg" alt="Perfil de usuario actualizado" width="100%"></td>
  </tr>
</table>

## 8. Cobros desde recepción

<table>
  <tr>
    <td width="50%"><strong>Tratamiento pendiente</strong><br><img src="screenshots/operations/41-recepcion-pago-pendiente.jpg" alt="Pago pendiente en recepción" width="100%"></td>
    <td width="50%"><strong>Selección del pago</strong><br><img src="screenshots/operations/42-recepcion-pago-seleccionado.jpg" alt="Pago seleccionado en recepción" width="100%"></td>
  </tr>
  <tr>
    <td colspan="2"><strong>Pago confirmado</strong><br><img src="screenshots/operations/43-recepcion-pago-confirmado.jpg" alt="Pago confirmado en recepción" width="100%"></td>
  </tr>
</table>

## 9. Atención médica

<table>
  <tr>
    <td width="50%"><strong>Panel del médico</strong><br><img src="screenshots/operations/29-medico-dashboard.jpg" alt="Panel del médico" width="100%"></td>
    <td width="50%"><strong>Próximas citas</strong><br><img src="screenshots/operations/30-medico-proximas-citas.jpg" alt="Próximas citas del médico" width="100%"></td>
  </tr>
  <tr>
    <td width="50%"><strong>Atención seleccionada</strong><br><img src="screenshots/operations/31-medico-atencion-seleccionada.jpg" alt="Atención médica seleccionada" width="100%"></td>
    <td width="50%"><strong>Diagnóstico guardado</strong><br><img src="screenshots/operations/32-medico-diagnostico-guardado.jpg" alt="Diagnóstico guardado" width="100%"></td>
  </tr>
  <tr>
    <td width="50%"><strong>Tratamiento agregado</strong><br><img src="screenshots/operations/33-medico-tratamiento-agregado.jpg" alt="Tratamiento agregado" width="100%"></td>
    <td width="50%"><strong>Prescripción registrada</strong><br><img src="screenshots/operations/34-medico-prescripcion-registrada.jpg" alt="Prescripción registrada" width="100%"></td>
  </tr>
  <tr>
    <td colspan="2"><strong>Cita finalizada</strong><br><img src="screenshots/operations/35-medico-cita-finalizada.jpg" alt="Cita médica finalizada" width="100%"></td>
  </tr>
</table>

## 10. Historial y pacientes del médico

<table>
  <tr>
    <td width="50%"><strong>Historial de citas</strong><br><img src="screenshots/operations/36-medico-historial.jpg" alt="Historial de citas del médico" width="100%"></td>
    <td width="50%"><strong>Detalle del historial</strong><br><img src="screenshots/operations/37-medico-historial-detalle.jpg" alt="Detalle del historial médico" width="100%"></td>
  </tr>
  <tr>
    <td width="50%"><strong>Pacientes atendidos</strong><br><img src="screenshots/operations/38-medico-pacientes.jpg" alt="Pacientes atendidos" width="100%"></td>
    <td width="50%"><strong>Citas del paciente</strong><br><img src="screenshots/operations/39-medico-paciente-citas.jpg" alt="Citas del paciente consultadas por el médico" width="100%"></td>
  </tr>
  <tr>
    <td colspan="2"><strong>Tratamientos del paciente</strong><br><img src="screenshots/operations/40-medico-paciente-tratamiento.jpg" alt="Tratamientos del paciente" width="100%"></td>
  </tr>
</table>

## 11. Activación y acceso del paciente

<table>
  <tr>
    <td width="50%"><strong>Activación preparada</strong><br><img src="screenshots/operations/44-paciente-activacion-preparada.jpg" alt="Activación de cuenta preparada" width="100%"></td>
    <td width="50%"><strong>Cuenta creada</strong><br><img src="screenshots/operations/45-paciente-cuenta-creada.jpg" alt="Cuenta de paciente creada" width="100%"></td>
  </tr>
  <tr>
    <td colspan="2"><strong>Panel del paciente</strong><br><img src="screenshots/operations/46-paciente-dashboard.jpg" alt="Panel del paciente" width="100%"></td>
  </tr>
</table>

## 12. Expediente, citas y pagos del paciente

<table>
  <tr>
    <td width="50%"><strong>Expediente personal</strong><br><img src="screenshots/operations/47-paciente-expediente.jpg" alt="Expediente del paciente" width="100%"></td>
    <td width="50%"><strong>Historial de pagos</strong><br><img src="screenshots/operations/48-paciente-pagos.jpg" alt="Pagos del paciente" width="100%"></td>
  </tr>
  <tr>
    <td width="50%"><strong>Total pagado por periodo</strong><br><img src="screenshots/operations/49-paciente-total-pagado.jpg" alt="Total pagado por periodo" width="100%"></td>
    <td width="50%"><strong>Mis citas</strong><br><img src="screenshots/operations/50-paciente-mis-citas.jpg" alt="Citas del paciente" width="100%"></td>
  </tr>
  <tr>
    <td width="50%"><strong>Consulta de disponibilidad</strong><br><img src="screenshots/operations/51-paciente-horarios-disponibles.jpg" alt="Horarios disponibles para el paciente" width="100%"></td>
    <td width="50%"><strong>Cita agendada</strong><br><img src="screenshots/operations/52-paciente-cita-agendada.jpg" alt="Cita agendada por el paciente" width="100%"></td>
  </tr>
  <tr>
    <td width="50%"><strong>Pago pendiente</strong><br><img src="screenshots/operations/53-paciente-pago-pendiente.jpg" alt="Pago pendiente del paciente" width="100%"></td>
    <td width="50%"><strong>Método de pago seleccionado</strong><br><img src="screenshots/operations/54-paciente-pago-seleccionado.jpg" alt="Pago seleccionado por el paciente" width="100%"></td>
  </tr>
  <tr>
    <td width="50%"><strong>Pago confirmado</strong><br><img src="screenshots/operations/55-paciente-pago-confirmado.jpg" alt="Pago confirmado por el paciente" width="100%"></td>
    <td width="50%"><strong>Perfil actualizado</strong><br><img src="screenshots/operations/56-paciente-perfil-actualizado.jpg" alt="Perfil del paciente actualizado" width="100%"></td>
  </tr>
</table>

## Notas de la validación

- La misma pantalla de acceso dirige al usuario al portal correspondiente según
  su rol.
- Los pagos se probaron con datos ficticios y no utilizaron una pasarela bancaria
  externa.
- El envío final de cambios de contraseña no se automatizó para evitar modificar
  credenciales de demostración; el formulario de perfil sí fue validado.
- La eliminación de pacientes está restringida por diseño para proteger el
  expediente clínico y su trazabilidad.

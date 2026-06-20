# Especificación de Requisitos de Software — BurgerPOS

**Versión**: 2.3
**Fecha**: 2026-06-16
**Proyecto**: Punto de Venta para Hamburguesería "BurgerPOS"
**Materia**: Procesos de Desarrollo de Software (PDS)
**Estándar de referencia**: IEEE 830-1998

---

## Historial de revisiones

| Versión | Fecha | Autor | Descripción del cambio |
|---|---|---|---|
| 1.0 | 2026-XX-XX | Equipo BurgerPOS | Versión entregada en el primer avance (referencia en `Avances-Previos/requisitos-v1.md`). |
| 2.0 | 2026-06-16 | Equipo BurgerPOS | Rediseño desde cero: se incorporan los procesos de corte de caja, cancelación de ventas, combos y bitácora de auditoría. Se alinea con IEEE 830 y se añade matriz de trazabilidad para amarrar las 5 entregas del curso. |
| 2.1 | 2026-06-16 | Equipo BurgerPOS | Se acota el alcance del ticket a **emisión digital únicamente** (vista en pantalla y descarga); la impresión física queda explícitamente fuera del sistema y se trata como un proceso del negocio independiente. |
| 2.2 | 2026-06-16 | Equipo BurgerPOS | Se añade **RF-13 Aplicar descuentos en el cobro** mediante campañas pre-configuradas por el administrador (porcentaje o monto fijo, alcance global o por línea, vigencia por fechas/días/horarios). Quedan diferidos a v2 los descuentos manuales con autorización. |
| 2.3 | 2026-06-16 | Equipo BurgerPOS | Tres incorporaciones: (1) **modificadores con precio** sobre productos (extra queso, doble carne, sin tocino) que sí afectan el subtotal —distintos de las notas libres—; (2) **vista de meseros para órdenes listas** que cierra el ciclo cocinero→mesero del RF-04; (3) **RF-14 Configurar datos del establecimiento** (nombre, dirección, contacto, RFC opcional) para identidad del ticket digital. Se documenta la **regla de negocio de precios históricos**: una venta cobrada conserva los precios al momento de la transacción. |

---

## 1. Introducción

### 1.1 Propósito

Este documento describe de manera completa el comportamiento esperado del sistema **BurgerPOS**, un punto de venta web para pequeños establecimientos del giro hamburguesería. Está dirigido a:

- El **equipo de desarrollo**, como referencia única para la implementación, las pruebas y el despliegue.
- El **profesor de la materia**, como evidencia formal de la disciplina de requisitos aplicada al proyecto.
- Los **stakeholders del negocio simulado** (dueño y operadores), como contrato funcional de lo que el sistema hará y no hará.

Esta versión es el insumo principal de la **Entrega 1** del curso y la base de los modelos de proceso, comportamiento, pruebas y despliegue de las Entregas 2 a 5.

### 1.2 Alcance del producto

**BurgerPOS** es una aplicación web (ASP.NET Core con Razor Pages) que automatiza la operación diaria de una hamburguesería pequeña: registro y autenticación del personal, gestión del menú, toma digital de órdenes, despacho hacia cocina con cola FIFO, cobro con efectivo o tarjeta —incluyendo aplicación de **descuentos por campaña**—, emisión **digital** de tickets, control básico de inventario, cierre de turno con corte de caja y reportes de ventas.

**Lo que el sistema hace:**

- Sustituye los post-its, el menú físico y los registros en Excel por un flujo digital.
- Mantiene en una base de datos PostgreSQL la verdad única de productos, órdenes, ventas, inventario y turnos.
- Se despliega como aplicación local sobre Linux, contenerizable mediante Docker para garantizar portabilidad y reproducibilidad del despliegue.

**Lo que el sistema NO hace** (restricciones de alcance heredadas del documento v1 y vigentes):

- No se integra directamente con la terminal de Mercado Pago; solo registra que el cobro fue con tarjeta y la comisión bancaria asociada.
- No emite facturación electrónica (CFDI / SAT).
- No gestiona reservaciones de mesa.
- No expone una aplicación móvil para clientes ni se integra con plataformas de delivery.
- No realiza nómina ni contabilidad fiscal del negocio.
- **No imprime tickets en hardware físico**. El sistema genera el ticket digital (visible en pantalla y exportable como archivo); cualquier impresión queda fuera del alcance y se considera un proceso operativo del negocio, ajeno a BurgerPOS.

### 1.3 Definiciones, acrónimos y abreviaturas

| Término | Significado |
|---|---|
| **POS** | Point of Sale, punto de venta. |
| **ERS** | Especificación de Requisitos de Software. |
| **RF** | Requisito Funcional. |
| **RNF** | Requisito No Funcional. |
| **CU** | Caso de Uso. |
| **FIFO** | First In, First Out — política de despacho de cocina por orden de llegada. |
| **Orden** | Solicitud de uno o más productos para una mesa, para llevar o para mostrador, antes de ser cobrada. |
| **Venta** | Orden ya cobrada con su ticket digital emitido. |
| **Ticket digital** | Comprobante de venta generado por el sistema y visualizado en pantalla o exportado como archivo (PDF/HTML). BurgerPOS no realiza impresión física. |
| **Turno de caja** | Periodo entre la apertura de caja (declaración del fondo inicial) y el corte (arqueo final). |
| **Corte de caja** | Proceso de conciliación entre el efectivo físico y el esperado por el sistema al cerrar el turno. |
| **Insumo** | Materia prima cuya existencia disminuye al vender productos (paquetes de pan, kg de carne, etc.). |
| **Combo** | Producto que agrupa varios productos individuales a precio especial. Definido en el menú con su propio precio. |
| **Modificador** | Opción configurable sobre un producto que **altera su precio**: aditivo (extra queso +$15, doble carne +$30) o sustractivo (sin tocino −$5). Distinto de una **nota libre**, que es texto para cocina sin efecto en el precio (ej. "sin cebolla", "término medio"). |
| **Nota libre** | Texto opcional adjunto a una línea de orden con indicaciones para cocina, sin impacto en el precio. |
| **Campaña de descuento** | Promoción pre-configurada por el administrador que reduce el subtotal al cobrar. Se define con tipo (porcentaje o monto fijo), alcance (toda la venta o producto/categoría), vigencia (fechas, días de la semana, horarios) y estado (activa, pausada, expirada). |
| **Descuento aplicado** | Instancia concreta de una campaña de descuento ejecutada sobre una venta específica, con el monto deducido registrado en bitácora. |
| **Datos del establecimiento** | Información identitaria del negocio configurada por el administrador (nombre comercial, dirección, teléfono, RFC opcional) que se incluye en cada ticket digital. |
| **Precio histórico** | Precio de un producto vigente en el momento en que se cobró una venta. Las ventas pasadas se reportan con el precio histórico aunque el catálogo se actualice después. |
| **IVA** | Impuesto al Valor Agregado en México (16%). |
| **CRUD** | Create, Read, Update, Delete. |
| **BPMN2** | Business Process Model and Notation, versión 2. |
| **UML** | Unified Modeling Language. |
| **IEEE 830** | Estándar para la redacción de Especificaciones de Requisitos de Software. |

### 1.4 Referencias

- IEEE Std 830-1998: *Recommended Practice for Software Requirements Specifications*.
- Documento de requisitos v1 entregado en el primer avance: `Avances-Previos/requisitos-v1.md`.
- Microsoft Docs: *ASP.NET Core Razor Pages* (https://learn.microsoft.com/aspnet/core/razor-pages/).
- Object Management Group: *BPMN 2.0 Specification* y *UML 2.5.1 Specification*.

### 1.5 Visión general del documento

El documento sigue la estructura recomendada por IEEE 830:

- La **Sección 2** ofrece un panorama del producto, sus usuarios y sus restricciones.
- La **Sección 3** detalla los requisitos funcionales, no funcionales, de interfaz y reglas de negocio.
- La **Sección 4** narra las funcionalidades como historias de usuario.
- La **Sección 5** presenta la matriz de trazabilidad que conecta cada RF con su caso de uso, su proceso modelado y su caso de prueba — el hilo conductor entre las 5 entregas del curso.
- La **Sección 6** contiene el glosario extendido.

---

## 2. Descripción general

### 2.1 Perspectiva del producto

BurgerPOS es un producto **autocontenido**: no es un módulo de un sistema mayor ni depende de servicios externos en línea. Vive dentro del propio establecimiento como aplicación web servida por un servidor Linux local (físico o virtualizado) al que se conectan los dispositivos del personal mediante la red local.

```
┌──────────────────────────────────────────────────────┐
│            Hamburguesería (red local)                │
│                                                      │
│      ┌────────┐    ┌────────┐    ┌────────┐          │
│      │ Tablet │    │  PC    │    │ Tablet │          │
│      │ mesero │    │ cobro  │    │ cocina │          │
│      └───┬────┘    └───┬────┘    └───┬────┘          │
│          │             │             │               │
│          └─────────────┴─────────────┘               │
│                        │                             │
│               ┌────────▼────────┐                    │
│               │  Servidor local │                    │
│               │  BurgerPOS +    │                    │
│               │  PostgreSQL     │                    │
│               └─────────────────┘                    │
└──────────────────────────────────────────────────────┘
```

El producto se distribuye en dos contenedores Docker (uno para la aplicación, otro para la base de datos) coordinados por Docker Compose, lo cual habilita los escenarios de despliegue exigidos por la Entrega 5.

### 2.2 Funciones del producto (resumen)

A alto nivel, BurgerPOS habilita las siguientes funciones, que se detallan como RF en la Sección 3.1:

1. **Autenticación y trazabilidad del personal** con declaración del rol del turno.
2. **Gestión del menú** (productos individuales, combos, categorías y **modificadores con precio**).
3. **Toma digital de órdenes** con modalidades mesa / mostrador / para llevar, selección de modificadores y notas libres.
4. **Despacho a cocina y entrega**: cola FIFO con vista de cocinero **y vista de meseros para órdenes listas**.
5. **Cobro de ventas** en efectivo o tarjeta, incluyendo propinas y cálculo de cambio.
6. **Emisión de tickets digitales** con folio consecutivo, visualizables en pantalla y exportables como archivo.
7. **Control de inventario** a nivel de insumo con descuento automático por receta.
8. **Reportes de ventas** diarios, semanales y mensuales.
9. **Turno de caja y corte** con apertura, operación y arqueo.
10. **Cancelación y reembolso** de órdenes y ventas con justificación.
11. **Bitácora de auditoría** que registra autor y momento de cada operación sensible.
12. **Gestión y aplicación de descuentos** mediante campañas pre-configuradas por el administrador (porcentaje o monto fijo) que el operador aplica al cobrar.
13. **Configuración de datos del establecimiento** (nombre, dirección, contacto, RFC opcional) que identifican al negocio en los tickets digitales.

### 2.3 Características de los usuarios

BurgerPOS reconoce **dos tipos de cuenta** y **tres roles operativos** que rotan diariamente:

| Cuenta | Quién es | Acceso | Conocimiento técnico |
|---|---|---|---|
| **Administrador** | El dueño del establecimiento. Único, no rota. No trabaja en piso. | Total: catálogo, inventario, reportes, gestión de personal, configuración. | Básico-medio (usuario doméstico de computadora). |
| **Operador** | Los 5 trabajadores. Cuentas individuales. | Operación diaria: tomar órdenes, cocinar, cobrar, abrir/cerrar caja. | Nulo a básico — pueden ser personas sin experiencia previa con software. |

Cada **Operador** declara al iniciar sesión el **rol operativo** que desempeñará ese día (Cocinero, Mesero o Encargado de Cobro). El rol **no restringe** qué pantallas puede abrir — todos los operadores tienen acceso a las mismas funciones — pero sí queda registrado para reportes y bitácora. Esto refleja la realidad del negocio: en una hamburguesería pequeña cualquiera puede ayudar en cualquier estación, y el sistema no debe estorbar.

El **Administrador**, en cambio, es el único que accede a catálogo, inventario, configuración y reportes financieros consolidados.

### 2.4 Restricciones

#### Restricciones técnicas
- **R-T1** El sistema debe implementarse en **.NET 10 con Razor Pages** y **xUnit** para pruebas (impuesto por la materia).
- **R-T2** La base de datos debe ser **PostgreSQL** para garantizar portabilidad entre los tres escenarios de despliegue.
- **R-T3** El sistema debe poder desplegarse en **Docker** y orquestarse con **Docker Compose**.
- **R-T4** Debe ser accesible vía **navegador web moderno** desde cualquier dispositivo de la red local (PC, tablet, smartphone).

#### Restricciones de negocio
- **R-N1** El IVA mexicano vigente es **16%** y se desglosa en cada ticket.
- **R-N2** Los folios de venta son **consecutivos y únicos** por establecimiento, sin permitir saltos.
- **R-N3** Una venta cobrada no puede modificarse; solo puede cancelarse mediante un proceso de **anulación con justificación**, que queda en bitácora.

#### Restricciones de alcance heredadas del documento v1
- Sin integración directa con la terminal de Mercado Pago.
- Sin facturación electrónica (CFDI).
- Sin reservaciones.
- Sin app móvil para clientes ni integraciones con delivery.

### 2.5 Suposiciones y dependencias

- **S-1** El establecimiento cuenta con red local WiFi estable durante el horario de operación.
- **S-2** El servidor local cuenta con UPS o respaldo eléctrico mínimo para evitar pérdidas por apagón.
- **S-3** El cobro con tarjeta se realiza en una terminal Mercado Pago **independiente**; BurgerPOS solo registra que se usó tarjeta y la comisión, no se comunica con la terminal.
- **S-4** Los productos del menú pueden modelarse con un **recetario simple** que indica cuánto consume cada producto de cada insumo (sin necesidad de gramaje fino).
- **S-5** El navegador del dispositivo del cliente interno cumple con estándares modernos (Chrome/Edge/Firefox actuales).
- **S-6** Si el negocio requiere copias físicas del ticket, lo resuelve fuera del sistema (por ejemplo, descargando el archivo y mandándolo a imprimir desde el navegador o desde otra herramienta del SO); la entrega del ticket impreso no forma parte de los flujos de BurgerPOS.

### 2.6 Reparto de requisitos en futuras versiones

Para mantener el alcance de la primera versión razonable, se difieren a versiones posteriores:

| Posible v2.x | Descripción |
|---|---|
| División de cuenta (split) | Permitir que una orden se cobre en partes a clientes distintos. |
| Programa de lealtad | Acumulación de puntos por cliente recurrente. |
| Modo offline | Continuar tomando órdenes si el servidor local cae temporalmente y sincronizar después. |
| Multi-sucursal | Consolidar reportes de varias hamburgueserías en un mismo administrador. |
| Integración con CFDI | Facturación electrónica para clientes que la soliciten. |
| Descuentos manuales con autorización | Que el operador pueda capturar un descuento arbitrario (no pre-configurado) con autorización explícita del administrador (PIN o flujo de aprobación). En v1 solo se aplican campañas pre-configuradas. |

---

> **Continuará en §3 Requisitos específicos** — siguiente parte del documento.

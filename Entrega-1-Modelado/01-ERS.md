# Especificación de Requisitos de Software — BurgerPOS

**Versión**: 2.6
**Fecha**: 2026-06-16
**Proyecto**: Punto de Venta para Hamburguesería "BurgerPOS"
**Materia**: Procesos de Desarrollo de Software (PDS)
**Estándar de referencia**: IEEE 830-1998

---

## Historial de revisiones

| Versión | Fecha | Autor | Descripción del cambio |
|---|---|---|---|
| 1.0 | 2026-XX-XX | Sosa Fernández · Guerrero Cabrera · Vázquez | Versión entregada en el primer avance (referencia en `Avances-Previos/requisitos-v1.md`). |
| 2.0 | 2026-06-16 | Sosa Fernández · Guerrero Cabrera · Vázquez | Rediseño desde cero: se incorporan los procesos de corte de caja, cancelación de ventas, combos y bitácora de auditoría. Se alinea con IEEE 830 y se añade matriz de trazabilidad para amarrar las 5 entregas del curso. |
| 2.1 | 2026-06-16 | Sosa Fernández · Guerrero Cabrera · Vázquez | Se acota el alcance del ticket a **emisión digital únicamente** (vista en pantalla y descarga); la impresión física queda explícitamente fuera del sistema y se trata como un proceso del negocio independiente. |
| 2.2 | 2026-06-16 | Sosa Fernández · Guerrero Cabrera · Vázquez | Se añade **RF-13 Aplicar descuentos en el cobro** mediante campañas pre-configuradas por el administrador (porcentaje o monto fijo, alcance global o por línea, vigencia por fechas/días/horarios). Quedan diferidos a v2 los descuentos manuales con autorización. |
| 2.3 | 2026-06-16 | Sosa Fernández · Guerrero Cabrera · Vázquez | Tres incorporaciones: (1) **modificadores con precio** sobre productos (extra queso, doble carne, sin tocino) que sí afectan el subtotal —distintos de las notas libres—; (2) **vista de meseros para órdenes listas** que cierra el ciclo cocinero→mesero del RF-04; (3) **RF-14 Configurar datos del establecimiento** (nombre, dirección, contacto, RFC opcional) para identidad del ticket digital. Se documenta la **regla de negocio de precios históricos**: una venta cobrada conserva los precios al momento de la transacción. |
| 2.4 | 2026-06-16 | Sosa Fernández · Guerrero Cabrera · Vázquez | Se aclara explícitamente que la arquitectura por defecto es **on-premise** (servidor local en el establecimiento) y se añade una nota de **portabilidad a nube** en §2.1: el empaquetado en contenedores Docker permite reproducir el despliegue en un proveedor cloud si el cliente lo requiere en el futuro. Esto deja la decisión de hospedaje como atributo de instalación, no como restricción técnica del producto. |
| 2.5 | 2026-06-20 | Sosa Fernández · Guerrero Cabrera · Vázquez | Se cierra un gap detectado en revisión: la **gestión inicial del catálogo de insumos y la definición de recetas** no estaban explícitas. RF-02 incorpora la **receta del producto** (qué insumos consume y cuántos por unidad vendida) como parte de la configuración del producto. RF-07 se renombra a "Gestionar inventario de insumos" y se reescribe con cinco pasos explícitos: CRUD de insumos, registro de entradas, **ajuste manual con justificación** (para diferencias de conteo físico), descuento automático al vender y alertas de nivel bajo. |
| 2.6 | 2026-06-20 | Sosa Fernández · Guerrero Cabrera · Vázquez | Se **redistribuyen las responsabilidades de inventario** entre Operador y Administrador para reflejar la operación real del negocio. Operador participa en: registrar entradas (recepción de mercancía) y reportar **mermas** con motivo obligatorio desde lista predefinida (quemado, caído, caducidad, otro). Administrador queda exclusivo en: CRUD del catálogo de insumos, **ajustes libres de saldo** (post conteo físico) y configuración de niveles de alerta. Se actualiza §2.3 con el reparto y se añaden los términos **Merma** y **Ajuste de saldo** al glosario. |

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
| **Merma** | Disminución del saldo de un insumo por causas operativas (quemado, caído, derramado, caducado, etc.), reportada por un Operador o Administrador con motivo obligatorio elegido de una lista predefinida o capturado como texto libre. Se distingue del ajuste de saldo por estar acotada a razones predefinidas y no permitir fijar cualquier valor arbitrario. |
| **Ajuste de saldo** | Modificación libre del saldo de un insumo realizada exclusivamente por el Administrador para reflejar el resultado de un conteo físico o corregir errores acumulados. Requiere motivo obligatorio y queda en bitácora; es el control más sensible del inventario por su capacidad de fijar cualquier cantidad sin restricción. |
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

**Nota sobre portabilidad de despliegue.** El escenario por defecto descrito arriba es **on-premise** (servidor en el propio establecimiento) y es el contemplado por la primera versión del producto. Sin embargo, la arquitectura contenerizada hace que el mismo paquete pueda ejecutarse en un proveedor cloud (DigitalOcean, AWS EC2, Azure VM, etc.) si el cliente requiriera acceso remoto del administrador, respaldos gestionados por un proveedor o tercerización del mantenimiento del servidor. Esa modalidad alternativa queda contemplada por la arquitectura pero fuera del alcance de la versión 1; documentarla aquí evita que una decisión futura del cliente fuerce un rediseño.

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

El **Administrador**, en cambio, es el único que accede al **catálogo del menú** (productos, combos, recetas, modificadores y categorías), al **CRUD del catálogo de insumos** y configuración de niveles de alerta, a los **ajustes libres de saldo** de inventario, a la **configuración general** del establecimiento y a los **reportes financieros consolidados**. Los Operadores **sí participan en las operaciones cotidianas de inventario** —registrar entradas cuando llega mercancía del proveedor y reportar mermas con motivo obligatorio— porque son ellos quienes están presentes durante esas actividades en la realidad del negocio; lo que queda restringido al Administrador es el **catálogo** (configuración) y los **ajustes libres** (vector de fraude potencial).

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

## 3. Requisitos específicos

Esta sección detalla los requisitos de BurgerPOS al nivel necesario para implementarlos y probarlos. Cada requisito funcional sigue la plantilla:

- **Descripción** — qué debe hacer el sistema.
- **Actores** — quién dispara o participa.
- **Entradas** — datos requeridos para ejecutar.
- **Proceso** — pasos del sistema.
- **Salidas** — resultados observables.
- **Prioridad** — Alta / Media / Baja para priorización del backlog.
- **Fuente** — de dónde proviene el requisito (negocio, restricción, gap detectado, etc.).
- **Procesos relevantes asociados** — referencia a los 10 procesos modelados en la Entrega 2 (P-01 a P-10).

### 3.1 Requisitos funcionales

#### RF-01 — Autenticar usuario y declarar rol del día

**Descripción.** El sistema autentica a los usuarios mediante credenciales únicas. Al iniciar sesión, los Operadores declaran qué rol operativo desempeñarán durante el turno (cocinero, mesero o encargado de cobro). El rol declarado no restringe el acceso a las pantallas, pero queda registrado para reportes y bitácora.

- **Actores.** Operador, Administrador, Sistema de autenticación.
- **Entradas.** Nombre de usuario, contraseña; rol operativo del día (solo aplica a Operadores).
- **Proceso.**
  1. El sistema valida las credenciales contra la BD.
  2. Si es Operador, solicita la declaración del rol operativo.
  3. Crea una sesión activa y la asocia al usuario y al rol declarado.
  4. Redirige a la página de inicio correspondiente.
- **Salidas.** Sesión activa, registro de inicio de sesión en bitácora, redirección al panel del usuario.
- **Prioridad.** Alta.
- **Fuente.** Negocio (RNF-01 usabilidad + necesidad de trazabilidad por persona).
- **Procesos relevantes asociados.** P-01 (Autenticar usuario).

#### RF-02 — Gestionar el menú (productos, combos, categorías, modificadores y recetas)

**Descripción.** El Administrador realiza el CRUD del catálogo: productos individuales, combos, categorías y modificadores con precio. Los cambios deben reflejarse de inmediato en los dispositivos operativos sin requerir reinicio. Los modificadores son opciones que alteran el precio del producto (aditivos como "extra queso +$15", "doble carne +$30", o sustractivos como "sin tocino −$5") y son distintos de las notas libres, que solo son texto para cocina sin impacto en el precio.

Cada producto se asocia adicionalmente a una **receta** que especifica qué insumos consume y cuánto de cada uno por unidad vendida. La receta es lo que permite el descuento automático de inventario al cobrar (RF-07). Los insumos referenciados deben existir previamente en el catálogo de inventario gestionado por RF-07; conceptualmente, primero el administrador da de alta sus insumos y luego define los productos que los consumen.

- **Actores.** Administrador, Sistema.
- **Entradas.** Datos del producto (nombre, descripción, precio base, categoría, indicador de disponibilidad, imagen opcional); composición del combo (productos incluidos + precio especial); lista de modificadores aplicables (nombre + delta de precio); nombre y orden de categorías; **receta del producto** (lista de pares insumo + cantidad consumida por unidad vendida, donde los insumos referenciados existen en el catálogo de RF-07).
- **Proceso.**
  1. El administrador crea, edita o desactiva productos, combos, modificadores y categorías.
  2. El sistema valida campos obligatorios y precios no negativos.
  3. Un producto puede marcarse como "no disponible" sin eliminarlo (para cuando se agota un insumo en el día).
  4. Al crear o editar un producto, el administrador define su **receta** seleccionando uno o más insumos del catálogo (RF-07) y capturando la cantidad consumida por unidad vendida (por ejemplo: "Hamburguesa Clásica" → 1 pan + 1 carne + 1 rebanada de queso). La receta puede modificarse en cualquier momento; las ventas pasadas conservan el consumo histórico ya registrado.
  5. Las pantallas operativas leen el catálogo actualizado de la BD en cada operación.
- **Salidas.** Catálogo actualizado con recetas, cambios visibles en pantallas de toma de orden y cocina.
- **Prioridad.** Alta.
- **Fuente.** Negocio.
- **Procesos relevantes asociados.** P-02 (Gestionar producto del menú).

#### RF-03 — Tomar orden digital

**Descripción.** El Operador (en rol de mesero o cobro) crea una orden, le asocia una modalidad de servicio (mesa, mostrador o para llevar), agrega líneas con cantidad, modificadores seleccionados y notas libres opcionales. La orden puede editarse libremente antes de enviarse a cocina; una vez enviada, queda visible en la vista de cocina.

- **Actores.** Operador (mesero o cobro).
- **Entradas.** Modalidad (Mesa / Mostrador / Para llevar), número de mesa (si aplica), productos seleccionados con cantidad, modificadores activos por línea, notas libres por línea.
- **Proceso.**
  1. El operador crea una orden en estado **Borrador**.
  2. Agrega, edita o elimina líneas hasta confirmar.
  3. El sistema calcula un subtotal preliminar (sumando producto base + modificadores aditivos − sustractivos) por línea y total.
  4. Al enviar, valida que existan productos vigentes (no marcados "no disponible") y mueve la orden a **EnviadaACocina**.
  5. Notifica la vista de cocina con la nueva orden.
- **Salidas.** Orden con líneas, modificadores y notas; subtotal preliminar; entrada en cola FIFO de cocina.
- **Prioridad.** Alta.
- **Fuente.** Negocio.
- **Procesos relevantes asociados.** P-03 (Tomar orden), P-04 (Enviar orden a cocina).

#### RF-04 — Despachar órdenes (vista de cocina FIFO + vista de meseros)

**Descripción.** La cocina visualiza las órdenes en orden de llegada (FIFO) con todo el detalle necesario para prepararlas. El cocinero marca cada orden como "lista" cuando termina. Los meseros tienen una vista complementaria donde aparecen las órdenes listas para entregar, con indicador visual claro, para cerrar el ciclo cocinero → mesero sin necesidad de ir físicamente a cocina a revisar.

- **Actores.** Operador (cocinero), Operador (mesero).
- **Entradas.** Cola de órdenes en estado **EnviadaACocina**; acción del cocinero al marcar lista; acción del mesero al confirmar entrega.
- **Proceso.**
  1. La cocina muestra las órdenes ordenadas por hora de envío (FIFO), con número de orden, modalidad/mesa, líneas con cantidad, modificadores, notas, y hora.
  2. El cocinero marca una orden como **Lista**, momento en el que desaparece de su vista activa.
  3. La orden lista aparece en la **vista de meseros** con indicador visual (badge, color) y la mesa o identificador.
  4. El mesero marca la orden como **Entregada** al cliente.
  5. Las órdenes completadas se mueven al historial accesible para reportes.
- **Salidas.** Orden en estado **Lista** → **Entregada**; historial actualizado.
- **Prioridad.** Alta.
- **Fuente.** Negocio + gap del v1 (no había canal explícito cocinero→mesero).
- **Procesos relevantes asociados.** P-05 (Preparar y marcar orden lista).

#### RF-05 — Procesar cobro de venta

**Descripción.** El Operador (cobro) liquida una orden ya entregada (o lista, en caso de mostrador). Captura método de pago, propina opcional y descuento por campaña opcional. El sistema calcula subtotal, descuento aplicado, IVA, total, y —si el pago es en efectivo— el cambio a devolver. Al confirmar, la venta queda registrada con folio consecutivo único e inmutable.

- **Actores.** Operador (cobro), Sistema.
- **Entradas.** Orden a cobrar (estado **Entregada** o **Lista**), método de pago (Efectivo / Tarjeta), monto recibido (solo efectivo), propina opcional, campaña de descuento vigente (opcional).
- **Proceso.**
  1. El sistema carga la orden con sus líneas y subtotal.
  2. El operador puede agregar propina (monto o porcentaje sugerido).
  3. El operador puede seleccionar **una** campaña de descuento de la lista de campañas vigentes en ese momento (ver RF-13).
  4. El sistema recalcula: subtotal − descuento → base gravable; aplica IVA 16% → total.
  5. Si el método es Tarjeta, el sistema solicita confirmación y registra la comisión bancaria configurada.
  6. Si el método es Efectivo, el operador captura el monto recibido y el sistema calcula el cambio.
  7. El sistema asigna folio consecutivo único, registra la venta, dispara el descuento de inventario (RF-07) y la emisión del ticket (RF-06).
- **Salidas.** Venta registrada con folio, cambio (efectivo), inventario actualizado, ticket disponible para emitir.
- **Prioridad.** Alta.
- **Fuente.** Negocio + R-N1 (IVA 16%), R-N2 (folios consecutivos), R-N3 (venta inmutable).
- **Procesos relevantes asociados.** P-06 (Procesar cobro).

#### RF-06 — Emitir ticket digital

**Descripción.** El sistema genera un ticket digital con identidad profesional del establecimiento al cerrar una venta. El ticket se muestra en pantalla con vista previa y puede exportarse como archivo (PDF/HTML) para descarga. **BurgerPOS no realiza impresión física**; cualquier copia impresa es responsabilidad operativa del negocio fuera del sistema.

- **Actores.** Operador, Sistema.
- **Entradas.** Venta registrada (resultado de RF-05).
- **Proceso.**
  1. El sistema toma los datos del establecimiento configurados en RF-14 (nombre comercial, dirección, teléfono, RFC opcional).
  2. Genera el ticket con: cabecera del establecimiento, fecha y hora, folio consecutivo, modalidad y mesa, detalle de productos con cantidad y **precios históricos** (los vigentes al momento del cobro), modificadores y sus deltas, subtotal, descuento aplicado (si lo hubo, con nombre de campaña), IVA 16%, propina, total, método de pago, y operador responsable.
  3. Lo presenta en pantalla con opción de exportar.
- **Salidas.** Ticket en pantalla, archivo descargable.
- **Prioridad.** Alta.
- **Fuente.** Negocio + R-N1, R-N2.
- **Procesos relevantes asociados.** P-07 (Generar y emitir ticket digital).

#### RF-07 — Gestionar inventario de insumos

**Descripción.** El sistema mantiene un catálogo de insumos del negocio (paquetes de pan, kg de carne, refrescos, queso, lechuga, etc.) con su existencia actual y nivel de alerta. Las responsabilidades se reparten entre Administrador y Operadores para reflejar la operación real del negocio: el Administrador gestiona el **catálogo** (alta de insumos, configuración de alertas) y los **ajustes libres** de saldo (típicamente tras un conteo físico), mientras que los Operadores manejan la **operación cotidiana** —registrar las entradas cuando llega mercancía del proveedor y reportar mermas con motivo obligatorio cuando algo se daña, se cae o caduca durante la operación. Al confirmarse una venta, el sistema descuenta automáticamente los insumos según la receta definida en cada producto vendido (RF-02). Cuando un saldo cae por debajo de su nivel de alerta, el sistema lo señaliza visiblemente.

El flujo lógico de configuración inicial es: el Administrador (1) da de alta sus insumos, (2) define los productos con sus recetas en RF-02 referenciando los insumos creados; a partir de ahí, los Operadores manejan el día a día (entradas y mermas) y cada venta confirmada descuenta inventario automáticamente.

- **Actores.** Administrador, Operador, Sistema.
- **Entradas.**
  - **Administrador**: datos del insumo (nombre, unidad de medida, existencia inicial, nivel de alerta) para CRUD; datos del ajuste libre (insumo, nueva cantidad, motivo obligatorio).
  - **Operador o Administrador**: datos de la entrada (insumo, cantidad, fecha, proveedor opcional, costo opcional); datos de la merma (insumo, cantidad, motivo desde lista predefinida o texto libre).
  - **Sistema**: ventas confirmadas (provenientes de RF-05).
- **Proceso.**
  1. **CRUD del catálogo de insumos (solo Administrador).** Crea, edita o desactiva insumos. Cada insumo tiene nombre único, unidad de medida (piezas, kg, litros, gramos, etc.), existencia inicial al momento de su creación y nivel de alerta configurable.
  2. **Registrar entrada de inventario (Operador o Administrador).** Cuando llega mercancía del proveedor, el usuario captura una entrada que **aumenta** el saldo del insumo. Queda en bitácora (RF-12) con autor, fecha, proveedor opcional y costo opcional.
  3. **Reportar merma (Operador o Administrador).** Cuando un insumo se daña, se cae, se caduca o se pierde durante la operación, el usuario reporta una merma que **disminuye** el saldo del insumo. El motivo es obligatorio y se elige de la siguiente lista predefinida:
     - *Se quemó o se dañó al preparar* — típico en cocina.
     - *Se cayó o se derramó* — accidentes operativos.
     - *Caducidad o fecha vencida* — insumos perecederos.
     - *Otro* — requiere texto libre obligatorio adicional describiendo el caso.
     La merma queda en bitácora con autor, fecha, motivo y cantidad.
  4. **Ajustar saldo libremente (solo Administrador).** Cuando un conteo físico difiere del saldo del sistema (por mermas no reportadas, errores acumulados, robo, etc.), el Administrador captura la cantidad correcta y un motivo obligatorio. El sistema calcula la diferencia, la registra junto con motivo, responsable, fecha y hora; el saldo del insumo se ajusta al valor capturado. Esta operación es deliberadamente exclusiva del Administrador porque permite fijar cualquier valor sin restricción y es un vector potencial de fraude; el motivo obligatorio + bitácora son los controles que la mitigan.
  5. **Descontar automáticamente al vender (Sistema).** Al confirmar una venta (RF-05), el sistema descuenta cada insumo según la receta definida en cada producto vendido (RF-02). Si la venta incluye un combo (RF-11), descuenta los insumos de cada producto componente. Si el descuento dejara un saldo por debajo de su nivel de alerta, dispara la alerta del paso siguiente.
  6. **Alertar nivel bajo (Sistema).** Compara cada saldo con el nivel de alerta del insumo. Cuando uno está por debajo, lo señaliza visiblemente en el panel del Administrador y en las pantallas operativas.
- **Salidas.** Catálogo de insumos actualizado, saldos correctos, alertas visibles, historial completo de movimientos (entradas, mermas, ajustes, descuentos por venta) en bitácora con autor identificado.
- **Prioridad.** Alta.
- **Fuente.** Negocio + gaps detectados en revisión 2026-06-20 (creación de catálogo y separación de responsabilidades operador/administrador no estaban explícitas en v2.4-v2.5).
- **Procesos relevantes asociados.** P-08 (Registrar entrada de inventario) cubre el reabastecimiento, que es el movimiento de mayor frecuencia y puede ser ejecutado tanto por Operador como por Administrador. Las mermas y los ajustes libres son operaciones secundarias que se cubren por casos de prueba (Entrega 4) sin BPMN propio.

#### RF-08 — Generar reportes de ventas

**Descripción.** El Administrador consulta reportes de ventas por periodo (diario, semanal, mensual o rango personalizado) con indicadores agregados, desglose por método de pago, productos más vendidos, descuentos aplicados, propinas, IVA, comisiones bancarias y ganancias brutas/netas.

- **Actores.** Administrador.
- **Entradas.** Rango de fechas (predeterminado o personalizado), filtros opcionales (modalidad, operador, categoría).
- **Proceso.**
  1. El sistema agrega las ventas del rango.
  2. Calcula: total bruto, número de transacciones, ingreso promedio por venta, productos top (cantidad e ingreso), monto total de propinas, monto total descontado por campaña, IVA recaudado, comisiones bancarias acumuladas, ganancia bruta (total − comisiones) y neta (bruta − IVA).
  3. Desglose por método de pago (efectivo vs. tarjeta).
  4. Presenta los resultados en pantalla con tablas y, opcionalmente, gráficos simples.
- **Salidas.** Reporte consultable en pantalla.
- **Prioridad.** Media.
- **Fuente.** Negocio.
- **Procesos relevantes asociados.** P-10 (Generar reporte de ventas).

#### RF-09 — Abrir y cerrar turno de caja (corte de caja)

**Descripción.** El Operador (cobro) abre el turno declarando el fondo inicial de caja; durante el turno, todas las ventas en efectivo se asocian al turno abierto. Al cierre, el operador (o administrador) realiza el arqueo: cuenta el efectivo físico y lo captura. El sistema calcula el efectivo esperado (fondo inicial + ventas en efectivo − retiros) y muestra la diferencia. El turno cerrado queda en historial.

- **Actores.** Operador (cobro), Administrador.
- **Entradas.** Fondo de caja inicial (apertura), retiros parciales opcionales con justificación, efectivo contado al cierre, observaciones.
- **Proceso.**
  1. El operador inicia un nuevo turno con el fondo inicial declarado. Solo puede haber un turno abierto a la vez.
  2. Durante el turno, las ventas en efectivo (RF-05) se acumulan en su saldo esperado.
  3. Al cerrar, el operador captura el efectivo contado.
  4. El sistema calcula: esperado = fondo inicial + ventas efectivo − retiros; diferencia = contado − esperado.
  5. El turno pasa a estado **Cerrado**; queda accesible en historial con todos sus movimientos.
- **Salidas.** Reporte de arqueo (esperado, contado, diferencia), turno cerrado, bitácora actualizada.
- **Prioridad.** Alta.
- **Fuente.** Gap detectado en el rediseño (era ausente en v1) — crítico para POS real.
- **Procesos relevantes asociados.** P-09 (Realizar corte de caja).

#### RF-10 — Cancelar orden o reembolsar venta

**Descripción.** El sistema permite cancelar órdenes no cobradas en cualquier momento, y anular ventas ya cobradas mediante un proceso restringido al Administrador con justificación obligatoria. Una venta anulada no borra el folio (sigue siendo consecutivo), pero queda marcada y revierte el inventario consumido.

- **Actores.** Operador (cancelar orden), Administrador (anular venta).
- **Entradas.** Identificador de orden o venta, motivo de cancelación/anulación.
- **Proceso.**
  1. **Caso A — Cancelar orden (pre-cobro).** El operador cancela una orden en estado Borrador, EnviadaACocina, EnPreparación, Lista o Entregada. El sistema pide motivo y marca la orden como **Cancelada**. Si la orden ya estaba en cocina, notifica para detener la preparación.
  2. **Caso B — Anular venta (post-cobro).** Requiere autenticación adicional del Administrador. El admin justifica el motivo. El sistema marca la venta como **Anulada** conservando el folio, revierte el inventario consumido, registra un movimiento de bitácora visible en reportes.
- **Salidas.** Orden/Venta marcada como cancelada/anulada; inventario revertido (caso B); registro en bitácora.
- **Prioridad.** Media.
- **Fuente.** R-N3 (venta inmutable salvo anulación con justificación).
- **Procesos relevantes asociados.** Transversal a P-03, P-05, P-06.

#### RF-11 — Configurar y vender combos

**Descripción.** El Administrador define combos como agrupaciones de productos a un precio especial (típicamente menor que la suma de los individuales). Para efectos de catálogo, un combo se trata como una unidad vendible; para efectos de inventario, descuenta los insumos de cada producto que lo compone según las recetas.

- **Actores.** Administrador (configuración), Operador (uso en orden).
- **Entradas.** Nombre del combo, descripción, productos que lo componen, precio especial, categoría asignada, disponibilidad.
- **Proceso.**
  1. El administrador crea un combo seleccionando productos existentes del catálogo y fijando un precio único.
  2. El sistema valida que los productos componentes existan y estén activos.
  3. Al venderse un combo (a través de RF-03), el sistema lo trata como una línea de orden con su precio combo.
  4. Al cobrarse (RF-05), el inventario descuenta los insumos de **cada producto componente** según sus recetas.
- **Salidas.** Combo disponible en menú, ventas con desglose de combos en reportes (RF-08).
- **Prioridad.** Media.
- **Fuente.** Negocio.
- **Procesos relevantes asociados.** Extensión de P-02 (Gestionar producto del menú).

#### RF-12 — Mantener bitácora de auditoría

**Descripción.** El sistema registra automáticamente toda operación sensible: inicio/cierre de sesión, alta/modificación/eliminación de productos y combos, cambios de precio, configuración de campañas de descuento, apertura/cierre de turno, ventas, anulaciones, cancelaciones, entradas de inventario y modificaciones a datos del establecimiento. La bitácora es consultable por el Administrador con filtros por fecha, usuario, tipo de acción y entidad.

- **Actores.** Sistema (escritura automática), Administrador (consulta).
- **Entradas.** Cualquier acción sensible disparada por usuarios.
- **Proceso.**
  1. Cada acción sensible escribe un registro con: timestamp, usuario, rol declarado, tipo de acción, entidad afectada, identificador de la entidad, datos relevantes (valor anterior/nuevo si aplica), IP/dispositivo origen.
  2. La bitácora es de solo lectura para los usuarios (solo el sistema escribe).
  3. El administrador la consulta con filtros y paginación.
- **Salidas.** Registros en BD; vista de consulta para administrador.
- **Prioridad.** Media.
- **Fuente.** Compliance interno + gap del v1.
- **Procesos relevantes asociados.** Transversal a todos los procesos.

#### RF-13 — Gestionar y aplicar descuentos por campaña

**Descripción.** El Administrador configura campañas de descuento que el Operador aplica en el momento del cobro. Una campaña define tipo (porcentaje o monto fijo), alcance (toda la venta, una categoría, o un producto específico), vigencia (rango de fechas, días de la semana, horarios opcionales) y estado (activa/pausada/expirada). En el cobro, el sistema lista solo las campañas vigentes y aplicables.

- **Actores.** Administrador (configura), Operador (aplica), Sistema (filtra y calcula).
- **Entradas.** Para configurar: nombre, tipo (% o $), valor, alcance, vigencia, estado. Para aplicar: selección del operador entre campañas vigentes al cobrar.
- **Proceso.**
  1. **Configuración.** El administrador crea, edita, pausa o expira campañas. El sistema valida que no haya valores absurdos (porcentaje > 100%, monto fijo > total típico, etc.).
  2. **Aplicación en cobro.** Cuando el operador procesa un cobro (RF-05), el sistema consulta las campañas con estado activo cuya vigencia incluya el momento actual y cuyo alcance aplique a la venta.
  3. El operador selecciona **una** campaña (o ninguna).
  4. El sistema calcula el monto descontado según tipo y alcance, lo aplica al subtotal y registra el descuento.
  5. La aplicación queda en bitácora (RF-12).
- **Salidas.** Campaña aplicada, monto descontado registrado, ticket reflejando el descuento (RF-06), reportes agregando descuentos (RF-08).
- **Prioridad.** Media.
- **Fuente.** Negocio (días de oferta y promociones recurrentes).
- **Procesos relevantes asociados.** Integrado en P-06 (Procesar cobro).

#### RF-14 — Configurar datos del establecimiento

**Descripción.** El Administrador captura y mantiene actualizados los datos identitarios del negocio que se imprimen en cada ticket digital: nombre comercial, dirección, teléfono de contacto, RFC opcional, leyenda fiscal opcional. Estos datos no son operativos pero son necesarios para que los tickets tengan identidad profesional.

- **Actores.** Administrador.
- **Entradas.** Nombre comercial, dirección, teléfono, RFC opcional, leyenda al pie del ticket opcional.
- **Proceso.**
  1. El administrador accede al panel de configuración del establecimiento.
  2. Edita los datos y guarda.
  3. El sistema valida formatos básicos (RFC válido si se captura).
  4. Los datos actualizados aparecen en los tickets generados a partir de ese momento; los tickets anteriores conservan los datos que tenían (regla de **precio histórico** extendida a datos del establecimiento).
- **Salidas.** Datos del establecimiento actualizados, visibles en próximos tickets.
- **Prioridad.** Media.
- **Fuente.** Negocio (identidad del ticket).
- **Procesos relevantes asociados.** Insumo de P-07 (Generar y emitir ticket digital).

---

### 3.1.1 Resumen de prioridades

| Prioridad | Requisitos |
|---|---|
| **Alta** | RF-01, RF-02, RF-03, RF-04, RF-05, RF-06, RF-07, RF-09 |
| **Media** | RF-08, RF-10, RF-11, RF-12, RF-13, RF-14 |
| **Baja** | (sin requisitos baja en v1) |

Los RF de prioridad **Alta** conforman el núcleo operativo mínimo viable: sin alguno de ellos el negocio no puede operar el día a día. Los RF **Media** son indispensables para que el sistema sea completo y auditable, pero pueden implementarse en paralelo o posteriormente sin bloquear la operación inicial.

---

### 3.2 Requisitos no funcionales

Cada requisito no funcional sigue la plantilla **Descripción + Métrica/criterio de aceptación + Verificación**. La métrica es lo que permite decidir si el requisito está cumplido sin ambigüedad subjetiva.

#### RNF-01 — Facilidad de uso

**Descripción.** La interfaz debe ser comprensible para personal sin experiencia técnica previa. Dado que los Operadores rotan de rol diariamente, todos deben poder usar cualquier pantalla operativa sin necesidad de capacitación específica al rol.

**Métrica.** Un usuario nuevo (sin haber visto el sistema antes) logra completar una venta de prueba (crear orden → enviar a cocina → marcar lista → cobrar → emitir ticket) en menos de **10 minutos** tras una inducción básica de 5 minutos.

**Verificación.** Prueba de usabilidad con al menos tres usuarios no familiarizados, cronometrando el tiempo hasta completar la tarea y registrando los puntos donde se trabaron.

#### RNF-02 — Rapidez de operación

**Descripción.** Las acciones más frecuentes del operador deben ser más rápidas que el proceso manual con post-its que reemplazan. No deben requerir navegación profunda.

**Métricas.**
- Las acciones operativas comunes (agregar producto a orden, enviar a cocina, abrir cobro, ver cocina, marcar orden lista) se completan en **3 clics o menos** desde la pantalla de inicio del rol correspondiente.
- La latencia de respuesta de cualquier página en la red local es **menor a 500 ms** desde que se da clic hasta que se renderiza la siguiente vista.

**Verificación.** Auditoría manual contando clics en flujos típicos; medición de tiempo con las herramientas de desarrollador del navegador (pestaña Network) sobre un servidor de prueba con datos representativos.

#### RNF-03 — Accesibilidad multi-dispositivo

**Descripción.** El sistema debe ser utilizable desde cualquier dispositivo común dentro del establecimiento, sin instalar software adicional al navegador.

**Métricas.**
- Funciona correctamente en las últimas dos versiones de **Chrome, Edge y Firefox**.
- La interfaz es **responsiva** y usable en pantallas de PC (≥1024 px), tablet (≥768 px) y smartphone (≥360 px).
- No requiere ningún plugin, extensión o instalación adicional al navegador.

**Verificación.** Pruebas manuales en dispositivos reales (al menos una PC, una tablet y un smartphone) más validación con las herramientas de modo responsivo del navegador.

#### RNF-04 — Disponibilidad durante horario operativo

**Descripción.** El sistema debe estar disponible durante todo el horario de operación del establecimiento. Una caída en horario de servicio interrumpe directamente el ingreso del negocio.

**Métrica.** Disponibilidad efectiva **≥ 99 %** durante el horario operativo declarado del establecimiento. Esto admite hasta ~10 minutos de indisponibilidad por jornada de 16 horas (margen para reinicio puntual o intervención mínima).

**Verificación.** Monitoreo del servicio (health check del contenedor de aplicación + check de conexión a BD) durante una semana de operación; registro y análisis de incidentes.

#### RNF-05 — Persistencia y resistencia a fallos

**Descripción.** Los datos del negocio (ventas, inventario, configuración, bitácora) no deben perderse ante un cierre inesperado del sistema (corte eléctrico, kill del proceso, reinicio forzado).

**Métricas.**
- Toda transacción de venta confirmada se persiste de manera **atómica** en PostgreSQL antes de devolver respuesta al usuario.
- Tras una simulación de corte abrupto del contenedor (`docker kill`) mientras el sistema atiende tráfico, al reiniciar **cero ventas confirmadas se pierden** y ninguna venta queda en estado inconsistente.

**Verificación.** Caso de prueba ejecutado en Entrega 4: bajo carga simulada, ejecutar `docker kill burgerpos-app`, reiniciar, verificar integridad de la BD comparando contra el conteo de ventas confirmadas previas al corte.

#### RNF-06 — Portabilidad del despliegue

**Descripción.** El sistema debe poder ejecutarse en cualquier servidor Linux moderno con Docker instalado, sin depender de configuraciones específicas del entorno. Esto habilita los tres escenarios de despliegue exigidos por la Entrega 5 y permite cambiar de hospedaje (local → cloud, o entre proveedores) sin rediseño.

**Métricas.**
- El despliegue completo se hace con un único comando: `docker compose up -d` sobre el archivo `docker-compose.yml` provisto.
- El sistema arranca correctamente en una distribución Linux limpia recién instalada (Ubuntu 22.04 LTS o equivalente) con solo Docker como dependencia.
- La provisión automatizada con Vagrant + Puppet (Fase 3 de la Entrega 5) produce un servidor funcional sin intervención manual posterior.

**Verificación.** Las tres fases de la Entrega 5 sirven como evidencia: cada una despliega el mismo paquete de contenedores en un entorno Linux diferente, demostrando portabilidad.

---

### 3.3 Requisitos de interfaz

#### 3.3.1 Interfaces de usuario

BurgerPOS presenta una **aplicación web responsiva** servida por ASP.NET Core Razor Pages. La interfaz se organiza por **rol funcional** (no por rol declarado del operador, ya que recordamos que el rol no restringe acceso), con un menú lateral o superior que da entrada a las pantallas principales.

**Pantallas principales del sistema:**

| Pantalla | Quién la usa | Función |
|---|---|---|
| Login | Todos | Autenticación + declaración de rol del día (operadores) |
| Inicio del Operador | Operador | Atajos a Toma de orden, Cocina, Vista de meseros y Cobro |
| Inicio del Administrador | Administrador | Atajos a Menú, Inventario, Reportes, Bitácora, Configuración |
| Toma de orden | Operador | Crear orden, agregar productos, modificadores y notas, enviar a cocina |
| Cocina | Operador | Cola FIFO de órdenes pendientes; marcar lista |
| Vista de meseros | Operador | Listado de órdenes listas para entregar |
| Cobro | Operador | Procesar pago, aplicar descuento, generar ticket |
| Ticket digital (visor) | Operador, Administrador | Vista previa del ticket emitido; opción de exportar |
| Turno de caja | Operador, Administrador | Abrir turno, registrar retiros, arquear y cerrar |
| Inventario (operativo) | Operador, Administrador | Registrar entrada, reportar merma, consultar saldos |
| Inventario (catálogo) | Administrador | CRUD de insumos, ajuste libre, configurar alertas |
| Menú | Administrador | CRUD de productos, combos, modificadores, categorías y recetas |
| Campañas de descuento | Administrador | CRUD de campañas; pausar / expirar |
| Reportes | Administrador | Filtros por periodo y consulta de indicadores |
| Bitácora | Administrador | Consulta del registro de auditoría con filtros |
| Configuración del establecimiento | Administrador | Datos del negocio para el ticket |

**Principios de diseño visual:**

- **Tipografía legible** a distancia operativa (mínimo 16 px en cuerpos, 20 px en datos críticos como totales y números de mesa).
- **Código de color para estados**: gris para borrador, azul para en proceso, ámbar para alertas y nivel bajo, verde para listo/aprobado, rojo para errores y cancelaciones.
- **Acciones primarias** siempre visibles sin scroll; las secundarias en menús contextuales.
- **Confirmación obligatoria** para acciones destructivas (cancelar orden, anular venta, eliminar insumo).
- **Idioma español (México)** en toda la interfaz; moneda MXN con dos decimales; fechas en formato `dd/mm/yyyy hh:mm`; zona horaria `America/Mexico_City`.

#### 3.3.2 Interfaces de software

**Cliente — navegador web:**
- Chrome, Edge o Firefox en sus dos versiones más recientes al momento del despliegue.
- HTML5, CSS3, JavaScript ES2022. Sin Flash, sin Silverlight, sin Java applets.
- Cookies habilitadas para la sesión.

**Servidor — pila de ejecución:**
- **Sistema operativo**: distribución Linux moderna con Docker. Referencia: Ubuntu Server 22.04 LTS o equivalente (Debian 12, Rocky Linux 9).
- **Runtime de contenedores**: Docker 24.0 o superior.
- **Orquestación local**: Docker Compose v2.20 o superior.
- **Aplicación**: ASP.NET Core 10 con Razor Pages, autenticación vía ASP.NET Core Identity, ORM Entity Framework Core 10.
- **Base de datos**: PostgreSQL 16 (mínimo PostgreSQL 14). Persistencia en volumen Docker dedicado.
- **Comunicación interna**: la aplicación y la BD se comunican por la red interna de Docker Compose; los puertos publicados al host son únicamente el de la app (80 / 443).

**Protocolos:**
- HTTP/HTTPS para comunicación cliente-servidor.
- Conexión PostgreSQL nativa (puerto 5432) entre contenedores en red privada.

#### 3.3.3 Interfaces de hardware

BurgerPOS no requiere hardware especializado más allá del estándar de oficina:

- **Servidor**: cualquier máquina (física o virtual) capaz de correr Linux + Docker. Mínimo recomendado: 2 vCPU, 4 GB RAM, 20 GB de almacenamiento.
- **Dispositivos cliente**: PC, tablet o smartphone con navegador y conexión a la red del servidor.
- **Red**: WiFi y/o Ethernet del establecimiento conectando todos los dispositivos al servidor (ver §2.1).

No se requiere impresora térmica, lector de código de barras, cajón monedero electrónico ni báscula. Tampoco se integra con la terminal Mercado Pago (ver §2.4 y §2.5 S-3).

---

### 3.4 Reglas de negocio

Las reglas de negocio son restricciones derivadas del dominio del negocio y la regulación aplicable. A diferencia de los RF (qué hace el sistema) y los RNF (cómo lo hace), las RN son **invariantes que deben cumplirse siempre**, independientemente del flujo.

#### RN-01 — Tasa de IVA del 16 %

Toda venta calcula el IVA aplicando la tasa **16 %** (Impuesto al Valor Agregado vigente en México). La tasa está configurada como parámetro global del sistema y modificable solo por Administrador, con cambio registrado en bitácora. Las ventas pasadas conservan la tasa vigente al momento de su emisión (regla aplicable a histórico, ver RN-04).

#### RN-02 — Folios consecutivos sin saltos

Cada venta cobrada recibe un **folio único, incremental y sin saltos** por establecimiento. La generación del folio es transaccional con el registro de la venta para garantizar que no haya dos ventas con el mismo folio ni huecos en la secuencia. Una venta anulada (RF-10) **conserva su folio**; el folio no se reutiliza.

#### RN-03 — Inmutabilidad de la venta cobrada

Una vez cobrada, una venta **no puede modificarse**. La única operación posible es su **anulación** (RF-10), que requiere autenticación del Administrador y motivo obligatorio. La anulación no elimina la venta del sistema, solo la marca como anulada conservando su folio y revertiendo el inventario consumido.

#### RN-04 — Precios históricos en las ventas

Una venta cobrada conserva los **precios** de los productos y modificadores **vigentes al momento del cobro**. Si el Administrador edita un precio en el catálogo después, las ventas pasadas siguen reportando el precio que efectivamente cobraron. Esto aplica a precios base, modificadores y precios de combo.

#### RN-05 — Datos históricos del establecimiento en tickets

Los tickets emitidos por una venta conservan los **datos del establecimiento** (nombre comercial, dirección, RFC, leyenda) que estaban vigentes **al momento de la emisión** del ticket. Si el Administrador cambia esos datos después, los tickets previos no se modifican.

#### RN-06 — Orden de cálculo del cobro

El total a pagar se calcula en este orden estricto, para que el cálculo sea reproducible y verificable:

1. **Subtotal** = suma de (precio base + modificadores) × cantidad por cada línea de orden.
2. **Descuento aplicado** = monto resultante de la campaña seleccionada en RF-13 (si la hay).
3. **Base gravable** = Subtotal − Descuento.
4. **IVA** = Base gravable × 0.16.
5. **Propina** = monto capturado por el operador (opcional).
6. **Total a pagar** = Base gravable + IVA + Propina.

La **comisión bancaria** (en pagos con tarjeta) se registra aparte para el reporte financiero y no se cobra al cliente.

#### RN-07 — Una sola campaña de descuento por venta

En el cobro solo puede aplicarse **una** campaña de descuento por venta. Si el negocio requiere combinar promociones (ej.: "Martes 2×1 + 10 % adicional"), debe configurarse como una **campaña única que ya incluya ambos efectos**. Esta regla simplifica el cálculo, la auditoría y los reportes.

#### RN-08 — Combo desglosa insumos por componente

Al vender un combo (RF-11), el sistema descuenta los insumos según la **receta de cada producto componente**, no según una receta agregada del combo. Esto garantiza que los reportes de consumo y los saldos de inventario reflejen correctamente lo que efectivamente se preparó, aunque el cliente lo viera como un solo precio.

#### RN-09 — Modificadores afectan precio; notas libres no

Los **modificadores** (extra queso, doble carne, sin tocino) tienen un delta de precio definido en el catálogo y participan en el cálculo del subtotal (RN-06). Las **notas libres** ("sin cebolla", "término medio") son texto para la cocina y **no afectan el precio**. Esta distinción debe ser visible para el operador al momento de agregar a la orden.

#### RN-10 — Receta histórica en consumos de inventario

Si el Administrador edita la receta de un producto en RF-02, las ventas pasadas conservan el consumo registrado según la receta **vigente al momento de cada venta**. Los movimientos de inventario en bitácora son inmutables por venta.

---

## 4. Historias de usuario

Esta sección complementa los requisitos funcionales con una vista **centrada en el usuario y su motivación**, usando la plantilla ágil:

> Como **\<rol\>**, quiero **\<acción\>**, para **\<beneficio\>**.

Las historias agrupan las funcionalidades por el rol que las demanda y aterrizan el *por qué* del requisito, no solo el *qué*. Cada historia referencia el RF principal del que se desprende.

### 4.1 Historias del Administrador

| ID | Historia | RF |
|---|---|---|
| HU-01 | Como **Administrador**, quiero **dar de alta a mis trabajadores con su propia cuenta** para que el sistema registre con precisión quién hizo cada operación. | RF-01 |
| HU-02 | Como **Administrador**, quiero **gestionar el catálogo de productos, combos, modificadores y recetas** para mantener el menú actualizado y vinculado al inventario. | RF-02, RF-11 |
| HU-03 | Como **Administrador**, quiero **dar de alta mis insumos y definir sus niveles de alerta** para que el sistema controle el stock de manera automatizada. | RF-07 |
| HU-04 | Como **Administrador**, quiero **ajustar libremente el saldo de un insumo tras un conteo físico** para que el sistema refleje la realidad cuando hay diferencias. | RF-07 |
| HU-05 | Como **Administrador**, quiero **configurar campañas de descuento con vigencia específica** para promover ofertas en días u horarios determinados sin abrir la puerta a descuentos manuales discrecionales. | RF-13 |
| HU-06 | Como **Administrador**, quiero **capturar y mantener los datos identitarios de mi negocio** para que los tickets digitales tengan presentación profesional. | RF-14 |
| HU-07 | Como **Administrador**, quiero **consultar reportes de ventas por periodo** con desglose de propinas, IVA, comisiones y descuentos para tomar decisiones del negocio. | RF-08 |
| HU-08 | Como **Administrador**, quiero **consultar la bitácora de auditoría con filtros** para identificar irregularidades o reconstruir lo que pasó en una jornada. | RF-12 |
| HU-09 | Como **Administrador**, quiero **anular una venta ya cobrada con justificación obligatoria** para corregir errores serios sin perder trazabilidad. | RF-10 |

### 4.2 Historias del Operador (todos los roles)

| ID | Historia | RF |
|---|---|---|
| HU-10 | Como **Operador**, quiero **iniciar sesión con mi cuenta y declarar el rol del día** para que el sistema asocie mis operaciones a mi persona y a mi función del turno. | RF-01 |
| HU-11 | Como **Operador**, quiero **registrar la entrada de mercancía cuando llega el proveedor** para que el inventario se mantenga correcto sin esperar al administrador. | RF-07 |
| HU-12 | Como **Operador**, quiero **reportar una merma con motivo cuando algo se quemó, cayó o caducó** para que el sistema refleje la pérdida con trazabilidad. | RF-07 |
| HU-13 | Como **Operador**, quiero **ver alertas de insumos bajos** para avisar al administrador o gestionar el inventario antes de que se agote algo crítico. | RF-07 |

### 4.3 Historias del Mesero

| ID | Historia | RF |
|---|---|---|
| HU-14 | Como **Mesero**, quiero **tomar órdenes desde una tablet** para no usar libreta y reducir errores de transcripción hacia cocina. | RF-03 |
| HU-15 | Como **Mesero**, quiero **agregar modificadores con precio y notas libres al producto** para reflejar exactamente lo que pidió el cliente. | RF-03 |
| HU-16 | Como **Mesero**, quiero **editar la orden antes de enviarla a cocina** para corregir o ajustar lo que el cliente cambie de opinión. | RF-03 |
| HU-17 | Como **Mesero**, quiero **ver claramente cuáles de mis órdenes están listas para entregar** para no perder tiempo caminando a cocina a preguntar. | RF-04 |

### 4.4 Historias del Cocinero

| ID | Historia | RF |
|---|---|---|
| HU-18 | Como **Cocinero**, quiero **ver las órdenes pendientes en orden de llegada** para preparar con justicia y sin saltarme ninguna. | RF-04 |
| HU-19 | Como **Cocinero**, quiero **ver claramente los modificadores y notas por línea** para preparar correctamente lo que pidió cada cliente. | RF-04 |
| HU-20 | Como **Cocinero**, quiero **marcar una orden como lista** para que el mesero se entere y pueda entregarla pronto. | RF-04 |

### 4.5 Historias del Encargado de cobro

| ID | Historia | RF |
|---|---|---|
| HU-21 | Como **Encargado de cobro**, quiero **abrir mi turno declarando el fondo de caja inicial** para que el arqueo del cierre sea verificable. | RF-09 |
| HU-22 | Como **Encargado de cobro**, quiero **aplicar una campaña de descuento vigente en el momento del cobro** para honrar las ofertas que el cliente ve anunciadas. | RF-13 |
| HU-23 | Como **Encargado de cobro**, quiero **capturar método de pago, propina opcional y monto recibido** para que el cálculo del cambio sea automático y la venta quede completa. | RF-05 |
| HU-24 | Como **Encargado de cobro**, quiero **previsualizar el ticket digital antes de finalizar** para verificar que todo esté correcto. | RF-06 |
| HU-25 | Como **Encargado de cobro**, quiero **realizar el corte de caja al cerrar mi turno** para conciliar el efectivo físico con el esperado por el sistema. | RF-09 |
| HU-26 | Como **Encargado de cobro**, quiero **cancelar una orden no cobrada si el cliente desiste** para liberar la mesa sin generar una venta espuria. | RF-10 |

---

## 5. Matriz de trazabilidad

La matriz de trazabilidad es el **hilo conductor entre las cinco entregas** del curso. Demuestra que cada requisito funcional se rastrea hasta:

- **Sus casos de uso** (que se detallarán en el documento `02-CasosDeUso.md` de esta misma entrega).
- **Sus procesos modelados** (entrega 2: BPMN2, secuencia y comunicación; entrega 3: actividad y estado).
- **Sus casos de prueba** (entrega 4: documento + código xUnit).
- **Sus reglas de negocio y RNF** aplicables.

Las columnas **CU-XX** y **TC-XX** quedan como referencias que se llenarán a medida que los documentos correspondientes se redacten. Cada placeholder ya tiene su número reservado para mantener la correspondencia.

### 5.1 Matriz RF → todo lo demás

| RF | Nombre corto | Procesos | RN aplicables | RNF aplicables | CU planeados | TC planeados |
|---|---|---|---|---|---|---|
| RF-01 | Autenticar y declarar rol | P-01 | — | RNF-01, RNF-04 | CU-01 | TC-01 |
| RF-02 | Gestionar menú | P-02 | RN-04, RN-09, RN-10 | RNF-01 | CU-02, CU-03 | TC-02, TC-03 |
| RF-03 | Tomar orden | P-03, P-04 | RN-09 | RNF-02 | CU-04 | TC-04 |
| RF-04 | Despachar (cocina + meseros) | P-05 | — | RNF-02, RNF-04 | CU-05, CU-06 | TC-05 |
| RF-05 | Procesar cobro | P-06 | RN-01, RN-02, RN-03, RN-04, RN-06, RN-07 | RNF-02, RNF-05 | CU-07 | TC-06, TC-07 |
| RF-06 | Emitir ticket digital | P-07 | RN-02, RN-04, RN-05 | RNF-03 | CU-08 | TC-08 |
| RF-07 | Gestionar inventario | P-08 | RN-08, RN-10 | RNF-05 | CU-09, CU-10, CU-11, CU-12 | TC-09, TC-10, TC-11 |
| RF-08 | Reportes de ventas | P-10 | RN-04 | — | CU-13 | TC-12 |
| RF-09 | Abrir/cerrar turno | P-09 | — | RNF-05 | CU-14, CU-15 | TC-13 |
| RF-10 | Cancelar/anular | Transversal a P-03, P-06 | RN-02, RN-03 | RNF-05 | CU-16, CU-17 | TC-14 |
| RF-11 | Configurar combos | extiende P-02 | RN-08 | — | CU-18 | TC-15 |
| RF-12 | Bitácora | Transversal | — | RNF-05 | CU-19 | TC-16 |
| RF-13 | Descuentos por campaña | integrado a P-06 | RN-06, RN-07 | — | CU-20, CU-21 | TC-17 |
| RF-14 | Datos del establecimiento | insumo de P-07 | RN-05 | — | CU-22 | TC-18 |

### 5.2 Matriz inversa: Proceso → RF y entregas

Esta vista invierte la trazabilidad para ayudar al modelado de las **Entregas 2 y 3**: por cada proceso relevante, se identifica qué RF cubre y, por tanto, qué entrada de información necesita para sus diagramas.

| Proceso | Nombre | RF principal | RF colaterales | Objeto(s) de estado afectado(s) |
|---|---|---|---|---|
| P-01 | Autenticar usuario | RF-01 | — | SesiónDeUsuario |
| P-02 | Gestionar producto del menú | RF-02 | RF-11 | Producto |
| P-03 | Tomar orden | RF-03 | — | Orden, LíneaDeOrden, Mesa |
| P-04 | Enviar orden a cocina | RF-03 | RF-04 | Orden |
| P-05 | Preparar y marcar orden lista | RF-04 | — | Orden, LíneaDeOrden |
| P-06 | Procesar cobro | RF-05 | RF-10, RF-13 | Orden, Pago |
| P-07 | Generar y emitir ticket digital | RF-06 | RF-14 | Ticket |
| P-08 | Registrar entrada de inventario | RF-07 | — | InsumoInventario, EntradaInventario |
| P-09 | Realizar corte de caja | RF-09 | — | TurnoDeCaja |
| P-10 | Generar reporte de ventas | RF-08 | — | — (consulta de lectura) |

### 5.3 Cobertura cruzada de Reglas de Negocio

Esta tabla muestra **por cada regla de negocio** los RF que deben validarla en su implementación y en sus pruebas.

| RN | Regla | RF que la deben implementar | RF que la deben verificar |
|---|---|---|---|
| RN-01 | IVA 16 % | RF-05 | RF-05, RF-06, RF-08 |
| RN-02 | Folios consecutivos | RF-05 | RF-05, RF-06, RF-10 |
| RN-03 | Venta inmutable | RF-05 | RF-10 |
| RN-04 | Precios históricos | RF-02 | RF-05, RF-06, RF-08 |
| RN-05 | Datos del establecimiento históricos | RF-14 | RF-06 |
| RN-06 | Orden de cálculo del cobro | RF-05 | RF-05, RF-13 |
| RN-07 | Una campaña de descuento por venta | RF-13 | RF-05, RF-13 |
| RN-08 | Combo desglosa insumos por componente | RF-11 | RF-07 |
| RN-09 | Modificadores afectan precio; notas no | RF-02 | RF-03, RF-05 |
| RN-10 | Receta histórica | RF-02 | RF-07 |

### 5.4 Cobertura de RNF por RF crítico

Esta vista cierra el círculo: cada RNF debe ser comprobable contra al menos un RF que lo someta a prueba.

| RNF | RF que la prueban |
|---|---|
| RNF-01 Facilidad de uso | RF-01, RF-02, RF-03 |
| RNF-02 Rapidez | RF-03, RF-04, RF-05 |
| RNF-03 Accesibilidad | RF-06 (visualización de ticket en cualquier dispositivo) |
| RNF-04 Disponibilidad | RF-01 (capacidad de iniciar sesión), RF-04 |
| RNF-05 Persistencia | RF-05, RF-07, RF-09, RF-10, RF-12 |
| RNF-06 Portabilidad | (verificado por Entrega 5 completa) |

---

## 6. Glosario extendido

Esta sección consolida y expande el glosario presentado en §1.3, incluyendo todos los términos que aparecen a lo largo del documento. Ordenado alfabéticamente.

| Término | Definición |
|---|---|
| **Administrador** | Cuenta de usuario única asociada al dueño del establecimiento. No rota de rol. Tiene acceso al catálogo, configuración, reportes consolidados, ajustes libres de inventario y bitácora. Ver §2.3. |
| **Ajuste de saldo** | Modificación libre del saldo de un insumo realizada exclusivamente por el Administrador para reflejar el resultado de un conteo físico o corregir errores acumulados. Requiere motivo obligatorio y queda en bitácora. Ver RF-07 paso 4. |
| **Anulación de venta** | Operación restringida al Administrador, con motivo obligatorio, que marca una venta cobrada como inválida sin eliminarla del sistema. Conserva el folio y revierte el inventario consumido. Ver RF-10 y RN-03. |
| **Apertura de caja** | Acto de declarar el fondo inicial de efectivo al iniciar un turno de caja. Solo puede haber un turno abierto a la vez. Ver RF-09. |
| **Arqueo** | Acto de contar el efectivo físico al cerrar un turno de caja y compararlo con el efectivo esperado por el sistema. La diferencia se calcula y queda en bitácora. Ver RF-09. |
| **ASP.NET Core Identity** | Componente de ASP.NET Core que provee autenticación, gestión de usuarios y contraseñas. Es el módulo donde se implementa RF-01. |
| **Base gravable** | Subtotal de la venta menos descuento aplicado. Es el monto sobre el que se calcula el IVA. Ver RN-06. |
| **Bitácora de auditoría** | Registro inmutable y consultable solo por Administrador, donde se escriben automáticamente todas las operaciones sensibles del sistema con autor, momento y datos relevantes. Ver RF-12. |
| **BPMN2** | Business Process Model and Notation, versión 2. Estándar OMG para diagramar procesos de negocio. Notación principal exigida por la Entrega 2. |
| **Campaña de descuento** | Promoción pre-configurada por el Administrador que reduce el subtotal al cobrar. Se define con tipo (porcentaje o monto fijo), alcance, vigencia y estado. Ver RF-13. |
| **Cancelación de orden** | Marca una orden no cobrada como inválida. Puede ejecutarla cualquier Operador con motivo obligatorio. Distinto de anulación de venta. Ver RF-10. |
| **Caso de uso (CU)** | Descripción de un escenario de interacción entre uno o más actores y el sistema. Se documentan en `02-CasosDeUso.md`. |
| **Cierre de turno** | Acto formal de cerrar un turno de caja tras realizar el arqueo. El turno cerrado queda en historial y no puede modificarse. Ver RF-09. |
| **Cocinero** | Rol operativo que un Operador declara para el turno. No restringe acceso a pantallas, pero queda registrado para reportes. Ver §2.3. |
| **Combo** | Producto que agrupa varios productos individuales a un precio especial. Al venderse, descuenta los insumos de cada producto componente según sus recetas. Ver RF-11 y RN-08. |
| **Comisión bancaria** | Porcentaje que el banco descuenta al establecimiento por cobros con tarjeta. Se registra para reportes financieros y no se cobra al cliente. Ver RF-05 y RN-06. |
| **Conteo físico** | Procedimiento operativo (manual, fuera del sistema) donde se cuentan las existencias reales de un insumo. El resultado se ingresa al sistema mediante un Ajuste de saldo. Ver RF-07. |
| **Corte de caja** | Sinónimo de cierre de turno con arqueo. Proceso de conciliación entre el efectivo físico contado y el efectivo esperado. Ver RF-09. |
| **CRUD** | Create, Read, Update, Delete. Las cuatro operaciones básicas sobre una entidad persistente. |
| **Datos del establecimiento** | Información identitaria del negocio (nombre comercial, dirección, teléfono, RFC opcional) que se incluye en cada ticket digital. Configurable por el Administrador. Ver RF-14 y RN-05. |
| **Descuento aplicado** | Instancia concreta de una campaña de descuento ejecutada sobre una venta específica, con el monto deducido registrado en bitácora. Ver RF-13. |
| **Diagrama de actividad** | Diagrama UML que modela el flujo de control de un proceso paso a paso. Exigido por la Entrega 3. |
| **Diagrama de comunicación** | Diagrama UML que muestra las interacciones entre objetos enfocándose en sus relaciones estructurales. Exigido por la Entrega 2. |
| **Diagrama de secuencia** | Diagrama UML que muestra las interacciones entre objetos ordenadas en el tiempo. Exigido por la Entrega 2. |
| **Diagrama de cambio de estado** | Diagrama UML (máquina de estados) que modela los estados posibles de un objeto y las transiciones entre ellos. Exigido por la Entrega 3. |
| **Docker** | Plataforma de contenerización usada para empaquetar BurgerPOS y sus dependencias. Versión mínima 24.0. |
| **Docker Compose** | Herramienta de orquestación de múltiples contenedores Docker. Versión mínima v2.20. Eje de la Fase 2 de la Entrega 5. |
| **Encargado de cobro** | Rol operativo declarado por un Operador para el turno. Responsable del cobro, apertura y cierre de caja. Ver §2.3. |
| **Entity Framework Core** | ORM oficial de .NET usado para la capa de persistencia hacia PostgreSQL. Versión 10. |
| **Entrada de inventario** | Registro que aumenta el saldo de un insumo, típicamente al recibir mercancía del proveedor. La capturan Operador o Administrador. Ver RF-07 paso 2. |
| **ERS** | Especificación de Requisitos de Software. Este documento, basado en IEEE 830. |
| **FIFO** | First In, First Out — política de despacho de cocina por orden de llegada de las órdenes. |
| **Folio** | Número único, consecutivo y sin saltos asignado a cada venta cobrada por establecimiento. Ver RN-02. |
| **Fondo de caja inicial** | Monto en efectivo declarado por el Operador al abrir un turno de caja. Sirve como referencia para el arqueo. Ver RF-09. |
| **IEEE 830** | Estándar IEEE 830-1998 para la redacción de Especificaciones de Requisitos de Software. Estructura este documento. |
| **Insumo** | Materia prima cuya existencia disminuye al vender productos (pan, carne, queso, refrescos, etc.). Tiene unidad de medida, saldo y nivel de alerta. Ver RF-07. |
| **IVA** | Impuesto al Valor Agregado en México (16 %). Se aplica sobre la base gravable. Ver RN-01 y RN-06. |
| **LAN** | Local Area Network, red local. La red WiFi/Ethernet del establecimiento que conecta dispositivos al servidor BurgerPOS. Ver §2.1. |
| **Línea de orden** | Cada producto seleccionado dentro de una orden, con su cantidad, modificadores y notas. Ver RF-03. |
| **Merma** | Disminución del saldo de un insumo por causas operativas (quemado, caído, derramado, caducado). La reporta Operador o Administrador con motivo obligatorio elegido de una lista predefinida. Ver RF-07 paso 3. |
| **Mesa** | Identificador físico al que se asocia una orden cuando la modalidad de servicio es "Mesa". Tiene su propia máquina de estados (Libre, Ocupada, ConCuentaAbierta, PorLimpiar). |
| **Mesero** | Rol operativo declarado por un Operador para el turno. Responsable de tomar órdenes y entregar pedidos listos. Ver §2.3. |
| **Modalidad de servicio** | Tipo de orden: Mesa, Mostrador (comer aquí sin mesa asignada) o Para llevar. Ver RF-03. |
| **Modificador** | Opción configurable sobre un producto que altera su precio: aditivo (extra queso +$15, doble carne +$30) o sustractivo (sin tocino −$5). Distinto de una nota libre. Ver RF-02 y RN-09. |
| **MXN** | Peso mexicano. Moneda única del sistema. Mostrado con dos decimales. |
| **Nivel de alerta** | Saldo mínimo configurado por insumo por debajo del cual el sistema despliega una alerta visual. Configurable por Administrador. Ver RF-07 paso 6. |
| **Nota libre** | Texto opcional adjunto a una línea de orden con indicaciones para cocina, sin impacto en el precio (ej. "sin cebolla", "término medio"). Ver RN-09. |
| **Operador** | Cuenta de usuario asociada a uno de los trabajadores del establecimiento. Declara su rol operativo al iniciar sesión. Ver §2.3. |
| **Orden** | Solicitud de uno o más productos para una mesa, para llevar o para mostrador, antes de ser cobrada. Tiene su propia máquina de estados. Ver RF-03. |
| **Pago** | Entidad que representa la transacción monetaria de una venta. Tiene método (Efectivo o Tarjeta), monto, propina y campaña aplicada si la hay. |
| **PostgreSQL** | Sistema de gestión de base de datos relacional usado por BurgerPOS. Versión mínima 14, recomendada 16. |
| **POS** | Point of Sale, punto de venta. |
| **Precio histórico** | Precio de un producto vigente en el momento en que se cobró una venta. Las ventas pasadas se reportan con el precio histórico aunque el catálogo se actualice después. Ver RN-04. |
| **Producto** | Ítem vendible del menú (hamburguesa, refresco, etc.) con su propio precio base, modificadores opcionales, receta y categoría. Ver RF-02. |
| **Propina** | Monto opcional adicional al total de la venta, declarado por el cliente y capturado por el Operador. No se grava con IVA en este sistema. Ver RF-05. |
| **Puppet** | Herramienta de gestión de configuración usada en la Fase 3 de la Entrega 5 para automatizar la instalación de Docker y dependencias sobre la VM creada por Vagrant. |
| **Razor Pages** | Framework de ASP.NET Core para construir aplicaciones web centradas en páginas. Es el patrón obligatorio del proyecto. |
| **Receta** | Lista de insumos y cantidades consumidas por unidad de un producto vendido. Definida en RF-02, usada para descuento automático de inventario en RF-07. Ver RN-10. |
| **Reembolso** | Devolución del monto cobrado tras la anulación de una venta. Operación administrativa exclusiva del Administrador. Ver RF-10. |
| **Reporte de ventas** | Documento generado bajo demanda por el Administrador que agrega ventas de un periodo con indicadores financieros. Ver RF-08. |
| **RFC** | Registro Federal de Contribuyentes. Identificador fiscal opcional del establecimiento, incluido en tickets si está capturado. Ver RF-14. |
| **Rol operativo** | Función que un Operador declara al iniciar sesión (Cocinero, Mesero, Encargado de cobro). No restringe acceso pero queda registrado. Ver §2.3. |
| **Sesión de usuario** | Periodo entre el inicio de sesión (RF-01) y el cierre. Tiene su propia máquina de estados. |
| **Subtotal** | Suma de (precio base + modificadores) × cantidad por cada línea de orden, antes de descuento e IVA. Ver RN-06. |
| **TDD** | Test-Driven Development. Práctica no obligada en este proyecto, pero alineada con la Entrega 4. |
| **Ticket digital** | Comprobante de venta generado por el sistema, visualizable en pantalla y exportable como archivo (PDF/HTML). BurgerPOS no realiza impresión física. Ver RF-06. |
| **Trazabilidad** | Capacidad de rastrear un requisito desde su declaración hasta su implementación, prueba y verificación. Materializada en la matriz de §5. |
| **Turno de caja** | Periodo entre la apertura de caja (declaración del fondo inicial) y el cierre (arqueo). Tiene su propia máquina de estados. Ver RF-09. |
| **UML** | Unified Modeling Language. Notación para diagramas de clases, secuencia, comunicación, actividad y estado. |
| **Vagrant** | Herramienta para crear y gestionar máquinas virtuales locales reproduciblemente. Eje de la Fase 3 de la Entrega 5. |
| **Venta** | Orden ya cobrada con su ticket digital emitido y registrada con folio consecutivo. Es inmutable. Ver RF-05 y RN-03. |
| **xUnit** | Framework de pruebas unitarias para .NET. Tecnología obligatoria de la Entrega 4. |

---

> **Fin del documento.** La ERS continúa siendo viva: cualquier cambio futuro debe reflejarse en el historial de revisiones de la cabecera, y propagarse a la matriz de trazabilidad (§5) para no perder coherencia con las entregas posteriores.

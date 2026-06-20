# Casos de Uso — BurgerPOS

**Versión**: 1.0
**Fecha**: 2026-06-20
**Documento complementario a**: `01-ERS.md` v2.6
**Diagrama PlantUML**: `diagramas/casos-de-uso.puml`

---

## 1. Introducción

Este documento detalla los **22 casos de uso** del sistema BurgerPOS, que materializan los 14 requisitos funcionales declarados en la ERS. Cada caso de uso describe un escenario concreto de interacción entre uno o más actores y el sistema, con su flujo principal y sus flujos alternos.

Los casos de uso están directamente trazados desde la **matriz de §5 de la ERS** y son el insumo principal de las **Entregas 2 y 3** (modelado de procesos y comportamiento).

## 2. Actores

| Actor | Descripción |
|---|---|
| **Administrador** | Dueño del establecimiento. Cuenta única, no rota. Acceso a catálogo, configuración, ajustes libres de inventario, reportes consolidados y bitácora. |
| **Operador** | Trabajador del establecimiento (5 personas). Declara un rol operativo (Cocinero, Mesero, Encargado de cobro) al iniciar sesión, sin que esto restrinja qué pantallas puede usar. |
| **Sistema** | Procesos automáticos disparados por eventos: descuento de inventario al cobrar, alertas de nivel bajo, generación de folios, registro en bitácora. |

## 3. Plantilla utilizada

Cada caso de uso sigue esta estructura:

- **ID** + **Nombre**
- **Actores** (principales y secundarios)
- **Precondiciones** — qué debe ser cierto para que el caso de uso pueda ejecutarse
- **Postcondiciones (éxito)** — qué queda cambiado en el sistema si todo sale bien
- **Flujo principal** — pasos numerados del escenario típico
- **Flujos alternos** — desviaciones razonables (entradas inválidas, decisiones distintas)
- **RF / RN / RNF relacionados** — trazabilidad con la ERS

## 4. Diagrama de Casos de Uso

El diagrama completo se encuentra en `diagramas/casos-de-uso.puml` (renderizable con PlantUML). Resumen del agrupamiento por contexto de dominio. **Los mismos siete paquetes se usan en el diagrama de clases**, lo que permite trazar directamente cada CU con sus entidades del modelo:

- **Identidad**: CU-01
- **Catálogo**: CU-02, CU-03, CU-18, CU-20, CU-22
- **Órdenes**: CU-04, CU-05, CU-06, CU-16
- **Cobro**: CU-07, CU-08, CU-17, CU-21
- **Turno**: CU-14, CU-15
- **Inventario**: CU-09, CU-10, CU-11, CU-12
- **Administración**: CU-13, CU-19

---

## 5. Casos de uso detallados

### CU-01 — Autenticar usuario y declarar rol del día

- **Actores**: Operador, Administrador, Sistema.
- **Precondiciones**: el usuario tiene cuenta activa y conoce sus credenciales.
- **Postcondiciones**: sesión activa creada; rol operativo registrado para el turno (solo Operadores); login asentado en bitácora.
- **Flujo principal**:
  1. El usuario abre la URL de BurgerPOS.
  2. Captura usuario y contraseña.
  3. El sistema valida las credenciales contra la BD.
  4. Si es Operador, el sistema solicita el rol operativo del día (Cocinero, Mesero o Encargado de cobro).
  5. El usuario selecciona el rol.
  6. El sistema crea la sesión, registra el evento en bitácora y redirige al panel correspondiente.
- **Flujos alternos**:
  - 3a. Credenciales inválidas → mensaje de error sin revelar si falló usuario o password; tras 5 intentos consecutivos, bloqueo temporal de 5 minutos.
  - 4a. Si es Administrador, omite el paso de rol y va directo al panel admin.
- **RF**: RF-01. **RNF**: RNF-01, RNF-04.

---

### CU-02 — Gestionar producto del menú

- **Actores**: Administrador.
- **Precondiciones**: sesión activa de Administrador; existen categorías al menos.
- **Postcondiciones**: producto creado/editado/desactivado; cambio en bitácora; visible en pantallas operativas.
- **Flujo principal**:
  1. El admin entra al panel "Menú" → "Productos".
  2. Selecciona "Nuevo producto" o un producto existente.
  3. Captura nombre, descripción, precio base, categoría, disponibilidad.
  4. Agrega modificadores opcionales (nombre + delta de precio).
  5. Invoca CU-03 para definir la receta del producto.
  6. Guarda. El sistema valida, persiste y registra en bitácora.
- **Flujos alternos**:
  - 3a. Precio negativo o nombre vacío → error de validación, no se guarda.
  - 6a. El admin marca el producto como "no disponible" sin eliminarlo (cuando se agota un insumo en el día).
- **RF**: RF-02. **RN**: RN-04, RN-09, RN-10.

---

### CU-03 — Definir receta del producto

- **Actores**: Administrador.
- **Precondiciones**: producto en edición (CU-02); existen insumos en el catálogo (CU-09).
- **Postcondiciones**: receta del producto registrada (lista de insumos con cantidad por unidad vendida).
- **Flujo principal**:
  1. Dentro del editor de producto, el admin entra a la pestaña "Receta".
  2. Selecciona un insumo del catálogo.
  3. Captura cantidad consumida por unidad vendida.
  4. Repite los pasos 2-3 por cada insumo necesario.
  5. Guarda. El sistema persiste la receta y la asocia al producto.
- **Flujos alternos**:
  - 2a. Si no existe el insumo necesario, el admin abandona el flujo, ejecuta CU-09 y regresa.
  - 3a. Cantidad ≤ 0 → error de validación.
- **RF**: RF-02. **RN**: RN-10.

---

### CU-04 — Tomar orden

- **Actores**: Operador (típicamente Mesero o Encargado de cobro), Sistema.
- **Precondiciones**: sesión activa; existen productos disponibles en el menú.
- **Postcondiciones**: orden en estado `EnviadaACocina`; visible en pantalla de cocina.
- **Flujo principal**:
  1. El operador entra a "Tomar orden".
  2. Selecciona modalidad: Mesa, Mostrador o Para llevar. Si es Mesa, captura el número.
  3. Selecciona productos del menú; para cada uno, define cantidad, modificadores y nota libre opcional.
  4. Revisa el resumen y el subtotal preliminar.
  5. Presiona "Enviar a cocina".
  6. El sistema valida productos disponibles, marca la orden como `EnviadaACocina` y la encola en FIFO.
- **Flujos alternos**:
  - 3a. El operador modifica/elimina líneas antes de enviar (la orden sigue siendo borrador).
  - 5a. Algún producto fue marcado "no disponible" mientras se tomaba la orden → el sistema avisa, el operador lo retira de la orden.
- **RF**: RF-03. **RN**: RN-09. **RNF**: RNF-02.

---

### CU-05 — Atender órdenes en cocina (FIFO)

- **Actores**: Operador (típicamente Cocinero), Sistema.
- **Precondiciones**: existe al menos una orden en estado `EnviadaACocina`.
- **Postcondiciones**: orden marcada como `Lista`; aparece en CU-06 para el mesero.
- **Flujo principal**:
  1. El cocinero entra a la pantalla "Cocina".
  2. El sistema muestra las órdenes ordenadas por hora de envío (FIFO), con número, modalidad/mesa, líneas con cantidad, modificadores y notas, y hora.
  3. El cocinero prepara la siguiente en cola.
  4. Cuando termina, presiona "Marcar lista" en esa orden.
  5. El sistema cambia el estado a `Lista`, la quita de la vista activa y la publica en la vista de meseros (CU-06).
- **Flujos alternos**:
  - 3a. Una orden requiere un insumo agotado → el cocinero invoca CU-11 (reportar merma o no disponibilidad) y notifica al mesero.
- **RF**: RF-04. **RNF**: RNF-02.

---

### CU-06 — Ver órdenes listas para entregar

- **Actores**: Operador (típicamente Mesero), Sistema.
- **Precondiciones**: existe al menos una orden en estado `Lista`.
- **Postcondiciones**: orden marcada como `Entregada`.
- **Flujo principal**:
  1. El mesero entra a la pantalla "Órdenes listas".
  2. El sistema muestra las órdenes en estado `Lista` con número, mesa/modalidad y resumen.
  3. El mesero entrega físicamente la orden al cliente.
  4. Marca la orden como "Entregada" en la pantalla.
  5. El sistema cambia el estado a `Entregada` y la mueve al historial activo de cuentas por cobrar.
- **Flujos alternos**:
  - 4a. El cliente devuelve un producto antes de aceptarlo → el mesero invoca CU-16 (cancelar orden) o ajusta líneas según política del negocio.
- **RF**: RF-04. **RNF**: RNF-02.

---

### CU-07 — Procesar cobro

- **Actores**: Operador (típicamente Encargado de cobro), Sistema.
- **Precondiciones**: existe una orden en estado `Entregada` o `Lista` (mostrador); hay un turno de caja abierto (CU-14).
- **Postcondiciones**: venta registrada con folio consecutivo; inventario descontado; ticket digital disponible; orden marcada como `Cobrada`.
- **Flujo principal**:
  1. El operador busca o selecciona la orden a cobrar.
  2. El sistema muestra subtotal, líneas y modificadores aplicados.
  3. (Opcional) El operador invoca CU-21 para aplicar una campaña de descuento vigente.
  4. El operador captura propina opcional.
  5. El operador selecciona método de pago: Efectivo o Tarjeta.
     - **Efectivo**: captura monto recibido; el sistema calcula el cambio.
     - **Tarjeta**: el sistema registra que fue tarjeta y la comisión bancaria configurada.
  6. El sistema calcula según RN-06: subtotal − descuento → base gravable → +IVA → +propina → total.
  7. El operador confirma el cobro.
  8. El sistema asigna folio consecutivo único, registra la venta, descuenta inventario por receta (CU-10 automático), genera el ticket digital y abre CU-08.
- **Flujos alternos**:
  - 5a. Efectivo insuficiente → error, el operador pide diferencia.
  - 7a. El operador cancela antes de confirmar → la orden vuelve a su estado anterior, no se genera venta.
- **RF**: RF-05. **RN**: RN-01, RN-02, RN-03, RN-04, RN-06, RN-07. **RNF**: RNF-02, RNF-05.

---

### CU-08 — Visualizar y exportar ticket digital

- **Actores**: Operador, Administrador, Sistema.
- **Precondiciones**: venta registrada.
- **Postcondiciones**: ticket mostrado en pantalla; opcionalmente archivo descargado.
- **Flujo principal**:
  1. El sistema toma los datos del establecimiento configurados (CU-22) y los datos completos de la venta.
  2. Genera el ticket con: cabecera del establecimiento, fecha y hora, folio, modalidad y mesa, líneas con precios históricos (RN-04), modificadores y deltas, subtotal, descuento (si lo hubo, con nombre de campaña), IVA, propina, total, método de pago, operador.
  3. Lo presenta en pantalla con opción "Exportar como PDF" o "Exportar como HTML".
  4. (Opcional) El usuario descarga el archivo.
- **Flujos alternos**:
  - 4a. El usuario solo cierra la vista sin exportar (el ticket queda accesible desde el historial).
- **RF**: RF-06, RF-14. **RN**: RN-02, RN-04, RN-05. **RNF**: RNF-03.

---

### CU-09 — Dar de alta insumo

- **Actores**: Administrador.
- **Precondiciones**: sesión activa de Administrador.
- **Postcondiciones**: insumo creado en el catálogo, listo para ser referenciado en recetas (CU-03) y movimientos (CU-10, CU-11).
- **Flujo principal**:
  1. El admin entra a "Inventario" → "Catálogo de insumos".
  2. Selecciona "Nuevo insumo".
  3. Captura nombre único, unidad de medida (piezas, kg, litros, gramos), existencia inicial, nivel de alerta.
  4. Guarda. El sistema valida y persiste; queda en bitácora.
- **Flujos alternos**:
  - 3a. Nombre duplicado → error de validación.
  - 3b. Valores negativos en existencia inicial o nivel de alerta → error.
- **RF**: RF-07.

---

### CU-10 — Registrar entrada de inventario

- **Actores**: Operador, Administrador, Sistema.
- **Precondiciones**: existe el insumo en el catálogo (CU-09).
- **Postcondiciones**: saldo del insumo incrementado; movimiento registrado en bitácora.
- **Flujo principal**:
  1. El usuario entra a "Inventario" → "Registrar entrada".
  2. Selecciona el insumo.
  3. Captura cantidad recibida; opcionalmente proveedor y costo.
  4. Confirma.
  5. El sistema incrementa el saldo, registra la entrada con autor, fecha, proveedor y costo en bitácora.
- **Flujos alternos**:
  - 3a. Cantidad ≤ 0 → error.
  - 5a. **Caso automático** (Sistema): al confirmar una venta (CU-07), el sistema descuenta cada insumo según las recetas (este flujo es la contraparte automática de la entrada manual).
- **RF**: RF-07.

---

### CU-11 — Reportar merma

- **Actores**: Operador, Administrador.
- **Precondiciones**: existe el insumo en el catálogo.
- **Postcondiciones**: saldo del insumo disminuido; merma registrada con motivo en bitácora.
- **Flujo principal**:
  1. El usuario entra a "Inventario" → "Reportar merma".
  2. Selecciona el insumo y la cantidad mermada.
  3. Elige motivo de la lista predefinida: "Se quemó o se dañó al preparar", "Se cayó o se derramó", "Caducidad o fecha vencida", "Otro".
  4. Si selecciona "Otro", captura texto libre obligatorio describiendo el caso.
  5. Confirma.
  6. El sistema disminuye el saldo, registra la merma con autor, fecha, motivo y cantidad en bitácora.
- **Flujos alternos**:
  - 2a. Cantidad mayor que saldo disponible → error (el saldo no puede quedar negativo por merma; si pasó, requiere ajuste libre vía CU-12).
  - 4a. Si elige "Otro" sin texto libre → error de validación.
- **RF**: RF-07.

---

### CU-12 — Ajustar saldo de inventario

- **Actores**: Administrador.
- **Precondiciones**: sesión activa de Administrador; existe el insumo.
- **Postcondiciones**: saldo del insumo fijado a la cantidad capturada; ajuste registrado con motivo obligatorio en bitácora.
- **Flujo principal**:
  1. El admin entra a "Inventario" → "Ajuste de saldo".
  2. Selecciona el insumo.
  3. Captura la cantidad correcta (resultado del conteo físico) y un motivo obligatorio.
  4. El sistema calcula la diferencia (anterior − nueva) y muestra una confirmación con el cambio.
  5. El admin confirma.
  6. El sistema fija el saldo, registra el ajuste con cantidad anterior, nueva, diferencia, motivo, responsable y fecha en bitácora.
- **Flujos alternos**:
  - 3a. Motivo vacío → error.
  - 5a. El admin cancela tras revisar la confirmación → no se aplica.
- **RF**: RF-07. **RN**: ninguna específica adicional; queda en bitácora visible siempre.

---

### CU-13 — Consultar reportes de ventas

- **Actores**: Administrador.
- **Precondiciones**: sesión activa de Administrador.
- **Postcondiciones**: reporte mostrado en pantalla.
- **Flujo principal**:
  1. El admin entra a "Reportes".
  2. Selecciona rango (diario, semanal, mensual o personalizado).
  3. Opcionalmente aplica filtros (modalidad, operador, categoría).
  4. El sistema agrega las ventas y calcula indicadores: total bruto, # transacciones, productos top, ingreso promedio, propinas, descuentos aplicados, IVA recaudado, comisiones bancarias, ganancia bruta y neta.
  5. Muestra el reporte en pantalla.
- **Flujos alternos**:
  - 4a. Sin ventas en el periodo → mensaje "sin datos" sin error.
- **RF**: RF-08. **RN**: RN-04.

---

### CU-14 — Abrir turno de caja

- **Actores**: Operador (Encargado de cobro), Administrador.
- **Precondiciones**: no hay otro turno abierto.
- **Postcondiciones**: turno abierto y listo para registrar ventas en efectivo.
- **Flujo principal**:
  1. El usuario entra a "Turno de caja" → "Abrir turno".
  2. Captura el fondo inicial de efectivo.
  3. Confirma.
  4. El sistema crea el turno con estado `Abierto`, lo asocia al operador y registra el evento en bitácora.
- **Flujos alternos**:
  - 1a. Ya existe un turno abierto → el sistema redirige al turno existente sin crear uno nuevo.
- **RF**: RF-09. **RNF**: RNF-05.

---

### CU-15 — Cerrar turno y arquear

- **Actores**: Operador (Encargado de cobro), Administrador.
- **Precondiciones**: existe un turno abierto.
- **Postcondiciones**: turno cerrado con su reporte de arqueo en historial.
- **Flujo principal**:
  1. El usuario entra a "Turno de caja" → "Cerrar turno".
  2. El sistema calcula el efectivo esperado: fondo inicial + ventas en efectivo del turno − retiros registrados.
  3. El usuario cuenta físicamente el efectivo en caja y captura el monto contado y observaciones opcionales.
  4. El sistema calcula la diferencia (contado − esperado) y muestra el reporte de arqueo.
  5. El usuario confirma el cierre.
  6. El sistema mueve el turno a estado `Cerrado` y lo deja accesible en historial; queda en bitácora.
- **Flujos alternos**:
  - 4a. La diferencia es significativa (configurable, ej. > $50) → el sistema solicita motivo obligatorio.
- **RF**: RF-09. **RNF**: RNF-05.

---

### CU-16 — Cancelar orden

- **Actores**: Operador (puede ser cualquier rol).
- **Precondiciones**: existe una orden en estado distinto a `Cobrada` o `Cancelada`.
- **Postcondiciones**: orden marcada como `Cancelada`; si estaba en cocina, se notifica para detener preparación.
- **Flujo principal**:
  1. El operador localiza la orden a cancelar.
  2. Presiona "Cancelar orden".
  3. Captura motivo obligatorio.
  4. Confirma.
  5. El sistema cambia el estado a `Cancelada` y, si estaba en cocina, notifica a la pantalla de cocina para detener preparación. Queda en bitácora.
- **Flujos alternos**:
  - 1a. Si la orden ya fue cobrada, el operador es redirigido a CU-17 (anular venta) y se le informa que requiere admin.
- **RF**: RF-10. **RN**: RN-03 (no afecta porque no es post-cobro).

---

### CU-17 — Anular venta

- **Actores**: Administrador, Sistema.
- **Precondiciones**: existe la venta cobrada; sesión activa de Administrador (puede requerir re-autenticación explícita).
- **Postcondiciones**: venta marcada como anulada con folio conservado; inventario revertido; movimiento en bitácora.
- **Flujo principal**:
  1. El admin localiza la venta en el historial.
  2. Presiona "Anular venta".
  3. El sistema solicita re-autenticación con la contraseña del admin.
  4. El admin captura motivo obligatorio.
  5. Confirma.
  6. El sistema marca la venta como `Anulada` (conservando folio por RN-02), revierte el descuento de inventario y registra el evento en bitácora.
- **Flujos alternos**:
  - 3a. Contraseña incorrecta → error y aborto.
  - 4a. Motivo vacío → error de validación.
- **RF**: RF-10. **RN**: RN-02, RN-03.

---

### CU-18 — Configurar combo

- **Actores**: Administrador.
- **Precondiciones**: existen los productos componentes en el catálogo.
- **Postcondiciones**: combo creado/editado/desactivado y disponible para venta.
- **Flujo principal**:
  1. El admin entra a "Menú" → "Combos".
  2. Selecciona "Nuevo combo" o uno existente.
  3. Captura nombre, descripción, categoría y precio especial.
  4. Selecciona los productos componentes con la cantidad de cada uno.
  5. Guarda. El sistema valida que los componentes estén activos y persiste.
- **Flujos alternos**:
  - 4a. Un componente está desactivado → error, el admin lo reemplaza o lo reactiva.
  - 3a. Precio especial mayor o igual a la suma de los componentes → el sistema advierte (no impide, pero registra una alerta para revisión).
- **RF**: RF-11. **RN**: RN-08.

---

### CU-19 — Consultar bitácora de auditoría

- **Actores**: Administrador.
- **Precondiciones**: sesión activa de Administrador.
- **Postcondiciones**: registros mostrados según filtros aplicados.
- **Flujo principal**:
  1. El admin entra a "Bitácora".
  2. Aplica filtros opcionales (rango de fechas, usuario, tipo de evento, entidad).
  3. El sistema lista los registros con paginación: timestamp, usuario, rol declarado, tipo, entidad, identificador, valor anterior/nuevo si aplica.
  4. El admin puede ver el detalle completo de cualquier registro.
- **Flujos alternos**:
  - 3a. Sin coincidencias → mensaje "sin resultados".
- **RF**: RF-12. **RNF**: RNF-05.

---

### CU-20 — Configurar campaña de descuento

- **Actores**: Administrador.
- **Precondiciones**: sesión activa de Administrador.
- **Postcondiciones**: campaña creada/editada/pausada/expirada; visible a operadores solo si cumple vigencia.
- **Flujo principal**:
  1. El admin entra a "Descuentos" → "Campañas".
  2. Selecciona "Nueva campaña" o una existente.
  3. Captura nombre, tipo (porcentaje o monto fijo), valor, alcance (toda la venta, categoría, producto), vigencia (rango de fechas, días de la semana, horarios opcionales) y estado inicial.
  4. El sistema valida (ej. porcentaje ≤ 100%, fechas coherentes).
  5. Guarda. Queda en bitácora.
- **Flujos alternos**:
  - 3a. Valor inválido (negativo, > 100% si porcentaje) → error.
  - 5a. El admin pausa o expira una campaña existente directamente desde la lista, sin entrar al detalle.
- **RF**: RF-13.

---

### CU-21 — Aplicar descuento en cobro

- **Actores**: Operador (Encargado de cobro), Sistema.
- **Precondiciones**: hay al menos una campaña vigente cuyo alcance aplica a la venta en curso.
- **Postcondiciones**: descuento aplicado al subtotal; monto descontado registrado en la venta y en bitácora.
- **Flujo principal**:
  1. Dentro de CU-07, el operador presiona "Aplicar descuento".
  2. El sistema lista las campañas vigentes y aplicables.
  3. El operador selecciona **una** campaña.
  4. El sistema calcula el monto a descontar según tipo y alcance.
  5. Muestra el subtotal con descuento.
  6. El operador continúa con CU-07 desde el paso 4 (propina) o cancela el descuento para elegir otro.
- **Flujos alternos**:
  - 2a. No hay campañas vigentes → mensaje informativo, el operador no aplica nada.
  - 6a. El operador cambia o quita la campaña antes de confirmar el cobro.
- **RF**: RF-13. **RN**: RN-06, RN-07.

---

### CU-22 — Configurar datos del establecimiento

- **Actores**: Administrador.
- **Precondiciones**: sesión activa de Administrador.
- **Postcondiciones**: datos del establecimiento actualizados; visibles en próximos tickets emitidos (RN-05).
- **Flujo principal**:
  1. El admin entra a "Configuración" → "Establecimiento".
  2. Edita nombre comercial, dirección, teléfono, RFC opcional, leyenda al pie del ticket opcional.
  3. Guarda.
  4. El sistema valida formatos básicos (ej. RFC mexicano si se captura) y persiste. Queda en bitácora.
- **Flujos alternos**:
  - 4a. RFC con formato inválido → error de validación.
- **RF**: RF-14. **RN**: RN-05.

---

## 6. Cómo generar el diagrama

El diagrama de Casos de Uso está en `diagramas/casos-de-uso.puml`. Para renderizarlo:

**Opción 1 — En línea (más rápido para revisar):**
1. Abrir [https://www.plantuml.com/plantuml](https://www.plantuml.com/plantuml).
2. Pegar el contenido del archivo `.puml`.
3. Descargar la imagen en PNG o SVG.

**Opción 2 — VS Code:**
1. Instalar la extensión "PlantUML" de jebbs.
2. Abrir el `.puml` y presionar `Alt + D` para previsualizar.
3. Exportar con `Ctrl + Shift + P` → "PlantUML: Export Current Diagram".

**Opción 3 — Línea de comandos:**
```bash
# Requiere Java y el jar de PlantUML
java -jar plantuml.jar diagramas/casos-de-uso.puml
# Genera diagramas/casos-de-uso.png
```

Los demás diagramas (`paquetes.puml`, `componentes.puml`, `clases.puml`) se renderizan exactamente igual.

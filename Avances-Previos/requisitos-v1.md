# Requisitos BurgerPOS — Versión entregada originalmente

> Este documento se conserva como referencia histórica. La versión vigente vivirá en `Entrega-1-Modelado/01-ERS.md` y se rediseña desde cero para corregir gaps detectados (corte de caja, cancelaciones, propinas, combos, auditoría).

## Contexto del negocio

- Restaurante pequeño de hamburguesas
- 5 trabajadores con roles rotativos (no fijos)
- Roles operativos: cocinero (2), mesero (2), encargado de cobro (1)
- Cualquier trabajador puede desempeñar cualquier rol en un día dado
- Proceso actual: todo manual con post-its, Excel y menú físico

---

## Requerimientos funcionales

### RF-01: Gestión de usuarios con roles rotativos
- Registrar a los 5 trabajadores
- Login propio por usuario
- Roles no fijos; cualquier trabajador puede acceder a las funciones de su rol del día
- Dos niveles: trabajador (tomar órdenes, cobrar) y administrador (menú, inventario, reportes)

### RF-02: Gestión del menú digital
- Admin: agregar, editar, eliminar productos
- Producto: nombre, descripción, precio, categoría
- Categorías: hamburguesas, hot dogs, bebidas, extras
- Marcar producto como "no disponible" sin eliminarlo
- Cambios de precio/disponibilidad reflejados inmediatamente

### RF-03: Toma de órdenes digital
- Crear nueva orden
- Asociar a mesa o identificador (llevar / comer aquí)
- Seleccionar productos y cantidades
- Notas/modificaciones por producto ("sin cebolla", "extra queso")
- Editar antes de enviar a cocina
- Al enviar, aparece en vista de cocina

### RF-04: Vista de cocina FIFO
- Órdenes en el orden recibido (FIFO)
- Cada orden muestra: #, mesa, productos con notas, hora
- Cocinero marca "lista"
- Completadas se mueven a historial

### RF-05: Cobro y cierre de venta
- Calcular subtotal, IVA, total
- Métodos: efectivo o tarjeta
- Efectivo: capturar monto recibido y calcular cambio
- Tarjeta: registrar comisión bancaria
- Al completar, venta queda registrada

### RF-06: Tickets
- Generar ticket profesional
- Incluir: nombre, fecha/hora, folio consecutivo, productos, subtotal, IVA, total, método de pago
- Imprimible (idealmente térmica)
- Vista previa en pantalla

### RF-07: Inventario básico
- Registro de insumos con existencia
- Descuento automático al vender
- Alerta de nivel bajo (configurable por admin)
- Admin registra entradas
- Granularidad: por producto/porción (no ingrediente)

### RF-08: Reportes
- Diarios, semanales, mensuales
- Total ventas, # transacciones, productos top, ingreso promedio, ganancias brutas/netas, impuestos, comisiones
- Desglose por método de pago
- Consulta en pantalla

---

## Requerimientos no funcionales

- **RNF-01** Usabilidad: interfaz intuitiva, sin capacitación extensa
- **RNF-02** Rapidez: más rápido que manual, ≤3 clics en acciones comunes
- **RNF-03** Accesibilidad: navegador web, multi-dispositivo
- **RNF-04** Disponibilidad: local, todo el horario operativo
- **RNF-05** Datos: persistencia en BD, sin pérdida ante cierre inesperado

## Restricciones

- Sin integración directa con terminal MercadoPago
- Sin facturación electrónica (SAT/CFDI)
- Sin reservaciones
- Sin app móvil para clientes
- Sin integración con plataformas de delivery

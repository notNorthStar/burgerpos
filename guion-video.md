# Guion BurgerPOS — Video de demostración

---

## SEGMENTO 1 — David (Admin + Despliegue)
**~4-5 min**

**[Pantalla: terminal, carpeta raíz del repo]**

> "BurgerPOS es un sistema POS web para hamburgueserías desarrollado en .NET 10 con arquitectura limpia y desplegado mediante Vagrant y Puppet sobre Ubuntu 24.04."

```bash
git clone -b deploy https://github.com/notNorthStar/burgerpos.git
cd burgerpos
vagrant up
```

> "Con un solo comando, Vagrant levanta la máquina virtual, Puppet instala Docker y levanta los contenedores automáticamente."

*(dejar correr el aprovisionamiento, pueden acelerar en edición)*

> "Una vez listo, accedemos en el navegador."

**[Abrir `http://localhost:8081`]**

> "Iniciamos sesión como administrador."

- Email: `admin@burgerpos.com` / Password: `Admin123!`
- *(Al ser Admin, entra directo al inicio sin elegir rol)*

**[Mostrar navbar → recorrer cada sección brevemente]**

> "El administrador tiene acceso completo al sistema."

1. **Menú → Productos**: mostrar lista, filtrar por categoría, abrir un producto y su receta
2. **Inventario → Insumos**: mostrar saldos actuales
3. **Admin → Campañas**: crear una campaña de descuento activa
4. **Admin → Configuración**: mostrar datos del establecimiento
5. **Admin → Reportes**: filtrar por fecha, mostrar totales y top productos
6. **Admin → Bitácora**: mostrar eventos registrados
7. **Admin → Empleados**: mostrar lista de empleados creados

> "El panel de administración centraliza el control total del negocio: catálogo, inventario, descuentos y auditoría."

---

## SEGMENTO 2 — Alfonso (Mesero)
**~3-4 min**

*(Alfonso corre la app local: `docker compose up -d` + `dotnet run`)*
*(Debe tener productos y mesas ya cargados en su BD)*

**[Abrir la app, pantalla de login]**

> "Como empleado con rol de mesero, inicio sesión y declaro mi rol para el turno."

- Login con su usuario
- Seleccionar **Mesero** en la pantalla de rol del día

**[Ir a Ordenes → Nueva orden]**

> "Tomamos la orden de la Mesa 3."

- Modalidad: **Mesa**, seleccionar **Mesa 3**
- Crear orden
- Agregar producto: "Hamburguesa Clásica", nota libre: "Sin cebolla"
- Agregar complementos: marcar "Extra Queso", "Extra Guacamole"
- Agregar otro producto: "Refresco"
- Clic en **Enviar a cocina**

> "La orden queda enviada a cocina. Mientras se prepara, podemos ver su estado."

**[Ir a Ordenes → ver estado "EnviadaACocina"]**

> "Cuando cocina marca la orden como lista, nos llega en la vista de meseros."

**[Ir a Meseros]**

- Mostrar la orden lista
- Clic en **Entregar al cliente**

> "La orden fue entregada. Procedemos al cobro."

**[Ir a Ordenes → abrir la orden → Cobrar]**

- Seleccionar método: **Efectivo**, monto recibido: $250
- Cobrar

> "El sistema genera el ticket automáticamente con el cambio calculado."

**[Mostrar ticket generado]**

---

## SEGMENTO 3 — Natalia (Cocinero)
**~2-3 min**

*(Natalia corre la app local con su propia instancia)*
*(Debe tener al menos una orden enviada a cocina en su BD)*

**[Login → seleccionar rol Cocinero]**

> "Desde la estación de cocina, el cocinero ve en tiempo real las órdenes pendientes."

**[Ir a Cocina]**

> "Cada tarjeta muestra el número de orden, la modalidad, la hora y los productos a preparar — incluyendo los complementos y notas especiales del cliente."

- Mostrar una orden con complementos indentados y nota libre

**[Clic en "En prep."]**

> "Marcamos la orden en preparación para indicar que ya empezamos a trabajarla."

- La tarjeta cambia a amarillo con badge "EnPreparacion"

**[Agregar un producto extra a la orden (desde Nueva orden) y reenviar]**

> "Si el cliente agrega algo extra, el mesero lo registra y reenvía a cocina. Solo aparecen los nuevos ítems, no toda la orden de nuevo."

**[Clic en "✓ Lista"]**

> "Al marcar la orden como lista, el sistema descuenta automáticamente los insumos del inventario según la receta de cada producto. La orden pasa a la vista del mesero para su entrega."

---

## Cierre (cualquiera de los tres)

> "BurgerPOS implementa un flujo completo desde la toma de orden hasta el cobro, con roles diferenciados, control de inventario automático y despliegue reproducible con Vagrant y Puppet."

---

## Notas de producción

- Cada quien graba su segmento por separado y se edita en orden
- Alfonso y Natalia no necesitan Vagrant — con `docker compose up -d` + `dotnet run` es suficiente
- Pre-cargar datos antes de grabar: categorías, productos con recetas, insumos, mesas, al menos un empleado por rol
- El `vagrant up` de David puede acelerarse 2x en edición

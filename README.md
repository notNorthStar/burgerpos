# 🍔 BurgerPOS

**A Burger Store Designed Point Of Sale.**

Proyecto final de la materia **Procesos de Desarrollo de Software (PDS)** de la Licenciatura en Ingeniería en Sistemas y Tecnologías de la Información, **Universidad Veracruzana**.

> **Estado del proyecto**: en planificación y modelado. Avanzando entrega por entrega.

---

## 📋 Resumen

| | |
|---|---|
| **Stack** | .NET 10 + ASP.NET Core Razor Pages |
| **Base de datos** | PostgreSQL |
| **Pruebas** | xUnit |
| **Despliegue** | Linux (bare-metal, Docker Compose, Vagrant + Puppet) |
| **Documentación** | Markdown en cada carpeta de entrega |
| **Estándar de requisitos** | IEEE 830-1998 |

BurgerPOS automatiza la operación diaria de una hamburguesería: autenticación del personal con roles rotativos, gestión del menú (productos, combos, modificadores con precio), toma digital de órdenes, despacho a cocina con cola FIFO, cobro con efectivo o tarjeta, aplicación de descuentos por campaña, emisión de tickets digitales, control de inventario, corte de caja, cancelaciones y reportes de ventas.

---

## 🗂️ Estructura del repositorio

```
ProyectoFinal-PDS/
├── Entrega-1-Modelado/         ERS, casos de uso, diagramas de paquetes/componentes/clases
├── Entrega-2-Procesos/         10 BPMN2 + 10 secuencia UML + 10 comunicación UML
├── Entrega-3-Comportamiento/   10 diagramas de actividad + 10 de cambio de estado
├── Entrega-4-Pruebas/          10 casos de prueba + proyecto xUnit + código funcional
├── Entrega-5-Despliegue/       Fase 1: Linux manual / Fase 2: Docker Compose / Fase 3: Vagrant + Puppet
├── Codigo/                     Solución .NET (BurgerPOS.sln)
└── Avances-Previos/            Versiones anteriores de documentos (referencia histórica)
```

Cada carpeta `Entrega-N` contendrá sus propios documentos en markdown y sus diagramas.

---

## 📦 Las 5 entregas del curso

| # | Entrega | Contenido principal | Estado |
|---|---|---|---|
| 1 | Modelado inicial | ERS (IEEE 830), Casos de Uso, Diag. de Paquetes, Componentes y Clases | 🟡 En curso (ERS §1-§2 redactadas) |
| 2 | Procesos | 10 BPMN2 detallados + 10 secuencia UML + 10 comunicación UML | ⏳ Pendiente |
| 3 | Comportamiento | 10 diagramas de actividad + 10 diagramas de cambio de estado | ⏳ Pendiente |
| 4 | Pruebas | 10 casos de prueba (≥8 con CRUD a BD) + xUnit + código funcional | ⏳ Pendiente |
| 5 | Despliegue (3 fases) | Linux bare-metal · Docker Compose · Vagrant + Puppet | ⏳ Pendiente |

---

## 🔄 Los 10 procesos relevantes

Las entregas 2 y 3 piden los mismos 10 procesos del negocio modelados en cuatro notaciones distintas. Estos son:

1. Autenticar usuario (login con rol del día)
2. Gestionar producto del menú (CRUD)
3. Tomar orden
4. Enviar orden a cocina
5. Preparar y marcar orden lista (cocina)
6. Procesar cobro (efectivo + tarjeta + propina + descuento)
7. Generar y emitir ticket digital
8. Registrar entrada de inventario
9. Realizar corte de caja (cierre de turno)
10. Generar reporte de ventas

## 🧩 Los 10 objetos con cambio de estado (entrega 3)

1. Orden
2. LíneaDeOrden
3. Producto
4. Pago
5. Ticket
6. TurnoDeCaja
7. InsumoInventario
8. SesiónDeUsuario
9. Mesa
10. EntradaInventario

---

## 🏃 Cómo correr el proyecto (próximamente)

Esta sección se completará cuando el código de la `Entrega-4-Pruebas` esté listo. La idea es que en pocos comandos cualquiera pueda levantar BurgerPOS:

```bash
# Fase 2 (Docker Compose)
cd Entrega-5-Despliegue/fase-2-docker-compose
docker compose up -d
```

---

## 📚 Referencias

- IEEE Std 830-1998 — *Recommended Practice for Software Requirements Specifications*
- [ASP.NET Core Razor Pages](https://learn.microsoft.com/aspnet/core/razor-pages/)
- OMG — *BPMN 2.0 Specification* · *UML 2.5.1 Specification*

---

## 👥 Equipo de desarrollo

- Luis David Sosa Fernández
- Natalia Guerrero Cabrera
- Alfonso Vázquez

El proyecto se desarrolla siguiendo el proceso completo de ingeniería de software: planificación, modelado de requisitos, diseño, implementación, pruebas y despliegue a producción, aplicando los conocimientos adquiridos en la materia.

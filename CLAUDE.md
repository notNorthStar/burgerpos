# BurgerPOS — Contexto para Claude Code

> Este archivo se carga automáticamente al iniciar una sesión de Claude Code en este repositorio. Resume todo lo que necesitas saber para colaborar en el proyecto.

## Qué es BurgerPOS

Sistema **POS web para hamburgueserías pequeñas**. Proyecto final de la materia **Procesos de Desarrollo de Software (PDS)** de la Licenciatura en Ingeniería en Sistemas y Tecnologías de la Información, **Universidad Veracruzana**.

Es un proyecto de equipo, en la práctica David hace la mayor parte. Para detalles del equipo y motivación, ver `README.md`.

## Estado actual del proyecto

| Entrega | Estado | Contenido |
|---|---|---|
| 1. Modelado | ✅ Completa | ERS (IEEE 830 v2.6), 22 casos de uso, 4 diagramas UML |
| 2. Procesos | ✅ Completa | 30 diagramas (BPMN + secuencia + comunicación) × 10 procesos |
| 3. Comportamiento | ✅ Completa | 20 diagramas (actividad + estado) |
| **4. Pruebas** | 🟡 **En curso** | **10 casos de prueba xUnit + código funcional .NET** |
| 5. Despliegue | ⏳ Pendiente | Linux bare-metal + Docker Compose + Vagrant/Puppet |

**Donde estamos**: arrancando el código de la Entrega 4. Aún no hay código en `Codigo/`.

## Stack obligatorio (no negociable)

- **.NET 10** SDK (ya instalado, verificar con `dotnet --version`)
- **ASP.NET Core Razor Pages** (no MVC, no Blazor)
- **Entity Framework Core 10** como ORM
- **PostgreSQL 16** como base de datos (NO SQL Server — el sistema debe ser portable a Linux/Docker para la Entrega 5)
- **ASP.NET Core Identity** para autenticación con roles
- **xUnit** + **Testcontainers** (o EF InMemory) para pruebas
- **Docker** + **Docker Compose** para despliegue (Entrega 5)

## Arquitectura

Clean Architecture con **5 proyectos** dentro del solution `Codigo/BurgerPOS.sln`:

```
BurgerPOS.sln
├── BurgerPOS.Web           ← Razor Pages, controllers, wwwroot
├── BurgerPOS.Application   ← Services, DTOs, Validators, Interfaces
├── BurgerPOS.Domain        ← Entities, ValueObjects, Enums, Rules
├── BurgerPOS.Infrastructure ← EF Core, Identity, Repositories, Migrations
└── BurgerPOS.Tests         ← xUnit
```

**Dependencias** (Clean):
- `Web → Application → Domain`
- `Infrastructure → Domain` (implementa interfaces)
- `Web → Infrastructure` (solo para composición/DI)
- `Tests → todos`

**Domain agrupado en 7 contextos** (DDD bounded contexts):
`Identidad · Catalogo · Ordenes · Cobro · Turno · Inventario · Administracion`

Los mismos 7 nombres se usan en el diagrama de paquetes, clases y casos de uso para trazabilidad directa.

## Documentos fuente (LEER antes de implementar cualquier RF)

| Documento | Para qué |
|---|---|
| `Entrega-1-Modelado/01-ERS.md` | **Fuente única de RF, RNF, reglas de negocio, glosario, historias de usuario, matriz de trazabilidad** |
| `Entrega-1-Modelado/02-CasosDeUso.md` | Los 22 casos de uso con sus flujos paso a paso |
| `Entrega-1-Modelado/diagramas/clases.puml` | **Modelo de dominio canónico**: clases, atributos, enums, relaciones, métodos del dominio |
| `Entrega-1-Modelado/diagramas/paquetes.puml` | Estructura de los 5 proyectos .NET |
| `Entrega-1-Modelado/diagramas/componentes.puml` | Vista runtime Docker Compose |
| `Entrega-2-Procesos/secuencia/P-XX-*.puml` | **Para implementar un proceso**: ver el secuencia UML correspondiente, muestra qué services/repos invocar y en qué orden |
| `Entrega-3-Comportamiento/estados/<Objeto>.puml` | **Para implementar transiciones de estado** del objeto |

Antes de codear un caso de uso, **lee el `02-CasosDeUso.md` CU-XX correspondiente y el `Entrega-2-Procesos/secuencia/P-XX.puml`** — ahí están los nombres exactos de servicios, repositorios y métodos que ya documentamos.

## Reglas de negocio críticas (cumplir SIEMPRE)

| ID | Regla | Donde aplica |
|---|---|---|
| RN-01 | IVA 16% (México) | Cálculo del total en cobro |
| RN-02 | Folio consecutivo sin saltos | Transaccional en `INSERT Venta` |
| RN-03 | Venta cobrada es inmutable | Solo anulación con motivo de admin |
| RN-04 | Precios históricos | Ventas pasadas conservan precio del momento del cobro |
| RN-05 | Datos del establecimiento históricos | Tickets viejos conservan datos vigentes al emitirlos |
| RN-06 | Orden de cálculo: `subtotal − descuento → base gravable → +IVA → +propina = total` | Toda la lógica de cobro |
| RN-07 | Una sola campaña de descuento por venta | UI del cobro |
| RN-08 | Combo descuenta insumos **por componente** | InventarioService.DescontarPorVenta |
| RN-09 | Modificadores afectan precio; notas libres no | LineaOrden |
| RN-10 | Receta histórica en consumos | Movimientos de inventario son inmutables |

## Convenciones de código

- C# 12, `nullable` habilitado, treat warnings as errors en producción
- **Naming**:
  - `PascalCase` para clases, métodos públicos, propiedades
  - `camelCase` para parámetros y campos privados (`_camelCase` con underscore para fields)
  - Sufijo `Async` en métodos async
  - Sufijo `Repository` / `Service` / `DTO` por tipo
- **Estructura de archivo**: una clase por archivo, namespace = ruta del proyecto
- **DTOs** viven en `Application/DTOs`, **entidades** en `Domain/<Contexto>/Entities`
- **Validaciones**: `FluentValidation` para DTOs en `Application/Validators`
- **Migraciones EF Core**: en `Infrastructure/Persistence/Migrations`, generadas con `dotnet ef migrations add NombreCamelCase --project BurgerPOS.Infrastructure --startup-project BurgerPOS.Web`
- **UI**: español (México), `MXN` con 2 decimales, formato `dd/MM/yyyy HH:mm`, zona horaria `America/Mexico_City`
- **Comunicación con el usuario (David)**: en español

## Prioridades para la Entrega 4

**NO implementar los 14 RF**. El profe valora **arquitectura + pruebas > completitud**. Solo implementar lo necesario para que las **10 pruebas xUnit** tengan sustancia real, con ≥8 ejerciendo CRUD a BD.

Orden sugerido (validar antes de implementar):

1. **Scaffolding**: solution + 5 proyectos + referencias entre proyectos
2. **Infraestructura base**: EF Core + PostgreSQL via Docker para desarrollo, ASP.NET Identity
3. **Migración inicial**: tablas para Identidad, Catalogo (Producto/Categoria/Receta), Inventario (Insumo/Entrada), Ordenes (Orden/LineaOrden), Cobro (Venta/Pago/Ticket), Turno (TurnoCaja), Bitácora
4. **RF prioritarios** (mínimos para tener material de pruebas):
   - **RF-01 Autenticar** (login con Identity + declaración de rol)
   - **RF-02 Gestionar menú** (CRUD productos + recetas)
   - **RF-07 Inventario** (CRUD insumos + entrada + descuento auto)
   - **RF-05 Cobro** (orden básica + cobro con cálculo RN-06 + folio consecutivo)
5. **10 pruebas xUnit**: distribuidas entre los RF anteriores
6. **Razor Pages mínimas** para esos 4 RF (no estilizar, solo funcional)

Lo que SÍ se puede omitir en código (aunque esté en la ERS): reportes complejos, descuentos por campaña, bitácora completa, vista de cocina SignalR, exportar PDF. Se mencionan en la documentación pero no son necesarios para las pruebas.

## Casos de prueba xUnit (requisito del profe)

- **10 pruebas mínimo**, **≥8 con CRUD a BD real** (no mocks)
- Recomendado: **Testcontainers.PostgreSQL** para BD desechable por test class
- Alternativa más simple: EF InMemory (riesgo: no detecta problemas de migración real)
- Documento de casos de prueba en `Entrega-4-Pruebas/casos/casos-de-prueba.md`
- Código de pruebas en `Entrega-4-Pruebas/xunit/BurgerPOS.Tests.csproj`
- También copiar/referenciar las pruebas dentro de `Codigo/BurgerPOS.Tests/` (parte del solution)

## Workflow con David

Preferencias confirmadas a lo largo del proyecto:

- **Avanzar por partes con outline antes** de implementar piezas grandes; esperar OK
- **Salidas en markdown** cuando sea documentación (Obsidian-friendly)
- **Mensajes claros y concisos**, sin sobreexplicar lo obvio
- **Confirmar decisiones de arquitectura/diseño** antes de codear (especialmente las que afectan múltiples archivos)
- Cuando hay ambigüedad o tradeoffs, **proponer 2-3 opciones con recomendación** en lugar de elegir silenciosamente

## Repositorio

- GitHub: https://github.com/notNorthStar/burgerpos
- Branch: `main`
- Commits previos: 5 (ERS + Entregas 1, 2, 3)
- Git user ya configurado globalmente como `notNorthStar` / `dyzzyatacker117@gmail.com`
- **Hacer commit solo cuando David lo pida explícitamente.** Sí preparar mensajes propuestos.

## Equipo de desarrollo

- Luis David Sosa Fernández ← "David", el que escribe
- Natalia Guerrero Cabrera
- Alfonso Vázquez

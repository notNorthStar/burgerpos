using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BurgerPOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "folio_orden_seq");

            migrationBuilder.CreateSequence<int>(
                name: "folio_venta_seq");

            migrationBuilder.CreateTable(
                name: "bitacora_eventos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rol_operativo_declarado = table.Column<int>(type: "integer", nullable: true),
                    tipo = table.Column<int>(type: "integer", nullable: false),
                    entidad_afectada = table.Column<string>(type: "text", nullable: false),
                    entidad_id = table.Column<Guid>(type: "uuid", nullable: true),
                    valor_anterior = table.Column<string>(type: "text", nullable: true),
                    valor_nuevo = table.Column<string>(type: "text", nullable: true),
                    ip_origen = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bitacora_eventos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "campanias_descuento",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    tipo = table.Column<int>(type: "integer", nullable: false),
                    valor = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    alcance = table.Column<int>(type: "integer", nullable: false),
                    fecha_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dias_semana = table.Column<string>(type: "text", nullable: false),
                    hora_inicio = table.Column<TimeSpan>(type: "interval", nullable: true),
                    hora_fin = table.Column<TimeSpan>(type: "interval", nullable: true),
                    estado = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_campanias_descuento", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    orden_visual = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_categorias", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "combos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    precio_especial = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_combos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "datos_establecimiento",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre_comercial = table.Column<string>(type: "text", nullable: false),
                    direccion = table.Column<string>(type: "text", nullable: false),
                    telefono = table.Column<string>(type: "text", nullable: false),
                    rfc = table.Column<string>(type: "text", nullable: true),
                    leyenda_ticket = table.Column<string>(type: "text", nullable: true),
                    fecha_actualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_datos_establecimiento", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "insumos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    unidad = table.Column<int>(type: "integer", nullable: false),
                    saldo_actual = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    nivel_alerta = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_insumos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mesas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    numero = table.Column<int>(type: "integer", nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mesas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "turnos_caja",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    fondo_inicial = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    fecha_apertura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_cierre = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    operador_apertura_id = table.Column<Guid>(type: "uuid", nullable: false),
                    operador_cierre_id = table.Column<Guid>(type: "uuid", nullable: true),
                    efectivo_esperado = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    efectivo_contado = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    diferencia = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    observaciones = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_turnos_caja", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre_usuario = table.Column<string>(type: "text", nullable: false),
                    nombre_completo = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    rol = table.Column<int>(type: "integer", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_alta = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuario", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre_completo = table.Column<string>(type: "text", nullable: false),
                    rol = table.Column<int>(type: "integer", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_alta = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ventas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    folio = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('folio_venta_seq')"),
                    orden_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fecha_cobro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    operador_id = table.Column<Guid>(type: "uuid", nullable: false),
                    turno_caja_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    monto_descuento = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    base_gravable = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    iva = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    propina = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    anulada = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ventas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "productos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: false),
                    precio_base = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    categoria_id = table.Column<Guid>(type: "uuid", nullable: false),
                    disponible = table.Column<bool>(type: "boolean", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_productos", x => x.id);
                    table.ForeignKey(
                        name: "fk_productos_categorias_categoria_id",
                        column: x => x.categoria_id,
                        principalTable: "categorias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ajustes_saldo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    insumo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cantidad_anterior = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    cantidad_nueva = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    diferencia = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    motivo = table.Column<string>(type: "text", nullable: false),
                    admin_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ajustes_saldo", x => x.id);
                    table.ForeignKey(
                        name: "fk_ajustes_saldo_insumos_insumo_id",
                        column: x => x.insumo_id,
                        principalTable: "insumos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "entradas_inventario",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    insumo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cantidad = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    proveedor = table.Column<string>(type: "text", nullable: true),
                    costo = table.Column<decimal>(type: "numeric(18,4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_entradas_inventario", x => x.id);
                    table.ForeignKey(
                        name: "fk_entradas_inventario_insumos_insumo_id",
                        column: x => x.insumo_id,
                        principalTable: "insumos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mermas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    insumo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cantidad = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    motivo = table.Column<int>(type: "integer", nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mermas", x => x.id);
                    table.ForeignKey(
                        name: "fk_mermas_insumos_insumo_id",
                        column: x => x.insumo_id,
                        principalTable: "insumos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ordenes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    folio_orden = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('folio_orden_seq')"),
                    modalidad = table.Column<int>(type: "integer", nullable: false),
                    mesa_id = table.Column<Guid>(type: "uuid", nullable: true),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    operador_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ordenes", x => x.id);
                    table.ForeignKey(
                        name: "fk_ordenes_mesas_mesa_id",
                        column: x => x.mesa_id,
                        principalTable: "mesas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "rol_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rol_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_rol_claims_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sesiones_usuario",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rol_operativo = table.Column<int>(type: "integer", nullable: false),
                    inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sesiones_usuario", x => x.id);
                    table.ForeignKey(
                        name: "fk_sesiones_usuario_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuario_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuario_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_usuario_claims_usuarios_user_id",
                        column: x => x.user_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuario_logins",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuario_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_usuario_logins_usuarios_user_id",
                        column: x => x.user_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuario_roles",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuario_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_usuario_roles_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_usuario_roles_usuarios_user_id",
                        column: x => x.user_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuario_tokens",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuario_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_usuario_tokens_usuarios_user_id",
                        column: x => x.user_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "descuentos_aplicados",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    campania_id = table.Column<Guid>(type: "uuid", nullable: false),
                    venta_id = table.Column<Guid>(type: "uuid", nullable: false),
                    monto_calculado = table.Column<decimal>(type: "numeric(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_descuentos_aplicados", x => x.id);
                    table.ForeignKey(
                        name: "fk_descuentos_aplicados_ventas_venta_id",
                        column: x => x.venta_id,
                        principalTable: "ventas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "pagos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    venta_id = table.Column<Guid>(type: "uuid", nullable: false),
                    metodo = table.Column<int>(type: "integer", nullable: false),
                    monto_recibido = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    cambio = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    comision_bancaria = table.Column<decimal>(type: "numeric(18,4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pagos", x => x.id);
                    table.ForeignKey(
                        name: "fk_pagos_ventas_venta_id",
                        column: x => x.venta_id,
                        principalTable: "ventas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tickets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    venta_id = table.Column<Guid>(type: "uuid", nullable: false),
                    contenido_snapshot = table.Column<string>(type: "text", nullable: false),
                    fecha_emision = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tickets", x => x.id);
                    table.ForeignKey(
                        name: "fk_tickets_ventas_venta_id",
                        column: x => x.venta_id,
                        principalTable: "ventas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "componentes_combo",
                columns: table => new
                {
                    combo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    producto_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cantidad = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_componentes_combo", x => new { x.combo_id, x.producto_id });
                    table.ForeignKey(
                        name: "fk_componentes_combo_combos_combo_id",
                        column: x => x.combo_id,
                        principalTable: "combos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_componentes_combo_productos_producto_id",
                        column: x => x.producto_id,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "modificadores",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    producto_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    delta_precio = table.Column<decimal>(type: "numeric(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_modificadores", x => x.id);
                    table.ForeignKey(
                        name: "fk_modificadores_productos_producto_id",
                        column: x => x.producto_id,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recetas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    producto_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recetas", x => x.id);
                    table.ForeignKey(
                        name: "fk_recetas_productos_producto_id",
                        column: x => x.producto_id,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lineas_orden",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    orden_id = table.Column<Guid>(type: "uuid", nullable: false),
                    producto_id = table.Column<Guid>(type: "uuid", nullable: true),
                    combo_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cantidad = table.Column<int>(type: "integer", nullable: false),
                    precio_unitario = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    nota_libre = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lineas_orden", x => x.id);
                    table.ForeignKey(
                        name: "fk_lineas_orden_ordenes_orden_id",
                        column: x => x.orden_id,
                        principalTable: "ordenes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lineas_receta",
                columns: table => new
                {
                    receta_id = table.Column<Guid>(type: "uuid", nullable: false),
                    insumo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cantidad = table.Column<decimal>(type: "numeric(12,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lineas_receta", x => new { x.receta_id, x.insumo_id });
                    table.ForeignKey(
                        name: "fk_lineas_receta_recetas_receta_id",
                        column: x => x.receta_id,
                        principalTable: "recetas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lineas_modificador",
                columns: table => new
                {
                    linea_orden_id = table.Column<Guid>(type: "uuid", nullable: false),
                    modificador_id = table.Column<Guid>(type: "uuid", nullable: false),
                    delta_aplicado = table.Column<decimal>(type: "numeric(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lineas_modificador", x => new { x.linea_orden_id, x.modificador_id });
                    table.ForeignKey(
                        name: "fk_lineas_modificador_lineas_orden_linea_orden_id",
                        column: x => x.linea_orden_id,
                        principalTable: "lineas_orden",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ajustes_saldo_insumo_id",
                table: "ajustes_saldo",
                column: "insumo_id");

            migrationBuilder.CreateIndex(
                name: "ix_componentes_combo_producto_id",
                table: "componentes_combo",
                column: "producto_id");

            migrationBuilder.CreateIndex(
                name: "ix_descuentos_aplicados_venta_id",
                table: "descuentos_aplicados",
                column: "venta_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_entradas_inventario_insumo_id",
                table: "entradas_inventario",
                column: "insumo_id");

            migrationBuilder.CreateIndex(
                name: "ix_lineas_orden_orden_id",
                table: "lineas_orden",
                column: "orden_id");

            migrationBuilder.CreateIndex(
                name: "ix_mermas_insumo_id",
                table: "mermas",
                column: "insumo_id");

            migrationBuilder.CreateIndex(
                name: "ix_mesas_numero",
                table: "mesas",
                column: "numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_modificadores_producto_id",
                table: "modificadores",
                column: "producto_id");

            migrationBuilder.CreateIndex(
                name: "ix_ordenes_mesa_id",
                table: "ordenes",
                column: "mesa_id");

            migrationBuilder.CreateIndex(
                name: "ix_pagos_venta_id",
                table: "pagos",
                column: "venta_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_productos_categoria_id",
                table: "productos",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "ix_recetas_producto_id",
                table: "recetas",
                column: "producto_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_rol_claims_role_id",
                table: "rol_claims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "roles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sesiones_usuario_usuario_id",
                table: "sesiones_usuario",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_tickets_venta_id",
                table: "tickets",
                column: "venta_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_usuario_claims_user_id",
                table: "usuario_claims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_usuario_logins_user_id",
                table: "usuario_logins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_usuario_roles_role_id",
                table: "usuario_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "usuarios",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "usuarios",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ventas_folio",
                table: "ventas",
                column: "folio",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ajustes_saldo");

            migrationBuilder.DropTable(
                name: "bitacora_eventos");

            migrationBuilder.DropTable(
                name: "campanias_descuento");

            migrationBuilder.DropTable(
                name: "componentes_combo");

            migrationBuilder.DropTable(
                name: "datos_establecimiento");

            migrationBuilder.DropTable(
                name: "descuentos_aplicados");

            migrationBuilder.DropTable(
                name: "entradas_inventario");

            migrationBuilder.DropTable(
                name: "lineas_modificador");

            migrationBuilder.DropTable(
                name: "lineas_receta");

            migrationBuilder.DropTable(
                name: "mermas");

            migrationBuilder.DropTable(
                name: "modificadores");

            migrationBuilder.DropTable(
                name: "pagos");

            migrationBuilder.DropTable(
                name: "rol_claims");

            migrationBuilder.DropTable(
                name: "sesiones_usuario");

            migrationBuilder.DropTable(
                name: "tickets");

            migrationBuilder.DropTable(
                name: "turnos_caja");

            migrationBuilder.DropTable(
                name: "usuario_claims");

            migrationBuilder.DropTable(
                name: "usuario_logins");

            migrationBuilder.DropTable(
                name: "usuario_roles");

            migrationBuilder.DropTable(
                name: "usuario_tokens");

            migrationBuilder.DropTable(
                name: "combos");

            migrationBuilder.DropTable(
                name: "lineas_orden");

            migrationBuilder.DropTable(
                name: "recetas");

            migrationBuilder.DropTable(
                name: "insumos");

            migrationBuilder.DropTable(
                name: "usuario");

            migrationBuilder.DropTable(
                name: "ventas");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "ordenes");

            migrationBuilder.DropTable(
                name: "productos");

            migrationBuilder.DropTable(
                name: "mesas");

            migrationBuilder.DropTable(
                name: "categorias");

            migrationBuilder.DropSequence(
                name: "folio_orden_seq");

            migrationBuilder.DropSequence(
                name: "folio_venta_seq");
        }
    }
}

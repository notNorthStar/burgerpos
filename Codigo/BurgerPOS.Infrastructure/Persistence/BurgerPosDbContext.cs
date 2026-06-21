using System.Text.Json;
using BurgerPOS.Domain.Administracion.Entities;
using BurgerPOS.Domain.Catalogo.Entities;
using BurgerPOS.Domain.Cobro.Entities;
using BurgerPOS.Domain.Inventario.Entities;
using BurgerPOS.Domain.Identidad.Entities;
using BurgerPOS.Domain.Ordenes.Entities;
using BurgerPOS.Domain.Turno.Entities;
using BurgerPOS.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BurgerPOS.Infrastructure.Persistence;

public class BurgerPosDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public BurgerPosDbContext(DbContextOptions<BurgerPosDbContext> options) : base(options) { }

    // Identidad
    public DbSet<SesionUsuario> SesionesUsuario => Set<SesionUsuario>();

    // Catalogo
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Modificador> Modificadores => Set<Modificador>();
    public DbSet<Receta> Recetas => Set<Receta>();
    public DbSet<LineaReceta> LineasReceta => Set<LineaReceta>();
    public DbSet<Combo> Combos => Set<Combo>();
    public DbSet<ComponenteCombo> ComponentesCombos => Set<ComponenteCombo>();
    public DbSet<CampaniaDescuento> CampaniasDescuento => Set<CampaniaDescuento>();
    public DbSet<DatosEstablecimiento> DatosEstablecimiento => Set<DatosEstablecimiento>();

    // Inventario
    public DbSet<Insumo> Insumos => Set<Insumo>();
    public DbSet<EntradaInventario> EntradasInventario => Set<EntradaInventario>();
    public DbSet<Merma> Mermas => Set<Merma>();
    public DbSet<AjusteSaldo> AjustesSaldo => Set<AjusteSaldo>();

    // Ordenes
    public DbSet<Mesa> Mesas => Set<Mesa>();
    public DbSet<Orden> Ordenes => Set<Orden>();
    public DbSet<LineaOrden> LineasOrden => Set<LineaOrden>();
    public DbSet<LineaModificador> LineasModificador => Set<LineaModificador>();

    // Cobro
    public DbSet<Venta> Ventas => Set<Venta>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<DescuentoAplicado> DescuentosAplicados => Set<DescuentoAplicado>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    // Turno
    public DbSet<TurnoCaja> TurnosCaja => Set<TurnoCaja>();

    // Administracion
    public DbSet<BitacoraEvento> BitacoraEventos => Set<BitacoraEvento>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ── Identity: renombrar tablas a snake_case ──────────────────────────
        builder.Entity<ApplicationUser>().ToTable("usuarios");
        builder.Entity<IdentityRole<Guid>>().ToTable("roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("usuario_roles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("usuario_claims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("usuario_logins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("usuario_tokens");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("rol_claims");

        // ── Secuencias para folios (RN-02) ───────────────────────────────────
        builder.HasSequence<int>("folio_orden_seq").StartsAt(1).IncrementsBy(1);
        builder.HasSequence<int>("folio_venta_seq").StartsAt(1).IncrementsBy(1);

        // ── Identidad ────────────────────────────────────────────────────────
        builder.Entity<SesionUsuario>(e =>
        {
            e.HasKey(s => s.Id);
            e.ToTable("sesiones_usuario");
        });

        // ── Catalogo ─────────────────────────────────────────────────────────
        builder.Entity<Categoria>(e =>
        {
            e.HasKey(c => c.Id);
            e.ToTable("categorias");
            e.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
        });

        builder.Entity<Producto>(e =>
        {
            e.HasKey(p => p.Id);
            e.ToTable("productos");
            e.Property(p => p.Nombre).IsRequired().HasMaxLength(150);
            e.Property(p => p.PrecioBase).HasColumnType("numeric(18,4)");
            e.HasOne(p => p.Categoria).WithMany().HasForeignKey(p => p.CategoriaId);
            e.HasOne(p => p.Receta).WithOne(r => r.Producto).HasForeignKey<Receta>(r => r.ProductoId);
        });

        builder.Entity<Modificador>(e =>
        {
            e.HasKey(m => m.Id);
            e.ToTable("modificadores");
            e.Property(m => m.DeltaPrecio).HasColumnType("numeric(18,4)");
            e.HasOne(m => m.Producto).WithMany(p => p.Modificadores).HasForeignKey(m => m.ProductoId);
        });

        builder.Entity<Receta>(e =>
        {
            e.HasKey(r => r.Id);
            e.ToTable("recetas");
            e.HasMany(r => r.Lineas).WithOne().HasForeignKey(l => l.RecetaId);
        });

        builder.Entity<LineaReceta>(e =>
        {
            e.HasKey(l => new { l.RecetaId, l.InsumoId });
            e.ToTable("lineas_receta");
            e.Property(l => l.Cantidad).HasColumnType("numeric(12,4)");
        });

        builder.Entity<Combo>(e =>
        {
            e.HasKey(c => c.Id);
            e.ToTable("combos");
            e.Property(c => c.PrecioEspecial).HasColumnType("numeric(18,4)");
            e.HasMany(c => c.Componentes).WithOne(cc => cc.Combo).HasForeignKey(cc => cc.ComboId);
        });

        builder.Entity<ComponenteCombo>(e =>
        {
            e.HasKey(cc => new { cc.ComboId, cc.ProductoId });
            e.ToTable("componentes_combo");
            e.HasOne(cc => cc.Producto).WithMany().HasForeignKey(cc => cc.ProductoId);
        });

        builder.Entity<CampaniaDescuento>(e =>
        {
            e.HasKey(c => c.Id);
            e.ToTable("campanias_descuento");
            e.Property(c => c.Valor).HasColumnType("numeric(18,4)");
            e.Property(c => c.DiasSemana)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions?)null) ?? new List<int>())
                .HasColumnType("text");
        });

        builder.Entity<DatosEstablecimiento>(e =>
        {
            e.HasKey(d => d.Id);
            e.ToTable("datos_establecimiento");
        });

        // ── Inventario ───────────────────────────────────────────────────────
        builder.Entity<Insumo>(e =>
        {
            e.HasKey(i => i.Id);
            e.ToTable("insumos");
            e.Property(i => i.SaldoActual).HasColumnType("numeric(12,4)");
            e.Property(i => i.NivelAlerta).HasColumnType("numeric(12,4)");
        });

        builder.Entity<EntradaInventario>(e =>
        {
            e.HasKey(ei => ei.Id);
            e.ToTable("entradas_inventario");
            e.Property(ei => ei.Cantidad).HasColumnType("numeric(12,4)");
            e.Property(ei => ei.Costo).HasColumnType("numeric(18,4)");
            e.HasOne(ei => ei.Insumo).WithMany().HasForeignKey(ei => ei.InsumoId);
        });

        builder.Entity<Merma>(e =>
        {
            e.HasKey(m => m.Id);
            e.ToTable("mermas");
            e.Property(m => m.Cantidad).HasColumnType("numeric(12,4)");
            e.HasOne(m => m.Insumo).WithMany().HasForeignKey(m => m.InsumoId);
        });

        builder.Entity<AjusteSaldo>(e =>
        {
            e.HasKey(a => a.Id);
            e.ToTable("ajustes_saldo");
            e.Property(a => a.CantidadAnterior).HasColumnType("numeric(12,4)");
            e.Property(a => a.CantidadNueva).HasColumnType("numeric(12,4)");
            e.Property(a => a.Diferencia).HasColumnType("numeric(12,4)");
            e.HasOne(a => a.Insumo).WithMany().HasForeignKey(a => a.InsumoId);
        });

        // ── Ordenes ──────────────────────────────────────────────────────────
        builder.Entity<Mesa>(e =>
        {
            e.HasKey(m => m.Id);
            e.ToTable("mesas");
            e.HasIndex(m => m.Numero).IsUnique();
        });

        builder.Entity<Orden>(e =>
        {
            e.HasKey(o => o.Id);
            e.ToTable("ordenes");
            e.Property(o => o.FolioOrden)
                .HasDefaultValueSql("nextval('folio_orden_seq')");
            e.Property(o => o.Subtotal).HasColumnType("numeric(18,4)");
            e.HasOne(o => o.Mesa).WithMany().HasForeignKey(o => o.MesaId).IsRequired(false);
            e.HasMany(o => o.Lineas).WithOne().HasForeignKey(l => l.OrdenId);
        });

        builder.Entity<LineaOrden>(e =>
        {
            e.HasKey(l => l.Id);
            e.ToTable("lineas_orden");
            e.Property(l => l.PrecioUnitario).HasColumnType("numeric(18,4)");
            e.HasMany(l => l.Modificadores).WithOne(m => m.LineaOrden).HasForeignKey(m => m.LineaOrdenId);
        });

        builder.Entity<LineaModificador>(e =>
        {
            e.HasKey(lm => new { lm.LineaOrdenId, lm.ModificadorId });
            e.ToTable("lineas_modificador");
            e.Property(lm => lm.DeltaAplicado).HasColumnType("numeric(18,4)");
        });

        // ── Cobro ────────────────────────────────────────────────────────────
        builder.Entity<Venta>(e =>
        {
            e.HasKey(v => v.Id);
            e.ToTable("ventas");
            e.Property(v => v.Folio)
                .HasDefaultValueSql("nextval('folio_venta_seq')");
            e.HasIndex(v => v.Folio).IsUnique();
            e.Property(v => v.Subtotal).HasColumnType("numeric(18,4)");
            e.Property(v => v.MontoDescuento).HasColumnType("numeric(18,4)");
            e.Property(v => v.BaseGravable).HasColumnType("numeric(18,4)");
            e.Property(v => v.Iva).HasColumnType("numeric(18,4)");
            e.Property(v => v.Propina).HasColumnType("numeric(18,4)");
            e.Property(v => v.Total).HasColumnType("numeric(18,4)");
            e.HasOne(v => v.Pago).WithOne(p => p.Venta).HasForeignKey<Pago>(p => p.VentaId);
            e.HasOne(v => v.Ticket).WithOne(t => t.Venta).HasForeignKey<Ticket>(t => t.VentaId);
            e.HasOne(v => v.DescuentoAplicado).WithOne().HasForeignKey<DescuentoAplicado>(d => d.VentaId).IsRequired(false);
        });

        builder.Entity<Pago>(e =>
        {
            e.HasKey(p => p.Id);
            e.ToTable("pagos");
            e.Property(p => p.MontoRecibido).HasColumnType("numeric(18,4)");
            e.Property(p => p.Cambio).HasColumnType("numeric(18,4)");
            e.Property(p => p.ComisionBancaria).HasColumnType("numeric(18,4)");
        });

        builder.Entity<DescuentoAplicado>(e =>
        {
            e.HasKey(d => d.Id);
            e.ToTable("descuentos_aplicados");
            e.Property(d => d.MontoCalculado).HasColumnType("numeric(18,4)");
        });

        builder.Entity<Ticket>(e =>
        {
            e.HasKey(t => t.Id);
            e.ToTable("tickets");
        });

        // ── Turno ────────────────────────────────────────────────────────────
        builder.Entity<TurnoCaja>(e =>
        {
            e.HasKey(t => t.Id);
            e.ToTable("turnos_caja");
            e.Property(t => t.FondoInicial).HasColumnType("numeric(18,4)");
            e.Property(t => t.EfectivoEsperado).HasColumnType("numeric(18,4)");
            e.Property(t => t.EfectivoContado).HasColumnType("numeric(18,4)");
            e.Property(t => t.Diferencia).HasColumnType("numeric(18,4)");
        });

        // ── Administracion ───────────────────────────────────────────────────
        builder.Entity<BitacoraEvento>(e =>
        {
            e.HasKey(b => b.Id);
            e.ToTable("bitacora_eventos");
        });
    }
}

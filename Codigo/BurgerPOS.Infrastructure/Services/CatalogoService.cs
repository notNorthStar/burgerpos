using BurgerPOS.Application.Interfaces.Services;
using BurgerPOS.Domain.Catalogo.Entities;
using BurgerPOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BurgerPOS.Infrastructure.Services;

public class CatalogoService : ICatalogoService
{
    private readonly BurgerPosDbContext _context;

    public CatalogoService(BurgerPosDbContext context) => _context = context;

    public async Task<Categoria> CrearCategoriaAsync(string nombre, int ordenVisual = 0)
    {
        var categoria = Categoria.Crear(nombre, ordenVisual);
        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();
        return categoria;
    }

    public async Task<Categoria?> ObtenerCategoriaAsync(Guid id)
        => await _context.Categorias.FindAsync(id);

    public async Task<List<Categoria>> ObtenerCategoriasAsync()
        => await _context.Categorias.OrderBy(c => c.OrdenVisual).ToListAsync();

    public async Task ActualizarCategoriaAsync(Guid id, string nombre, int ordenVisual)
    {
        var categoria = await _context.Categorias.FindAsync(id)
            ?? throw new KeyNotFoundException($"Categoría {id} no encontrada.");
        categoria.ActualizarNombre(nombre);
        categoria.ActualizarOrden(ordenVisual);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarCategoriaAsync(Guid id)
    {
        var categoria = await _context.Categorias.FindAsync(id)
            ?? throw new KeyNotFoundException($"Categoría {id} no encontrada.");
        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();
    }

    public async Task<Producto> CrearProductoAsync(string nombre, string descripcion,
        decimal precioBase, Guid categoriaId)
    {
        var producto = Producto.Crear(nombre, descripcion, precioBase, categoriaId);
        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();
        return producto;
    }

    public async Task<Producto?> ObtenerProductoAsync(Guid id)
        => await _context.Productos
            .Include(p => p.Modificadores)
            .Include(p => p.Receta).ThenInclude(r => r!.Lineas)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<List<Producto>> ObtenerProductosActivosAsync()
        => await _context.Productos.Where(p => p.Activo).OrderBy(p => p.Nombre).ToListAsync();

    public async Task ActualizarPrecioAsync(Guid productoId, decimal nuevoPrecio)
    {
        var producto = await _context.Productos.FindAsync(productoId)
            ?? throw new KeyNotFoundException($"Producto {productoId} no encontrado.");
        producto.ActualizarPrecio(nuevoPrecio);
        await _context.SaveChangesAsync();
    }

    public async Task<Receta> AgregarRecetaAsync(Guid productoId,
        IEnumerable<(Guid insumoId, decimal cantidad)> lineas)
    {
        var receta = Receta.Crear(productoId);
        foreach (var (insumoId, cantidad) in lineas)
            receta.AgregarLinea(insumoId, cantidad);
        _context.Recetas.Add(receta);
        await _context.SaveChangesAsync();
        return receta;
    }
}

using BurgerPOS.Domain.Catalogo.Entities;

namespace BurgerPOS.Application.Interfaces.Services;

public interface ICatalogoService
{
    Task<Categoria> CrearCategoriaAsync(string nombre, int ordenVisual = 0);
    Task<Categoria?> ObtenerCategoriaAsync(Guid id);
    Task<List<Categoria>> ObtenerCategoriasAsync();
    Task ActualizarCategoriaAsync(Guid id, string nombre, int ordenVisual);
    Task EliminarCategoriaAsync(Guid id);

    Task<Producto> CrearProductoAsync(string nombre, string descripcion, decimal precioBase, Guid categoriaId);
    Task<Producto?> ObtenerProductoAsync(Guid id);
    Task<List<Producto>> ObtenerProductosActivosAsync();
    Task ActualizarPrecioAsync(Guid productoId, decimal nuevoPrecio);

    Task<Receta> AgregarRecetaAsync(Guid productoId, IEnumerable<(Guid insumoId, decimal cantidad)> lineas);
}

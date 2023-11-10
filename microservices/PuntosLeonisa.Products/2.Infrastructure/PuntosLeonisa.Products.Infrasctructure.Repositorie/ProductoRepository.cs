using PuntosLeonisa.Products.Domain;
using PuntosLeonisa.Products.Domain.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;

namespace PuntosLeonisa.Products.Infrasctructure.Repositorie;
public class ProductoRepository : Repository<Producto>, IProductoRepository
{
    private readonly ProductContext _context;
    public ProductoRepository(ProductContext context) :base(context)
    {
       
    }

    public async Task<IEnumerable<Producto>> GetFiltro(string categoria, double? precioMin, double? precioMax, string genero, string proveedor)
    {
        var query = _context.Set<Producto>().AsQueryable();

        if (!string.IsNullOrEmpty(categoria))
        {
            query = query.Where(p => p.CategoriaNombre == categoria);
        }

        if (precioMin.HasValue)
        {
            query = query.Where(p => p.Precio >= precioMin.Value);
        }

        if (precioMax.HasValue)
        {
            query = query.Where(p => p.Precio <= precioMax.Value);
        }

        if (!string.IsNullOrEmpty(genero))
        {
            query = query.Where(p => p.Genero == genero);
        }

        if (!string.IsNullOrEmpty(proveedor))
        {
            query = query.Where(p => p.Proveedor == proveedor);
        }

        return await query.ToListAsync();
    }

}


using System;
namespace PuntosLeonisa.Products.Domain.Interfaces
{
    public interface IProductoRepository : IRepository<Producto>
    {
        Task<IEnumerable<Producto>> GetFiltro(string categoria, double? precioMin, double? precioMax, string genero, string proveedor);
    }
}



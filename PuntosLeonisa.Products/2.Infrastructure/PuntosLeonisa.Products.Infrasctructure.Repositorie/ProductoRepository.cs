using PuntosLeonisa.Products.Domain;
using PuntosLeonisa.Products.Domain.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace PuntosLeonisa.Products.Infrasctructure.Repositorie;
public class ProductoRepository : Repository<Producto>, IProductoRepository
{
    public ProductoRepository(DbContext context) : base(context)
    {
    }


}


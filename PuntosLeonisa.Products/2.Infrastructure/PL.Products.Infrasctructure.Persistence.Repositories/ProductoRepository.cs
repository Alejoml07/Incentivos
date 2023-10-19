using PuntosLeonisa.Products.Domain;
using PuntosLeonisa.Products.Domain.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace PL.Products.Infrasctructure.Persistence.Repositories;
public class ProductoRepository : Repository<Producto>, IProductoRepository
{
    public ProductoRepository(DbContext context) : base(context)
    {
    }

    
}


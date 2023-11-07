using PuntosLeonisa.Infrasctructure.Core.Repository;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Seguridad.Domain.Interfaces;

namespace PuntosLeonisa.Seguridad.Infrasctructure.Repositorie;
public class ProveedorRepository : Repository<Proveedor>, IProveedorRepository
{
    public ProveedorRepository(SeguridadContext context) : base(context)
    {
    }
}


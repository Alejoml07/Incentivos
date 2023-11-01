using PuntosLeonisa.Infrasctructure.Core.Repository;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Seguridad.Domain.Interfaces;

namespace PuntosLeonisa.Seguridad.Infrasctructure.Repositorie;
public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(SeguridadContext context) : base(context)
    {
    }
}


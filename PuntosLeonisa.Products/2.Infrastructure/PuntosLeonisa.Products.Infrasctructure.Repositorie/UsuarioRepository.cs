using PuntosLeonisa.Products.Domain;
using PuntosLeonisa.Products.Domain.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace PuntosLeonisa.Products.Infrasctructure.Repositorie;
public class UsuarioRepository : Repository<Domain.Model.Usuario>, IUsuarioRepository
{
    public UsuarioRepository(DbContext context) : base(context)
    {
    }
}


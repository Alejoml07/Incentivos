using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Fidelizacion.Domain.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie;
public class UsuarioInfoPuntosRepository : Repository<UsuarioInfoPuntos>, IUsuarioInfoPuntosRepository
{
    private readonly FidelizacionContext context;

    public UsuarioInfoPuntosRepository(FidelizacionContext context) : base(context)
    {
        this.context = context;
    }

    public FidelizacionContext Context => context;

    public async Task<UsuarioInfoPuntos> GetUsuarioByCedula(string? cedula)
    {
        //traeme el primer registro que coincida con la cedula
        var response = await context.Set<UsuarioInfoPuntos>().Where(x => x.Cedula == cedula).FirstOrDefaultAsync();
        return response;
    }

    public async Task<UsuarioInfoPuntos> GetUsuarioByEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            // Loguear o manejar según corresponda
            return null;
        }

        try
        {
            var respuesta =  context.Set<UsuarioInfoPuntos>()
                                         .AsNoTracking()
                                         .FirstOrDefault(x => x.Email == email);
            return respuesta;
        }
        catch (Exception ex)
        {
            // Manejar o registrar el error
            // Log.Error(ex, "Error al obtener usuario por email: {Email}", email);
            throw;
        }
    }

}


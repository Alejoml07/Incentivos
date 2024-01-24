using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;

namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie
{
    public class ExtractosRepository : Repository<Extractos>, IExtractosRepository
    {
        internal FidelizacionContext _context;
        public ExtractosRepository(FidelizacionContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Extractos>> GetExtractosByUsuario(string cedula)
        {
            var result = await _context.Set<Extractos>().Where(x => x.Usuario.Cedula == cedula).ToListAsync();

            return result;

        }
    }
}


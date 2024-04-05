using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using System.IO.Hashing;

namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie
{
    public class ExtractosRepository : Repository<Extractos>, IExtractosRepository
    {
        internal FidelizacionContext _context;
        public ExtractosRepository(FidelizacionContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Extractos>> GetExtractosByUser(string? cedula)
        {
            return await _context.Set<Extractos>().Where(x => x.Usuario.Cedula == cedula).ToListAsync();
        }

        public async Task<IEnumerable<Extractos>> GetExtractosByUserAndDate(ReporteDto data)
        {
            if (data.Cedula == "" && data.Mes != "" && data.Anio != "")
            {
                var result = await _context.Set<Extractos>().Where(x => x.Mes == data.Mes && x.Anio == data.Anio).ToListAsync();
                return result;
            }
            if(data.Cedula != "" && data.Mes != "" && data.Anio != "")
            {
                var result = await _context.Set<Extractos>().Where(x => x.Usuario.Cedula == data.Cedula && x.Mes == data.Mes && x.Anio == data.Anio).ToListAsync();
                return result;
            }
            else
            {
                var result = await _context.Set<Extractos>().Where(x => x.Fecha >= data.FechaInicio && x.Fecha <= data.FechaFin).ToListAsync();
                return result;
            }

           
        }

        public async Task<IEnumerable<Extractos>> GetExtractosByUsuario(string cedula)
        {
            var result = await _context.Set<Extractos>().Where(x => x.Usuario.Cedula == cedula).ToListAsync();
            return result;
        }
    }
}


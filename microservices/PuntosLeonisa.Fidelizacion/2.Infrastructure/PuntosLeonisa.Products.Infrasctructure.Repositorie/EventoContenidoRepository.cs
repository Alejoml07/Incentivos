using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie
{
    public class EventoContenidoRepository : Repository<EventoContenido>, IEventoContenidoRepository
    {
        internal FidelizacionContext _context;
        public EventoContenidoRepository(FidelizacionContext context) : base(context)
        {
            _context = context;
        }

        public async Task<EventoContenido> GetEventoContenidoByEvento(EventoContenido data)
        {
            var query = _context.Set<EventoContenido>().Where(x => x.TipoEvento == data.TipoEvento).FirstOrDefault();
            return query;

        }
    }
}

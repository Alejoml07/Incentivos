using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;

namespace PuntosLeonisa.Seguridad.Infrasctructure.Repositorie
{
    public class TratamientoDatosRepository : Repository<TratamientoDatosDto>, ITratamientoDatosRepository
    {
        internal SeguridadContext _context;
        public TratamientoDatosRepository(SeguridadContext context) : base(context)
        {
            _context = context;
        }
    }
}

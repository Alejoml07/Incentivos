using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Domain.Core.Repository;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;

namespace PuntosLeonisa.Seguridad.Infrasctructure.Repositorie
{
    public class TokenRepository : Repository<TokenDto>, ITokenRepository
    {
        internal SeguridadContext _context;

        public TokenRepository(SeguridadContext context) : base(context)
        {
            _context = context;

        }

        public async Task<TokenDto> GetUsuarioByToken(string token)
        {
            //GetUsuarioByToken
            var usuario =  await _context.Set<TokenDto>().FirstOrDefaultAsync(u => u.Token == token);
            return usuario;
        }
    }
}
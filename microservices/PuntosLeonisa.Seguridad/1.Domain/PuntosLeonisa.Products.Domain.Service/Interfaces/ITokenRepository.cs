using PuntosLeonisa.Domain.Core.Repository;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Seguridad.Domain.Service.Interfaces
{
    public interface ITokenRepository : IRepository<TokenDto>
    {
        Task<TokenDto> GetUsuarioByToken(string token);
    }
}

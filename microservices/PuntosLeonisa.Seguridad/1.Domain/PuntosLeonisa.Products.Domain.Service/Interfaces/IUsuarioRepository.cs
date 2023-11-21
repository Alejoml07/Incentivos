using PuntosLeonisa.Domain.Core.Repository;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using System;
namespace PuntosLeonisa.Seguridad.Domain.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<LoginDto> Login(string correo, string contrasena);
    }


}

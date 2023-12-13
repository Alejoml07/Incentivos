using PuntosLeonisa.Domain.Core.Repository;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using System;
namespace PuntosLeonisa.Seguridad.Domain.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<Usuario?> Login(LoginDto loginDto);

        Task<bool> CambiarPwd(CambioPwdDto cambioContraseñaDto);

        Task<bool> RecuperarPwd(PasswordRecoveryRequestDto data);

        Task<bool> SendCustomEmailToUser(string email);
    }


}

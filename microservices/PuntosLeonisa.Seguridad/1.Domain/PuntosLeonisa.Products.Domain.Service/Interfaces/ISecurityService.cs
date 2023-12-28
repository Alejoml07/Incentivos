using PuntosLeonisa.Seguridad.Domain.Service.Enum;

namespace PuntosLeonisa.Seguridad.Domain.Service.Interfaces
{
    public interface ISecurityService
    {
        string HasPassword(string? password);
        PasswordVerifyResult VerifyPassword(string password, string? pwd);

        string GenerarHTML(string urlRestablecer);

    }
}

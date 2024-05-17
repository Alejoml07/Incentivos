using PuntosLeonisa.Fidelizacion.Domain.Model;

namespace PuntosLeonisa.Fidelizacion.Domain.Interfaces
{
    public interface IUsuarioInfoPuntosRepository : IRepository<UsuarioInfoPuntos>
    {
        Task<UsuarioInfoPuntos> GetUsuarioByEmail(string? email);
        Task<UsuarioInfoPuntos> GetUsuarioByCedula(string? cedula);
    }
}

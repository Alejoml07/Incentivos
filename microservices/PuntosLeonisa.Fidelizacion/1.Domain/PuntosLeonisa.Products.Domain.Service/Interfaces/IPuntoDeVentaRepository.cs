using PuntosLeonisa.Fidelizacion.Domain.Model.Carrito;

namespace PuntosLeonisa.Seguridad.Domain.Service.Interfaces
{
    public interface IPuntoDeVentaRepository : IRepository<PuntoDeVenta>
    {
        Task<PuntoDeVenta> GetPuntoDeVentaByCodigo(string codigo);
    }
}

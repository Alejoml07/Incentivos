using PuntosLeonisa.Fidelizacion.Application.Core.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.PuntoDeVenta;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using PuntosLeonisa.Products.Application.Core.Interfaces;

namespace PuntosLeonisa.Seguridad.Application.Core
{
    public interface IPuntoDeVentaApplication : IApplicationCore<PuntoDeVentaDto>,IPuntoVentaVarApplication,IAsignacionApplication, IPuntoVentaHistoriaApplication, ISeguimientoLiquidacionApplication
    {
        Task<GenericResponse<bool>> LiquidacionPuntosMes(LiquidacionPuntos data, int startIndex = 0, int userBatchSize = 100);
        Task<GenericResponse<bool>> AddAndDeleteVentaVarAndHistoria(LiquidacionPuntos data);
        Task<GenericResponse<IEnumerable<UsuarioInfoPuntos>>> GetInfoWithSpace();

        
    }
}

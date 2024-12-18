using PuntosLeonisa.Fidelizacion.Domain.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUsuarioInfoPuntosRepository UsuarioInfoPuntosRepository { get; }
        IMovimientoPuntosRepository PuntosRepository { get; }
        ICarritoRepository CarritoRepository { get; }
        IWishListRepository WishListRepository { get; }
        ISmsRepository SmsRepository { get; }
        IUsuarioRedencionRepository UsuarioRedencionRepository { get; }
        IFidelizacionPuntosRepository FidelizacionPuntosRepository { get; }
        IExtractosRepository ExtractosRepository { get; }
        IVariableRepository VariableRepository { get; }
        IPuntoVentaVarRepository PuntoVentaVarRepository { get; }
        IAsignacionRepository AsignacionRepository { get; }
        IPuntoVentaHistoriaRepository PuntoVentaHistoria { get; }
        IUsuarioScannerRepository UsuarioScannerRepository { get; }
        IGarantiaRepository GarantiaRepository { get; }
        IPuntoDeVentaRepository PuntoDeVentaRepository { get; }
        ISeguimientoLiquidacionRepository SeguimientoLiquidacionRepository { get; }
        IEventoContenidoRepository EventoContenidoRepository { get; }
        ILogEstadoRepository LogEstadoRepository { get; }





        // Otros repositorios relacionados...

        Task SaveChangesAsync();
        void SaveChangesSync();
    }
}

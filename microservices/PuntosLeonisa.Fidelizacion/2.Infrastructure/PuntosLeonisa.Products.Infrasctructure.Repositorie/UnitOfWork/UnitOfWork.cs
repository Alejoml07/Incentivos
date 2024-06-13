using PuntosLeonisa.Fidelizacion.Domain.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Service.UnitOfWork;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;
using PuntosLeonisa.Seguridad.Infrasctructure.Repositorie;

public class UnitOfWork : IUnitOfWork
{
    private readonly FidelizacionContext _fidelizacionContext;
    private IUsuarioInfoPuntosRepository _usuarioInfoPuntosRepository;
    private IMovimientoPuntosRepository _puntosRepository;
    private ICarritoRepository _carritoRepository;
    private IWishListRepository _wishListRepository;
    private ISmsRepository _smsRepository;
    private IFidelizacionPuntosRepository _fidelizacionPuntosRepository;
    private IUsuarioRedencionRepository _usuarioRedencionRepository;
    private IExtractosRepository _extractosRepository;
    private IVariableRepository _variableRepository;
    private IPuntoVentaVarRepository _puntoVentaVarRepository;
    private IPuntoDeVentaRepository _puntoDeVentaRepository;
    private IAsignacionRepository _asignacionRepository;
    private IPuntoVentaHistoriaRepository _puntoVentaHistoriaRepository;
    private IUsuarioScannerRepository _usuarioScannerRepository;


    public UnitOfWork(FidelizacionContext fidelizacionContext)
    {
        _fidelizacionContext = fidelizacionContext;
    }

    public IUsuarioInfoPuntosRepository UsuarioInfoPuntosRepository
    {
        get
        {
            _usuarioInfoPuntosRepository ??= new UsuarioInfoPuntosRepository(_fidelizacionContext);
            return _usuarioInfoPuntosRepository;
        }
    }

    public IMovimientoPuntosRepository PuntosRepository
    {
        get
        {
            _puntosRepository ??= new MovimientoPuntosRepository(_fidelizacionContext);
            return _puntosRepository;
        }
    }

    public ICarritoRepository CarritoRepository
    {
        get
        {
            _carritoRepository ??= new CarritoRepository(_fidelizacionContext);
            return _carritoRepository;
        }
    }

    public IUsuarioRedencionRepository UsuarioRedencionRepository
    {
        get
        {
            _usuarioRedencionRepository ??= new UsuarioRedencionRepository(_fidelizacionContext);
            return _usuarioRedencionRepository;
        }
    }

    public IWishListRepository WishListRepository
    {
        get
        {
            _wishListRepository ??= new WishListRepository(_fidelizacionContext);
            return _wishListRepository;
        }
    }

    public ISmsRepository SmsRepository
    {
        get
        {
            _smsRepository ??= new SmsRepository(_fidelizacionContext);
            return _smsRepository;
        }
    }

    public IFidelizacionPuntosRepository FidelizacionPuntosRepository
    {
        get
        {
            _fidelizacionPuntosRepository ??= new FidelizacionPuntosRepository(_fidelizacionContext);
            return _fidelizacionPuntosRepository;
        }
    }

    public IExtractosRepository ExtractosRepository
    {
        get
        {
            _extractosRepository ??= new ExtractosRepository(_fidelizacionContext);
            return _extractosRepository;
        }
    }

    public IVariableRepository VariableRepository
    {
        get
        {
            _variableRepository ??= new VariableRepository(_fidelizacionContext);
            return _variableRepository;
        }
    }

    public IPuntoDeVentaRepository PuntoDeVentaRepository
    {
        get
        {
            _puntoDeVentaRepository ??= new PuntoDeVentaRepository(_fidelizacionContext);
            return _puntoDeVentaRepository;
        }
    }

    public IPuntoVentaVarRepository PuntoVentaVarRepository
    {
        get
        {
            _puntoVentaVarRepository ??= new PuntoVentaVarRepository(_fidelizacionContext);
            return _puntoVentaVarRepository;
        }
    }

    public IAsignacionRepository AsignacionRepository
    {
        get
        {
            _asignacionRepository ??= new AsignacionRepository(_fidelizacionContext);
            return _asignacionRepository;
        }
    }

    public IPuntoVentaHistoriaRepository PuntoVentaHistoria
    {
        get
        {
            _puntoVentaHistoriaRepository ??= new PuntoVentaHistoriaRepository(_fidelizacionContext);
            return _puntoVentaHistoriaRepository;
        }
        
    }

    public IUsuarioScannerRepository UsuarioScannerRepository
    {
        get
        {
            _usuarioScannerRepository ??= new UsuarioScannerRepository(_fidelizacionContext);
            return _usuarioScannerRepository;
        }
    }

    public void SaveChangesSync()
    {
        _fidelizacionContext.SaveChanges();
    }

    public void Dispose()
    {
        _fidelizacionContext.Dispose();
    }

    public async Task SaveChangesAsync()
    {
        await _fidelizacionContext.SaveChangesAsync();
    }

}
using PuntosLeonisa.Fidelizacion.Domain.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Service.UnitOfWork;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;

public class UnitOfWork : IUnitOfWork
{
    private readonly FidelizacionContext _fidelizacionContext;
    private IUsuarioInfoPuntosRepository _usuarioInfoPuntosRepository;
    private IPuntosManualRepository _puntosRepository;
    private ICarritoRepository _carritoRepository;
    private IWishListRepository _wishListRepository;
    private ISmsRepository _smsRepository;
    private IUsuarioRedencionRepository  _usuarioRedencionRepository;

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

    public IPuntosManualRepository PuntosRepository
    {
        get
        {
            _puntosRepository ??= new PuntosManualRepository(_fidelizacionContext);
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
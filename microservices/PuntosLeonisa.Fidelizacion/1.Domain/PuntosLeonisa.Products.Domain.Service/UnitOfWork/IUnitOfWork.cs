using PuntosLeonisa.Fidelizacion.Domain.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUsuarioInfoPuntosRepository UsuarioInfoPuntosRepository { get; }
        IPuntosManualRepository PuntosRepository { get; }
        ICarritoRepository CarritoRepository { get; }
        IWishListRepository WishListRepository { get; }
        ISmsRepository SmsRepository { get; }
        IUsuarioRedencionRepository UsuarioRedencionRepository { get; }



        // Otros repositorios relacionados...

        Task SaveChangesAsync();
        void SaveChangesSync();
    }
}

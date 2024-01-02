using PuntosLeonisa.Fidelizacion.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces
{
    public interface IUsuarioRedencionRepository : IRepository<UsuarioRedencion>
    {
        int GetNroPedido();

        IEnumerable<UsuarioRedencion> GetRedencionesWithProductsByProveedor(string proveedor);
    }
}

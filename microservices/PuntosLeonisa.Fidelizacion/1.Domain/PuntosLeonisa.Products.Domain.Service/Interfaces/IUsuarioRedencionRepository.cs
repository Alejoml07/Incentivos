using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Garantias;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Usuarios;
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
        IEnumerable<UsuarioRedencion> GetRedencionesWithProductsByEmail(string email);
        IEnumerable<UsuarioRedencion> GetReporteRedencion(ReporteDto data);
        Task<UsuarioRedencion> GetUsuarioRedencionByNroPedido(int nropedido);
        Task<IEnumerable<UsuarioRedencion>> GetUsuariosRedencionPuntosByTipoUsuarioAndProveedor(TipoUsuarioDto[] data);

    }
}

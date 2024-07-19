using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces
{
    public interface IGarantiaRepository : IRepository<Garantia>
    {
        int GetNroGarantia();
        Task<IEnumerable<Garantia>> GetGarantiaByProveedorOrAll(TipoUsuarioDto[] data);
        Task<IEnumerable<Garantia>> GetGarantiaByEmail(string email);
    }
}

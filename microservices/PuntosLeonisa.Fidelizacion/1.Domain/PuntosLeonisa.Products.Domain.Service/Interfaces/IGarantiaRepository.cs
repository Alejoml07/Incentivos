using PuntosLeonisa.Fidelizacion.Domain.Model;
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
        Task<IEnumerable<Garantia>> GetGarantiaByProveedorOrAll(string proveedor);
    }
}

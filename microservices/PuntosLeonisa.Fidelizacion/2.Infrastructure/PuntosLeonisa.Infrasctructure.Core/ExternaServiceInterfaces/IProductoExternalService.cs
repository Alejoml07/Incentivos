using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Infrasctructure.Core.ExternaServiceInterfaces
{
    public interface IProductoExternalService
    {
        Task<GenericResponse<IEnumerable<bool>>> UpdateInventory(ProductoRefence[] data);
        Task<GenericResponse<ProductoRefence>> GetProductByEAN(string ean);

    }
}

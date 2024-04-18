using PuntosLeonisa.Domain.Core.Repository;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Seguridad.Domain.Model;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.PuntosDeVenta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Seguridad.Domain.Service.Interfaces
{
    public interface IPuntoDeVentaRepository : IRepository<PuntoDeVentaDto>
    {
    }
}

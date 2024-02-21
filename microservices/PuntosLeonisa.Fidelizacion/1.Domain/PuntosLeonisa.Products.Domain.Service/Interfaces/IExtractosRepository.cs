using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces
{
    public interface IExtractosRepository : IRepository<Extractos>
    {
        Task<IEnumerable<Extractos>> GetExtractosByUsuario(string cedula);

        Task<IEnumerable<Extractos>> GetExtractosByDate(ReporteDto data);
    }
}

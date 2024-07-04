using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Products.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces
{
    public interface IVariableRepository : IRepository<Variable>
    {
        Task<Variable> GetVariablesByCodigo(string codigo);
        Task<Variable> GetVariablesParaBase(PuntoVentaVar data);
    }
}

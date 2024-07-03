using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Variables;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie
{
    public class VariableRepository : Repository<Variable>, IVariableRepository
    {
        private readonly FidelizacionContext context;

        public VariableRepository(FidelizacionContext context) : base(context)
        {
            this.context = context;
        }

        public Task<Variable> GetVariablesByCodigo(string codigo)
        {
            var response = context.Set<Variable>().Where(x => x.Codigo == codigo).FirstOrDefault();
            return Task.FromResult(response);
        }
    }
}

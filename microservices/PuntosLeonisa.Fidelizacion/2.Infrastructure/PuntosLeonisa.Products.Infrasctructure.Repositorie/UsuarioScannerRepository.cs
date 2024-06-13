using PuntosLeonisa.Fidelizacion.Domain.Model;
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
    public class UsuarioScannerRepository : Repository<UsuarioScanner>, IUsuarioScannerRepository
    {
        private readonly FidelizacionContext context;

        public UsuarioScannerRepository(FidelizacionContext context) : base(context)
        {
            this.context = context;
        }
    }
}

using PuntosLeonisa.Infrasctructure.Core.Repository;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Fidelizacion.Domain.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Model;

namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie;
public class PuntosManualRepository : Repository<PuntosManual>, IPuntosManualRepository
{
    public PuntosManualRepository(FidelizacionContext context) : base(context)
    {
    }
}


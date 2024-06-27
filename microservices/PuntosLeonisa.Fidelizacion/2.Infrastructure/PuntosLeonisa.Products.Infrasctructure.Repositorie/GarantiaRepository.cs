using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;

namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie
{
    public class GarantiaRepository : Repository<Garantia>, IGarantiaRepository
    {
        private readonly FidelizacionContext context;
        public GarantiaRepository(FidelizacionContext context) : base(context)
        {
            this.context = context;
        }

        public Task<IEnumerable<Garantia>> GetGarantiaByEmail(string email)
        {
            return Task.FromResult(this.context.Set<Garantia>().Where(x => x.Email == email).AsEnumerable());
            
        }

        public Task<IEnumerable<Garantia>> GetGarantiaByProveedorOrAll(string proveedor)
        {
            //si el proveedor es nulo o vacio, se retornan todas las garantias
            if (proveedor != "0")
            {
                return Task.FromResult(this.context.Set<Garantia>().Where(x => x.Proveedor == proveedor).AsEnumerable());
                
            }
            else
            {
                return Task.FromResult(this.context.Set<Garantia>().AsEnumerable());
            }
            
        }

        public int GetNroGarantia()
        {
            return this.context.Set<Garantia>().Count();
        }
    }
}

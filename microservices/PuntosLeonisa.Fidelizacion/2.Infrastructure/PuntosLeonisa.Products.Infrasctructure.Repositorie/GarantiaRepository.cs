using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Usuarios;
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

        public async Task<IEnumerable<Garantia>> GetGarantiaByProveedorOrAll(TipoUsuarioDto[] data)
        {
            var garantias = new HashSet<Garantia>();

            foreach (var item in data)
            {
                if (item.TipoUsuario == "0" && item.Proveedor == "0")
                {
                    var todasGarantias = await context.Set<Garantia>().ToListAsync();
                    garantias.UnionWith(todasGarantias);
                }
                if (item.TipoUsuario == "0" && item.Proveedor != "0")
                {
                    var usuariosRedencion = await context.Set<Garantia>().Where(x => x.Proveedor == item.Proveedor).ToListAsync();

                    garantias.UnionWith(usuariosRedencion);
                }
                if (item.TipoUsuario != "0" && item.Proveedor == "0")
                {
                    var productosPorTipoUsuario = await context.Set<Garantia>()
                                                                .Where(x => x.TipoUsuario == item.TipoUsuario)
                                                                .ToListAsync();
                    garantias.UnionWith(productosPorTipoUsuario);
                }
            }

            return garantias.ToList();
        }

        public int GetNroGarantia()
        {
            return this.context.Set<Garantia>().Count();
        }
    }
}

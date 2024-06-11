using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Cosmos.Storage.Internal;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using System.Linq;
using System.Runtime.InteropServices;

namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie
{
    public class UsuarioRedencionRepository : Repository<UsuarioRedencion>, IUsuarioRedencionRepository
    {
        private readonly FidelizacionContext context;

        public UsuarioRedencionRepository(FidelizacionContext context) : base(context)
        {
            this.context = context;
        }

        public int GetNroPedido()
        {
            return this.context.Set<UsuarioRedencion>().Count();
        }

        public IEnumerable<UsuarioRedencion> GetRedencionesWithProductsByEmail(string email)
        {
            var data = this.context.Set<UsuarioRedencion>().Where(x => x.Usuario.Email == email).ToList();
            return data;
        }

        public IEnumerable<UsuarioRedencion> GetRedencionesWithProductsByProveedor(string proveedor)
        {
            // Asegúrate de que el valor del proveedor esté limpio y sea seguro para evitar inyecciones SQL
            // Por ejemplo, reemplaza comillas simples para prevenir inyecciones
            var proveedorSeguro = proveedor.Replace("'", "''");

            // Construye tu consulta SQL con el valor del proveedor insertado directamente
            string sqlQuery = $"SELECT * FROM c WHERE EXISTS(SELECT VALUE p FROM p IN c.ProductosCarrito WHERE p.ProveedorLite.Nombres = '{proveedorSeguro}')";

            // Ejecuta la consulta
            var results = context.Set<UsuarioRedencion>()
                                 .FromSqlRaw(sqlQuery)
                                 .ToList();

            return results;
        }

        public IEnumerable<UsuarioRedencion> GetReporteRedencion(ReporteDto data)
        {
            if (data.TipoUsuario != "" && data.FechaFin != null && data.FechaInicio != null && data.Proveedor != "")
            {
                data.FechaFin = new DateTime(data.FechaFin.Value.Year, data.FechaFin.Value.Month, data.FechaFin.Value.Day, 23, 59, 59);
                var red = this.context.Set<UsuarioRedencion>()
                    .Where(x => x.Usuario.TipoUsuario == data.TipoUsuario
                                && x.FechaRedencion >= data.FechaInicio
                                && x.FechaRedencion <= data.FechaFin)
                    .AsEnumerable()
                    .Where(x => x.ProductosCarrito != null && x.ProductosCarrito.Any(p => p != null && p.ProveedorLite != null && p.ProveedorLite.Nit == data.Proveedor))
                    .ToList();
                return red;
            }
            if (data.FechaFin != null && data.FechaInicio != null && data.TipoUsuario != "")
            {
                data.FechaFin = new DateTime(data.FechaFin.Value.Year, data.FechaFin.Value.Month, data.FechaFin.Value.Day, 23, 59, 59);
                var redencion = this.context.Set<UsuarioRedencion>()
                    .Where(x => x.FechaRedencion >= data.FechaInicio
                                && x.FechaRedencion <= data.FechaFin
                                && x.Usuario.TipoUsuario == data.TipoUsuario)
                    .ToList();
                return redencion;
            }
            if (data.TipoUsuario != "" && data.Proveedor != "")
            {
                var redencion = this.context.Set<UsuarioRedencion>()
                    .Where(x => x.Usuario.TipoUsuario == data.TipoUsuario)
                    .AsEnumerable()
                    .Where(x => x.ProductosCarrito != null && x.ProductosCarrito.Any(p => p != null && p.ProveedorLite != null && p.ProveedorLite.Nit == data.Proveedor))
                    .ToList();
                return redencion;
            }
            if(data.Proveedor != "" && data.FechaFin != null && data.FechaInicio != null)
            {
                data.FechaFin = new DateTime(data.FechaFin.Value.Year, data.FechaFin.Value.Month, data.FechaFin.Value.Day, 23, 59, 59);
                var redencion = this.context.Set<UsuarioRedencion>()
                    .Where(x => x.FechaRedencion >= data.FechaInicio
                                && x.FechaRedencion <= data.FechaFin)
                    .AsEnumerable()
                    .Where(x => x.ProductosCarrito != null && x.ProductosCarrito.Any(p => p != null && p.ProveedorLite != null && p.ProveedorLite.Nit == data.Proveedor))
                    .ToList();
                return redencion;
            }
            if (data.FechaFin != null && data.FechaInicio != null)
            {
                data.FechaFin = new DateTime(data.FechaFin.Value.Year, data.FechaFin.Value.Month, data.FechaFin.Value.Day, 23, 59, 59);
                var redencion = this.context.Set<UsuarioRedencion>()
                    .Where(x => x.FechaRedencion >= data.FechaInicio
                                && x.FechaRedencion <= data.FechaFin)
                    .ToList();
                return redencion;
            }
            if (data.TipoUsuario != "")
            {
                var redencion = this.context.Set<UsuarioRedencion>()
                    .Where(x => x.Usuario.TipoUsuario == data.TipoUsuario)
                    .ToList();
                return redencion;
            }
            if (!string.IsNullOrEmpty(data.Proveedor))
            {
                var redencion = this.context.Set<UsuarioRedencion>()
                    .AsEnumerable()
                    .Where(x => x.ProductosCarrito != null && x.ProductosCarrito.Any(p => p != null && p.ProveedorLite != null && p.ProveedorLite.Nit == data.Proveedor))
                    .ToList();
                return redencion;
            }
            else
            {
                var redencion = this.context.Set<UsuarioRedencion>().ToList();
                return redencion;
            }
        }

        public async Task<UsuarioRedencion> GetUsuarioRedencionByNroPedido(int data)
        {
            var redencion = await this.context.Set<UsuarioRedencion>().Where(x => x.NroPedido == data).FirstOrDefaultAsync();
            return redencion;
        }                              
    }
}

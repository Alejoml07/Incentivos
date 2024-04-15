using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Cosmos.Storage.Internal;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
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
            data.FechaFin = new DateTime(data.FechaFin.Value.Year, data.FechaFin.Value.Month, data.FechaFin.Value.Day, 23, 59, 59);
            var redencion = this.context.Set<UsuarioRedencion>().Where(x => x.FechaRedencion >= data.FechaInicio && x.FechaRedencion <= data.FechaFin).ToList();
            return redencion;
        }
    }
}

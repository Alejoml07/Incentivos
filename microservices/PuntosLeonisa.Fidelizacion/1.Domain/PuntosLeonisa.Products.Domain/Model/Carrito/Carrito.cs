using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Products.Domain.Model;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Carrito
{
    public class Carrito
    {
        public string Id { get; set; }

        public Usuario User { get; set; }

        public ProductoRefence Product { get; set; }

    }
}

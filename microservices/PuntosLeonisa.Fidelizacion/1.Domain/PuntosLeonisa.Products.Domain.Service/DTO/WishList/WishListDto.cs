using PuntosLeonisa.fidelizacion.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Usuarios;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.WishList
{
    public class WishListDto
    {
        public string Id { get; set; }

        public UsuarioDto User { get; set; }

        public ProductLiteDto Product { get; set; }
    }
}

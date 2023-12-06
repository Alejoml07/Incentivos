using PuntosLeonisa.fidelizacion.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Carrito
{
    public class CarritoDto
    {
        public string Id { get; set; }

        public UsuarioDto User { get; set; }

        public ProductLiteDto Product { get; set; }

        public string Cantidad { get; set; }
    }
}

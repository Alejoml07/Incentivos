using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Products.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.MovimientoPuntos
{
    public class MovimientoPuntosDto
    {
        public string? Id { get; set; }

        public UsuarioInfoPuntos? Usuario { get; set; }

        public DateTime? Fecha { get; set; }

        public string? Puntos { get; set; }
    }
}

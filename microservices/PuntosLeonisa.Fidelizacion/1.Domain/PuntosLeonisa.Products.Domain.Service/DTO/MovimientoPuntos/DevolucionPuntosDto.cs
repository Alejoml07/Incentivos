using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.MovimientoPuntos
{
    public class DevolucionPuntosDto
    {
        public string? Id { get; set; }

        public int? PuntosADevolver { get; set; }

        public string? Email { get; set; }

        public ProductoRefence? Producto { get; set; }

    }
}

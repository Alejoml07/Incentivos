using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Products.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.FidelizacionPuntos
{
    public class InfoPuntosDto
    {
        public Usuario Usuario { get; set; }

        public UsuarioInfoPuntos Puntos { get; set; }
    }
}

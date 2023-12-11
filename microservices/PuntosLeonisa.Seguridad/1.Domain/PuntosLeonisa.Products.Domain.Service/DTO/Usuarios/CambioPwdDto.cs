using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios
{
    public class CambioPwdDto
    {
        public string Email { get; set; }
        public string ContraseñaActual { get; set; }
        public string NuevaContraseña { get; set; }
        public string? ConfirmarNuevaContraseña { get; set; }
    }
}

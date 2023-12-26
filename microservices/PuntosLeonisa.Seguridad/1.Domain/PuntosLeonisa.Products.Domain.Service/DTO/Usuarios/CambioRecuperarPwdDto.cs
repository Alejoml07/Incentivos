using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios
{
    public class CambioRecuperarPwdDto
    {
        public string Email { get; set; }
        public string NuevaContrasena { get; set; }
        public string ConfirmarNuevaContrasena { get; set; }
    }
}

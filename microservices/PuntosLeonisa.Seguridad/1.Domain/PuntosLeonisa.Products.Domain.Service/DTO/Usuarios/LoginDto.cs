using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios
{
    public class LoginDto
    {
        public string Email { get; set; }

        public string? Pwd { get; set; }
    }
}

using PuntosLeonisa.Products.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios
{
    public class TokenDto
    {
        public string? Id { get; set; }
        public string? Token { get; set; }
        public string? Email { get; set; }
        public string? Tipo { get; set; }
        public UsuarioDto? Usuario { get; set; }
    }
}

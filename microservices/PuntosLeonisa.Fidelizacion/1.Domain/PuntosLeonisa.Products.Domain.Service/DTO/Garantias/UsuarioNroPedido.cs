using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Garantias
{
    public class UsuarioNroPedido
    {
        public int NroPedido { get; set; }
        public string Email { get; set; }
        public string? TipoUsuario { get; set; }
    }
}

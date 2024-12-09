using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Scanner
{
    public class PeticionCedulaDto
    {
        public string? codigoPais { get; set; }
        public string? nombre { get; set; }
        public string? correo { get; set; }
        public long? identificacion { get; set; }
        public long? telefono { get; set; }
    }
}

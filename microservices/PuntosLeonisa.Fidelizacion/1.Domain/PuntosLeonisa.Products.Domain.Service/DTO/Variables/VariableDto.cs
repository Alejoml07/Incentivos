using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Variables
{
    public class VariableDto
    {
        
        public string? Id { get; set; }
        public string? Nombre { get; set; }
        public string? Base { get; set; }
        public string? Status { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string? Eliminado { get; set; }
        public string? Codigo { get; set; }
    
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Products.Domain.Service.DTO.GestorDeContenido
{
    public class GestorDeContenidoDto
    {
        public string? Id { get; set; }
        public FileInfo Banner { get; set; }
        public string Archivo { get; set; }
    }
}

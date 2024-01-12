using PuntosLeonisa.Fidelizacion.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion
{
    public class AddNroGuiaYTransportadora
    {
        public string Id { get; set; }
        public ProductoRefence Producto { get; set; }
    }
}

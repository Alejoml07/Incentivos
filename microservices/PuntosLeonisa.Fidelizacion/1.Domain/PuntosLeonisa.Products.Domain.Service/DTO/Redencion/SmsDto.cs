using PuntosLeonisa.Products.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion
{
    public class SmsDto
    {
        public string Id { get; set; }
        public Usuario Usuario { get; set; }
        public string Codigo { get; set; }
    }
}

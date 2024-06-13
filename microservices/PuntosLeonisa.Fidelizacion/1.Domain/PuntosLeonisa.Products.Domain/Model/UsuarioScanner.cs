using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Model
{
    public class UsuarioScanner
    {
        public string? Id { get; set; }
        public string? mensajeError { get; set; }
        public string? codigoError { get; set; }
        public List<DataCompradora>? dataCompradoras { get; set; }
    }

    public class DataCompradora
    {
        public string? codigoPais { get; set; }
        public string? codigoPaisOP { get; set; }
        public long? codigoInternoDLM { get; set; }
        public long? identificacion { get; set; }
        public string? nombre { get; set; }
        public int? telefono1 { get; set; }
        public int? telefono2 { get; set; }
        public long? celular { get; set; }
        public string? correo { get; set; }
        public string? zona { get; set; }
        public int? codigoCanal { get; set; }
        public string? descripcionCanal { get; set; }
        public string? estado { get; set; }
    }
}

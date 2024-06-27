using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Garantias
{
    public class GarantiaDto
    {
        public string? Id { get; set; }
        public int? NroTicket { get; set; }
        public DateTime? FechaReclamacion { get; set; }
        public int? NroPedido { get; set; }
        public string? Proveedor { get; set; }
        public string? Producto { get; set; }
        public string? Observacion { get; set; }
        public string? Imagen1 { get; set; }
        public string? Imagen2 { get; set; }
        public string? Imagen3 { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Celular { get; set; }
        public string? Email { get; set; }
        public string? Direccion { get; set; }
        public string? Departamento { get; set; }
        public string? Ciudad { get; set; }
        public string? Estado { get; set; }
        public string? ObservacionEstado { get; set; }
        public string? ObservacionProveedor { get; set; }
        public string? EAN { get; set; }
    }
}

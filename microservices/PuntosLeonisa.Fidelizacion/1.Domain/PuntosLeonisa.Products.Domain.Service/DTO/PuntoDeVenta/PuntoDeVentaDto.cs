namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.PuntoDeVenta
{
    public class PuntoDeVentaDto
    {
        public string? Id { get; set; }
        public string? Nombre { get; set; }
        public string? Codigo { get; set; }
        public string? Ciudad { get; set; }
        public string? Superficie { get; set; }
        public string? Agencia { get; set; }
        public string? Formato { get; set; }
        public string? Status { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public int? Eliminado { get; set; }
    }
}

namespace PuntosLeonisa.Fidelizacion.Domain.Model
{
    public class UsuarioInfoPuntos
    {
        public string? Cedula { get; set; }

        public string? Nombres { get; set; }

        public string? Apellidos { get; set; }

        public string? Email { get; set; }

        public int PuntosDisponibles { get; set; }
        public int PuntosEnCarrito { get; set; }
        public int PuntosRedimidos { get; set; }

        public int PuntosAcumulados { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}

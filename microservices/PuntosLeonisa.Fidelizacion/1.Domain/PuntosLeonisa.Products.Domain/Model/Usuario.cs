using Newtonsoft.Json;

namespace PuntosLeonisa.Products.Domain.Model
{
    public class Usuario : IDisposable, ICloneable
    {
        public string? Id { get; set; }

        public string? Cedula { get; set; }

        public string? Nombres { get; set; }

        public string? Apellidos { get; set; }

        public string? Genero { get; set; }

        public string? Email { get; set; }

        public string? Celular { get; set; }

        public string? TipoUsuario { get; set; }

        public string? Estado { get; set; }

        public string? FechaCambioEstado { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public string? Agencia { get; set; }

        public string? CentroCosto { get; set; }

        public string? Empresa { get; set; }

        public string? Contraseña { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {

            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}

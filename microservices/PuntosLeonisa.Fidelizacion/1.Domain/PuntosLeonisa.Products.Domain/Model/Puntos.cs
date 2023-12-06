using Newtonsoft.Json;
using PuntosLeonisa.Products.Domain.Model;

namespace PuntosLeonisa.Fidelizacion.Domain.Model
{
    public class PuntosManual : IDisposable
    {
        public string? Id { get; set; }

        public Usuario Usuario { get; set; }

        public string? Puntos { get; set; }

        public string? Month { get; set; }

        public string? Year { get; set; }

        public string? Observaciones { get; set; }

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

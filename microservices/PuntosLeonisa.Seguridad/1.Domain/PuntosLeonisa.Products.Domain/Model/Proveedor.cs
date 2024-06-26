using Newtonsoft.Json;


namespace PuntosLeonisa.Products.Domain.Model
{
    public class Proveedor : IDisposable
    {
        public string Id { get; set; }

        public string? Nit { get; set; }

        public string? Nombres { get; set; }

        public string? Email { get; set; }

        public double? Descuento { get; set; }
        public string? CiudadRestringida { get; set; }

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
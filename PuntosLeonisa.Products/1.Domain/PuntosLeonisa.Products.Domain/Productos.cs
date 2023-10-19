using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace PuntosLeonisa.Products.Domain;
public class Productos :IDisposable
{
    [JsonProperty(PropertyName = "id")]
    public string? Id { get; set; }
   
    public string? Referencia { get; set; }
  
    public string? Nombre { get; set; }
    
    public string? Video { get; set; }
    
    public string? Caracteristicas { get; set; }

    public string? Descripcion { get; set; }
  
    public float? Puntos { get; set; }

    public string? TiempoEntrega { get; set; }

    public int? Estado { get; set; }
 
    public DateTime? Fecha { get; set; }

    public string? ImagenPrincipal { get; set; }

    public string? Imagen1 { get; set; }

    public string? Imagen2 { get; set; }

    public string? Imagen3 { get; set; }

    public string? Proveedor { get; set; }

    public string? Correo { get; set; }

    public int? TipoPremio { get; set; }

    public int? Actualizado { get; set; }

    public string? UrlImagen { get; set; }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync()
    {
        throw new NotImplementedException();
    }

    public override string ToString()

    {
        return JsonConvert.SerializeObject(this);
    }

}


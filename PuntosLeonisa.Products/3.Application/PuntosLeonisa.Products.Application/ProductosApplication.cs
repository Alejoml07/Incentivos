
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Products.Domain;
using PuntosLeonisa.Products.Infrasctructure.Repositorie;

namespace PuntosLeonisa.Products.Application;

public class ProductosApplication
{

    public ProductosApplication()
    {

    }

    public async Task<IEnumerable<Producto>> GetAll()
    {
        
        var repository = new ProductoRepository(new CosmoDB());

        return  await repository.GetAll();
    }


    public async void GuardarProducto(Producto producto)
    {

        try
        {
            var repository = new ProductoRepository(new CosmoDB());

            //var producto1 = new Producto()
            //#region Inserting Products
            //{
            //    Id = Guid.NewGuid().ToString().Replace("-",""),
            //    Referencia = "Pl123",
            //    Nombre = "Planca",
            //    Video = "Melo",
            //    Caracteristicas = "Melisimo",
            //    Descripcion = "Calienta melo",
            //    Puntos = 1042,
            //    TiempoEntrega = "1 mes",
            //    Estado = 1,
            //    Fecha = new DateTime().ToShortDateString(),
            //    ImagenPrincipal = "Ya por favor",
            //    Imagen1 = "Para que",
            //    Imagen2 = "Tantas",
            //    Imagen3 = "Imagenes",
            //    Proveedor = "Rappi",
            //    Correo = "Yaporfavor@gmail.com",
            //    TipoPremio = 1,
            //    Actualizado = 2,
            //    UrlImagen = "Imagen.png"
            //};

            await repository.Add(producto);
            //#endregion
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
     


    }

}
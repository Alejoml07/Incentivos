
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
            await repository.Add(producto);
     
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
     


    }

}
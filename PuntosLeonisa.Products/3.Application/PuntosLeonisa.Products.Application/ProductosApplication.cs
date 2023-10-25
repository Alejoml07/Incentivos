
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Products.Domain;
using PuntosLeonisa.Products.Infrasctructure.Common;
using PuntosLeonisa.Products.Infrasctructure.Repositorie;

namespace PuntosLeonisa.Products.Application;

public class ProductosApplication
{

    public ProductosApplication()
    {

    }

    public async Task<Producto> GetById(string id)
    {
        var repository = new ProductoRepository(new CosmoDB());
        return await repository.GetById(id);
    }

    public async Task<Producto> Delete(string id)
    {
        var repository = new ProductoRepository(new CosmoDB());
        var ToDelete = await this.GetById(id);
        if (ToDelete == null)
        {
            throw new ArgumentException("Producto no encontrado");
        }

        await repository.Delete(ToDelete);
        return ToDelete;

    }

    public async Task<IEnumerable<Producto>> GetAll()
    {
        var repository = new ProductoRepository(new CosmoDB());
        return await repository.GetAll();
    }


    public async void GuardarProducto(Producto producto)
    {

        try
        {
            var repository = new ProductoRepository(new CosmoDB());
            var azureHelper = new AzureHelper("DefaultEndpointsProtocol=https;AccountName=stgactincentivos;AccountKey=mtBoBaUJu8BKcHuCfdWzk1au7Upgif0rlzD+BlfAJZBsvQ02CiGzCNG5gj1li10GF8RpUwz6h+Mj+AStMOwyTA==;EndpointSuffix=core.windows.net");
            //antes de guardar se debe subir la imagen

            if (!string.IsNullOrEmpty(producto.UrlImagen1))
            {
                byte[] bytes = Convert.FromBase64String(producto.UrlImagen1);
                producto.UrlImagen1 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
            }
            if (!string.IsNullOrEmpty(producto.UrlImagen2))
            {
                byte[] bytes = Convert.FromBase64String(producto.UrlImagen2);
                producto.UrlImagen2 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
            }
            if (!string.IsNullOrEmpty(producto.UrlImagen3))
            {
                byte[] bytes = Convert.FromBase64String(producto.UrlImagen3);
                producto.UrlImagen3 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
            }
            if (!string.IsNullOrEmpty(producto.UrlImagen4))
            {
                byte[] bytes = Convert.FromBase64String(producto.UrlImagen4);
                producto.UrlImagen4 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
            }
            if (!string.IsNullOrEmpty(producto.UrlImagen5))
            {
                byte[] bytes = Convert.FromBase64String(producto.UrlImagen5);
                producto.UrlImagen5 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
            }


            await repository.Add(producto);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }



    }

    public async void LoadProducts(Producto[] productos)
    {

        try
        {
            var repository = new ProductoRepository(new CosmoDB());
            foreach (var producto in productos)
            {
                producto.Id = Guid.NewGuid().ToString();
                await repository.Add(producto);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }
}
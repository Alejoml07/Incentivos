using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Products.Domain;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Products.Infrasctructure.Common;
using PuntosLeonisa.Products.Infrasctructure.Repositorie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Products.Application;
public class UsuariosApplication
{

    public UsuariosApplication()
    {

    }

    public async Task<Usuario> GetById(string id)
    {
        var repository = new UsuarioRepository(new CosmoDBContext());
        return await repository.GetById(id);
    }

    public async Task<Usuario> Delete(string id)
    {
        var repository = new UsuarioRepository(new CosmoDBContext());
        var ToDelete = await this.GetById(id);
        if (ToDelete == null)
        {
            throw new ArgumentException("Producto no encontrado");
        }

        await repository.Delete(ToDelete);
        return ToDelete;

    }

    public async Task<IEnumerable<Usuario>> GetAll()
    {
        var repository = new UsuarioRepository(new CosmoDBContext());
        return await repository.GetAll();
    }


    public async void GuardarUsuario(Usuario usuario)
    {

        try
        {
            var repository = new UsuarioRepository(new CosmoDBContext());
            if (!string.IsNullOrEmpty(usuario.Id))
            {
                await repository.Update(usuario);
                return;
            }
            usuario.Id = Guid.NewGuid().ToString();

            var azureHelper = new AzureHelper("DefaultEndpointsProtocol=https;AccountName=stgactincentivos;AccountKey=mtBoBaUJu8BKcHuCfdWzk1au7Upgif0rlzD+BlfAJZBsvQ02CiGzCNG5gj1li10GF8RpUwz6h+Mj+AStMOwyTA==;EndpointSuffix=core.windows.net");
            //antes de guardar se debe subir la imagen

            await repository.Add(usuario);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }



    }

    public async void LoadUsers(Usuario[] usuarios)
    {

        try
        {
            var repository = new UsuarioRepository(new CosmoDBContext());
            foreach (var usuario in usuarios)
            {
                usuario.Id = Guid.NewGuid().ToString();
                await repository.Add(usuario);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }
}
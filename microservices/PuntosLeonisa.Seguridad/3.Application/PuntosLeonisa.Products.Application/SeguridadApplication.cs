
using AutoMapper;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Seguridad.Application.Core;
using PuntosLeonisa.Seguridad.Domain.Interfaces;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.Communication;

namespace PuntosLeonisa.Seguridad.Application;

public class SeguridadApplication : IUsuarioApplication
{
    private readonly IMapper mapper;
    private readonly IUsuarioRepository usuarioRepository;
    private readonly GenericResponse<UsuarioDto> response;

    public SeguridadApplication(IMapper mapper, IUsuarioRepository usuarioRepository)
    {
        if (usuarioRepository is null)
        {
            throw new ArgumentNullException(nameof(usuarioRepository));
        }

        this.mapper = mapper;
        this.usuarioRepository = usuarioRepository;
        response = new GenericResponse<UsuarioDto>();
    }

    public async Task<GenericResponse<UsuarioDto>> Add(UsuarioDto value)
    {
        try
        {
            //TODO: Hacer las validaciones
            var usuario = mapper.Map<Usuario>(value);
            await usuarioRepository.Add(usuario);
            response.Result = value;
            return response;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    

    public async Task<GenericResponse<UsuarioDto[]>> AddRange(UsuarioDto[] value)
    {
        try
        {
            var usuarios = mapper.Map<Usuario[]>(value);

            foreach (var usuario in usuarios)
            {
                usuario.Id = Guid.NewGuid().ToString();
            }

            await usuarioRepository.AddRange(usuarios);
            var responseOnly = new GenericResponse<UsuarioDto[]>
            {
                Result = value
            };

            return responseOnly;

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public Task<GenericResponse<UsuarioDto>> Delete(UsuarioDto value)
    {
        throw new NotImplementedException();
    }

    public async Task<GenericResponse<UsuarioDto>> DeleteById(string id)
    {
        try
        {
            var ToDelete = await GetById(id) ?? throw new ArgumentException("Usuario no encontrado");
            var usuarioToDelete = mapper.Map<Usuario>(ToDelete.Result);
            await usuarioRepository.Delete(usuarioToDelete);

            return ToDelete;
        }
        catch (Exception)
        {
            throw;
        }

    }

    public async Task<GenericResponse<IEnumerable<UsuarioDto>>> GetAll()
    {
        var usuarios = await usuarioRepository.GetAll();
        var usuarioDto = mapper.Map<UsuarioDto[]>(usuarios);
        var responseOnly = new GenericResponse<IEnumerable<UsuarioDto>>
        {
            Result = usuarioDto
        };

        return responseOnly;
    }

    public async Task<GenericResponse<UsuarioDto>> GetById(string id)
    {
        var responseRawData = await usuarioRepository.GetById(id);
        var responseData = mapper.Map<UsuarioDto>(responseRawData);
        response.Result = responseData;

        return response;
    }

    public async Task<GenericResponse<UsuarioDto>> Update(UsuarioDto value)
    {
        try
        {
            var response = await usuarioRepository.GetById(value.Cedula ?? "");
            if (response != null)
            {
                mapper.Map(value, response);
                await usuarioRepository.Update(response);
            }
            this.response.Result = value;
            return this.response;
        }
        catch (Exception)
        {
            throw;
        }
    }


}
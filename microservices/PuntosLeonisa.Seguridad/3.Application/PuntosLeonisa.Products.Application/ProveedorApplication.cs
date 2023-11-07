using AutoMapper;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Seguridad.Application.Core;
using PuntosLeonisa.Seguridad.Domain.Interfaces;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.Communication;

namespace PuntosLeonisa.Seguridad.Application;

public class ProveedorApplication : IProveedorApplication
{
    private readonly IMapper mapper;
    private readonly IProveedorRepository proveedorRepository;
    private readonly GenericResponse<ProveedorDto> response;

    public ProveedorApplication(IMapper mapper, IProveedorRepository proveedorRepository)
    {
        if (proveedorRepository is null)
        {
            throw new ArgumentNullException(nameof(proveedorRepository));
        }

        this.mapper = mapper;
        this.proveedorRepository = proveedorRepository;
        response = new GenericResponse<ProveedorDto>();
    }

    public async Task<GenericResponse<ProveedorDto>> Add(ProveedorDto value)
    {
        try
        {
            var proveedorExist = await this.proveedorRepository.GetById(value.Nit ?? "");
            if (proveedorExist != null)
            {
                mapper.Map(value, proveedorExist);
                await proveedorRepository.Update(proveedorExist);

            }
            else
            {

                //TODO: Hacer las validaciones
                var proveedor = mapper.Map<Proveedor>(value);
                proveedor.Id = Guid.NewGuid().ToString();
                await proveedorRepository.Add(proveedor);
            }
            response.Result = value;

            return response;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }



    public async Task<GenericResponse<ProveedorDto[]>> AddRange(ProveedorDto[] value)
    {
        try
        {
            var proveedores = mapper.Map<Proveedor[]>(value);

            foreach (var proveedor in proveedores)
            {
                proveedor.Id = Guid.NewGuid().ToString();
            }

            await proveedorRepository.AddRange(proveedores);
            var responseOnly = new GenericResponse<ProveedorDto[]>
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

    public Task<GenericResponse<ProveedorDto>> Delete(ProveedorDto value)
    {
        throw new NotImplementedException();
    }

    public async Task<GenericResponse<ProveedorDto>> DeleteById(string id)
    {
        try
        {
            var ToDelete = await this.proveedorRepository.GetById(id) ?? throw new ArgumentException("Usuario no encontrado");

            await proveedorRepository.Delete(ToDelete);
            response.Result = mapper.Map<ProveedorDto>(ToDelete);
            return response;
        }
        catch (Exception)
        {
            throw;
        }

    }

    public async Task<GenericResponse<IEnumerable<ProveedorDto>>> GetAll()
    {
        var proveedores = await proveedorRepository.GetAll();
        var proveedorDto = mapper.Map<ProveedorDto[]>(proveedores);
        var responseOnly = new GenericResponse<IEnumerable<ProveedorDto>>
        {
            Result = proveedorDto
        };

        return responseOnly;
    }

    public async Task<GenericResponse<ProveedorDto>> GetById(string id)
    {
        var responseRawData = await proveedorRepository.GetById(id);
        var responseData = mapper.Map<ProveedorDto>(responseRawData);
        response.Result = responseData;

        return response;
    }

    public async Task<GenericResponse<ProveedorDto>> Update(ProveedorDto value)
    {
        try
        {
            var response = await proveedorRepository.GetById(value.Nit ?? "");
            if (response != null)
            {
                mapper.Map(value, response);
                await proveedorRepository.Update(response);
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

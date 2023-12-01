
using AutoMapper;
using Newtonsoft.Json.Linq;
using PuntosLeonisa.fidelizacion.Domain.Service.DTO.PuntosManuales;
using PuntosLeonisa.Fidelizacion.Domain.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using PuntosLeonisa.Seguridad.Application.Core;

namespace PuntosLeonisa.Fidelizacion.Application;

public class FidelizacionApplication : IPuntosManualApplication
{
    private readonly IMapper mapper;
    private readonly IPuntosManualRepository puntosRepository;
    private readonly GenericResponse<PuntosManualDto> response;
    private readonly IWishListRepository wishRepository;
    private readonly GenericResponse<WishListDto> response2;
    public FidelizacionApplication(IMapper mapper, IPuntosManualRepository puntosRepository, IWishListRepository wishRepository)
    {
        if (puntosRepository is null)
        {
            throw new ArgumentNullException(nameof(puntosRepository));
        }

        if(wishRepository is null)
        {
            throw new ArgumentNullException(nameof(wishRepository));
        }

        this.mapper = mapper;
        this.puntosRepository = puntosRepository;
        this.wishRepository = wishRepository;
        response2 = new GenericResponse<WishListDto>();
        response = new GenericResponse<PuntosManualDto>();
    }

    public async Task<GenericResponse<PuntosManualDto>> Add(PuntosManualDto value)
    {
        try
        {
            //TODO: Hacer las validaciones
            var puntos = mapper.Map<PuntosManual>(value);
            await puntosRepository.Add(puntos);
            response.Result = value;
            return response;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    

    public async Task<GenericResponse<PuntosManualDto[]>> AddRange(PuntosManualDto[] value)
    {
        try
        {
            var puntos = mapper.Map<PuntosManual[]>(value);

            foreach (var punto in puntos)
            {
                punto.Id = Guid.NewGuid().ToString();
            }

            await puntosRepository.AddRange(puntos);
            var responseOnly = new GenericResponse<PuntosManualDto[]>
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

    public Task<GenericResponse<PuntosManualDto>> Delete(PuntosManualDto value)
    {
        throw new NotImplementedException();
    }

    public async Task<GenericResponse<PuntosManualDto>> DeleteById(string id)
    {
        try
        {
            var ToDelete = await this.puntosRepository.GetById(id) ?? throw new ArgumentException("Puntos no encontrados");
            var puntosToDelete = mapper.Map<PuntosManualDto>(ToDelete);

            await puntosRepository.Delete(ToDelete);
            this.response.Result = puntosToDelete;
            return this.response;
        }
        catch (Exception)
        {
            throw;
        }

    }

    public async Task<GenericResponse<IEnumerable<PuntosManualDto>>> GetAll()
    {
        var puntos = await puntosRepository.GetAll();
        var puntosDto = mapper.Map<PuntosManualDto[]>(puntos);
        var responseOnly = new GenericResponse<IEnumerable<PuntosManualDto>>
        {
            Result = puntosDto
        };

        return responseOnly;
    }

    public async Task<GenericResponse<PuntosManualDto>> GetById(string id)
    {
        var responseRawData = await puntosRepository.GetById(id);
        var responseData = mapper.Map<PuntosManualDto>(responseRawData);
        response.Result = responseData;

        return response;
    }

    public async Task<GenericResponse<PuntosManualDto>> Update(PuntosManualDto value)
    {
        try
        {
            var response = await puntosRepository.GetById(value.Id ?? "");
            if (response != null)
            {
                mapper.Map(value, response);
                await puntosRepository.Update(response);
            }
            this.response.Result = value;
            return this.response;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<WishListDto>> WishList(WishListDto wishList)
    {
        var wish = await wishRepository.WishList(wishList);
        if(wishList != null)
        {
            wishList.Id = Guid.NewGuid().ToString();
            this.response2.Result = wishList;
        }
        return wish;
    }
}
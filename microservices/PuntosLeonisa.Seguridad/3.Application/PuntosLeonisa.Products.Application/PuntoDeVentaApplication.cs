using AutoMapper;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using PuntosLeonisa.Products.Application.Core.Interfaces;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Seguridad.Application.Core;
using PuntosLeonisa.Seguridad.Domain.Interfaces;
using PuntosLeonisa.Seguridad.Domain.Model;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.PuntosDeVenta;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.Communication;
using PuntosLeonisa.Seguridad.Infrasctructure.Repositorie;

namespace PuntosLeonisa.Seguridad.Application
{
    public class PuntoDeVentaApplication : IPuntoDeVentaApplication
    {
        private readonly IMapper mapper;
        private readonly IPuntoDeVentaRepository puntoDeVentaRepository;
        private readonly GenericResponse<PuntoDeVentaDto> response;

        public PuntoDeVentaApplication(IMapper mapper, IPuntoDeVentaRepository puntoDeVentaRepository)
        {
            if (puntoDeVentaRepository is null)
            {
                throw new ArgumentNullException(nameof(puntoDeVentaRepository));
            }

            this.mapper = mapper;
            this.puntoDeVentaRepository = puntoDeVentaRepository;
            response = new GenericResponse<PuntoDeVentaDto>();
        }

        public async Task<GenericResponse<PuntoDeVentaDto>> Add(PuntoDeVentaDto value)
        {
            try
            {
                var puntoDeVenta = await this.puntoDeVentaRepository.GetById(value.Id ?? "");
                if (puntoDeVenta != null)
                {
                    //agregar mapping de datos de punto de venta
                    mapper.Map(value, puntoDeVenta);
                    await this.puntoDeVentaRepository.Update(puntoDeVenta);
                    return response;
                }
                else
                {
                    var punto = this.mapper.Map<PuntoDeVenta>(value);
                    await this.puntoDeVentaRepository.Add(punto);
                    return response;
                }
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task<GenericResponse<PuntoDeVentaDto[]>> AddRange(PuntoDeVentaDto[] value)
        {
            try
            {

                foreach (var item in value)
                {
                    await this.puntoDeVentaRepository.Add(mapper.Map<PuntoDeVenta>(item));

                }
                return new GenericResponse<PuntoDeVentaDto[]>
                {
                    IsSuccess = true,
                    Message = "Puntos de venta añadidos correctamente",
                    Result = value
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<GenericResponse<PuntoDeVentaDto>> Delete(PuntoDeVentaDto value)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResponse<PuntoDeVentaDto>> DeleteById(string id)
        {
            try
            {
                var ToDelete = await this.puntoDeVentaRepository.GetById(id) ?? throw new ArgumentException("Usuario no encontrado");

                await puntoDeVentaRepository.Delete(ToDelete);
                response.Result = mapper.Map<PuntoDeVentaDto>(ToDelete);
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<GenericResponse<IEnumerable<PuntoDeVentaDto>>> GetAll()
        {
            var puntos = await puntoDeVentaRepository.GetAll();
            var PuntoDto = mapper.Map<PuntoDeVentaDto[]>(puntos);
            var responseOnly = new GenericResponse<IEnumerable<PuntoDeVentaDto>>
            {
                Result = PuntoDto
            };

            return responseOnly;

        }

        public async Task<GenericResponse<PuntoDeVentaDto>> GetById(string id)
        {
            try
            {
                var response = await this.puntoDeVentaRepository.GetById(id);
                var responseDto = this.mapper.Map<PuntoDeVentaDto>(response);
                if (response != null)
                {
                    return new GenericResponse<PuntoDeVentaDto>
                    {
                        IsSuccess = true,
                        Message = "Punto de venta encontrado",
                        Result = responseDto
                    };
                }
                else
                {
                    return new GenericResponse<PuntoDeVentaDto>
                    {
                        IsSuccess = false,
                        Message = "Punto de venta no encontrado",
                        Result = null
                    };
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public Task<GenericResponse<PuntoDeVentaDto>> Update(PuntoDeVentaDto value)
        {
            throw new NotImplementedException();
        }
    }
}

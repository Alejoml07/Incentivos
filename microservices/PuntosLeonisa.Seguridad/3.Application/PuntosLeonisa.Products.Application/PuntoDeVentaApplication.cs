using AutoMapper;
using PuntosLeonisa.Seguridad.Application.Core;
using PuntosLeonisa.Seguridad.Domain.Interfaces;
using PuntosLeonisa.Seguridad.Domain.Model;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.PuntosDeVenta;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.Communication;

namespace PuntosLeonisa.Seguridad.Application
{
    public class PuntoDeVentaApplication : IPuntoDeVentaApplication
    {
        private readonly IMapper mapper;
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IPuntoDeVentaRepository puntoDeVentaRepository;
        private readonly GenericResponse<PuntoDeVentaDto> response;

        public PuntoDeVentaApplication(IMapper mapper, IUsuarioRepository usuarioRepository, IPuntoDeVentaRepository puntoDeVentaRepository)
        {
            if (puntoDeVentaRepository is null)
            {
                throw new ArgumentNullException(nameof(puntoDeVentaRepository));
            }

            this.mapper = mapper;
            this.usuarioRepository = usuarioRepository;
            this.puntoDeVentaRepository = puntoDeVentaRepository;
            response = new GenericResponse<PuntoDeVentaDto>();
        }

        public async Task<GenericResponse<PuntoDeVentaDto>> Add(PuntoDeVentaDto value)
        {
            try
            {
                var puntoDeVenta = this.puntoDeVentaRepository.GetById(value.Id);
                if (puntoDeVenta != null)
                {
                    await this.puntoDeVentaRepository.Update(value);
                    return response;
                }
                else
                {
                    value.Id = Guid.NewGuid().ToString();
                    await this.puntoDeVentaRepository.Add(value);
                    return response;
                }
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public Task<GenericResponse<PuntoDeVentaDto[]>> AddRange(PuntoDeVentaDto[] value)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResponse<PuntoDeVentaDto>> Delete(PuntoDeVentaDto value)
        {
            try
            {
                var response = await this.puntoDeVentaRepository.GetById(value.Id);
                if (response != null)
                {
                    await this.puntoDeVentaRepository.Delete(value);
                    return new GenericResponse<PuntoDeVentaDto>
                    {
                        IsSuccess = true,
                        Message = "Punto de venta eliminado correctamente"
                    };
                }
                else
                {
                    return new GenericResponse<PuntoDeVentaDto>
                    {
                        IsSuccess = false,
                        Message = "El punto de venta no existe"
                    };
                }
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public Task<GenericResponse<PuntoDeVentaDto>> DeleteById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResponse<IEnumerable<PuntoDeVentaDto>>> GetAll()
        {
            try
            {
                var response = await this.puntoDeVentaRepository.GetAll();
                if(response != null)
                {
                    return new GenericResponse<IEnumerable<PuntoDeVentaDto>>
                    {
                        IsSuccess = true,
                        Message = "Puntos de venta encontrados",
                        Result = response
                    };
                }
                else
                {
                    return new GenericResponse<IEnumerable<PuntoDeVentaDto>>
                    {
                        IsSuccess = false,
                        Message = "No se encontraron puntos de venta",
                        Result = null

                    };
                }
                
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task<GenericResponse<PuntoDeVentaDto>> GetById(string id)
        {
            try
            {
                var response = await this.puntoDeVentaRepository.GetById(id);
                if (response != null)
                {
                    return new GenericResponse<PuntoDeVentaDto>
                    {
                        IsSuccess = true,
                        Message = "Punto de venta encontrado",
                        Result = response
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

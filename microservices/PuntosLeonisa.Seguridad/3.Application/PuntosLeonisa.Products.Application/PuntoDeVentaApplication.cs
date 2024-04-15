using AutoMapper;
using PuntosLeonisa.Seguridad.Application.Core;
using PuntosLeonisa.Seguridad.Domain.Interfaces;
using PuntosLeonisa.Seguridad.Domain.Model;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.Communication;

namespace PuntosLeonisa.Seguridad.Application
{
    public class PuntoDeVentaApplication : IPuntoDeVentaApplication
    {
        private readonly IMapper mapper;
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IPuntoDeVentaRepository puntoDeVentaRepository;
        private readonly GenericResponse<PuntoDeVenta> response;

        public PuntoDeVentaApplication(IMapper mapper, IUsuarioRepository usuarioRepository, IPuntoDeVentaRepository puntoDeVentaRepository)
        {
            if (puntoDeVentaRepository is null)
            {
                throw new ArgumentNullException(nameof(puntoDeVentaRepository));
            }

            this.mapper = mapper;
            this.usuarioRepository = usuarioRepository;
            this.puntoDeVentaRepository = puntoDeVentaRepository;
            response = new GenericResponse<PuntoDeVenta>();
        }

        public async Task<GenericResponse<PuntoDeVenta>> Add(PuntoDeVenta value)
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

        public Task<GenericResponse<PuntoDeVenta[]>> AddRange(PuntoDeVenta[] value)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResponse<PuntoDeVenta>> Delete(PuntoDeVenta value)
        {
            try
            {
                var response = await this.puntoDeVentaRepository.GetById(value.Id);
                if (response != null)
                {
                    await this.puntoDeVentaRepository.Delete(value);
                    return new GenericResponse<PuntoDeVenta>
                    {
                        IsSuccess = true,
                        Message = "Punto de venta eliminado correctamente"
                    };
                }
                else
                {
                    return new GenericResponse<PuntoDeVenta>
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

        public Task<GenericResponse<PuntoDeVenta>> DeleteById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResponse<IEnumerable<PuntoDeVenta>>> GetAll()
        {
            try
            {
                var response = await this.puntoDeVentaRepository.GetAll();
                if(response != null)
                {
                    return new GenericResponse<IEnumerable<PuntoDeVenta>>
                    {
                        IsSuccess = true,
                        Message = "Puntos de venta encontrados",
                        Result = response
                    };
                }
                else
                {
                    return new GenericResponse<IEnumerable<PuntoDeVenta>>
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

        public async Task<GenericResponse<PuntoDeVenta>> GetById(string id)
        {
            try
            {
                var response = await this.puntoDeVentaRepository.GetById(id);
                if (response != null)
                {
                    return new GenericResponse<PuntoDeVenta>
                    {
                        IsSuccess = true,
                        Message = "Punto de venta encontrado",
                        Result = response
                    };
                }
                else
                {
                    return new GenericResponse<PuntoDeVenta>
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

        public Task<GenericResponse<PuntoDeVenta>> Update(PuntoDeVenta value)
        {
            throw new NotImplementedException();
        }
    }
}

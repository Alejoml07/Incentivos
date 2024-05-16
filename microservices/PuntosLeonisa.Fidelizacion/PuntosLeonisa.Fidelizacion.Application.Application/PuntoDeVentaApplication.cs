using AutoMapper;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Model.Carrito;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.PuntoDeVenta;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Service.UnitOfWork;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using PuntosLeonisa.Infrasctructure.Core.ExternaServiceInterfaces;
using PuntosLeonisa.Seguridad.Application.Core;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;

namespace PuntosLeonisa.Seguridad.Application
{
    public class PuntoDeVentaApplication : IPuntoDeVentaApplication
    {
        private readonly IMapper mapper;
        private readonly IPuntoDeVentaRepository puntoDeVentaRepository;
        private readonly IPuntoVentaVarRepository puntoVentaVarRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IUsuarioExternalService usuarioExternalService;
        private readonly GenericResponse<PuntoDeVentaDto> response;

        public PuntoDeVentaApplication(IMapper mapper, IPuntoDeVentaRepository puntoDeVentaRepository, IPuntoVentaVarRepository puntoVentaVarRepository, IUnitOfWork unitOfWork, IUsuarioExternalService usuarioExternalService)
        {
            if (puntoDeVentaRepository is null)
            {
                throw new ArgumentNullException(nameof(puntoDeVentaRepository));
            }

            this.mapper = mapper;
            this.puntoDeVentaRepository = puntoDeVentaRepository;
            this.puntoVentaVarRepository = puntoVentaVarRepository;
            this.usuarioExternalService = usuarioExternalService;
            this.unitOfWork = unitOfWork;
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

        public async Task<GenericResponse<IEnumerable<AsignacionDto[]>>> AddAsignacion(AsignacionDto[] data)
        {
            try
            {
                foreach (var item in data)
                {
                    item.Id = Guid.NewGuid().ToString();
                    await this.unitOfWork.AsignacionRepository.Add(mapper.Map<Asignacion>(item));
                }
                return new GenericResponse<IEnumerable<AsignacionDto[]>>
                {
                    IsSuccess = true,
                    Message = "Asignación añadida correctamente",
                    Result = new List<AsignacionDto[]>
                    {
                        data
                    }
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<GenericResponse<IEnumerable<PuntoVentaVarDto[]>>> AddPuntoVentaVar(PuntoVentaVarDto[] data)
        {
            try
            {
                foreach (var item in data)
                {
                    item.Id = Guid.NewGuid().ToString();
                    await this.unitOfWork.PuntoVentaVarRepository.Add(mapper.Map<PuntoVentaVar>(item));
                }
                return new GenericResponse<IEnumerable<PuntoVentaVarDto[]>>
                {
                    IsSuccess = true,
                    Message = "Registro añadido correctamente",
                    Result = new List<PuntoVentaVarDto[]>
                    {
                        data
                    }
                };
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

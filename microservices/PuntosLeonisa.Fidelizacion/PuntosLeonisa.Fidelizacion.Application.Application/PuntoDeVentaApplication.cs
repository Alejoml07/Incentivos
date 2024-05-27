using AutoMapper;
using Polly.Caching;
using PuntosLeonisa.Fidelizacion.Domain;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Model.Carrito;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.FidelizacionPuntos;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.PuntoDeVenta;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Service.UnitOfWork;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using PuntosLeonisa.Infrasctructure.Core.ExternaServiceInterfaces;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Seguridad.Application.Core;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;
using System.Windows.Markup;

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

        public async Task<GenericResponse<IEnumerable<PuntoVentaHistoriaDto[]>>> AddPuntoVentaHistoria(PuntoVentaHistoriaDto[] data)
        {
            try
            {
                foreach (var item in data)
                {
                    item.Id = Guid.NewGuid().ToString();
                    await this.unitOfWork.PuntoVentaHistoria.Add(mapper.Map<PuntoVentaHistoria>(item));
                }
                return new GenericResponse<IEnumerable<PuntoVentaHistoriaDto[]>>
                {
                    IsSuccess = true,
                    Message = "Registro añadido correctamente",
                    Result = new List<PuntoVentaHistoriaDto[]>
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


        public async Task<GenericResponse<bool>> EliminarExcels(LiquidacionPuntos data)
        {
            try
            {
                var ToDeleteHistoria = await this.unitOfWork.PuntoVentaHistoria.GetPuntoVentaHistoriaByMesAndAnio(data);
                var ToDeleteVar = await this.unitOfWork.PuntoVentaVarRepository.GetPuntoVentaVarByMesAndAnio(data);
                if (ToDeleteHistoria != null)
                {
                    foreach (var item in ToDeleteHistoria)
                    {
                        await this.unitOfWork.PuntoVentaHistoria.Delete(item);
                    }
                    await this.unitOfWork.SaveChangesAsync();
                }
                if (ToDeleteVar != null)
                {
                    foreach (var item in ToDeleteVar)
                    {
                        await this.unitOfWork.PuntoVentaVarRepository.Delete(item);
                    }
                    await this.unitOfWork.SaveChangesAsync();
                }
                return new GenericResponse<bool>
                {
                    IsSuccess = true,
                    Message = "Registros eliminados correctamente",
                    Result = true
                };             
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<GenericResponse<bool>> AddExcels(PuntoVentaVarDto data)
        {
            try
            {
                var ToAddHistoria = new PuntoVentaHistoriaDto
                {
                    Id = Guid.NewGuid().ToString(),
                    IdPuntoVenta = data.IdPuntoVenta,
                    IdPresupuesto = data.Presupuesto,
                    Mes = data.Mes,
                    Ano = data.Anio,
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now
                };
                await this.unitOfWork.PuntoVentaVarRepository.Add(mapper.Map<PuntoVentaVar>(data));
                await this.unitOfWork.PuntoVentaHistoria.Add(mapper.Map<PuntoVentaHistoria>(ToAddHistoria));
                await this.unitOfWork.SaveChangesAsync();
                return new GenericResponse<bool>
                {
                    IsSuccess = true,
                    Message = "Registros añadidos correctamente",
                    Result = true
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<GenericResponse<bool>> LiquidacionPuntosMes(LiquidacionPuntos data)
        {
            try
            {
                EliminarExcels(data);
                AddExcels(data.Registro);
                string mes = "";
                int mesNumerico;

                if (int.TryParse(data.Fecha.Mes, out mesNumerico))
                {

                    if (mesNumerico < 10)
                    {
                        mes = "0" + data.Fecha.Mes;
                    }
                    else
                    {
                        mes = data.Fecha.Mes;
                    }
                }


                var user = new ValidarUsuarioDto
                {
                    NombreUsuario = "43614188",
                    Contrasena = "43614188"
                };

                var result = await this.usuarioExternalService.ValidarUsuario(user);
                var Token = result.Data.Token;

                var fecha = new Fecha
                {
                    Anho = data.Fecha.Anho,
                    Mes = mes,
                };

                var resultAser = await this.usuarioExternalService.GetUsuarioTPA(fecha, Token);

                var cont = 0;
                if (resultAser != null)
                {
                    foreach (var item in resultAser)
                    {
                        cont++;
                        var valuser = await this.usuarioExternalService.GetUserLiteByCedula(item.Cedula);
                        if(valuser == null)
                        {
                            var NuevoUser = new Usuario()
                            {
                                Cedula = item.Cedula,
                                Nombres = "pendiente",
                                Apellidos = "pendiente",
                                Email = "pendiente",                                
                            };
                            var NuevoInfo = new UsuarioInfoPuntos()
                            {
                                //crear infopuntos
                                Cedula = item.Cedula,
                                PuntosAcumulados = 0,
                                PuntosDisponibles = 0,
                                PuntosEnCarrito = 0,
                                PuntosRedimidos = 0,
                            };
                            await this.usuarioExternalService.AddUsuarioLiquidacion(NuevoUser);
                            await this.unitOfWork.UsuarioInfoPuntosRepository.Add(NuevoInfo);
                            await this.unitOfWork.SaveChangesAsync();
                            valuser = await this.usuarioExternalService.GetUserLiteByCedula(item.Cedula);
                        }

                    }
                }
                return new GenericResponse<bool>
                {
                    IsSuccess = true,
                    Message = "Liquidación de puntos realizada correctamente",
                    Result = true
                };
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

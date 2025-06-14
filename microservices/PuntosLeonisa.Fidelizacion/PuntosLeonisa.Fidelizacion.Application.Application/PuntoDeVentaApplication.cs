﻿using AutoMapper;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Model.Carrito;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Carrito;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.PuntoDeVenta;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Service.UnitOfWork;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using PuntosLeonisa.Infrasctructure.Core.ExternaServiceInterfaces;
using PuntosLeonisa.Products.Domain.Model;
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
                var puntoDeVenta = await this.unitOfWork.PuntoDeVentaRepository.GetPuntoDeVentaByCodigo(value.Codigo);
                if (puntoDeVenta != null)
                {
                    //agregar mapping de datos de punto de venta
                    mapper.Map(value, puntoDeVenta);
                    await this.unitOfWork.PuntoDeVentaRepository.Update(puntoDeVenta);
                    return response;
                }
                else
                {
                    value.Id = await this.unitOfWork.PuntoDeVentaRepository.GetAll().ContinueWith(x => x.Result.Count()+300.ToString());
                    value.Eliminado = 0;
                    var punto = this.mapper.Map<PuntoDeVenta>(value);
                    await this.unitOfWork.PuntoDeVentaRepository.Add(punto);
                    await this.unitOfWork.SaveChangesAsync();
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
                    await this.unitOfWork.PuntoDeVentaRepository.Add(mapper.Map<PuntoDeVenta>(item));
                    await this.unitOfWork.SaveChangesAsync();

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
                var historia = new PuntoVentaHistoria
                {
                    Mes = data.Fecha.Mes,
                    Ano = data.Fecha.Anho
                };

                var ventaVar = new PuntoVentaVar
                {
                    Mes = data.Fecha.Mes,
                    Anio = data.Fecha.Anho
                };
                var seguimiento = new SeguimientoLiquidacion
                {
                    Mes = data.Fecha.Mes,
                    Anio = data.Fecha.Anho
                };
                var ToDeleteHistoria = await this.unitOfWork.PuntoVentaHistoria.DeletePuntoVentaHistoriaByMesAndAnio(historia);
                var ToDeleteVar = await this.unitOfWork.PuntoVentaVarRepository.DeletePuntoVentaVarByMesAndAnio(ventaVar);
                var ToDeleteSeg = await this.unitOfWork.SeguimientoLiquidacionRepository.DeleteSeguimientoByMesYAnio(seguimiento);
                await this.unitOfWork.SaveChangesAsync();
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

        public async Task<GenericResponse<bool>> LiquidacionPuntosMes(LiquidacionPuntos data)
        {
            try
            {
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
                        if (valuser == null)
                        {
                            var NuevoUser = new Usuario()
                            {
                                Cedula = item.Cedula.Trim(),
                                Nombres = "pendiente",
                                Apellidos = "pendiente",
                                Email = "pendiente",
                            };
                            var NuevoInfo = new UsuarioInfoPuntos()
                            {
                                Cedula = item.Cedula.Trim(),
                                PuntosAcumulados = 0,
                                PuntosDisponibles = 0,
                                PuntosEnCarrito = 0,
                                PuntosRedimidos = 0,
                            };
                            await this.usuarioExternalService.AddUsuarioLiquidacion(NuevoUser);
                            await this.unitOfWork.UsuarioInfoPuntosRepository.Add(NuevoInfo);

                            valuser = await this.usuarioExternalService.GetUserLiteByCedula(item.Cedula);
                        }
                        var vari = new PuntoVentaVar
                        {
                            Mes = data.Fecha.Mes,
                            Anio = data.Fecha.Anho,
                            CodigoPuntoVenta = item.CodigoPuntoVenta,
                        };
                        var variables = await this.unitOfWork.PuntoVentaVarRepository.GetPuntosByCodigoUsuario(vari);
                        
                        if (variables.Count() > 0)
                        {
                            foreach (var item2 in variables)
                            {
                                double? ptsobt = 0;
                                string porcentajeString = item.Porcentaje;
                                float porcentajeFloat = float.Parse(porcentajeString);
                                float porclaborado = porcentajeFloat / 100;
                                var cumplimiento = item2.Cumplimiento / 100;

                                //var bases = await unitOfWork.VariableRepository.GetVariablesParaBase(item2);

                                if (item2.IdVariable == "22" && item2.Cumplimiento >= 100.5)
                                {
                                    if (item2.Cumplimiento > 111)
                                    {
                                        cumplimiento = 1.11;
                                    }
                                    else
                                    {
                                        cumplimiento = item2.Cumplimiento / 100;
                                    }
                                    var one_port = 0.01;

                                    ptsobt = porclaborado * (cumplimiento - one_port) * item2.Base;
                                    ptsobt = Math.Round((double)ptsobt);
                                }
                                else
                                {
                                    ptsobt = 0;
                                }


                                var consp = new PuntoVentaVar
                                {
                                    Mes = data.Fecha.Mes,
                                    Anio = data.Fecha.Anho,
                                    IdPuntoVenta = item2.IdPuntoVenta,
                                    IdVariable = "22",
                                };
                                var resultadoConsultaPresupuesto = await unitOfWork.PuntoVentaVarRepository.GetConsultaPresupuesto(consp);
                                
                                if (resultadoConsultaPresupuesto.Cumplimiento >= 100.5 && item2.IdVariable != "22" && item2.Cumplimiento >= 99.5)
                                {
                                    if (item2.Cumplimiento > 110)
                                    {
                                        cumplimiento = 1.10;
                                    }
                                    else
                                    {
                                        cumplimiento = item2.Cumplimiento / 100;
                                    }
                                    ptsobt = (porclaborado * cumplimiento * item2.Base);
                                    ptsobt = Math.Round((double)ptsobt);
                                }

                                //if (item2.Cumplimiento >= 104.5)
                                //{
                                //    if (item2.IdVariable == "4")
                                //    {
                                //        ptsobt = ptsobt * 2;
                                //    }
                                //    else if (item2.IdVariable == "414")
                                //    {
                                //        ptsobt = ptsobt * 1.5;
                                //    }
                                //}
                                //var puntosUsuario = await this.unitOfWork.UsuarioInfoPuntosRepository.GetUsuarioByCedula(item.Cedula);
                                //if (puntosUsuario == null)
                                //{
                                //    var NuevoInfo = new UsuarioInfoPuntos()
                                //    {
                                //        Cedula = item.Cedula.Trim(),
                                //        PuntosAcumulados = 0,
                                //        PuntosDisponibles = 0,
                                //        PuntosEnCarrito = 0,
                                //        PuntosRedimidos = 0,
                                //    };

                                //    await this.unitOfWork.UsuarioInfoPuntosRepository.Add(NuevoInfo);
                                //    await this.unitOfWork.SaveChangesAsync();
                                //}
                                //puntosUsuario = await this.unitOfWork.UsuarioInfoPuntosRepository.GetUsuarioByCedula(item.Cedula);
                                //if (puntosUsuario != null)
                                //{
                                    //puntosUsuario.PuntosAcumulados += (int)ptsobt;
                                    //puntosUsuario.PuntosDisponibles += (int)ptsobt;
                                    //await this.unitOfWork.UsuarioInfoPuntosRepository.Update(puntosUsuario);
                                    var seguimiento = new SeguimientoLiquidacion
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        Cedula = item.Cedula,
                                        Mes = data.Fecha.Mes,
                                        Anio = data.Fecha.Anho,
                                        PtoVenta = item2.CodigoPuntoVenta,
                                        Cumplimiento = item2.Cumplimiento,
                                        Puntos = (int)ptsobt,
                                        IdVariable = item2.IdVariable,
                                        Porcentaje = item.Porcentaje,
                                        NombreVariable = item2.NombreVariable,
                                        NombrePtoVenta = item2.NombrePtoVenta,
                                     
                                    };
                                    await AddSeguimientoLiquidacion(seguimiento);
                                    
                                    //var extracto = new Extractos
                                    //{
                                    //    Id = Guid.NewGuid().ToString(),
                                    //    Anio = data.Fecha.Anho,
                                    //    Mes = data.Fecha.Mes,
                                    //    ValorMovimiento = (int)ptsobt,
                                    //    Descripcion = "Liquidación de puntos por mes",
                                    //    OrigenMovimiento = "Liquidación de puntos por mes",
                                    //    Fecha = DateTime.Now.AddHours(-5)
                                    //};
                                    //var usuario = new Usuario()
                                    //{
                                    //    Cedula = item.Cedula
                                    //};                                   
                                    //extracto.Usuario = usuario;
                                    //await this.unitOfWork.ExtractosRepository.Add(extracto);
                                    await this.unitOfWork.SaveChangesAsync();
                                //}
                            }
                        }
                    }
                }
                await this.unitOfWork.SaveChangesAsync();
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

        


        public async Task<GenericResponse<bool>> AddAndDeleteVentaVarAndHistoria(LiquidacionPuntos data)
        {
            try
            {
                //        var Excel = new Dictionary<string, List<PuntoVentaVarDto>>();
                //        foreach (var item in data.Registro)
                //        {
                //            item.Id = Guid.NewGuid().ToString();
                //            if (Excel.TryGetValue(item.Id, out List<PuntoVentaVarDto> exc))
                //            {
                //                exc.Add(item);
                //                Excel[item.Id] = exc;
                //            }
                //            else
                //            {
                //                var items = new List<PuntoVentaVarDto>
                //                {
                //                    item
                //                };
                //                Excel[item.Id] = items;
                //            }
                //        }
                //        var datos = Excel.ToArray();

                //        for (int i = 0; i < datos.Length; i++)
                //        {
                //            var item = datos[i].Value.FirstOrDefault();
                //            if (item.IdPuntoVenta != null && !string.IsNullOrEmpty(item.IdVariable))
                //            {
                //                var id_ptventa = await this.unitOfWork.PuntoDeVentaRepository.GetPuntoDeVentaByCodigo(item.IdPuntoVenta);
                //                var id_variable = await this.unitOfWork.VariableRepository.GetVariablesByCodigo(item.IdVariable);
                //                if(id_ptventa == null || id_variable == null)
                //                {
                //                    continue;
                //                }
                //                var pventa = new PuntoVentaVarDto
                //                {
                //                    Mes = data.Fecha.Mes,
                //                    Anio = data.Fecha.Anho,
                //                    IdPuntoVenta = id_ptventa.Id,
                //                    IdVariable = id_variable.Id
                //                };
                //                var valptexisten = await this.unitOfWork.PuntoVentaVarRepository.GetPuntoVentaVar(pventa);
                //                if (item.Presupuesto == null || item.Presupuesto == "")
                //                {
                //                    item.Presupuesto = "0";
                //                }
                //                else if (item.ValReal == null || item.ValReal == "")
                //                {
                //                    item.ValReal = "0";
                //                }
                //                else if (item.Cumplimiento == null)
                //                {
                //                    item.Cumplimiento = 0;
                //                }
                //                if (valptexisten != null)
                //                {
                //                    var exist = new PuntoVentaVar
                //                    {
                //                        Presupuesto = item.Presupuesto,
                //                        ValReal = item.ValReal,
                //                        Cumplimiento = item .Cumplimiento
                //                    };
                //                    await this.unitOfWork.PuntoVentaVarRepository.Update(exist);
                //                }
                //                else
                //                {
                //                    var exist = new PuntoVentaVar
                //                    {
                //                        Id = Guid.NewGuid().ToString(),
                //                        IdPuntoVenta = id_ptventa.Id,
                //                        CodigoPuntoVenta = id_ptventa.Codigo.ToString(),
                //                        IdVariable = id_variable.Id,
                //                        Mes = data.Fecha.Mes,
                //                        Anio = data.Fecha.Anho,
                //                        Presupuesto = item.Presupuesto,
                //                        ValReal = item.ValReal,
                //                        Cumplimiento = item.Cumplimiento,
                //                    };
                //                    await this.unitOfWork.PuntoVentaVarRepository.Add(exist);
                //                }
                //                var PtoHistoria = new PuntoVentaHistoria
                //                {
                //                    IdPuntoVenta = id_ptventa.Id,
                //                    Mes = data.Fecha.Mes,
                //                    Ano = data.Fecha.Anho
                //                };
                //                var validarPtVentaHistoria = await this.unitOfWork.PuntoVentaHistoria.GetPuntoVentaHistoriaById(PtoHistoria);

                //                if (validarPtVentaHistoria.Count() == 0)
                //                {
                //                    var crearRegistro = new PuntoVentaHistoria
                //                    {
                //                        Id = Guid.NewGuid().ToString(),
                //                        IdVariable = id_variable.Id,
                //                        IdPuntoVenta = id_ptventa.Id,
                //                        IdPresupuesto = 600,
                //                        Mes = data.Fecha.Mes,
                //                        Ano = data.Fecha.Anho,
                //                    };
                //                    await this.unitOfWork.PuntoVentaHistoria.Add(crearRegistro);
                //                }
                //                else
                //                {
                //                    var variablesP = validarPtVentaHistoria.FirstOrDefault().IdVariable;
                //                    var arrayVariables = variablesP.Split("|");

                //                    var arrayVariablesFinal = new List<string>();
                //                    foreach (var itemV in arrayVariables)
                //                    {
                //                        if (itemV != null)
                //                        {
                //                            arrayVariablesFinal.Add(itemV);
                //                        }
                //                    }

                //                    arrayVariablesFinal.Add(id_variable.Id);
                //                    var variablesString = string.Join("|", arrayVariablesFinal);
                //                    var updateRegistro = await this.unitOfWork.PuntoVentaHistoria.GetById(validarPtVentaHistoria.FirstOrDefault().Id);
                //                    updateRegistro.IdVariable = variablesString;
                //                    updateRegistro.Mes = data.Fecha.Mes;
                //                    updateRegistro.Ano = data.Fecha.Anho;
                //                    await this.unitOfWork.PuntoVentaHistoria.Update(updateRegistro);

                //                }
                //            }
                //        }
                //        await this.unitOfWork.SaveChangesAsync();
                //        return new GenericResponse<bool>
                //        {
                //            IsSuccess = true,
                //            Message = "Registros añadidos correctamente",
                //            Result = true
                //        };
                //    }
                //    catch (Exception)
                //    {

                //        throw;
                //    }
                //}
                foreach (var item in data.Registro)
                {
                    double bas = 0;
                    if (item.IdPuntoVenta != null && !string.IsNullOrEmpty(item.IdVariable))
                    {
                        var id_ptventa = await this.unitOfWork.PuntoDeVentaRepository.GetPuntoDeVentaByCodigo(item.IdPuntoVenta);
                        var id_variable = await this.unitOfWork.VariableRepository.GetVariablesByCodigo(item.IdVariable);
                        if (id_ptventa == null || id_variable == null)
                        {
                            continue;
                        }
                        
                        var pventa = new PuntoVentaVarDto
                        {
                            Mes = data.Fecha.Mes,
                            Anio = data.Fecha.Anho,
                            IdPuntoVenta = id_ptventa.Id,
                            IdVariable = id_variable.Id
                        };

                        var valptexisten = await this.unitOfWork.PuntoVentaVarRepository.GetPuntoVentaVar(pventa);

                        if (item.Presupuesto == null || item.Presupuesto == "")
                        {
                            item.Presupuesto = "0";
                        }
                        else if (item.ValReal == null || item.ValReal == "")
                        {
                            item.ValReal = "0";
                        }
                        else if (item.Cumplimiento == null)
                        {
                            item.Cumplimiento = 0;
                        }                        
                        var bases = await unitOfWork.VariableRepository.GetVariablesParaBase(item);
                        
                        if (bases != null)
                        {
                            bas = (double)bases.Base;
                        }

                        if (valptexisten != null)
                        {
                            valptexisten.Base = bas;
                            valptexisten.Presupuesto = item.Presupuesto;
                            valptexisten.ValReal = item.ValReal;
                            valptexisten.Cumplimiento = item.Cumplimiento;

                            await this.unitOfWork.PuntoVentaVarRepository.Update(valptexisten);
                            await this.unitOfWork.SaveChangesAsync();
                        }
                        else
                        {
                            var exist = new PuntoVentaVar
                            {
                                Id = Guid.NewGuid().ToString(),
                                IdPuntoVenta = id_ptventa.Id,
                                CodigoPuntoVenta = id_ptventa.Codigo.ToString(),
                                IdVariable = id_variable.Id,
                                Base = bas,
                                Mes = data.Fecha.Mes,
                                Anio = data.Fecha.Anho,
                                Presupuesto = item.Presupuesto,
                                ValReal = item.ValReal,
                                Cumplimiento = item.Cumplimiento,
                                NombrePtoVenta = id_ptventa.Nombre,
                                NombreVariable = id_variable.Nombre
                            };
                            await this.unitOfWork.PuntoVentaVarRepository.Add(exist);
                            await this.unitOfWork.SaveChangesAsync();
                        }
                        var PtoHistoria = new PuntoVentaHistoria
                        {
                            IdPuntoVenta = id_ptventa.Id,
                            Mes = data.Fecha.Mes,
                            Ano = data.Fecha.Anho
                        };
                        var validarPtVentaHistoria = await this.unitOfWork.PuntoVentaHistoria.GetPuntoVentaHistoriaById(PtoHistoria);

                        if (validarPtVentaHistoria.Count() == 0)
                        {
                            var crearRegistro = new PuntoVentaHistoria
                            {
                                Id = Guid.NewGuid().ToString(),
                                IdVariable = id_variable.Id,
                                IdPuntoVenta = id_ptventa.Id,
                                IdPresupuesto = 600,
                                Mes = data.Fecha.Mes,
                                Ano = data.Fecha.Anho,
                            };
                            await this.unitOfWork.PuntoVentaHistoria.Add(crearRegistro);
                            await this.unitOfWork.SaveChangesAsync();
                        }
                        else
                        {
                            var variablesP = validarPtVentaHistoria.FirstOrDefault().IdVariable;
                            var arrayVariables = variablesP.Split("|");

                            var arrayVariablesFinal = new List<string>();
                            foreach (var itemV in arrayVariables)
                            {
                                if (itemV != null)
                                {
                                    arrayVariablesFinal.Add(itemV);
                                }
                            }

                            arrayVariablesFinal.Add(id_variable.Id);
                            var variablesString = string.Join("|", arrayVariablesFinal);
                            var updateRegistro = await this.unitOfWork.PuntoVentaHistoria.GetById(validarPtVentaHistoria.FirstOrDefault().Id);
                            updateRegistro.IdVariable = variablesString;
                            updateRegistro.Mes = data.Fecha.Mes;
                            updateRegistro.Ano = data.Fecha.Anho;
                            await this.unitOfWork.PuntoVentaHistoria.Update(updateRegistro);
                            await this.unitOfWork.SaveChangesAsync();

                        }
                    }
                }
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

        public Task<GenericResponse<PuntoDeVentaDto>> Update(PuntoDeVentaDto value)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddSeguimientoLiquidacion(SeguimientoLiquidacion data)
        {
            try
            {
                await this.unitOfWork.SeguimientoLiquidacionRepository.Add(data);
                await this.unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<GenericResponse<IEnumerable<UsuarioInfoPuntos>>> GetInfoWithSpace()
        {
            try
            {
                var info = await this.unitOfWork.UsuarioInfoPuntosRepository.GetAll();
                info = info.Where(x => x.Cedula.Contains(" "));
                return new GenericResponse<IEnumerable<UsuarioInfoPuntos>>
                {
                    IsSuccess = true,
                    Message = "Información encontrada",
                    Result = info
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<GenericResponse<bool>> ValidarLiquidacion()
        {
            var vari = new PuntoVentaVar
            {
                Mes = "5",
                Anio = "2024",
                CodigoPuntoVenta = "020220",
                IdVariable = "5"

            };
            var variables = await this.unitOfWork.PuntoVentaVarRepository.GetPuntosByCodigoUsuario(vari);
            foreach (var item in variables)
            {
                double cumplimiento = 0;
                double porcentaje = 1.04;
                double Cump = 125;
                if (Cump > 110)
                {
                    cumplimiento = 110 / 100;
                }
                else
                {
                    cumplimiento = Cump / 100;
                }
                var ptsobt = (porcentaje * cumplimiento * item.Base);
            }
            
            return new GenericResponse<bool>
            {
                IsSuccess = true,
                Message = "Liquidación de puntos realizada correctamente",
                Result = true
            };
        }

        public async Task<GenericResponse<IEnumerable<SeguimientoLiquidacion>>> GetSeguimientoLiquidacion(Fechas data)
        {
            try
            {
                var seguimientos = await this.unitOfWork.SeguimientoLiquidacionRepository.GetSeguimientoLiquidacion(data);
                //necesito retornar cedulas que no se repitan y sumar sus puntos
                return new GenericResponse<IEnumerable<SeguimientoLiquidacion>>
                {
                    IsSuccess = true,
                    Message = "Seguimientos encontrados",
                    Result = seguimientos
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<GenericResponse<bool>> ActualizarCarritoLideres()
        {
            try
            {
                var lideres = await this.usuarioExternalService.GetUsuarios();

                // Aplicar el filtro y obtener solo los usuarios líderes
                var lideresFiltrados = lideres.Result
                    .Where(x => x.TipoUsuario == "Lideres" || x.TipoUsuario == "Lideres ZE")
                    .ToList();

                // Verificar si se encontraron líderes
                if (lideresFiltrados.Count > 0)
                {
                    foreach (var lider in lideresFiltrados)
                    {
                        var usuarioInfo = await this.unitOfWork.UsuarioInfoPuntosRepository.GetUsuarioByCedula(lider.Cedula);
                        if(usuarioInfo == null)
                        {
                            continue;
                        }
                        var puntosEnCarrito = await this.unitOfWork.CarritoRepository.GetPuntosEnCarrito(lider.Email);
                        var puntosEnCarritoSum = puntosEnCarrito.Sum(x => x.Product.Puntos * x.Product.Quantity);
                        usuarioInfo.PuntosEnCarrito = puntosEnCarritoSum;
                        await this.unitOfWork.UsuarioInfoPuntosRepository.Update(usuarioInfo);
                        await this.unitOfWork.SaveChangesAsync();
                    }

                    return new GenericResponse<bool>
                    {
                        IsSuccess = true,
                        Message = "Carrito de líderes actualizado correctamente",
                        Result = true
                    };
                }

                return new GenericResponse<bool>
                {
                    IsSuccess = false,
                    Message = "No se encontraron líderes",
                    Result = false
                };
            }
            catch (Exception ex)
            {
                // Se recomienda loggear el error para facilitar el diagnóstico
                throw new Exception("Ocurrió un error al actualizar el carrito de líderes.", ex);
            }
        }

        public async Task<GenericResponse<bool>> ActualizarPuntosLideres()
        {
            try
            {
                var lideres = await this.usuarioExternalService.GetUsuarios();

                // Aplicar el filtro y obtener solo los usuarios líderes
                var lideresFiltrados = lideres.Result
                    .Where(x => x.TipoUsuario == "Lideres" || x.TipoUsuario == "Lideres ZE")
                    .ToList();

                if (lideresFiltrados.Count() > 0)
                {
                    foreach (var lider in lideresFiltrados)
                    {
                        var usuarioInfo = await this.unitOfWork.UsuarioInfoPuntosRepository.GetUsuarioByCedula(lider.Cedula);
                        if (usuarioInfo == null)
                        {
                            continue;
                        }
                        usuarioInfo.PuntosDisponibles = 0;
                        await this.unitOfWork.UsuarioInfoPuntosRepository.Update(usuarioInfo);
                        await this.unitOfWork.SaveChangesAsync();
                    }

                    return new GenericResponse<bool>
                    {
                        IsSuccess = true,
                        Message = "Puntos de líderes actualizados correctamente",
                        Result = true
                    };

                }
                return new GenericResponse<bool>
                {
                    IsSuccess = false,
                    Message = "No se encontraron líderes",
                    Result = false
                };

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

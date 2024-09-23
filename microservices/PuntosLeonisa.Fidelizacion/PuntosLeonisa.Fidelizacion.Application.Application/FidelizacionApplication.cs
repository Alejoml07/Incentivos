using AutoMapper;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json.Linq;
using PuntosLeonisa.fidelizacion.Domain.Service.DTO.PuntosManuales;
using PuntosLeonisa.Fidelizacion.Domain;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Carrito;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.FidelizacionPuntos;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Garantias;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.MovimientoPuntos;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Scanner;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Variables;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.WishList;
using PuntosLeonisa.Fidelizacion.Domain.Service.UnitOfWork;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.DTO;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Helpers;
using PuntosLeonisa.Infrasctructure.Core.ExternaServiceInterfaces;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Seguridad.Application.Core;
using System.Data;
using System.Text;

namespace PuntosLeonisa.Fidelizacion.Application;

public class FidelizacionApplication : IFidelizacionApplication
{
    private readonly IMapper mapper;
    private readonly GenericResponse<PuntosManualDto> response;
    private readonly GenericResponse<WishListDto> response2;
    private readonly IUsuarioExternalService usuarioExternalService;
    private readonly IProductoExternalService productoExternalService;
    private readonly IOrdenOPExternalService ordenOPExternalService;
    private readonly IUsuarioScannerExternalService usuarioScannerExternalService;
    private readonly IUnitOfWork unitOfWork;
    private readonly GenericResponse<Carrito> response3;
    private readonly GenericResponse<SmsDto> response4;
    private readonly GenericResponse<Extractos> response5;
    public FidelizacionApplication(IMapper mapper,
        IUsuarioExternalService usuarioExternalService,
        IProductoExternalService productoExternalService,
        IOrdenOPExternalService ordenOPExternalService,
        IUsuarioScannerExternalService usuarioScannerExternalService,
        IUnitOfWork unitOfWork)
    {


        this.mapper = mapper;
        this.usuarioExternalService = usuarioExternalService;
        this.productoExternalService = productoExternalService;
        this.ordenOPExternalService = ordenOPExternalService;
        this.usuarioScannerExternalService = usuarioScannerExternalService;
        this.unitOfWork = unitOfWork;
        response = new GenericResponse<PuntosManualDto>();
        response2 = new GenericResponse<WishListDto>();
        response3 = new GenericResponse<Carrito>();
        response4 = new GenericResponse<SmsDto>();
        response5 = new GenericResponse<Extractos>();

    }

    public async Task<GenericResponse<PuntosManualDto>> Add(PuntosManualDto value)
    {
        try
        {
            //TODO: Hacer las validaciones
            var puntos = mapper.Map<MovimientoPuntos>(value);
            await this.unitOfWork.PuntosRepository.Add(puntos);
            await this.unitOfWork.SaveChangesAsync();
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
            var usersByEmail = new List<Usuario>();
            var puntos = new List<MovimientoPuntos>();

            foreach (var punto in value)
            {
                punto.Id = Guid.NewGuid().ToString();
                if (usersByEmail.Any(x => x.Cedula == punto.Cedula))
                {
                    var puntosUsuarioExistente = usersByEmail.Where(x => x.Cedula == punto.Cedula).FirstOrDefault();
                    puntos.Add(new MovimientoPuntos
                    {
                        Id = Guid.NewGuid().ToString(),
                        Puntos = punto.Puntos,
                        Month = punto.Month,
                        Year = punto.Year,
                        Observaciones = punto.Observaciones,
                        Usuario = new Usuario()
                        {
                            Cedula = puntosUsuarioExistente.Cedula,
                            Nombres = puntosUsuarioExistente.Nombres,
                            Apellidos = puntosUsuarioExistente.Apellidos,
                            Email = puntosUsuarioExistente.Email,
                            Celular = puntosUsuarioExistente.Celular
                        }
                    });
                }
                else
                {
                    var response = await this.usuarioExternalService.GetUserLiteByCedula(punto.Cedula);
                    if (response.Result != null)
                    {
                        var user = mapper.Map<Usuario>(response.Result);
                        usersByEmail.Add(user);
                        puntos.Add(new MovimientoPuntos
                        {
                            Id = Guid.NewGuid().ToString(),
                            Puntos = punto.Puntos,
                            Month = punto.Month,
                            Year = punto.Year,
                            Observaciones = punto.Observaciones,
                            Usuario = new Usuario()
                            {
                                Cedula = user.Cedula,
                                Nombres = user.Nombres,
                                Apellidos = user.Apellidos,
                                Email = user.Email,
                                Celular = user.Celular
                            }
                        });
                    }
                }
            }
            //Agrupar puntos por usuario y sumar puntos en UsuarioInfoPuntos
            var puntosPorUsuario = puntos.GroupBy(x => x.Usuario?.Cedula).Select(x => new UsuarioInfoPuntos
            {
                Cedula = x.Key,
                PuntosAcumulados = x.Sum(x => int.Parse(x.Puntos ?? "0")),
                PuntosRedimidos = 0,
                PuntosEnCarrito = 0,
                PuntosDisponibles = x.Sum(x => int.Parse(x.Puntos ?? "0")),
                Nombres = x.FirstOrDefault().Usuario?.Nombres,
                Email = x.FirstOrDefault().Usuario?.Email,
                Apellidos = x.FirstOrDefault().Usuario?.Apellidos,
                FechaActualizacion = DateTime.Now
            }).ToArray();


            await this.AddUsuarioInfoPuntosRange(puntosPorUsuario);
            await this.unitOfWork.PuntosRepository.AddRange(puntos.ToArray());
            await this.unitOfWork.SaveChangesAsync();

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

    public async Task<GenericResponse<UsuarioInfoPuntos>> AddUsuarioInfoPuntos(UsuarioInfoPuntos value)
    {
        try
        {
            // validar si existe y si si actualiza y sino agrega
            var usuario = await this.unitOfWork.UsuarioInfoPuntosRepository.GetById(value.Cedula);
            if (usuario != null)
            {
                usuario.PuntosAcumulados += value.PuntosAcumulados;
                usuario.PuntosDisponibles += value.PuntosDisponibles;
                usuario.PuntosEnCarrito += value.PuntosEnCarrito;
                await this.unitOfWork.UsuarioInfoPuntosRepository.Update(usuario);
                await this.unitOfWork.SaveChangesAsync();
                return new GenericResponse<UsuarioInfoPuntos>
                {
                    Result = value
                };
            }
            else
            {
                await this.unitOfWork.UsuarioInfoPuntosRepository.Add(value);
                await this.unitOfWork.SaveChangesAsync();
                return new GenericResponse<UsuarioInfoPuntos>
                {
                    Result = value
                };
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<GenericResponse<UsuarioInfoPuntos[]>> AddUsuarioInfoPuntosRange(UsuarioInfoPuntos[] value)
    {
        try
        {
            foreach (var item in value)
            {
                await this.AddUsuarioInfoPuntos(item);
            }
            return new GenericResponse<UsuarioInfoPuntos[]>
            {
                Result = value
            };
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<GenericResponse<Carrito>> CarritoAdd(Carrito carrito)
    {
        try
        {
            if (carrito.Id == null)
            {
                carrito.Id = Guid.NewGuid().ToString();
                await this.unitOfWork.CarritoRepository.Add(carrito);
                //await this.unitOfWork.SaveChangesAsync();
                //agregar a usuarioinfo puntos
                var usuarioInfoPuntos = await this.unitOfWork.UsuarioInfoPuntosRepository.GetUsuarioByCedula(carrito.User.Cedula);
                if (usuarioInfoPuntos != null)
                {
                    usuarioInfoPuntos.PuntosEnCarrito = (int)carrito.Product.Puntos * carrito.Product.Quantity;
                    await this.unitOfWork.UsuarioInfoPuntosRepository.Update(usuarioInfoPuntos);
                    await this.unitOfWork.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Usuario no encontrado");
                }
                response3.Result = carrito;
                return response3;
            }
            else
            {
                // Update
                var carritoToUpdate = await this.unitOfWork.CarritoRepository.GetById(carrito.Id);
                if (carritoToUpdate != null)
                {
                    mapper.Map(carrito, carritoToUpdate);
                    await this.unitOfWork.CarritoRepository.Update(carritoToUpdate);
                    var usuarioInfoPuntos = await this.unitOfWork.UsuarioInfoPuntosRepository.GetUsuarioByEmail(carrito.User.Email);
                    if (usuarioInfoPuntos != null)
                    {
                        usuarioInfoPuntos.PuntosEnCarrito += (int?)carrito.Product?.Puntos * carrito.Product?.Quantity;
                        carritoToUpdate.Product.Quantity = carrito.Product.Quantity;
                        await this.unitOfWork.CarritoRepository.Update(carritoToUpdate);
                        await this.unitOfWork.UsuarioInfoPuntosRepository.Update(usuarioInfoPuntos);
                        await this.unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception("Usuario no encontrado");
                    }
                    response3.Result = carrito;
                    return response3;
                }
                else
                {
                    throw new Exception("Carrito no encontrado");
                }
            }


        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<bool> CarritoDeleteById(string id)
    {
        try
        {
            var carrito = await this.unitOfWork.CarritoRepository.GetById(id);
            if (carrito != null)
            {
                await this.unitOfWork.CarritoRepository.Delete(carrito);
                await this.unitOfWork.SaveChangesAsync();
                return true;
            }
            return false;

        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<GenericResponse<IEnumerable<Carrito>>> CarritoGetByUser(string id)
    {
        try
        {
            var carrito = await this.unitOfWork.CarritoRepository.GetByPredicateAsync(carritoRepository => carritoRepository.User.Email == id);
            var response3 = new GenericResponse<IEnumerable<Carrito>>();
            if (carrito != null)
            {
                response3.Result = carrito;
            }
            return response3;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<GenericResponse<PuntosManualDto>> Delete(PuntosManualDto value)
    {
        try
        {
            await this.unitOfWork.PuntosRepository.Delete(mapper.Map<MovimientoPuntos>(value));
            response.Result = value;
            return response;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<GenericResponse<UsuarioInfoPuntos>> Delete(UsuarioInfoPuntos value)
    {
        try
        {
            await this.unitOfWork.UsuarioInfoPuntosRepository.Delete(value);
            return new GenericResponse<UsuarioInfoPuntos>
            {
                Result = value
            };
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<GenericResponse<PuntosManualDto>> DeleteById(string id)
    {
        try
        {
            var ToDelete = await this.unitOfWork.UsuarioInfoPuntosRepository.GetById(id) ?? throw new ArgumentException("Puntos no encontrados");
            var puntosToDelete = mapper.Map<PuntosManualDto>(ToDelete);

            await this.unitOfWork.UsuarioInfoPuntosRepository.Delete(ToDelete);
            this.response.Result = puntosToDelete;
            return this.response;
        }
        catch (Exception)
        {
            throw;
        }

    }

    public Task<GenericResponse<UsuarioInfoPuntos>> DeleteUsuarioInfoPuntos(UsuarioInfoPuntos value)
    {
        throw new NotImplementedException();
    }

    public async Task<GenericResponse<UsuarioInfoPuntos>> DeleteUsuarioInfoPuntosById(string id)
    {
        try
        {
            var usuario = await this.unitOfWork.UsuarioInfoPuntosRepository.GetById(id);
            if (usuario != null)
            {
                await this.unitOfWork.UsuarioInfoPuntosRepository.Delete(usuario);
                return new GenericResponse<UsuarioInfoPuntos>
                {
                    Result = usuario
                };
            }
            return null;

        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<GenericResponse<IEnumerable<PuntosManualDto>>> GetAll()
    {
        var puntos = await this.unitOfWork.PuntosRepository.GetAll();
        var puntosDto = mapper.Map<PuntosManualDto[]>(puntos);
        var responseOnly = new GenericResponse<IEnumerable<PuntosManualDto>>
        {
            Result = puntosDto
        };

        return responseOnly;
    }

    public async Task<GenericResponse<PuntosManualDto>> GetById(string id)
    {
        var responseRawData = await this.unitOfWork.UsuarioInfoPuntosRepository.GetById(id);
        var responseData = mapper.Map<PuntosManualDto>(responseRawData);
        response.Result = responseData;

        return response;
    }

    public async Task<GenericResponse<IEnumerable<UsuarioInfoPuntos>>> GetUsuarioInfoPuntosAll()
    {
        var response = new GenericResponse<IEnumerable<UsuarioInfoPuntos>>();
        try
        {
            var usuario = await this.unitOfWork.UsuarioInfoPuntosRepository.GetAll();
            response.Result = usuario;
            return response;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<GenericResponse<UsuarioInfoPuntos>> GetUsuarioInfoPuntosById(string id)
    {
        try
        {
            var puntosEnCarrito = this.unitOfWork.CarritoRepository.GetPuntosEnCarrito(id).GetAwaiter().GetResult();
            var puntosEnCarritoSum = puntosEnCarrito.Sum(x => x.Product.Puntos * x.Product.Quantity);
            var usuarioRegistrado = await this.usuarioExternalService.GetUserByEmail(id);
            if (!usuarioRegistrado.IsSuccess)
            {
                throw new Exception("Usuario no encontrado");
            }

            var usuarios = await this.unitOfWork.UsuarioInfoPuntosRepository.GetByPredicateAsync(p => p.Cedula == usuarioRegistrado.Result.Cedula);
            var usuario = new UsuarioInfoPuntos();
            if (usuarios == null || !usuarios.Any())
            {
                //create new usuarioInfoPuntos
                var usuarioInfoPuntosNuevo = new UsuarioInfoPuntos
                {
                    Cedula = usuarioRegistrado.Result.Cedula,
                    PuntosAcumulados = 0,
                    PuntosDisponibles = 0,
                    PuntosRedimidos = 0,
                    PuntosEnCarrito = puntosEnCarritoSum,
                    Nombres = usuarioRegistrado.Result.Nombres,
                    Apellidos = usuarioRegistrado.Result.Apellidos,
                    Email = usuarioRegistrado.Result.Email,
                    FechaActualizacion = DateTime.Now
                };
                await this.unitOfWork.UsuarioInfoPuntosRepository.Add(usuarioInfoPuntosNuevo);
                this.unitOfWork.SaveChangesSync();
                usuario = usuarioInfoPuntosNuevo;
            }
            else
            {
                usuario = usuarios.First();
                usuario.PuntosEnCarrito = puntosEnCarritoSum;
            }

            return new GenericResponse<UsuarioInfoPuntos>
            {
                Result = usuario
            };
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<GenericResponse<bool>> RedencionPuntos(UsuarioRedencion data)
    {
        try
        {
            var usuarioCompleto = this.usuarioExternalService.GetUserByEmail(data.Usuario.Email ?? "").GetAwaiter().GetResult();
            var usuarioInfoPuntos = this.unitOfWork.UsuarioInfoPuntosRepository.GetUsuarioByEmail(data.Usuario.Email).GetAwaiter().GetResult();
            data.Usuario.Agencia = usuarioCompleto.Result.Agencia;
            data.Usuario.Empresa = usuarioCompleto.Result.Empresa;
            data.Usuario = usuarioCompleto.Result;
            if (usuarioInfoPuntos != null)
            {
                data.PuntosRedimidos = data.GetSumPuntos();

                if (usuarioInfoPuntos.PuntosDisponibles < data.PuntosRedimidos)
                {
                    throw new Exception("Puntos a redimir no pueden ser mayores a los disponibles");
                }

                var res = await this.CreateRedencion(data);
                if (res.Message == "No se puede redimir 0 puntos")
                {
                    throw new Exception(res.Message);
                }
                if (res.Result)
                {
                    //validacion donde no los puntos a redimir no pueden ser mayores al disponible

                    usuarioInfoPuntos.PuntosDisponibles -= data.PuntosRedimidos;
                    usuarioInfoPuntos.PuntosRedimidos += data.PuntosRedimidos;
                    usuarioInfoPuntos.PuntosEnCarrito = 0;
                    usuarioInfoPuntos.FechaActualizacion = DateTime.Now;
                    this.unitOfWork.UsuarioInfoPuntosRepository.Update(usuarioInfoPuntos).GetAwaiter().GetResult();
                    ClearWishlistAndCart(data.Usuario);
                    unitOfWork.SaveChangesSync();
                    SendNotify(data);
                }

            }
            else
            {

                throw new Exception("Usuario no encontrado");
            }

            return new GenericResponse<bool>
            {
                Result = true
            };

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private async void SendNotify(UsuarioRedencion data)
    {
        string messageoCode = $"Mis suenos a un clic te dice que tu redencion de {data.PuntosRedimidos} puntos fue exitosa";
        await usuarioExternalService.SendSmsWithMessage(data.Usuario, messageoCode);
        await usuarioExternalService.UserSendEmailWithMessage(data);
        SendNotifyToProveedores(data);

    }

    private void SendNotifyToProveedores(UsuarioRedencion data)
    {
        var emailProveedoresFromProductosCarrito = data.ProductosCarrito?.Select(x => new { email = x.ProveedorLite?.Email, proveedor = x.ProveedorLite?.Nombres }).Distinct().ToList();
        if (emailProveedoresFromProductosCarrito == null)
        {
            return;
        }

        foreach (var item in emailProveedoresFromProductosCarrito)
        {
            var redencionPorProveedor = this.mapper.Map<UsuarioRedencion>(data);
            redencionPorProveedor.ProductosCarrito = data.ProductosCarrito?.Where(x => x.ProveedorLite?.Nombres == item.proveedor).ToList();
            redencionPorProveedor.Usuario = new Usuario
            {
                Email = item.email,
                TipoUsuario = data.Usuario.TipoUsuario
            };
            this.usuarioExternalService.UserSendEmailWithMessage(redencionPorProveedor);
        }
    }

    private async void ClearWishlistAndCart(Usuario usuario)
    {
        // obtener los datos del carrito y wishlist
        var carrito = this.unitOfWork.CarritoRepository.GetByPredicateAsync(x => x.User.Email == usuario.Email).GetAwaiter().GetResult();
        var wishlist = this.unitOfWork.WishListRepository.GetByPredicateAsync(x => x.User.Email == usuario.Email).GetAwaiter().GetResult();

        // eliminar los datos del carrito y wishlist
        foreach (var item in carrito)
        {
            this.unitOfWork.CarritoRepository.Delete(item).GetAwaiter().GetResult();
        }
        foreach (var item in wishlist)
        {
            this.unitOfWork.WishListRepository.Delete(item).GetAwaiter().GetResult();
        }

    }

    public async Task<GenericResponse<PuntosManualDto>> Update(PuntosManualDto value)
    {
        try
        {
            var response = await this.unitOfWork.UsuarioInfoPuntosRepository.GetById(value.Id ?? "");
            if (response != null)
            {
                mapper.Map(value, response);
                await this.unitOfWork.UsuarioInfoPuntosRepository.Update(response);
            }
            this.response.Result = value;
            return this.response;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<GenericResponse<UsuarioInfoPuntos>> Update(UsuarioInfoPuntos value)
    {
        try
        {
            await this.unitOfWork.UsuarioInfoPuntosRepository.Update(value);
            return new GenericResponse<UsuarioInfoPuntos>
            {
                Result = value
            };
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<GenericResponse<UsuarioInfoPuntos>> UpdateUsuarioInfoPuntos(UsuarioInfoPuntos value)
    {
        try
        {
            await this.unitOfWork.UsuarioInfoPuntosRepository.Update(value);
            return new GenericResponse<UsuarioInfoPuntos>
            {
                Result = value
            };
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<GenericResponse<WishListDto>> WishListAdd(WishListDto wishList)
    {
        try
        {
            wishList.Id = Guid.NewGuid().ToString();
            await this.unitOfWork.WishListRepository.Add(wishList);
            response2.Result = wishList;
            return response2;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<bool> WishListDeleteById(string id)
    {
        try
        {
            var wishlist = await this.unitOfWork.WishListRepository.GetById(id);
            if (wishlist != null)
            {
                await this.unitOfWork.WishListRepository.Delete(wishlist);
                return true;
            }
            return false;

        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<GenericResponse<IEnumerable<WishListDto>>> WishListGetByUser(string id)
    {
        try
        {
            var wishList = await this.unitOfWork.WishListRepository.GetByPredicateAsync(wishRepository => wishRepository.User.Email == id);
            var response = new GenericResponse<IEnumerable<WishListDto>>();
            if (wishList != null)
            {
                response.Result = wishList;
            }
            return response;
        }
        catch (Exception ex)
        {

            throw;
        }


    }

    public async Task<GenericResponse<SmsDto>> SaveCodeAndSendSms(SmsDto data)
    {
        try
        {
            var usuarioResponse = await this.usuarioExternalService.GetUserByEmail(data.Usuario?.Email ?? "");

            if (usuarioResponse.IsSuccess && usuarioResponse.Result != null)
            {
                data.Usuario = usuarioResponse.Result;
                var usuario = usuarioResponse.Result;
                if (usuario.Celular == null || usuario.Celular == "")
                {
                    throw new Exception("El usuario no tiene celular registrado");
                }
                usuario.Id = Guid.NewGuid().ToString();
                data.Codigo = FidelizacionHelper.GetCode();
                data.FechaCreacion = DateTime.Now;
                //data = await GetAndValidateCodigo(data);

                var responseSms = await this.usuarioExternalService.SendSmsWithCode(data);
                if (responseSms)
                {
                    data.Id = Guid.NewGuid().ToString();
                    await this.unitOfWork.SmsRepository.Add(data);
                    await this.unitOfWork.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Error al enviar el SMS , intente de nuevo , si persiste el problema contacte el administrador" +
                        "");
                }
            }
            this.response4.Result = data;
            return this.response4;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task<SmsDto> GetAndValidateCodigo(SmsDto data)
    {
        var codigoExiste = await this.unitOfWork.SmsRepository.GetByPredicateAsync(x => x.Codigo == data.Codigo);

        if (codigoExiste.Any())
        {
            data.Codigo = FidelizacionHelper.GetCode();
            data = await this.GetAndValidateCodigo(data);
        }

        return data;

    }

    public async Task<GenericResponse<bool>> ValidateCodeRedencion(SmsDto data)
    {
        try
        {
            var codigoExiste = await this.unitOfWork.SmsRepository.GetByPredicateAsync(x => x.Codigo == data.Codigo
            && x.Usuario.Email == data.Usuario.Email);

            if (codigoExiste.Any())
            {
                //validar tiempo del token 
                var codigo = codigoExiste.FirstOrDefault();
                if (codigo.FechaCreacion.AddMinutes(5) < DateTime.Now)
                {
                    throw new Exception("Codigo expirado");
                }
                return new GenericResponse<bool>
                {
                    Result = true
                };
            }
            else
            {
                throw new Exception("Codigo no valido");
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    public static string AsignarDepartamento(string departamento)
    {
        // Diccionario de departamentos y sus códigos
        Dictionary<string, string> codigosDepartamentos = new Dictionary<string, string>
    {
        { "ANTIOQUIA", "AN" },
        { "ATLÁNTICO", "AT" },
        { "BOLÍVAR", "BO" },
        { "BOYACÁ", "BY" },
        { "CALDAS", "CA" },
        { "CAQUETÁ", "CQ" },
        { "CAUCA", "CC" },
        { "CESAR", "CE" },
        { "CÓRDOBA", "CO" },
        { "CUNDINAMARCA", "CU" },
        { "CHOCÓ", "CH" },
        { "HUILA", "HU" },
        { "LA GUAJIRA", "GJ" },
        { "MAGDALENA", "MA" },
        { "META", "ME" },
        { "NARIÑO", "NA" },
        { "NORTE DE SANTANDER", "NS" },
        { "QUINDÍO", "QU" },
        { "RISARALDA", "RI" },
        { "SANTANDER", "SA" },
        { "SUCRE", "SU" },
        { "TOLIMA", "TO" },
        { "VALLE DEL CAUCA", "VA" },
        { "ARAUCA", "AC" },
        { "CASANARE", "CS" },
        { "PUTUMAYO", "PU" },
        { "SAN ANDRÉS", "SN" },
        { "AMAZONAS", "AM" },
        { "GUAINÍA", "GN" },
        { "GUAVIARE", "GV" },
        { "VAUPÉS", "VP" },
        { "VICHADA", "VC" }
    };

        // Verificar si el departamento está en el diccionario
        if (codigosDepartamentos.ContainsKey(departamento.ToUpper()))
        {
            return codigosDepartamentos[departamento.ToUpper()];
        }
        else
        {
            return departamento;
        }
    }


    public static string AsignarCodigo(string codigo)
    {
        // Diccionario de departamentos y sus códigos
        Dictionary<string, string> centroCosto = new Dictionary<string, string>
    {
        { "BARRANQUILLA", "060514" },
        { "BOGOTA", "030877" },
        { "CALI", "041845" },
        { "MEDELLIN", "023964" },

    };

        // Verificar si el departamento está en el diccionario
        if (centroCosto.ContainsKey(codigo.ToUpper()))
        {
            return centroCosto[codigo.ToUpper()];
        }
        else
        {
            return codigo;
        }
    }


    public async Task<GenericResponse<bool>> CreateRedencion(UsuarioRedencion data)
    {
        try
        {
            if (data.PuntosRedimidos == 0 && data.Envio.Observaciones != "Liquidacion puntos en bono")
            {
                return new GenericResponse<bool>
                {
                    Result = false,
                    Message = "No se puede redimir 0 puntos"
                };
            }
            data.Id = Guid.NewGuid().ToString();
            data.FechaRedencion = DateTime.Now;
            data.ValorMovimiento = data.ValorMovimiento * 85;
            if (data.ProductosCarrito.Any(p => p.ProveedorLite.Nit == "811044814"))
            {
                var tender = "";
                double basesubtotalConDosDecimales = Math.Round(((double)data.ProductosCarrito.Sum(x => x.Precio * x.Quantity)) / 1.19, 2);
                double subtotalConDosDecimales = Math.Round(((double)data.ProductosCarrito.Sum(x => x.Precio * x.Quantity)) / 1.19, 2);
                double taxConDosDecimales = Math.Round(((double)data.ProductosCarrito.Sum(x => x.Precio * x.Quantity) / 1.19) * 0.19, 2);
                double totalConDosDecimales = Math.Round((double)data.ProductosCarrito.Sum(x => x.Precio * x.Quantity), 2);
                if (data.Usuario.TipoUsuario == "Asesoras vendedoras")
                {
                    tender = "REL";
                }
                else
                {
                    tender = "REV";
                }
                var ordenop = new OrdenOP();
                var operationtype = new NroPedidoOP();
                var result = new ResultNroPedidoOp();
                if (data.ProductosCarrito.Any(p => p.ProveedorLite.Nit == "811044814"))
                {
                    result = this.ordenOPExternalService.GetNroOrdenOP(operationtype).GetAwaiter().GetResult();
                }
                ordenop.additionalField5 = "";
                ordenop.allowBackOrder = "y";
                ordenop.avscode = "";
                ordenop.baseSubTotal = basesubtotalConDosDecimales;
                ordenop.billingInformation = new BillingInformation
                {
                    addressLine1 = data.Envio.DireccionBasic,
                    addressLine2 = data.Envio.DireccionComplemento,
                    addressLine3 = "",
                    city = data.Envio.Ciudad,
                    colonia = "",
                    companyName = "",
                    country = "co",
                    emailAddress = data.Envio.Email,
                    firstName = data.Envio.Nombres,
                    lastName = data.Envio.Apellidos,
                    middleInitial = "",
                    municipioDelegacion = data.Envio.Ciudad,
                    phoneNumber = data.Envio.Celular,
                    stateProvince = AsignarDepartamento(data.Envio.Departamento),
                    zipCode = "",
                };

                ordenop.cid_CVV2Response = "";
                ordenop.comments = "";
                ordenop.countryCode = "574";
                ordenop.discount = 0;
                ordenop.dniID = data.Usuario.Cedula;
                ordenop.expirationDate = "";
                ordenop.freeShipping = false;
                ordenop.giftCertificateAmount = 0;
                ordenop.giftWrapping = "0";
                ordenop.invoice = "";
                ordenop.ipAddress = "";
                ordenop.languageId = 0;
                ordenop.loguinUser = "";
                if (data.Usuario.TipoUsuario == "Asesoras vendedoras")
                {
                    ordenop.macAddress = AsignarCodigo(data.Usuario.Agencia);
                }
                else
                {
                    ordenop.macAddress = "012138";
                }
                ordenop.memberId = 0;
                ordenop.message = "";
                ordenop.orderDate = DateTime.Now.ToShortDateString();
                ordenop.orderNumber = result.ParsedResult.sequentialGenerated;
                ordenop.orderRecipient = new OrderRecipient
                {
                    items = new List<Item>(),
                    address = new Address
                    {
                        addressLine1 = data.Envio.DireccionBasic,
                        addressLine2 = data.Envio.DireccionComplemento,
                        addressLine3 = "",
                        city = data.Envio.Ciudad,
                        colonia = "",
                        companyName = "",
                        country = "co",
                        emailAddress = data.Envio.Email,
                        firstName = data.Envio.Nombres,
                        lastName = data.Envio.Apellidos,
                        middleInitial = "",
                        municipioDelegacion = data.Envio.Ciudad,
                        phoneNumber = data.Envio.Celular,
                        stateProvince = AsignarDepartamento(data.Envio.Departamento),
                        zipCode = ""
                    },

                    baseSubTotal = basesubtotalConDosDecimales,
                    discount = 0,
                    giftMessageText = "",
                    giftWrapping = 0,
                    recipientId = 0,
                    shipping = "0",
                    shippingMethod = "",
                    subTotal = subtotalConDosDecimales,
                    tax = taxConDosDecimales,
                    total = totalConDosDecimales
                };
                ordenop.id = "";
                ordenop.purchaseOrder = "";
                ordenop.financialStatus = "";
                ordenop.customerId = "";
                ordenop.customerIdOrder = "";
                ordenop.currencyCodeOrder = "";
                ordenop.checkoutId = "";
                ordenop.language = "";
                ordenop.userId = "";
                ordenop.referalCode = "";
                ordenop.sourceName = "puntos";
                ordenop.shippingCarrier = "";
                ordenop.shippingTax = 0.0;
                ordenop.customerLocale = "";
                ordenop.trackingCode = "";
                ordenop.paymentId = "";
                ordenop.freeShipping = false;
                ordenop.avscode = "";
                ordenop.preauthDate = "";
                ordenop.preauthorization = "";
                ordenop.promotionCode = "";
                ordenop.shipComplete = "";
                ordenop.shipping = "0";
                ordenop.status = "";
                ordenop.subTotal = subtotalConDosDecimales;
                ordenop.tax = taxConDosDecimales;
                ordenop.tenderBank = "";
                ordenop.tenderCode = tender;
                ordenop.tenderReference = "leonisa";
                ordenop.total = totalConDosDecimales;
                ordenop.transactionId = "";

                foreach (var item in data.ProductosCarrito)
                {
                    item.Id = Guid.NewGuid().ToString();

                    if (item.ProveedorLite.Nit == "811044814")
                    {
                        var productITem = new Item()
                        {
                            barCode = item.EAN,
                            discount = 0,
                            giftCardExpirationDate = "01/01/0001",
                            giftCardFromName = "",
                            giftCardMessage = "",
                            giftCardNumber = "",
                            giftCardToEmailAddress = "",
                            giftCardToName = "",
                            giftCardVerification = 0,
                            giftQuantity = 0,
                            isGiftCard = "N",
                            isGiftWrap = "N",
                            isHardCopy = "N",
                            isOnSale = "999",
                            isTaxFree = "N",
                            itemName = item.Nombre,
                            itemPrice = (double)item.Precio,
                            price = (double)item.Precio,
                            quantity = (int)item.Quantity,
                            salePrice = (double)item.Precio,
                            sku = item.EAN
                        };

                        ordenop.orderRecipient.items.Add(productITem);
                    }
                }



                if (data.ProductosCarrito.Any(p => p.ProveedorLite.Nit == "811044814"))
                {
                    await this.ordenOPExternalService.EnviarOrdenOP(ordenop);
                }
                data.NroPedido = int.Parse(result.ParsedResult.sequentialGenerated);
            }

            foreach (var item in data.ProductosCarrito)
            {
                item.Id = Guid.NewGuid().ToString();
            }


            data.PuntosRedimidos = data.GetSumPuntos();
            if (data.ProductosCarrito.Any(p => p.ProveedorLite.Nit != "811044814"))
            {
                var redenciones = unitOfWork.UsuarioRedencionRepository.GetNroPedido() + 1;
                data.NroPedido = redenciones;
            }
            await this.unitOfWork.UsuarioRedencionRepository.Add(data);
            this.unitOfWork.SaveChangesSync();

            return new GenericResponse<bool>
            {
                Result = true
            };

        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public async Task<GenericResponse<IEnumerable<OrdenDto>>> GetUsuariosRedencionPuntos()
    {
        try
        {
            var redenciones = await this.unitOfWork.UsuarioRedencionRepository.GetAll();

            Parallel.ForEach(redenciones, (redencion) =>
            {
                redencion.Estado = redencion.GetEstadoOrden();
                foreach (var item in redencion.ProductosCarrito)
                {
                    item.Estado = item.GetEstadoOrdenItem();
                }
            });

            var OrdenesDto = mapper.Map<IEnumerable<OrdenDto>>(redenciones);
            var response = new GenericResponse<IEnumerable<OrdenDto>>();
            if (redenciones != null)
            {
                response.Result = OrdenesDto;
            }
            return response;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<GenericResponse<IEnumerable<OrdenDto>>> GetUsuariosRedencionPuntosByProveedor(string proveedor)
    {
        try
        {
            if (proveedor == null)
            {
                throw new Exception("Proveedor no puede ser nulo");
            }
            if (proveedor == "")
            {
                throw new Exception("Proveedor no puede ser vacio");
            }

            var redenciones = new List<UsuarioRedencion>();
            if (proveedor != "0")
            {
                redenciones = this.unitOfWork.UsuarioRedencionRepository.GetRedencionesWithProductsByProveedor(proveedor).ToList();

                foreach (var redencion in redenciones)
                {
                    redencion.ProductosCarrito = redencion?.ProductosCarrito?.Where(pc => pc.ProveedorLite?.Nombres == proveedor).ToList();
                }
            }
            else
            {
                redenciones = (await this.unitOfWork.UsuarioRedencionRepository.GetAll()).ToList();
            }

            Parallel.ForEach(redenciones, (redencion) =>
            {
                redencion.Estado = redencion.GetEstadoOrden();
                foreach (var item in redencion.ProductosCarrito)
                {
                    item.Estado = item.GetEstadoOrdenItem();
                }
            });


            var OrdenesDto = mapper.Map<IEnumerable<OrdenDto>>(redenciones);
            var response = new GenericResponse<IEnumerable<OrdenDto>>();
            if (redenciones != null)
            {
                response.Result = OrdenesDto.OrderByDescending(p => p.FechaRedencion);
            }
            return response;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<GenericResponse<IEnumerable<OrdenDto>>> GetUsuariosRedencionPuntosByEmail(string email)
    {
        try
        {
            if (email == null)
            {
                throw new Exception("Email no puede ser nulo");
            }
            if (email == "")
            {
                throw new Exception("Email no puede ser vacio");
            }

            var redenciones = new List<UsuarioRedencion>();

            if (email != "0")
            {
                redenciones = this.unitOfWork.UsuarioRedencionRepository.GetRedencionesWithProductsByEmail(email).ToList();
            }
            else
            {
                redenciones = (await this.unitOfWork.UsuarioRedencionRepository.GetAll()).ToList();
            }

            Parallel.ForEach(redenciones, (redencion) =>
            {
                redencion.Estado = redencion.GetEstadoOrden();
                foreach (var item in redencion.ProductosCarrito)
                {
                    item.Estado = item.GetEstadoOrdenItem();
                }
            });

            var OrdenesDto = mapper.Map<IEnumerable<OrdenDto>>(redenciones);
            var response = new GenericResponse<IEnumerable<OrdenDto>>();
            if (redenciones != null)
            {
                response.Result = OrdenesDto.OrderByDescending(p => p.FechaRedencion);
            }
            return response;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<GenericResponse<AddNroGuiaYTransportadora>> AddNroGuiaYTransportadora(AddNroGuiaYTransportadora data)
    {

        var redenciones = await this.unitOfWork.UsuarioRedencionRepository.GetById(data.Id);
        try
        {
            if (redenciones != null)
            {
                var redencionEncontrada = redenciones.ProductosCarrito.Where(x => x.Id == data.Producto.Id).FirstOrDefault();
                if (redencionEncontrada == null)
                {
                    throw new Exception("Redencion no encontrada");
                }

                redencionEncontrada.NroGuia = data.Producto.NroGuia;
                redencionEncontrada.Transportadora = data.Producto.Transportadora;
                redencionEncontrada.Estado = redencionEncontrada.GetEstadoOrdenItem();
                if (redencionEncontrada.Estado == EstadoOrdenItem.Pendiente)
                {
                    redencionEncontrada.FechaActPendiente = DateTime.Now;
                }
                if (redencionEncontrada.Estado == EstadoOrdenItem.Enviado)
                {
                    redencionEncontrada.FechaActEnviado = DateTime.Now;
                }
                if (redencionEncontrada.Estado == EstadoOrdenItem.Cancelado)
                {
                    redencionEncontrada.FechaActCancelado = DateTime.Now;
                }
                if (redencionEncontrada.Estado == EstadoOrdenItem.Entregado)
                {
                    redencionEncontrada.FechaActEntregado = DateTime.Now;
                }
                await this.unitOfWork.UsuarioRedencionRepository.Update(redenciones);
                await this.usuarioExternalService.UserSendEmailWithMessageAndState(redenciones);
                await this.unitOfWork.SaveChangesAsync();
                data.Producto = redencionEncontrada;
                var red = await this.unitOfWork.UsuarioRedencionRepository.GetById(data.Id);
                return new GenericResponse<AddNroGuiaYTransportadora>
                {

                    Result = data
                };

            }
            return null;
        }
        catch (Exception)
        {
            throw;
        }

    }

    public Task<GenericResponse<OrdenDto>> GetUsuariosRedencionPuntosById(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<GenericResponse<bool>> GuardarLiquidacionPuntos(IEnumerable<LiquidacionPuntosDto> data)
    {
        try
        {
            var emailGeneric = new EmailDTO()
            {
                Subject = "Liquidacion de puntos",
                SenderName = "Mis sueños a un clic",
                Message = "Se ha iniciado la liquidacion de puntos total registros " + data.Count().ToString(),
                Recipients = new string[] { "danielmg12361@gmail.com" }
            };

            //await this.usuarioExternalService.SendMailGeneric(emailGeneric);

            var usuariosEncontrados = new List<Usuario>();
            foreach (var item in data)
            {
                Usuario usuario = null;
                if (usuariosEncontrados.Count() > 0 && usuariosEncontrados.Any(u => u != null && u.Cedula == item.Cedula))
                {
                    usuario = usuariosEncontrados.First(u => u != null && u.Cedula == item.Cedula);
                }
                else
                {
                    var usuarioResult = this.usuarioExternalService.GetUserLiteByCedula(item.Cedula).GetAwaiter().GetResult();

                    if (usuarioResult.IsSuccess)
                    {
                        usuario = usuarioResult.Result;
                        if (usuario != null)
                        {
                            usuariosEncontrados.Add(usuario);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                if (usuario != null)
                    item.Usuario = (Usuario)usuario.Clone();
            }

            guardarLiquidacionPuntos(data);

            ////buscar la cedula y actualizar el usuarioinfopuntos
            //var cedulas = usuariosEncontrados.Select(x => x.Cedula).Distinct().ToList();
            //foreach (var cedula in cedulas)
            //{
            //    var usuario = usuariosEncontrados.Where(p => p?.Cedula == cedula).FirstOrDefault();
            //    var usuarioInfoPuntos = this.unitOfWork.UsuarioInfoPuntosRepository.GetUsuarioByEmail(usuario?.Email).GetAwaiter().GetResult();
            //    if (usuarioInfoPuntos != null)
            //    {
            //        var puntos = data.Where(x => x.Cedula == cedula).Sum(x => x.Puntos);
            //        usuarioInfoPuntos.PuntosAcumulados += (int?)puntos;
            //        usuarioInfoPuntos.PuntosDisponibles += (int?)puntos;
            //        usuarioInfoPuntos.FechaActualizacion = DateTime.Now;

            //        await this.unitOfWork.UsuarioInfoPuntosRepository.Update(usuarioInfoPuntos);



            //    }
            //    else
            //    {
            //        // create new usuarioinfopuntos

            //        var puntos = data.Where(x => x?.Cedula == cedula).Sum(x => x.Puntos);
            //        var usuarioInfoPuntosNuevo = new UsuarioInfoPuntos
            //        {
            //            Cedula = cedula,
            //            PuntosAcumulados = (int?)puntos,
            //            PuntosDisponibles = (int?)puntos,
            //            PuntosRedimidos = 0,
            //            PuntosEnCarrito = 0,
            //            Nombres = usuario.Nombres,
            //            Apellidos = usuario.Apellidos,
            //            Email = usuario.Email,
            //            FechaActualizacion = DateTime.Now

            //        };
            //        await this.unitOfWork.UsuarioInfoPuntosRepository.Add(usuarioInfoPuntosNuevo);

            //        //// add extracto and call function
            //        //var extracto = new Extractos
            //        //{
            //        //    Id = Guid.NewGuid().ToString(),
            //        //    Usuario = usuario,
            //        //    Fecha = DateTime.Now,
            //        //    ValorMovimiento = (int?)puntos,
            //        //    Descripcion = "Liquidacion de puntos",
            //        //    OrigenMovimiento = "Liquidacion de puntos",
            //        //};
            //        //this.unitOfWork.ExtractosRepository.Add(extracto);



            //    }

            //    this.unitOfWork.SaveChangesSync();

            //}

            emailGeneric = new EmailDTO()
            {
                Subject = "Liquidacion de puntos",
                SenderName = "Mis sueños a un clic",
                Message = "Se ha realizado la liquidacion de puntos total registros " + data.Count().ToString(),
                Recipients = new string[] { "danielmg12361@gmail.com" }
            };
            await this.usuarioExternalService.SendMailGeneric(emailGeneric);



            return new GenericResponse<bool>
            {
                Result = true
            };
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder(ex.Message);
            Exception auxException = ex.InnerException;
            while (auxException != null)
            {
                sb.Append(auxException.Message);
                sb.Append("-");
                auxException = auxException.InnerException;
            }

            var mensaje = $" La  cedula : {data.FirstOrDefault().Cedula} Se ha realizado la liquidacion de puntos con error {sb.ToString()}";
            var emailGeneric = new EmailDTO()
            {
                Subject = "Liquidacion de puntos",
                SenderName = "Mis sueños a un clic",
                Message = mensaje,
                Recipients = new string[] { "danielmg12361@gmail.com" }
            };
            this.usuarioExternalService.SendMailGeneric(emailGeneric);
            throw;
        }
    }

    private void guardarLiquidacionPuntos(IEnumerable<LiquidacionPuntosDto> data)
    {
        var liquidacionPuntos = data.Select(x => new FidelizacionPuntos
        {
            Anho = x.Ano,
            Mes = x.Mes,
            Porcentaje = x.Porcentaje,
            Puntos = (int?)x.Puntos,
            Publico = x.Publico,
            Id_Variable = x.Id_Variable,
            NombreVariable = x.NombreVariable,
            PuntoVenta = x.PuntoVenta,
            Usuario = x.Usuario,
            Cedula = x.Cedula
        }).ToArray();

        foreach (var item in liquidacionPuntos)
        {
            item.Id = Guid.NewGuid().ToString();
            var existe = this.unitOfWork.FidelizacionPuntosRepository.GetByPredicateAsync(p => p.PuntoVenta == item.PuntoVenta && p.Id_Variable == item.Id_Variable && p.Anho == item.Anho && p.Usuario.Cedula == item.Usuario.Cedula && p.Mes == item.Mes && p.Publico == 1).GetAwaiter().GetResult();
            if (existe == null || !existe.Any())
            {
                this.unitOfWork.FidelizacionPuntosRepository.Add(item).GetAwaiter().GetResult();

                // add extracto and call function
                var extracto = new Extractos()
                {
                    Id = Guid.NewGuid().ToString(),
                    Usuario = item.Usuario,
                    Fecha = DateTime.Now,
                    Anio = item.Anho.ToString(),
                    Mes = item.Mes.ToString(),
                    ValorMovimiento = (int?)item.Puntos,
                    Descripcion = $"En {item.PuntoVenta} con porcentaje {item.Porcentaje} para la variable {item.NombreVariable}",
                    OrigenMovimiento = "Liquidacion de puntos",
                };
                this.unitOfWork.ExtractosRepository.Add(extracto).GetAwaiter().GetResult();

            }
            else
            {
                var itemToUpdate = existe.FirstOrDefault();
                itemToUpdate.Puntos = item.Puntos;
                itemToUpdate.Porcentaje = item.Porcentaje;
                this.unitOfWork.FidelizacionPuntosRepository.Update(itemToUpdate).GetAwaiter().GetResult();
            }
        }

        this.unitOfWork.SaveChangesSync();

    }


    public async Task<GenericResponse<int>> DevolucionPuntosYCancelarEstado(DevolucionPuntosDto data)
    {
        var ordenes = await this.unitOfWork.UsuarioRedencionRepository.GetById(data.Id);
        var puntos = await this.unitOfWork.UsuarioInfoPuntosRepository.GetUsuarioByEmail(data.Email);

        try
        {
            if (ordenes != null)
            {
                var proveedor = string.Empty;
                foreach (var orden in ordenes.ProductosCarrito)
                {
                    if (orden.Id == data.Producto.Id)
                    {
                        orden.Estado = EstadoOrdenItem.Cancelado;
                        puntos.PuntosDisponibles += data.PuntosADevolver;
                        puntos.PuntosRedimidos -= data.PuntosADevolver;
                        proveedor = orden.ProveedorLite.Id;
                        break;
                    }
                }

                await this.unitOfWork.UsuarioRedencionRepository.Update(ordenes);
                await this.unitOfWork.UsuarioInfoPuntosRepository.Update(puntos);
                await this.unitOfWork.SaveChangesAsync();

                var ordenParaCorreo = this.mapper.Map(mapper.Map<UsuarioRedencion>(ordenes), new UsuarioRedencion());
                ordenParaCorreo.ProductosCarrito = ordenes.ProductosCarrito.Where(x => x.Id == data.Producto.Id).ToList();
                await this.usuarioExternalService.UserSendEmailWithMessageAndState(ordenParaCorreo);



                return new GenericResponse<int>
                {
                    Result = (int)EstadoOrdenItem.Cancelado
                };
            }

            return null;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<GenericResponse<bool>> AddExtracto(Extractos data)
    {
        try
        {
            var extractos = await this.unitOfWork.ExtractosRepository.GetById(data.Id);
            if (extractos == null)
            {
                if (data.OrigenMovimiento == "Redención" && data.ValorMovimiento == 0)
                {
                    return new GenericResponse<bool>
                    {
                        Result = false,
                        Message = "No se puede redimir 0 puntos"
                    };
                }
                data.Id = Guid.NewGuid().ToString();
                data.Fecha = DateTime.Now;
                data.Mes = DateTime.Now.Month.ToString();
                data.Anio = DateTime.Now.Year.ToString();
                await this.unitOfWork.ExtractosRepository.Add(data);
                await this.unitOfWork.SaveChangesAsync();
                return new GenericResponse<bool>
                {
                    Result = true
                };
            }
            return null;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<IEnumerable<Extractos>> GenerateExtratosByFidelizacionPuntos()
    {
        //var extractos = await this.unitOfWork.ExtractosRepository.GetByPredicateAsync(p => p.OrigenMovimiento == "Liquidacion de puntos" && p.Mes == "2");
        ////var extractosSinUsuario = extractos.Where(x => x.Usuario == null).ToList();
        var fidelizacionPuntos = await this.unitOfWork.FidelizacionPuntosRepository.GetByPredicateAsync(p => p.Mes == 2);

        //foreach (var extracto in extractos)
        //{
        //    await this.unitOfWork.ExtractosRepository.Delete(extracto);
        //    this.unitOfWork.SaveChangesSync();
        //}


        // buscar extractos en fidelizacion puntos
        //var extractosEnFidelizacion = fidelizacionPuntos.Where(x => extractos.Any(p => p.ValorMovimiento == x.Puntos && p.Usuario.Cedula == x.Cedula && p.Descripcion == $"Liquidacion de puntos en {x.PuntoVenta} con porcentaje {x.Porcentaje} para la varialbe {x.Id_Variable}")).ToList();

        // crear extractos
        var extractosCreados = new List<Extractos>();
        foreach (var item in fidelizacionPuntos)
        {
            //var usuario = await this.usuarioExternalService.GetUserByEmail(item.Usuario.Email);
            //if (usuario != null)
            //{
            var extracto = new Extractos
            {
                Id = Guid.NewGuid().ToString(),
                Usuario = item.Usuario,
                Fecha = item.FechaCreacion,
                ValorMovimiento = item.Puntos,
                Descripcion = $"Liquidacion de puntos en {item.PuntoVenta} con porcentaje {item.Porcentaje} para la varialbe {item.Id_Variable} para la cedula {item.Usuario.Cedula}",
                OrigenMovimiento = "Liquidacion de puntos",
                Mes = item.Mes.ToString(),
                Anio = item.Anho.ToString()

            };
            extractosCreados.Add(extracto);
            await this.unitOfWork.ExtractosRepository.Add(extracto);
            this.unitOfWork.SaveChangesSync();
            //}
        }

        return extractosCreados;
        // eliminar los extractos sin usuario
        //foreach (var item in extractosSinUsuario)
        //{
        //    await this.unitOfWork.ExtractosRepository.Delete(item);
        //}
        //return extractosSinUsuario;
    }

    public async Task<GenericResponse<IEnumerable<Extractos>>> GetExtractos()
    {
        var response = new GenericResponse<IEnumerable<Extractos>>();
        try
        {
            //get extractos
            var extractos = await this.unitOfWork.ExtractosRepository.GetAll();
            response.Result = extractos.OrderByDescending(p => p.Fecha);
            return response;

        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<GenericResponse<IEnumerable<bool>>> AddExtractos(Extractos[] data)
    {
        try
        {
            foreach (var extracto in data)
            {
                if (data.FirstOrDefault().OrigenMovimiento == "Puntos Adquiridos")
                {
                    var usuario = await this.usuarioExternalService.GetUserLiteByCedula(extracto.Usuario.Cedula);
                    if (usuario == null)
                    {
                        usuario = await this.usuarioExternalService.GetUserByEmail(extracto.Usuario.Email);
                    }
                    if (usuario != null)
                    {
                        extracto.Id = Guid.NewGuid().ToString();
                        extracto.Fecha = DateTime.Now;
                        extracto.Mes = DateTime.Now.Month.ToString();
                        extracto.Anio = DateTime.Now.Year.ToString();
                        extracto.Usuario = usuario.Result;
                    }
                    else
                    {
                        continue;
                    }
                }
                extracto.Id = Guid.NewGuid().ToString();
                extracto.Fecha = DateTime.Now;
                extracto.Mes = DateTime.Now.Month.ToString();
                extracto.Anio = DateTime.Now.Year.ToString();
            }
            await unitOfWork.ExtractosRepository.AddRange(data);
            await this.unitOfWork.SaveChangesAsync();
            return new GenericResponse<IEnumerable<bool>>
            {
                Result = new List<bool> { true }
            };

        }
        catch (Exception)
        {

            throw;
        }

    }

    public async Task<GenericResponse<IEnumerable<Extractos>>> GetExtractosByUsuario(string cedula)
    {
        try
        {
            var extractos = await this.unitOfWork.ExtractosRepository.GetExtractosByUsuario(cedula);
            var response = new GenericResponse<IEnumerable<Extractos>>();
            return new GenericResponse<IEnumerable<Extractos>>
            {
                Result = extractos.OrderByDescending(p => p.Fecha)
            };
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<GenericResponse<bool>> CambiarEstadoYLiquidarPuntos(string email)
    {
        try
        {
            var EANBono = "999999";
            await this.usuarioExternalService.CambiarEstado(email);
            var usuarioPuntos = this.unitOfWork.UsuarioInfoPuntosRepository.GetUsuarioByEmail(email).GetAwaiter().GetResult();
            var usuario = this.usuarioExternalService.GetUserByEmail(email).GetAwaiter().GetResult();
            if (usuarioPuntos == null && usuario != null)
            {

                // create new usuarioinfopuntos
                var extracto = await this.unitOfWork.ExtractosRepository.GetExtractosByUsuario(usuario.Result.Cedula);
                var puntos = extracto.Sum(x => x.ValorMovimiento);
                var usuarioInfoPuntosNuevo = new UsuarioInfoPuntos
                {
                    Cedula = usuario.Result.Cedula,
                    PuntosAcumulados = (int)puntos,
                    PuntosDisponibles = (int)puntos,
                    PuntosRedimidos = 0,
                    PuntosEnCarrito = 0,
                    Nombres = usuario.Result.Nombres,
                    Apellidos = usuario.Result.Apellidos,
                    Email = usuario.Result.Email,
                    FechaActualizacion = DateTime.Now

                };

                await this.unitOfWork.UsuarioInfoPuntosRepository.Add(usuarioInfoPuntosNuevo);
                await this.unitOfWork.SaveChangesAsync();
                usuarioPuntos = usuarioInfoPuntosNuevo;
            }
            var bono = this.productoExternalService.GetProductByEAN(EANBono).GetAwaiter().GetResult();
            if (usuarioPuntos != null && usuario != null && bono != null)
            {
                var redencionNueva = new UsuarioRedencion
                {
                    Id = Guid.NewGuid().ToString(),
                    FechaRedencion = DateTime.Now,
                    Usuario = usuario.Result,
                    PuntosRedimidos = usuarioPuntos.PuntosDisponibles,
                    Envio = new Domain.Model.UsuarioEnvio
                    {
                        Id = Guid.NewGuid().ToString(),
                        Nombres = usuario.Result.Nombres,
                        Celular = usuario.Result.Celular,
                        Departamento = "",
                        Ciudad = "",
                        Direccion = "",
                        Observaciones = "Liquidacion puntos en bono"
                    },
                    ProductosCarrito = new List<ProductoRefence>
                    {
                        new ProductoRefence
                        {
                            Id = bono.Result.Id,
                            EAN = bono.Result.EAN,
                            UrlImagen1 = bono.Result.UrlImagen1,
                            Caracteristicas = bono.Result.Caracteristicas,
                            Color = bono.Result.Color,
                            Descripcion = bono.Result.Descripcion,
                            Marca = bono.Result.Marca,
                            Precio = bono.Result.Precio,
                            PrecioOferta = bono.Result.PrecioOferta,
                            Proveedor = bono.Result.Proveedor,
                            Referencia = bono.Result.Referencia,
                            TiempoEntrega = bono.Result.TiempoEntrega,
                            Nombre = bono.Result.Nombre,
                            Puntos = bono.Result.Puntos,
                            ProveedorLite = bono.Result.ProveedorLite,
                            Estado = EstadoOrdenItem.Pendiente,
                            FechaCreacion = DateTime.Now,
                            Quantity = usuarioPuntos.PuntosDisponibles,
                            Correo = bono.Result.Correo,
                            CategoriaNombre = bono.Result.CategoriaNombre,
                            SubCategoriaNombre = bono.Result.SubCategoriaNombre,
                        }
                    }
                };
                redencionNueva.PuntosRedimidos = usuarioPuntos.PuntosDisponibles;
                usuarioPuntos.PuntosRedimidos += usuarioPuntos.PuntosDisponibles;
                usuarioPuntos.PuntosDisponibles = 0;

                await this.unitOfWork.UsuarioRedencionRepository.Add(redencionNueva);
                await this.unitOfWork.UsuarioInfoPuntosRepository.Update(usuarioPuntos);
                SendNotifyToProveedores(redencionNueva);
                await this.unitOfWork.SaveChangesAsync();
                return new GenericResponse<bool>
                {
                    Result = true
                };
            }
            return new GenericResponse<bool>
            {
                Result = false,
                Message = "Usuario no encontrado",
                IsSuccess = false
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

    public Task<GenericResponse<IEnumerable<UsuarioRedencion>>> GetReporteRedencion(ReporteDto data)
    {
        try
        {
            var reporteOriginal = this.unitOfWork.UsuarioRedencionRepository.GetReporteRedencion(data);
            //necesito separar las ordenes añadiendo unicamente un producto por orden
            var reporte = new List<UsuarioRedencion>();
            foreach (var item in reporteOriginal)
            {
                foreach (var producto in item.ProductosCarrito)
                {
                    if (producto.Nombre == "bono")
                    {
                        var orden = new UsuarioRedencion
                        {
                            Id = item.Id,
                            FechaRedencion = item.FechaRedencion,
                            NroPedido = item.NroPedido,
                            Usuario = item.Usuario,
                            PuntosRedimidos = (int)producto.Quantity,
                            Envio = item.Envio,
                            ProductosCarrito = new List<ProductoRefence> { producto }
                        };
                        reporte.Add(orden);
                    }
                    else
                    {

                        var orden = new UsuarioRedencion
                        {
                            Id = item.Id,
                            FechaRedencion = item.FechaRedencion,
                            NroPedido = item.NroPedido,
                            Usuario = item.Usuario,
                            PuntosRedimidos = (int)producto.Puntos,
                            Envio = item.Envio,
                            ProductosCarrito = new List<ProductoRefence> { producto }
                        };
                        reporte.Add(orden);
                    }
                }
            }
            return Task.FromResult(new GenericResponse<IEnumerable<UsuarioRedencion>>
            {
                Result = reporte
            });

        }
        catch (Exception)
        {

            throw;
        }
    }

    public Task<GenericResponse<IEnumerable<Extractos>>> UpdateMesYAño(ReporteDto data)
    {
        //Necesito traer todos los extractos y actualizar el mes y el año, el mes es igual a 1 y el año es igual a 2024
        var extractos = this.unitOfWork.ExtractosRepository.GetAll().GetAwaiter().GetResult();
        foreach (var item in extractos)
        {
            item.Mes = "1";
            item.Anio = "2024";
            this.unitOfWork.ExtractosRepository.Update(item);
        }
        this.unitOfWork.SaveChangesSync();
        return Task.FromResult(new GenericResponse<IEnumerable<Extractos>>
        {
            Result = extractos
        });
    }

    public Task<GenericResponse<IEnumerable<Extractos>>> GetExtractosByUserAndDate(ReporteDto data)
    {
        try
        {
            //crear lista de tipo Extractos
            var ext = new List<Extractos>();
            var extractos = this.unitOfWork.ExtractosRepository.GetExtractosByUserAndDate(data).GetAwaiter().GetResult();
            //if (data.TipoUsuario != "Administrador DLM" && data.TipoUsuario != "Administrador Retail")
            //{
            //    return Task.FromResult(new GenericResponse<IEnumerable<Extractos>>
            //    {
            //        Result = extractos.OrderByDescending(p => p.Fecha)
            //    });
            //}
            //foreach (var item in extractos)
            //{
            //    if(item.Usuario == null)
            //    {
            //        continue;
            //    }

            //    var user = this.usuarioExternalService.GetUserLiteByCedula(item.Usuario.Cedula).GetAwaiter().GetResult();
            //    if (user == null)
            //    {
            //        continue;
            //    }
            //    else
            //    {

            //        if (data.TipoUsuario == "Administrador DLM" && user.Result.TipoUsuario == "Lideres" || user.Result.TipoUsuario == "Lideres ZE")
            //        {
            //            ext.Add(item);
            //        }
            //        if (data.TipoUsuario == "Administrador Retail" && user.Result.TipoUsuario == "Asesoras vendedoras")
            //        {
            //            ext.Add(item);
            //        }
            //    }
            //}
            return Task.FromResult(new GenericResponse<IEnumerable<Extractos>>
            {
                Result = extractos.OrderByDescending(p => p.Fecha)
            });
        }
        catch (Exception ex)
        {

            throw ex;
        }
        
    }

    public Task<GenericResponse<IEnumerable<Extractos>>> UpdateUser()
    {
        //Necesito traer todos los extractos y actualizar el mes y el año, el mes es igual a 1 y el año es igual a 2024
        var extractos = this.unitOfWork.ExtractosRepository.GetAll().GetAwaiter().GetResult();
        foreach (var item in extractos)
        {
            var user = this.usuarioExternalService.GetUserLiteByCedula(item.Usuario.Cedula).GetAwaiter().GetResult();
            if (user != null)
            {
                item.Usuario = user.Result;
                this.unitOfWork.ExtractosRepository.Update(item);
            }

        }
        this.unitOfWork.SaveChangesSync();
        return Task.FromResult(new GenericResponse<IEnumerable<Extractos>>
        {
            Result = extractos
        });
    }

    public Task<GenericResponse<IEnumerable<UsuarioRedencion>>> UpdateEmpresaYAgencia()
    {
        var redenciones = this.unitOfWork.UsuarioRedencionRepository.GetAll().GetAwaiter().GetResult();
        foreach (var item in redenciones)
        {
            var user = this.usuarioExternalService.GetUserByEmail(item.Usuario.Email).GetAwaiter().GetResult();
            if (user != null)
            {
                item.Usuario.Empresa = user.Result.Empresa;
                item.Usuario.Agencia = user.Result.Agencia;
                this.unitOfWork.UsuarioRedencionRepository.Update(item);
            }
        }
        this.unitOfWork.SaveChangesSync();
        return Task.FromResult(new GenericResponse<IEnumerable<UsuarioRedencion>>
        {
            Result = redenciones
        });
    }

    public Task RecalcularPuntos()
    {
        var usuarioInfoPuntos = this.unitOfWork.UsuarioInfoPuntosRepository.GetAll().GetAwaiter().GetResult();
        foreach (var item in usuarioInfoPuntos)
        {
            var extractos = this.unitOfWork.ExtractosRepository.GetExtractosByUser(item.Cedula).GetAwaiter().GetResult();
            var puntosDisponibles = extractos.Sum(x => x.ValorMovimiento);
            var puntosAcumulados = extractos.Where(p => p.OrigenMovimiento == "Liquidacion de puntos" || p.OrigenMovimiento == "Puntos Adquiridos").Sum(x => x.ValorMovimiento);

            item.PuntosAcumulados = (int?)puntosAcumulados;
            item.PuntosDisponibles = (int?)puntosDisponibles;
            item.FechaActualizacion = DateTime.Now;
            this.unitOfWork.UsuarioInfoPuntosRepository.Update(item);
        }

        this.unitOfWork.SaveChangesSync();

        return Task.CompletedTask;
    }

    public async Task<GenericResponse<bool>> AddVariable(VariableDto value)
    {
        try
        {
            var variable = await this.unitOfWork.VariableRepository.GetById(value.Id);
            if (variable != null)
            {
                mapper.Map(value, variable);
                variable.FechaActualizacion = DateTime.Now;
                await this.unitOfWork.VariableRepository.Update(variable);
                await this.unitOfWork.SaveChangesAsync();
                return new GenericResponse<bool>
                {
                    Result = true
                };

            }
            else
            {
                var variableNueva = mapper.Map<Variable>(value);
                variableNueva.FechaCreacion = DateTime.Now;
                variableNueva.FechaActualizacion = DateTime.Now;
                await this.unitOfWork.VariableRepository.Add(variableNueva);
                await this.unitOfWork.SaveChangesAsync();
                return new GenericResponse<bool>
                {
                    Result = true
                };

            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    public Task<GenericResponse<bool>> DeleteVariableById(string id)
    {
        try
        {
            var variable = this.unitOfWork.VariableRepository.GetById(id).GetAwaiter().GetResult();
            if (variable != null)
            {
                this.unitOfWork.VariableRepository.Delete(variable);
                this.unitOfWork.SaveChangesSync();
                return Task.FromResult(new GenericResponse<bool>
                {
                    Result = true
                });
            }
            else
            {
                return Task.FromResult(new GenericResponse<bool>
                {
                    Message = "Variable no encontrada",
                    Result = false
                });

            }
        }
        catch (Exception)
        {

            throw;
        }

    }

    public async Task<GenericResponse<VariableDto>> GetVariableById(string id)
    {
        try
        {
            var variable = await this.unitOfWork.VariableRepository.GetById(id);
            if (variable != null)
            {
                var variableDto = mapper.Map<VariableDto>(variable);
                return new GenericResponse<VariableDto>
                {
                    Result = variableDto
                };
            }
            else
            {
                return new GenericResponse<VariableDto>
                {
                    Message = "Variable no encontrada",
                    Result = null
                };
            }
        }
        catch
        {
            throw;
        }

    }

    public async Task<GenericResponse<IEnumerable<VariableDto>>> GetVariables()
    {

        try
        {
            var variables = await this.unitOfWork.VariableRepository.GetAll();
            var variablesDto = mapper.Map<IEnumerable<VariableDto>>(variables);
            var response = new GenericResponse<IEnumerable<VariableDto>>
            {
                Result = variablesDto
            };

            return response;
        }
        catch
        {
            throw;
        }
    }

    public async Task<GenericResponse<bool>> AddVariables(Variable[] value)
    {
        try
        {
            foreach (var item in value)
            {
                await this.unitOfWork.VariableRepository.Add(item);
                await this.unitOfWork.SaveChangesAsync();
            }
            return new GenericResponse<bool>
            {
                Result = true,
                Message = "Variables creadas"
            };
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<GenericResponse<bool>> UpdateCorreoInfoPuntos(UpdateInfoDto data)
    {
        try
        {
            var response = await this.unitOfWork.UsuarioInfoPuntosRepository.GetUsuarioByCedula(data.Cedula);
            if (response != null)
            {
                response.Email = data.Email;
                await this.unitOfWork.UsuarioInfoPuntosRepository.Update(response);
                await this.unitOfWork.SaveChangesAsync();
                return new GenericResponse<bool>
                {
                    Result = true
                };
            }
            else
            {
                return new GenericResponse<bool>
                {
                    Result = false,
                    Message = "Usuario no encontrado en infopuntos"
                };

            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    public Task<GenericResponse<ReporteDto>> GetMetricasByState(ReporteDto data)
    {

        var reporteOriginal = this.unitOfWork.UsuarioRedencionRepository.GetReporteRedencion(data);
        data.ContadorPendiente = reporteOriginal.Where(x => x.ProductosCarrito.Any(p => p.Estado == EstadoOrdenItem.Pendiente)).Count();
        data.ContadorCancelado = reporteOriginal.Where(x => x.ProductosCarrito.Any(p => p.Estado == EstadoOrdenItem.Cancelado)).Count();
        data.ContadorEnviado = reporteOriginal.Where(x => x.ProductosCarrito.Any(p => p.Estado == EstadoOrdenItem.Enviado)).Count();
        data.ContadorEntregado = reporteOriginal.Where(x => x.ProductosCarrito.Any(p => p.Estado == EstadoOrdenItem.Entregado)).Count();

        return Task.FromResult(new GenericResponse<ReporteDto>
        {
            Result = data
        });

    }

    public async Task<GenericResponse<MetricasDto>> GetMetricasPorDia(ReporteDto data)
    {
        if (!data.FechaInicio.HasValue || !data.FechaFin.HasValue)
        {
            throw new ArgumentException("FechaInicio y FechaFin no pueden ser nulos.");
        }

        DateTime fechaInicio = data.FechaInicio.Value;
        DateTime fechaFin = data.FechaFin.Value;

        // Obtén el reporte original basado en el DTO de entrada
        var reporteOriginal = this.unitOfWork.UsuarioRedencionRepository.GetReporteRedencion(data);
        reporteOriginal = reporteOriginal
                .Where(x => x.ProductosCarrito.Any(p => p.Estado == EstadoOrdenItem.Pendiente))
                .ToList();
        // Crea una instancia del DTO para las métricas
        var metricas = new MetricasDto()
        {
            Contador1 = 0,
            Contador2 = 0,
            Contador3 = 0,
            Contador4 = 0,
            Contador5 = 0,
            Contador6 = 0,
            Contador7 = 0,
            Contador8 = 0,
            Contador9 = 0,
            Contador10 = 0,
            Contador11 = 0
        };


        foreach (var item in reporteOriginal)
        {
            if ((DateTime.Now - item.FechaRedencion.Value).Days == 1)
            {
                metricas.Contador1++;
            }
            if ((DateTime.Now - item.FechaRedencion.Value).Days == 2)
            {
                metricas.Contador2++;
            }
            if ((DateTime.Now - item.FechaRedencion.Value).Days == 3)
            {
                metricas.Contador3++;
            }
            if ((DateTime.Now - item.FechaRedencion.Value).Days == 4)
            {
                metricas.Contador4++;
            }
            if ((DateTime.Now - item.FechaRedencion.Value).Days == 5)
            {
                metricas.Contador5++;
            }
            if ((DateTime.Now - item.FechaRedencion.Value).Days == 6)
            {
                metricas.Contador6++;
            }
            if ((DateTime.Now - item.FechaRedencion.Value).Days == 7)
            {
                metricas.Contador7++;
            }
            if ((DateTime.Now - item.FechaRedencion.Value).Days == 8)
            {
                metricas.Contador8++;
            }
            if ((DateTime.Now - item.FechaRedencion.Value).Days == 9)
            {
                metricas.Contador9++;
            }
            if ((DateTime.Now - item.FechaRedencion.Value).Days == 10)
            {
                metricas.Contador10++;
            }
            if ((DateTime.Now - item.FechaRedencion.Value).Days > 10)
            {
                metricas.Contador11++;
            }

        }

        // Devuelve la respuesta con las métricas calculadas
        return await Task.FromResult(new GenericResponse<MetricasDto>
        {
            Result = metricas
        });
    }

    public async Task<IEnumerable<UsuarioRedencion>> GetMetricasGeneral(ReporteDto data)
    {
        try
        {
            var reporteOriginal = this.unitOfWork.UsuarioRedencionRepository.GetReporteRedencion(data);

            // Filtrar reporteOriginal unicamente por los productos que esten en estado pendiente
            reporteOriginal = reporteOriginal
                .Where(x => x.ProductosCarrito.Any(p => p.Estado == EstadoOrdenItem.Pendiente || p.Estado == EstadoOrdenItem.Enviado))
                .ToList();

            // Actualizar el contador pendiente
            foreach (var reporte in reporteOriginal)
            {
                foreach (var item in reporte.ProductosCarrito)
                {
                    if (item.ContadorPendiente == null)
                    {
                        item.ContadorPendiente = 0;
                    }
                    if (item.Estado == EstadoOrdenItem.Pendiente)
                    {
                        item.ContadorPendiente = (DateTime.Now - reporte.FechaRedencion.Value).Days;

                    }
                    else
                    {
                        continue;
                    }

                }
                await this.unitOfWork.UsuarioRedencionRepository.Update(reporte);
                await this.unitOfWork.SaveChangesAsync();

            }
            // Devolver la lista de UsuarioRedencion
            return reporteOriginal;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<GenericResponse<IEnumerable<bool>>> AddNroGuiaYTransportadoraMasivo(NroPedidoDto[] data)
    {
        try
        {
            foreach (var item in data)
            {
                var redencion = await this.unitOfWork.UsuarioRedencionRepository.GetUsuarioRedencionByNroPedido(item.NroPedido);
                var redencionEncontrada = redencion.ProductosCarrito.Where(x => x.EAN == item.Producto.EAN).FirstOrDefault();
                if (redencion != null && redencionEncontrada != null)
                {
                    redencionEncontrada.Estado = EstadoOrdenItem.Enviado;
                    redencionEncontrada.NroGuia = item.Producto.NroGuia;
                    redencionEncontrada.Transportadora = item.Producto.Transportadora;
                    redencionEncontrada.FechaActEnviado = DateTime.Now;
                    item.Producto = redencionEncontrada;
                    redencion.Estado = EstadoOrden.Enviado;
                    await this.unitOfWork.UsuarioRedencionRepository.Update(redencion);
                    await this.usuarioExternalService.UserSendEmailWithMessageAndState(redencion);
                }
                else
                {
                    continue;
                }
            }
            await this.unitOfWork.SaveChangesAsync();
            return new GenericResponse<IEnumerable<bool>>
            {
                Result = new List<bool> { true }
            };
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<GenericResponse<IEnumerable<bool>>> CambiarEstadoEntregadoMasivo(NroPedidoEntregadoDto[] data)
    {
        try
        {
            foreach (var item in data)
            {
                var redencion = await this.unitOfWork.UsuarioRedencionRepository.GetUsuarioRedencionByNroPedido(item.NroPedido);
                var redencionEncontrada = redencion.ProductosCarrito.Where(x => x.EAN == item.Producto.EAN).FirstOrDefault();
                if (redencion != null && redencionEncontrada != null)
                {
                    redencionEncontrada.Estado = EstadoOrdenItem.Entregado;
                    redencionEncontrada.FechaActEntregado = DateTime.Now;
                    item.Producto = redencionEncontrada;
                    redencion.Estado = EstadoOrden.Entregado;
                    await this.unitOfWork.UsuarioRedencionRepository.Update(redencion);
                    await this.usuarioExternalService.UserSendEmailWithMessageAndState(redencion);
                    
                }
                else
                {
                    continue;
                }
            }
            await this.unitOfWork.SaveChangesAsync();
            return new GenericResponse<IEnumerable<bool>>
            {
                Result = new List<bool> { true }
            };
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<GenericResponse<UsuarioScannerDto>> AddUsuarioScanner(PeticionCedulaDto data)
    {
        try
        {
            var usuarioScanner = await this.usuarioScannerExternalService.GetUsuarioScanner(data);
            if (usuarioScanner.codigoError == "004")
            {
                //crear lista de dataCompradoras
                var dataCompradoras = new List<DataCompradora>
                {
                    new DataCompradora
                    {
                        estado = "",
                        identificacion = data.identificacion
                    }
                };
                var UsuarioNoEncontrado = new UsuarioScanner
                {
                    Id = Guid.NewGuid().ToString(),
                    dataCompradoras = dataCompradoras
                };
                await this.unitOfWork.UsuarioScannerRepository.Add(UsuarioNoEncontrado);
                await this.unitOfWork.SaveChangesAsync();
                return new GenericResponse<UsuarioScannerDto>
                {
                    Result = mapper.Map<UsuarioScannerDto>(UsuarioNoEncontrado),
                    Message = "Usuario No Encontrado"

                };
            }
            else
            {
                if (usuarioScanner != null)
                {
                    if (usuarioScanner.dataCompradoras.FirstOrDefault().estado == "")
                    {
                        var userScanner = mapper.Map<UsuarioScanner>(usuarioScanner);
                        userScanner.Id = Guid.NewGuid().ToString();
                        await this.unitOfWork.UsuarioScannerRepository.Add(userScanner);
                        await this.unitOfWork.SaveChangesAsync();
                        return new GenericResponse<UsuarioScannerDto>
                        {
                            Result = mapper.Map<UsuarioScannerDto>(userScanner),
                            Message = "Usuario Activo"

                        };
                    }
                    else
                    {
                        var userScanner = mapper.Map<UsuarioScanner>(usuarioScanner);
                        userScanner.Id = Guid.NewGuid().ToString();
                        await this.unitOfWork.UsuarioScannerRepository.Add(userScanner);
                        await this.unitOfWork.SaveChangesAsync();
                        return new GenericResponse<UsuarioScannerDto>
                        {
                            Result = mapper.Map<UsuarioScannerDto>(userScanner),
                            Message = "Usuario Inactivo"
                        };
                    }
                }
                return new GenericResponse<UsuarioScannerDto>
                {
                    Result = null,
                    Message = "Usuario no encontrado"
                };
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<GenericResponse<DataCompradora[]>> GetUsuarioScanner()
    {
        try
        {
            var usuario = await this.unitOfWork.UsuarioScannerRepository.GetAll();
            // hacer una lista de tipo DataCompradoraDto
            var dataCompradoras = new List<DataCompradora>();
            foreach (var item in usuario)
            {
                foreach (var data in item.dataCompradoras)
                {
                    dataCompradoras.Add(data);
                }
            }
            return new GenericResponse<DataCompradora[]>
            {
                Result = mapper.Map<DataCompradora[]>(dataCompradoras)
            };
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<GenericResponse<AddNroGuiaYTransportadora>> CambiarEstadoEntregado(AddNroGuiaYTransportadora data)
    {

        var redenciones = await this.unitOfWork.UsuarioRedencionRepository.GetById(data.Id);
        try
        {
            if (redenciones != null)
            {
                var redencionEncontrada = redenciones.ProductosCarrito.Where(x => x.Id == data.Producto.Id).FirstOrDefault();
                if (redencionEncontrada == null)
                {
                    throw new Exception("Redencion no encontrada");
                }

                redencionEncontrada.Estado = EstadoOrdenItem.Entregado;
                redencionEncontrada.FechaActEntregado = DateTime.Now;
                await this.unitOfWork.UsuarioRedencionRepository.Update(redenciones);
                await this.usuarioExternalService.UserSendEmailWithMessageAndState(redenciones);
                await this.unitOfWork.SaveChangesAsync();
                data.Producto = redencionEncontrada;
                var red = await this.unitOfWork.UsuarioRedencionRepository.GetById(data.Id);
                return new GenericResponse<AddNroGuiaYTransportadora>
                {

                    Result = data
                };

            }
            return null;
        }
        catch (Exception)
        {
            throw;
        }

    }

    public async Task<GenericResponse<UsuarioRedencion>> GetUsuarioRedencionByNroPedido(UsuarioNroPedido data)
    {
        try
        {
            var redencion = await this.unitOfWork.UsuarioRedencionRepository.GetUsuarioRedencionByNroPedido(data.NroPedido);
            if (data.TipoUsuario == "Super Usuario")
            {
                return new GenericResponse<UsuarioRedencion>
                {
                    Result = redencion
                };
            }

            if (redencion.Usuario.Email != data.Email)
            {
                return new GenericResponse<UsuarioRedencion>
                {
                    Result = null,
                    Message = "Este Nro de pedido no esta asociado a este usuario"
                };
            }
            if (redencion != null)
            {
                return new GenericResponse<UsuarioRedencion>
                {
                    Result = redencion
                };
            }
            return new GenericResponse<UsuarioRedencion>
            {
                Result = null,
                Message = "Redencion no encontrada con ese Numero de pedido"
            };
        }
        catch (Exception)
        {

            throw;
        }
    }

    private static async Task UploadImageToGarantia(Garantia data)
    {
        var azureHelper = new AzureHelper("DefaultEndpointsProtocol=https;AccountName=stgactincentivos;AccountKey=mtBoBaUJu8BKcHuCfdWzk1au7Upgif0rlzD+BlfAJZBsvQ02CiGzCNG5gj1li10GF8RpUwz6h+Mj+AStMOwyTA==;EndpointSuffix=core.windows.net");
        if (!string.IsNullOrEmpty(data.Imagen1))
        {

            byte[] bytes = Convert.FromBase64String(data.Imagen1);
            data.Imagen1 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
        }
        if (!string.IsNullOrEmpty(data.Imagen2))
        {
            byte[] bytes = Convert.FromBase64String(data.Imagen2);
            data.Imagen2 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
        }
        if (!string.IsNullOrEmpty(data.Imagen3))
        {
            byte[] bytes = Convert.FromBase64String(data.Imagen3);
            data.Imagen3 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
        }
    }

    public async Task<GenericResponse<bool>> AddGarantia(Garantia data)
    {
        try
        {
            if (data.FechaRedencion < DateTime.Now.AddMonths(-6))
            {
                data.Estado = "Rechazada";
                data.Id = Guid.NewGuid().ToString();
                await UploadImageToGarantia(data);
                data.FechaReclamacion = DateTime.Now.AddHours(-5);
                data.EAN = data.EAN;
                data.ObservacionEstado = "Supera el limite de tiempo (6 Meses)";
                data.NroTicket = this.unitOfWork.GarantiaRepository.GetNroGarantia() + 1;
                await this.unitOfWork.GarantiaRepository.Add(data);
                await this.unitOfWork.SaveChangesAsync();
                return new GenericResponse<bool>
                {
                    Message = "Supera el limite de tiempo",
                    Result = false
                };
            }
            else
            {
                data.Estado = "Pendiente";
            }
            data.Id = Guid.NewGuid().ToString();
            await UploadImageToGarantia(data);
            data.FechaReclamacion = DateTime.Now.AddHours(-5);
            data.EAN = data.EAN;
            data.NroTicket = this.unitOfWork.GarantiaRepository.GetNroGarantia() + 1;
            await this.unitOfWork.GarantiaRepository.Add(data);
            await this.unitOfWork.SaveChangesAsync();
            await this.usuarioExternalService.SendMailGarantiaEnviada(data, data.CorreoProveedor);
            return new GenericResponse<bool>
            {
                Result = true
            };
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<GenericResponse<IEnumerable<GarantiaDto>>> GetGarantiasByProveedorOrAll(TipoUsuarioDto[] data)
    {
        try
        {
            var garantias = await this.unitOfWork.GarantiaRepository.GetGarantiaByProveedorOrAll(data);
            var garantiasDto = mapper.Map<IEnumerable<GarantiaDto>>(garantias);
            return new GenericResponse<IEnumerable<GarantiaDto>>
            {
                // necesito ordenar las garantias por nro de ticket de manera ascendente
                Result = garantiasDto.OrderByDescending(p => p.NroTicket)
            };
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<GenericResponse<bool>> CambiarEstadosGarantia(Garantia data)
    {
        try
        {
            var garantia = await this.unitOfWork.GarantiaRepository.GetById(data.Id);
            if (garantia != null)
            {
                garantia.Estado = data.Estado;
                garantia.ObservacionEstado = data.ObservacionEstado;
                garantia.ObservacionProveedor = data.ObservacionProveedor;
                garantia.NroGuia = data.NroGuia;
                garantia.Transportadora = data.Transportadora;
                garantia.FechaRedencion = data.FechaRedencion;
                garantia.TipoReclamacion = data.TipoReclamacion;
                await this.unitOfWork.GarantiaRepository.Update(garantia);
                await this.unitOfWork.SaveChangesAsync();
                await this.usuarioExternalService.SendMailGarantia(garantia);
            }

            return new GenericResponse<bool>
            {
                Result = true
            };
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<GenericResponse<IEnumerable<GarantiaDto>>> GetGarantiasByUser(string email)
    {
        try
        {
            var garantias = await this.unitOfWork.GarantiaRepository.GetGarantiaByEmail(email);
            var garantiasDto = mapper.Map<IEnumerable<GarantiaDto>>(garantias);
            return new GenericResponse<IEnumerable<GarantiaDto>>
            {
                Result = garantiasDto.OrderByDescending(p => p.FechaReclamacion)
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<GenericResponse<bool>> ActualizarYCrearInfoPuntos()
    {
        try
        {
            var usuario = await this.usuarioExternalService.GetUsuarios();
            foreach (var item in usuario.Result)
            {
                var usuarioInfoPuntos = await this.unitOfWork.UsuarioInfoPuntosRepository.GetUsuarioByCedula(item.Cedula);
                if (usuarioInfoPuntos == null)
                {
                    var extractos = await this.unitOfWork.ExtractosRepository.GetExtractosByUser(item.Cedula);
                    var puntosDisponibles = extractos.Sum(x => x.ValorMovimiento);
                    var puntosAcumulados = extractos.Where(p => p.OrigenMovimiento == "Liquidacion de puntos" || p.OrigenMovimiento == "Puntos Adquiridos").Sum(x => x.ValorMovimiento);
                    var usuarioInfoPuntosNuevo = new UsuarioInfoPuntos
                    {
                        Cedula = item.Cedula,
                        PuntosAcumulados = (int)puntosAcumulados,
                        PuntosDisponibles = (int)puntosDisponibles,
                        PuntosRedimidos = 0,
                        PuntosEnCarrito = 0,
                        Nombres = item.Nombres,
                        Apellidos = item.Apellidos,
                        Email = item.Email,
                        FechaActualizacion = DateTime.Now

                    };

                    await this.unitOfWork.UsuarioInfoPuntosRepository.Add(usuarioInfoPuntosNuevo);
                    await this.unitOfWork.SaveChangesAsync();
                }
                else
                {
                    var extractos = await this.unitOfWork.ExtractosRepository.GetExtractosByUser(item.Cedula);
                    var puntosDisponibles = extractos.Sum(x => x.ValorMovimiento);
                    var puntosAcumulados = extractos.Where(p => p.OrigenMovimiento == "Liquidacion de puntos" || p.OrigenMovimiento == "Puntos Adquiridos").Sum(x => x.ValorMovimiento);
                    usuarioInfoPuntos.PuntosAcumulados = (int)puntosAcumulados;
                    usuarioInfoPuntos.PuntosDisponibles = (int)puntosDisponibles;
                    usuarioInfoPuntos.FechaActualizacion = DateTime.Now;
                    await this.unitOfWork.UsuarioInfoPuntosRepository.Update(usuarioInfoPuntos);
                    await this.unitOfWork.SaveChangesAsync();
                }
            }
            return new GenericResponse<bool>
            {
                Result = true
            };
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<GenericResponse<IEnumerable<UsuarioRedencion>>> GetUsuariosByTipoUsuarioAndProveedor(TipoUsuarioDto[] data)
    {
        try
        {
            // Obtener las redenciones de la base de datos
            var redencionesTask = this.unitOfWork.UsuarioRedencionRepository.GetUsuariosRedencionPuntosByTipoUsuarioAndProveedor(data);
            var redenciones = await redencionesTask;

            // Recalcular el estado de cada redención
            foreach (var redencion in redenciones)
            {
                redencion.Estado = redencion.GetEstadoOrden();
            }

            // Ordenar por la fecha de redención
            var redencionesOrdenadas = redenciones.OrderByDescending(p => p.FechaRedencion);

            return new GenericResponse<IEnumerable<UsuarioRedencion>>
            {
                Result = redencionesOrdenadas
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<GenericResponse<bool>> AddEventoContenido(EventoContenido data)
    {

        try
        {
            var evento = await this.unitOfWork.EventoContenidoRepository.GetEventoContenidoByEvento(data);
            if (evento != null)
            {
                mapper.Map(data,evento);
                await this.unitOfWork.EventoContenidoRepository.Update(evento);
                await this.unitOfWork.SaveChangesAsync();
                return new GenericResponse<bool>
                {
                    Message = "Evento actualizado",
                    Result = true
                };
            }
            else
            {
                data.Id = Guid.NewGuid().ToString();
                await this.unitOfWork.EventoContenidoRepository.Add(data);
                await this.unitOfWork.SaveChangesAsync();
                return new GenericResponse<bool>
                {
                    Message = "Evento creado",
                    Result = true
                };
            }

        }
        catch (Exception)
        {

            throw;
        }

    }

    public Task<GenericResponse<EventoContenido>> GetEventoContenidoByEvento(EventoContenido data)
    {
        try
        {
            var eventos = this.unitOfWork.EventoContenidoRepository.GetEventoContenidoByEvento(data).GetAwaiter().GetResult();
            return Task.FromResult(new GenericResponse<EventoContenido>
            {
                Result = eventos
            });
        }
        catch (Exception)
        {

            throw;
        }
    }
}
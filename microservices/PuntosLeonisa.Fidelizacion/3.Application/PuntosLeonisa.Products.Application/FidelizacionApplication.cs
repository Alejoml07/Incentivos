using AutoMapper;
using Microsoft.Extensions.Azure;
using PuntosLeonisa.fidelizacion.Domain.Service.DTO.PuntosManuales;
using PuntosLeonisa.Fidelizacion.Application.Core.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Carrito;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.FidelizacionPuntos;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.MovimientoPuntos;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.WishList;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Service.UnitOfWork;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Helpers;
using PuntosLeonisa.Infrasctructure.Core.ExternaServiceInterfaces;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Seguridad.Application.Core;
using System.Data;

namespace PuntosLeonisa.Fidelizacion.Application;

public class FidelizacionApplication : IFidelizacionApplication
{
    private readonly IMapper mapper;
    private readonly GenericResponse<PuntosManualDto> response;
    private readonly GenericResponse<WishListDto> response2;
    private readonly IUsuarioExternalService usuarioExternalService;
    private readonly IProductoExternalService productoExternalService;
    private readonly IUnitOfWork unitOfWork;
    private readonly IUsuarioInfoPuntosApplication usuarioInfoPuntosApplication;


    private readonly GenericResponse<Carrito> response3;
    private readonly GenericResponse<SmsDto> response4;
    private readonly GenericResponse<Extractos> response5;
    public FidelizacionApplication(IMapper mapper,
        IUsuarioExternalService usuarioExternalService,
        IProductoExternalService productoExternalService,
        IUnitOfWork unitOfWork, IUsuarioInfoPuntosApplication usuarioInfoPuntosApplication
        )
    {


        this.mapper = mapper;
        this.usuarioExternalService = usuarioExternalService;
        this.productoExternalService = productoExternalService;
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
                var usuarioInfoPuntos = await this.unitOfWork.UsuarioInfoPuntosRepository.GetUsuarioByEmail(carrito.User.Email);
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
            if(!usuarioRegistrado.IsSuccess)
            {
                throw new Exception("Usuario no encontrado");
            }

            var usuarios = await this.unitOfWork.UsuarioInfoPuntosRepository.GetByPredicateAsync(p => p.Cedula == usuarioRegistrado.Result.Cedula);
            var usuario = new UsuarioInfoPuntos();
            if(usuarios == null || !usuarios.Any())
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
            var usuarioInfoPuntos = await this.unitOfWork.UsuarioInfoPuntosRepository.GetUsuarioByEmail(data.Usuario.Email);
            data.Usuario = usuarioCompleto.Result;
            if (usuarioInfoPuntos != null)
            {
                data.PuntosRedimidos = data.GetSumPuntos();
                if (usuarioInfoPuntos.PuntosDisponibles < data.PuntosRedimidos)
                {
                    throw new Exception("Puntos a redimir no pueden ser mayores a los disponibles");
                }

                var res = await this.CreateRedencion(data);
                if (res.Result)
                {
                    //validacion donde no los puntos a redimir no pueden ser mayores al disponible

                    usuarioInfoPuntos.PuntosDisponibles -= data.PuntosRedimidos;
                    usuarioInfoPuntos.PuntosRedimidos += data.PuntosRedimidos;
                    usuarioInfoPuntos.PuntosEnCarrito = 0;
                    await this.unitOfWork.UsuarioInfoPuntosRepository.Update(usuarioInfoPuntos);
                    ClearWishlistAndCart(data.Usuario);
                    unitOfWork.SaveChangesSync();
                    SendNotify(data);
                    if (data.ProductosCarrito.FirstOrDefault().Nombre != "bono")
                    {
                        await this.productoExternalService.UpdateInventory(data.ProductosCarrito.ToArray());
                    }
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
                Email = item.email
            };
            this.usuarioExternalService.UserSendEmailWithMessage(redencionPorProveedor);
        }
    }

    private async void ClearWishlistAndCart(Usuario usuario)
    {
        // obtener los datos del carrito y wishlist
        var carrito = await this.unitOfWork.CarritoRepository.GetByPredicateAsync(x => x.User.Email == usuario.Email);
        var wishlist = await this.unitOfWork.WishListRepository.GetByPredicateAsync(x => x.User.Email == usuario.Email);

        // eliminar los datos del carrito y wishlist
        foreach (var item in carrito)
        {
            await this.unitOfWork.CarritoRepository.Delete(item);
        }
        foreach (var item in wishlist)
        {
            await this.unitOfWork.WishListRepository.Delete(item);
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

    public async Task<GenericResponse<bool>> CreateRedencion(UsuarioRedencion data)
    {
        try
        {
            data.Id = Guid.NewGuid().ToString();
            data.FechaRedencion = DateTime.Now;
            foreach (var item in data.ProductosCarrito)
            {
                item.Id = Guid.NewGuid().ToString();
            }

            data.PuntosRedimidos = data.GetSumPuntos();
            var redenciones = unitOfWork.UsuarioRedencionRepository.GetNroPedido() + 1;
            data.NroPedido = redenciones;
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
                await this.unitOfWork.UsuarioRedencionRepository.Update(redenciones);
                await this.usuarioExternalService.UserSendEmailWithMessageAndState(redenciones);
                await this.unitOfWork.SaveChangesAsync();


                data.Producto = redencionEncontrada;
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

    public Task<GenericResponse<bool>> GuardarLiquidacionPuntos(IEnumerable<LiquidacionPuntosDto> data)
    {
        try
        {
            var usuariosEncontrados = new List<Usuario>();
            var datafilter = data.Take(20).ToList();
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
                    item.Usuario = usuario;
            }

            guardarLiquidacionPuntos(data);

            // buscar la cedula y actualizar el usuarioinfopuntos
            var cedulas = usuariosEncontrados.Select(x => x.Cedula).Distinct().ToList();
            foreach (var cedula in cedulas)
            {
                var usuario = usuariosEncontrados.Where(p => p?.Cedula == cedula).FirstOrDefault();
                var usuarioInfoPuntos = this.unitOfWork.UsuarioInfoPuntosRepository.GetUsuarioByEmail(usuario?.Email).GetAwaiter().GetResult();
                if (usuarioInfoPuntos != null)
                {
                    var puntos = data.Where(x => x.Cedula == cedula).Sum(x => x.Puntos);
                    usuarioInfoPuntos.PuntosAcumulados += (int?)puntos;
                    usuarioInfoPuntos.PuntosDisponibles += (int?)puntos;
                    this.unitOfWork.UsuarioInfoPuntosRepository.Update(usuarioInfoPuntos);

                    // add extracto and call function
                    var extracto = new Extractos()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Usuario = data.FirstOrDefault(x => x.Cedula == cedula).Usuario,
                        Fecha = DateTime.Now,
                        ValorMovimiento = (int?)puntos,
                        Descripcion = "Liquidacion de puntos",
                        OrigenMovimiento = "Liquidacion de puntos",
                    };
                    this.unitOfWork.ExtractosRepository.Add(extracto);

                }
                else
                {
                    // create new usuarioinfopuntos

                    var puntos = data.Where(x => x?.Cedula == cedula).Sum(x => x.Puntos);
                    var usuarioInfoPuntosNuevo = new UsuarioInfoPuntos
                    {
                        Cedula = cedula,
                        PuntosAcumulados = (int?)puntos,
                        PuntosDisponibles = (int?)puntos,
                        PuntosRedimidos = 0,
                        PuntosEnCarrito = 0,
                        Nombres = usuario.Nombres,
                        Apellidos = usuario.Apellidos,
                        Email = usuario.Email,
                        FechaActualizacion = DateTime.Now

                    };
                    this.unitOfWork.UsuarioInfoPuntosRepository.Add(usuarioInfoPuntosNuevo);

                    // add extracto and call function
                    var extracto = new Extractos
                    {
                        Id = Guid.NewGuid().ToString(),
                        Usuario = usuario,
                        Fecha = DateTime.Now,
                        ValorMovimiento = (int?)puntos,
                        Descripcion = "Liquidacion de puntos",
                        OrigenMovimiento = "Liquidacion de puntos",
                    };
                    this.unitOfWork.ExtractosRepository.Add(extracto);



                }

                this.unitOfWork.SaveChangesSync();

            }




            return Task.FromResult(new GenericResponse<bool>
            {
                Result = true
            });
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    private void guardarLiquidacionPuntos(IEnumerable<LiquidacionPuntosDto> data)
    {
        var liquidacionPuntos = data.Select(x => new FidelizacionPuntos
        {
            Id = Guid.NewGuid().ToString(),
            Anho = x.Ano,
            Mes = x.Mes,
            Porcentaje = x.Porcentaje,
            Puntos = (int?)x.Puntos,
            Publico = x.Publico,
            Id_Variable = x.Id_Variable,
            Usuario = x.Usuario
        }).ToArray();

        foreach (var item in liquidacionPuntos)
        {
            this.unitOfWork.FidelizacionPuntosRepository.Add(item);
            this.unitOfWork.SaveChangesSync();
        }
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
                if(data.FirstOrDefault().OrigenMovimiento == "Puntos Adquiridos")
                {
                    var usuario = await this.usuarioExternalService.GetUserByEmail(extracto.Usuario.Email);
                    if (usuario != null)
                    {
                        extracto.Id = Guid.NewGuid().ToString();
                        extracto.Fecha = DateTime.Now;
                        extracto.Mes = DateTime.Now.Month.ToString();
                        extracto.Anio = DateTime.Now.Year.ToString();
                        extracto.Usuario = usuario.Result;
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
            var bono = this.productoExternalService.GetProductByEAN(EANBono).GetAwaiter().GetResult();
            if (usuarioPuntos != null && usuario != null && bono != null)
            {
                var redencionNueva = new UsuarioRedencion
                {
                    Id = Guid.NewGuid().ToString(),
                    FechaRedencion = DateTime.Now,
                    Usuario = usuario.Result,
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
                await this.unitOfWork.UsuarioRedencionRepository.Add(redencionNueva);
                await this.unitOfWork.SaveChangesAsync();
                await this.usuarioExternalService.UserSendEmailWithMessage(redencionNueva);
                return new GenericResponse<bool>
                {
                    Result = true
                };
            } 
            return new GenericResponse<bool>
            {
                Result = false
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
        var extractos = this.unitOfWork.ExtractosRepository.GetExtractosByUserAndDate(data).GetAwaiter().GetResult();
        return Task.FromResult(new GenericResponse<IEnumerable<Extractos>>
        {
            Result = extractos.OrderByDescending(p => p.Fecha)
        });
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
}
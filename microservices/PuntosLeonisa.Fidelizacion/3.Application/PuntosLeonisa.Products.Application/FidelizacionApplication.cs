using AutoMapper;
using Newtonsoft.Json.Linq;
using PuntosLeonisa.fidelizacion.Domain.Service.DTO.PuntosManuales;
using PuntosLeonisa.Fidelizacion.Application.Core.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Carrito;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.WishList;
using PuntosLeonisa.Fidelizacion.Domain.Service.UnitOfWork;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Helpers;
using PuntosLeonisa.Infrasctructure.Core.ExternaServiceInterfaces;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Seguridad.Application.Core;

namespace PuntosLeonisa.Fidelizacion.Application;

public class FidelizacionApplication : IFidelizacionApplication
{
    private readonly IMapper mapper;
    private readonly GenericResponse<PuntosManualDto> response;
    private readonly GenericResponse<WishListDto> response2;
    private readonly IUsuarioExternalService usuarioExternalService;
    private readonly IUnitOfWork unitOfWork;
    private readonly IUsuarioInfoPuntosApplication usuarioInfoPuntosApplication;
    private readonly GenericResponse<Carrito> response3;
    private readonly GenericResponse<SmsDto> response4;
    public FidelizacionApplication(IMapper mapper,
        IUsuarioExternalService usuarioExternalService,
        IUnitOfWork unitOfWork, IUsuarioInfoPuntosApplication usuarioInfoPuntosApplication
        )
    {


        this.mapper = mapper;
        this.usuarioExternalService = usuarioExternalService;
        this.unitOfWork = unitOfWork;
        response = new GenericResponse<PuntosManualDto>();
        response2 = new GenericResponse<WishListDto>();
        response3 = new GenericResponse<Carrito>();
        response4 = new GenericResponse<SmsDto>();

    }

    public async Task<GenericResponse<PuntosManualDto>> Add(PuntosManualDto value)
    {
        try
        {
            //TODO: Hacer las validaciones
            var puntos = mapper.Map<PuntosManual>(value);
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
            var puntos = new List<PuntosManual>();

            foreach (var punto in value)
            {
                punto.Id = Guid.NewGuid().ToString();
                if (usersByEmail.Any(x => x.Cedula == punto.Cedula))
                {
                    var puntosUsuarioExistente = usersByEmail.Where(x => x.Cedula == punto.Cedula).FirstOrDefault();
                    puntos.Add(new PuntosManual
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
                        puntos.Add(new PuntosManual
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
                await this.unitOfWork.SaveChangesAsync();
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
            await this.unitOfWork.PuntosRepository.Delete(mapper.Map<PuntosManual>(value));
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
        var puntos = await this.unitOfWork.UsuarioInfoPuntosRepository.GetAll();
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
            var usuario = await this.unitOfWork.UsuarioInfoPuntosRepository.GetByPredicateAsync(p => p.Email == id);
            return new GenericResponse<UsuarioInfoPuntos>
            {
                Result = usuario.FirstOrDefault()
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
            var res = await this.CreateRedencion(data);
            if (res.Result)
            {

                var usuarioCompleto = this.usuarioExternalService.GetUserByEmail(data.Usuario.Email ?? "").GetAwaiter().GetResult();
                var usuarioInfoPuntos = await this.unitOfWork.UsuarioInfoPuntosRepository.GetUsuarioByEmail(data.Usuario.Email);
                data.Usuario = usuarioCompleto.Result;
                if (usuarioInfoPuntos != null)
                {
                    //validacion donde no los puntos a redimir no pueden ser mayores al disponible
                    if (usuarioInfoPuntos.PuntosDisponibles < data.PuntosRedimidos)
                    {
                        throw new Exception("Puntos a redimir no pueden ser mayores a los disponibles");
                    }
                    usuarioInfoPuntos.PuntosDisponibles -= data.PuntosRedimidos;
                    usuarioInfoPuntos.PuntosRedimidos += data.PuntosRedimidos;
                    await this.unitOfWork.UsuarioInfoPuntosRepository.Update(usuarioInfoPuntos);
                    ClearWishlistAndCart(data.Usuario);
                    unitOfWork.SaveChangesSync();
                    SendNotify(data);
                }
                else
                {
                    throw new Exception("Usuario no encontrado");
                }
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
                    redencion.ProductosCarrito = redencion.ProductosCarrito.Where(pc => pc.ProveedorLite.Nombres == proveedor).ToList();
                }
            }
            else
            {
                redenciones = (await this.unitOfWork.UsuarioRedencionRepository.GetAll()).ToList();
            }


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

    public async Task<GenericResponse<bool>> AddNroGuiaYTransportadora(OrdenDto data)
    {

        var redenciones = await this.unitOfWork.UsuarioRedencionRepository.GetById(data.Id);
        try
        {
            if(redenciones != null)
            {
                redenciones.NroGuia = data.NroGuia;
                redenciones.Transportadora = data.Transportadora;
                await this.unitOfWork.UsuarioRedencionRepository.Update(redenciones);
                await this.unitOfWork.SaveChangesAsync();
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
}
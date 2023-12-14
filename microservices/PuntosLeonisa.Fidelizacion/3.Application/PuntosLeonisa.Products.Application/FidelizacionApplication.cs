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

            var usuarioInfoPuntos = await this.unitOfWork.UsuarioInfoPuntosRepository.GetUsuarioByEmail(data.Usuario.Email);


            if (usuarioInfoPuntos != null)
            {
                usuarioInfoPuntos.PuntosDisponibles -= data.PuntosRedimidos;
                usuarioInfoPuntos.PuntosRedimidos += data.PuntosRedimidos;
                await this.unitOfWork.UsuarioInfoPuntosRepository.Update(usuarioInfoPuntos);
                unitOfWork.SaveChangesSync();
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
        catch (Exception)
        {

            throw;
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

    public static byte[] GenerateRandomSixDigitNumber()
    {

        Random random = new Random();
        var ran = random.Next(100000, 1000000);
        byte[] bytes = BitConverter.GetBytes(ran);
        return bytes;
    }

    public async Task<GenericResponse<SmsDto>> SaveCodeAndSendSms(SmsDto data)
    {
        try
        {
            var usuario = await this.unitOfWork.SmsRepository.GetById(data.Id ?? "");
            if (usuario != null)
            {
                usuario.Id = Guid.NewGuid().ToString();
                usuario.Codigo = GenerateRandomSixDigitNumber().ToString();
                await this.unitOfWork.SmsRepository.Add(usuario);
            }
            this.response4.Result = data;
            return this.response4;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
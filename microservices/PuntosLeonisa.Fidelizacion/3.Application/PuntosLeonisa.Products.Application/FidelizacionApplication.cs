using AutoMapper;
using PuntosLeonisa.fidelizacion.Domain.Service.DTO.PuntosManuales;
using PuntosLeonisa.Fidelizacion.Domain.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Carrito;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.WishList;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using PuntosLeonisa.Infrasctructure.Core.ExternaServiceInterfaces;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Seguridad.Application.Core;

namespace PuntosLeonisa.Fidelizacion.Application;

public class FidelizacionApplication : IFidelizacionApplication
{
    private readonly IMapper mapper;
    private readonly IPuntosManualRepository puntosRepository;
    private readonly GenericResponse<PuntosManualDto> response;
    private readonly IWishListRepository wishRepository;
    private readonly GenericResponse<WishListDto> response2;
    private readonly ICarritoRepository carritoRepository;
    private readonly IUsuarioExternalService usuarioExternalService;
    private readonly IUsuarioInfoPuntosRepository usuarioInfoPuntosRepository;
    private readonly GenericResponse<CarritoDto> response3;
    public FidelizacionApplication(IMapper mapper,
        IPuntosManualRepository puntosRepository,
        IWishListRepository wishRepository,
        ICarritoRepository carritoRepository,
        IUsuarioExternalService usuarioExternalService,
        IUsuarioInfoPuntosRepository usuarioInfoPuntosRepository
        )
    {
        if (puntosRepository is null)
        {
            throw new ArgumentNullException(nameof(puntosRepository));
        }

        if (wishRepository is null)
        {
            throw new ArgumentNullException(nameof(wishRepository));
        }

        this.mapper = mapper;
        this.puntosRepository = puntosRepository;
        this.wishRepository = wishRepository;
        this.carritoRepository = carritoRepository;
        this.usuarioExternalService = usuarioExternalService;
        this.usuarioInfoPuntosRepository = usuarioInfoPuntosRepository;
        response = new GenericResponse<PuntosManualDto>();
        response2 = new GenericResponse<WishListDto>();
        response3 = new GenericResponse<CarritoDto>();
    }

    public async Task<GenericResponse<PuntosManualDto>> Add(PuntosManualDto value)
    {
        try
        {
            //TODO: Hacer las validaciones
            var puntos = mapper.Map<PuntosManual>(value);
            await puntosRepository.Add(puntos);
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
                Nombres = x.FirstOrDefault().Usuario?.Nombres,
                Email = x.FirstOrDefault().Usuario?.Email,
                Apellidos = x.FirstOrDefault().Usuario?.Apellidos,
                FechaActualizacion = DateTime.Now
            }).ToArray();


            await this.AddUsuarioInfoPuntosRange(puntosPorUsuario);
            await puntosRepository.AddRange(puntos.ToArray());

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
            var usuario = await this.usuarioInfoPuntosRepository.GetById(value.Cedula);
            if (usuario != null)
            {
                usuario.PuntosAcumulados += value.PuntosAcumulados;
                await this.usuarioInfoPuntosRepository.Update(usuario);
                return new GenericResponse<UsuarioInfoPuntos>
                {
                    Result = value
                };
            }
            else
            {
                await this.usuarioInfoPuntosRepository.Add(value);
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

    public async Task<GenericResponse<CarritoDto>> CarritoAdd(CarritoDto carrito)
    {
        try
        {
            if (carrito.Id == null)
            {

                carrito.Id = Guid.NewGuid().ToString();
                await this.carritoRepository.Add(carrito);
                response3.Result = carrito;
                return response3;
            }
            else
            {
                // Update
                var carritoToUpdate = await this.carritoRepository.GetById(carrito.Id);
                if (carritoToUpdate != null)
                {
                    mapper.Map(carrito, carritoToUpdate);
                    await this.carritoRepository.Update(carritoToUpdate);
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
            var carrito = await this.carritoRepository.GetById(id);
            if (carrito != null)
            {
                await this.carritoRepository.Delete(carrito);
                return true;
            }
            return false;

        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<GenericResponse<IEnumerable<CarritoDto>>> CarritoGetByUser(string id)
    {
        try
        {
            var carrito = await this.carritoRepository.GetByPredicateAsync(carritoRepository => carritoRepository.User.Email == id);
            var response3 = new GenericResponse<IEnumerable<CarritoDto>>();
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
            await puntosRepository.Delete(mapper.Map<PuntosManual>(value));
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
            await this.usuarioInfoPuntosRepository.Delete(value);
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
            var ToDelete = await this.puntosRepository.GetById(id) ?? throw new ArgumentException("Puntos no encontrados");
            var puntosToDelete = mapper.Map<PuntosManualDto>(ToDelete);

            await puntosRepository.Delete(ToDelete);
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
            var usuario = await this.usuarioInfoPuntosRepository.GetById(id);
            if (usuario != null)
            {
                this.usuarioInfoPuntosRepository.Delete(usuario);
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
        var puntos = await puntosRepository.GetAll();
        var puntosDto = mapper.Map<PuntosManualDto[]>(puntos);
        var responseOnly = new GenericResponse<IEnumerable<PuntosManualDto>>
        {
            Result = puntosDto
        };

        return responseOnly;
    }

    public async Task<GenericResponse<PuntosManualDto>> GetById(string id)
    {
        var responseRawData = await puntosRepository.GetById(id);
        var responseData = mapper.Map<PuntosManualDto>(responseRawData);
        response.Result = responseData;

        return response;
    }

    public async Task<GenericResponse<IEnumerable<UsuarioInfoPuntos>>> GetUsuarioInfoPuntosAll()
    {
        var response = new GenericResponse<IEnumerable<UsuarioInfoPuntos>>();
        try
        {
            var usuario = await this.usuarioInfoPuntosRepository.GetAll();
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
            var usuario = await this.usuarioInfoPuntosRepository.GetById(id);
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

    public async Task<GenericResponse<PuntosManualDto>> Update(PuntosManualDto value)
    {
        try
        {
            var response = await puntosRepository.GetById(value.Id ?? "");
            if (response != null)
            {
                mapper.Map(value, response);
                await puntosRepository.Update(response);
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
            await this.usuarioInfoPuntosRepository.Update(value);
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
            await this.usuarioInfoPuntosRepository.Update(value);
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
            await this.wishRepository.Add(wishList);
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
            var wishlist = await this.wishRepository.GetById(id);
            if (wishlist != null)
            {
                await this.wishRepository.Delete(wishlist);
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
            var wishList = await this.wishRepository.GetByPredicateAsync(wishRepository => wishRepository.User.Email == id);
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

}
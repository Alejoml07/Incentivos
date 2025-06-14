﻿
using AutoMapper;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Seguridad.Application.Core;
using PuntosLeonisa.Seguridad.Domain.Interfaces;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.Communication;
using System.Text;
using Jose;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.Helpers;
using PuntosLeonisa.Infrasctructure.Core.ExternalServiceInterfaces;

namespace PuntosLeonisa.Seguridad.Application;

public class SeguridadApplication : IUsuarioApplication
{
    private readonly IMapper mapper;
    private readonly IUsuarioRepository usuarioRepository;
    private readonly ISecurityService securityService;
    private readonly ITokenRepository tokenRepository;
    private readonly IEmailExternalService emailExternalService;
    private readonly IGetUsuarioExternalService getUsuarioExternalService;
    private readonly GenericResponse<UsuarioDto> response;
    private readonly GenericResponse<TokenDto> response2;

    public SeguridadApplication(IMapper mapper, IUsuarioRepository usuarioRepository, ISecurityService securityService, ITokenRepository tokenRepository, IEmailExternalService emailExternalService, IGetUsuarioExternalService getUsuarioExternalService)
    {
        if (usuarioRepository is null)
        {
            throw new ArgumentNullException(nameof(usuarioRepository));
        }

        this.mapper = mapper;
        this.usuarioRepository = usuarioRepository;
        this.securityService = securityService;
        this.tokenRepository = tokenRepository;
        this.emailExternalService = emailExternalService;
        this.getUsuarioExternalService = getUsuarioExternalService;
        response = new GenericResponse<UsuarioDto>();
        response2 = new GenericResponse<TokenDto>();
    }

    public async Task<GenericResponse<UsuarioDto>> Add(UsuarioDto value)
    {
        try
        {

            var usuarioExist = await this.usuarioRepository.GetById(value.Cedula ?? "");
            if (usuarioExist != null)
            {
                value.Pwd = usuarioExist.Pwd;
                mapper.Map(value, usuarioExist);
                await usuarioRepository.Update(usuarioExist);
            }
            else
            {

                //TODO: Hacer las validaciones

                var usuario = mapper.Map<Usuario>(value);
                usuario.Email = usuario.Email.Trim();
                usuario.Celular = usuario.Celular.Trim();
                usuario.Cedula = usuario.Cedula.Trim();
                usuario.Id = Guid.NewGuid().ToString();
                await usuarioRepository.Add(usuario);
            }
            response.Result = value;

            return response;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }



    public async Task<GenericResponse<UsuarioDto[]>> AddRange(UsuarioDto[] value)
    {
        try
        {
            foreach (var item in value)
            {
                if (item.Pwd != null)
                    item.Pwd = securityService.HasPassword(item.Pwd.Trim());
                item.Cedula = item.Cedula.Trim();
                item.Email = item.Email.Trim();
                item.Celular = item.Celular.Trim();
            }

            var usuarios = mapper.Map<Usuario[]>(value);
            var usuariosParaAgregar = new List<Usuario>();
            var usuariosExistente = usuarioRepository.GetUsuariosByCedulas(usuarios.Select(p => p.Cedula).ToArray()).GetAwaiter().GetResult();

            // cruzar los usuarios que se van a agregar con los usuarios existentes en la base de datos.
            var usuarioCruce = usuarios.Where(x => usuariosExistente.Any(y => y.Cedula == x.Cedula)).ToArray();



            foreach (var usuario in usuarios)
            {
                // Verifica si el usuario ya existe en la base de datos.                
                if (!usuariosExistente.Any(x => x.Cedula == usuario.Cedula))
                {
                    // Si el usuario no existe, asigna un nuevo Id y lo agrega a la lista para agregar.
                    usuario.Id = Guid.NewGuid().ToString();
                    //usuariosParaAgregar.Add(usuario);
                    await usuarioRepository.Add(usuario);

                }
                else
                {
                    // Si el usuario ya existe, actualiza los datos del usuario.
                    var usuariomMap = usuariosExistente.FirstOrDefault(x => x.Cedula == usuario.Cedula);
                    mapper.Map(usuario, usuariomMap);
                    await usuarioRepository.Update(usuariomMap);
                }
            }


            var responseOnly = new GenericResponse<UsuarioDto[]>
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

    public async Task<GenericResponse<UsuarioResponseLiteDto>> Authentication(LoginDto login)
    {
        try
        {
            var usuario = await this.usuarioRepository.Login(login) ?? throw new UnauthorizedAccessException("Usuario no encontrado o Contraseña errada");

            var usuarioDto = mapper.Map<UsuarioResponseLiteDto>(usuario);
            var responseOnly = new GenericResponse<UsuarioResponseLiteDto>
            {
                Result = usuarioDto
            };

            return responseOnly;

        }
        catch (Exception)
        {

            throw;
        }
    }

    public Task<GenericResponse<UsuarioDto>> Delete(UsuarioDto value)
    {
        throw new NotImplementedException();
    }

    public async Task<GenericResponse<UsuarioDto>> DeleteById(string id)
    {
        try
        {
            var ToDelete = await this.usuarioRepository.GetById(id) ?? throw new ArgumentException("Usuario no encontrado");

            await usuarioRepository.Delete(ToDelete);
            response.Result = mapper.Map<UsuarioDto>(ToDelete);
            return response;
        }
        catch (Exception)
        {
            throw;
        }

    }

    public async Task<GenericResponse<IEnumerable<UsuarioDto>>> GetAll()
    {
        var usuarios = await usuarioRepository.GetAll();
        var usuarioDto = mapper.Map<UsuarioDto[]>(usuarios);
        var responseOnly = new GenericResponse<IEnumerable<UsuarioDto>>
        {
            Result = usuarioDto
        };

        return responseOnly;
    }

    public async Task<GenericResponse<UsuarioDto>> GetById(string id)
    {
        var responseRawData = await usuarioRepository.GetById(id);
        if (responseRawData == null)
        {
            var response2 = await this.getUsuarioExternalService.GetUsuarioByCedula(id);

            if (response2 == null)
            {
                response.IsSuccess = false;
                return response;
            }
            else
            {
                var usuario = mapper.Map<Usuario>(response2);
                usuario.Id = Guid.NewGuid().ToString();
                usuario.Email = usuario.Email.Trim();
                usuario.Celular = usuario.Celular.Trim();
                usuario.Cedula = usuario.Cedula.Trim();
                usuario.TipoUsuario = "Asesoras vendedoras";
                await usuarioRepository.Add(usuario);

                var responseData2 = mapper.Map<UsuarioDto>(usuario);
                response.Result = responseData2;

                return response;
            }
        }
        var responseData = mapper.Map<UsuarioDto>(responseRawData);
        response.Result = responseData;

        return response;
    }

    public async Task<GenericResponse<UsuarioDto>> Update(UsuarioDto value)
    {
        try
        {
            var response = await usuarioRepository.GetById(value.Cedula ?? "");
            if (response != null)
            {
                mapper.Map(value, response);
                await usuarioRepository.Update(response);
            }
            this.response.Result = value;
            return this.response;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<GenericResponse<bool>> CambiarPwd(CambioPwdDto cambioContraseñaDto)
    {
        var resultado = await usuarioRepository.CambiarPwd(cambioContraseñaDto);
        return new GenericResponse<bool> { Result = resultado };
    }

    public static string CreateToken(string email, string audience, string issuer)
    {
        var payload = new Dictionary<string, object>
                {
                    { "sub", email },
                    { "aud", audience },
                    { "iss", issuer },
                    { "exp", DateTimeOffset.UtcNow.AddHours(2).ToUnixTimeSeconds() }
                };

        var secretKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("keySecret") ?? "defaultSecret")); // Clave secreta

        string token = JWT.Encode(payload, Encoding.UTF8.GetBytes(secretKey), JwsAlgorithm.HS256);
        return token;
    }

    public async Task<GenericResponse<UsuarioResponseLiteDto>> GetByEmail(string email)
    {
        var responseRawData = await usuarioRepository.GetUsuarioByEmail(email);
        var responseData = mapper.Map<UsuarioResponseLiteDto>(responseRawData);
        var responseOnly = new GenericResponse<UsuarioResponseLiteDto>
        {
            Result = responseData
        };
        return responseOnly;
    }

    private async Task<GenericResponse<TokenDto>> GuardarToken(TokenDto data, bool isTokenEmpty = true)
    {
        var res = usuarioRepository.GetUsuarioByEmail(data.Usuario.Email).GetAwaiter().GetResult();
        var dto = this.mapper.Map<UsuarioDto>(res);
        try
        {
            data.Id = Guid.NewGuid().ToString();
            if (isTokenEmpty)
            {
                data.Token = SecurityHelper.GetCode();
            }
            data.Usuario = dto;

            this.tokenRepository.Add(data);
            response2.Result = data;
            return response2;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<GenericResponse<bool>> RecuperarPassword(UsuarioDto data)
    {
        try
        {
            var codeBase64 = Convert.ToBase64String(SecurityHelper.GenerateWithLargeCode());
            var resultToke = this.GuardarToken(new TokenDto() { Usuario = data, Token = codeBase64, Tipo = "ResetPwd" }, false);
            var urlReset = codeBase64.Replace("%/", "");
            var response = await this.emailExternalService.SendMailForResetPasswordByUser(data, urlReset);

            return new GenericResponse<bool>() { Result = true };
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    public async Task<GenericResponse<bool>> CambioRecuperarPwd(CambioRecuperarPwdDto data)
    {
        var resultado = await usuarioRepository.CambioRecuperarPwd(data);
        return new GenericResponse<bool> { Result = resultado };
    }

    public async Task<GenericResponse<UsuarioDto>> ValidarTokenCambiarContrasena(TokenDto token)
    {
        var usuario = await tokenRepository.GetUsuarioByToken(token.Token ?? "");
        try
        {
            if (usuario != null)
            {
                return new GenericResponse<UsuarioDto>() { Result = usuario?.Usuario };
            }
            return new GenericResponse<UsuarioDto>() { Result = null, IsSuccess = false, Message = "Token invalido" };
        }

        catch (Exception ex)
        {

            throw ex;
        }
    }

    public async Task<GenericResponse<bool>> ValidarCorreo(string email)
    {
        try
        {
            var exist = await usuarioRepository.GetUsuarioByEmail(email);

            if (exist == null)
            {
                var response = await this.getUsuarioExternalService.GetUsuarioPorEmail(email);

                if (response == null)
                {
                    return new GenericResponse<bool>() { IsSuccess = false };
                }
                else
                {
                    var usuario = mapper.Map<Usuario>(response);
                    var usuarioLocal = await this.usuarioRepository.GetById(response.Cedula ?? "");
                    if(usuarioLocal == null)
                    {
                        usuario.Id = Guid.NewGuid().ToString();
                        usuario.Email = usuario.Email.Trim();
                        usuario.Celular = usuario.Celular.Trim();
                        usuario.Cedula = usuario.Cedula.Trim();
                        usuario.TipoUsuario = "Asesoras vendedoras";
                        await usuarioRepository.Add(usuario);
                        return new GenericResponse<bool>() { Result = false };
                    }
                    else
                    {
                        var contra = usuarioLocal.Pwd;
                        mapper.Map(response, usuarioLocal);
                        usuarioLocal.Pwd = contra;
                        await this.usuarioRepository.Update(usuarioLocal);
                        var actCorreo = new UpdateInfoDto() { Cedula = usuarioLocal.Cedula, Email = usuarioLocal.Email.Trim().ToLower() };
                        await this.getUsuarioExternalService.UpdateCorreoInfoPuntos(actCorreo);
                        return new GenericResponse<bool>() { Result = true };
                    }
                    
                    
                }

            }

            if (exist.Pwd == null || exist.Pwd == "")
            {
                return new GenericResponse<bool>() { Result = false };
            }

            if (exist.Pwd != null && exist.Pwd != "")
            {
                var pwd2 = exist.Pwd;
                var response = await this.getUsuarioExternalService.GetUsuarioPorCedula(exist.Cedula);

                if (response != null)
                {
                    var usuarioLocal = await this.usuarioRepository.GetUsuarioByEmail(email);
                    if (usuarioLocal != null)
                    {
                        // Mapeamos las propiedades de response a usuarioLocal
                        mapper.Map(response, usuarioLocal);
                        usuarioLocal.Pwd = pwd2;
                        await this.usuarioRepository.Update(usuarioLocal);
                    }
                }
                return new GenericResponse<bool>() { Result = true };
            }


            return new GenericResponse<bool>() { IsSuccess = false };
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<GenericResponse<bool>> CambiarEstado(string email)
    {
        var usuario = await usuarioRepository.GetUsuarioByEmail(email);
        usuario.Estado = "Inactivo";
        await usuarioRepository.Update(usuario);
        return new GenericResponse<bool>() { Result = true };
    }

    public async Task<GenericResponse<IEnumerable<UsuarioBasicDto>>> GetUsuarioBasic()
    {
        var usuarios = await usuarioRepository.GetAll();
        var usuariosDto = mapper.Map<UsuarioBasicDto[]>(usuarios);
        var responseOnly = new GenericResponse<IEnumerable<UsuarioBasicDto>>
        {
            Result = usuariosDto
        };

        return responseOnly;

    }

    public Task<GenericResponse<bool>> UpdateEmailSinEspacios()
    {
        var usuarios = usuarioRepository.GetAll().GetAwaiter().GetResult();
        foreach (var item in usuarios)
        {
            item.Email = item.Email.Trim();
            item.Celular = item.Celular.Trim();
            item.Cedula = item.Cedula.Trim();
            usuarioRepository.Update(item);
        }
        return Task.FromResult(new GenericResponse<bool> { Result = true });
    }

    public async Task<GenericResponse<bool>> ResetearTodasLasContrasenas()
    {
        try
        {
            var usuarios = usuarioRepository.GetAll().GetAwaiter().GetResult();
            foreach (var item in usuarios)
            {
                item.Pwd = string.Empty;
                await usuarioRepository.Update(item);
            }

            return new GenericResponse<bool>() { Result = true };

        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    public async Task<GenericResponse<IEnumerable<Usuario>>> GetUsuariosByTipoUsuario(TiposUsuarioDto[] data)
    {
        try
        {
            var usuarios = await usuarioRepository.GetUsuariosByTipoUsuario(data);
            return new GenericResponse<IEnumerable<Usuario>>() { Result = usuarios };
        }
        catch (Exception)
        {

            throw;
        }
    }
}


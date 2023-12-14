
using AutoMapper;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Seguridad.Application.Core;
using PuntosLeonisa.Seguridad.Domain.Interfaces;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.Communication;
using System.Text;
using Jose;
using System.Net.Mail;
using System.Net;

namespace PuntosLeonisa.Seguridad.Application;

public class SeguridadApplication : IUsuarioApplication
{
    private readonly IMapper mapper;
    private readonly IUsuarioRepository usuarioRepository;
    private readonly ISecurityService securityService;
    private readonly GenericResponse<UsuarioDto> response;

    public SeguridadApplication(IMapper mapper, IUsuarioRepository usuarioRepository, ISecurityService securityService)
    {
        if (usuarioRepository is null)
        {
            throw new ArgumentNullException(nameof(usuarioRepository));
        }

        this.mapper = mapper;
        this.usuarioRepository = usuarioRepository;
        this.securityService = securityService;
        response = new GenericResponse<UsuarioDto>();
    }

    public async Task<GenericResponse<UsuarioDto>> Add(UsuarioDto value)
    {
        try
        {

            var usuarioExist = await this.usuarioRepository.GetById(value.Cedula ?? "");
            if (usuarioExist != null)
            {

                mapper.Map(value, usuarioExist);
                usuarioExist.Pwd = securityService.HasPassword(usuarioExist.Cedula);

                await usuarioRepository.Update(usuarioExist);

            }
            else
            {

                //TODO: Hacer las validaciones
                var usuario = mapper.Map<Usuario>(value);
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
            var usuarios = mapper.Map<Usuario[]>(value);
            var usuariosParaAgregar = new List<Usuario>();

            foreach (var usuario in usuarios)
            {
                // Verifica si el usuario ya existe en la base de datos.
                var usuarioExistente = await usuarioRepository.GetById(usuario.Cedula);
                if (usuarioExistente == null)
                {
                    // Si el usuario no existe, asigna un nuevo Id y lo agrega a la lista para agregar.
                    usuario.Id = Guid.NewGuid().ToString();
                    usuariosParaAgregar.Add(usuario);
                }
                // Si el usuario ya existe, simplemente continúa con el siguiente.
            }

            // Agrega todos los usuarios nuevos a la base de datos.
            if (usuariosParaAgregar.Any())
            {
                await usuarioRepository.AddRange(usuariosParaAgregar.ToArray());
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
    public void SendEmail(string recipientEmail, string code)
    {
        try
        {
            var fromAddress = new MailAddress("tuemail@gmail.com", "Tu Nombre");
            var toAddress = new MailAddress(recipientEmail, "Nombre del Destinatario");
            const string fromPassword = "TuContraseñaDeGmail";
            const string subject = "Tu Asunto Aquí";
            string body = $"Aquí va el mensaje con el código: {code}";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
        catch (Exception ex)
        {
            // Manejo de errores
            Console.WriteLine($"No se pudo enviar el correo electrónico: {ex.Message}");
        }
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
}


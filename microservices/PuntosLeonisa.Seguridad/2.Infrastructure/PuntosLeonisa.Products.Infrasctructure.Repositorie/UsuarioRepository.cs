using PuntosLeonisa.Infrasctructure.Core.Repository;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Seguridad.Domain.Interfaces;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;
using PuntosLeonisa.Seguridad.Domain.Service.Enum;
using System.Net.Mail;
using System.Net;

namespace PuntosLeonisa.Seguridad.Infrasctructure.Repositorie;
public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    internal SeguridadContext _context;
    private readonly ISecurityService securityService;

    public UsuarioRepository(SeguridadContext context, ISecurityService securityService) : base(context)
    {
        _context = context;
        this.securityService = securityService;

    }

    public async Task<Usuario?> Login(LoginDto loginDto)
    {
        // Buscar el usuario por correo

        var usuario = await _context.Set<Usuario>().FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (usuario == null)
        {
            // Usuario no encontrado
            return null;

        }

        // Verificar la contraseña

        var result = securityService.VerifyPassword(loginDto.Pwd, usuario.Pwd);

        if (result != PasswordVerifyResult.Success)
        {
            // Contraseña incorrecta
            return null;
        }



        return usuario;
    }

    public async Task<bool> CambiarPwd(CambioPwdDto cambioContraseñaDto)
    {
        var usuario = await _context.Set<Usuario>().FirstOrDefaultAsync(u => u.Email == cambioContraseñaDto.Email);
        if (usuario != null)
        {
            if (cambioContraseñaDto.NuevaContraseña == cambioContraseñaDto.ConfirmarNuevaContraseña)
            {
                var resultadoVerificacion = securityService.VerifyPassword(cambioContraseñaDto.ContraseñaActual, usuario.Pwd);
                if (resultadoVerificacion == PasswordVerifyResult.Success)
                {
                    usuario.Pwd = securityService.HasPassword(cambioContraseñaDto.NuevaContraseña);
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
        }
        return false;
    }

    public async Task<bool> RecuperarPwd(PasswordRecoveryRequestDto email)
    {
        var usuario = await _context.Set<Usuario>().FirstOrDefaultAsync(u => u.Email == email.Email);

        if (usuario != null)
        {
            usuario.Pwd = usuario.Cedula;
        }

        return true;

    }

    public async Task<bool> SendCustomEmailToUser(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<Usuario> GetUsuarioByEmail(string email)
    {
        var item = await _context.Set<Usuario>().FirstOrDefaultAsync(u => u.Email == email);
        if (item != null)
        {
            return item;
        }
        else
        {
            return null;
        }
    }

    public async Task<bool> CambioRecuperarPwd(CambioRecuperarPwdDto data)
    {
        var usuario = await _context.Set<Usuario>().FirstOrDefaultAsync(u => u.Email == data.Email);
        if (usuario != null)
        {
            if (data.NuevaContrasena == data.ConfirmarNuevaContrasena)
            {
                usuario.Pwd = securityService.HasPassword(data.NuevaContrasena);
                _context.Update(usuario);
                await _context.SaveChangesAsync();
                return true;

            }
        }
        return false;
    }

    public async Task<IEnumerable<Usuario>> GetUsuariosByCedulas(string[] cedulas)
    {
        var usuarios = _context.Set<Usuario>().Where(u => cedulas.Contains(u.Cedula));
        return usuarios.AsEnumerable();
    }
}




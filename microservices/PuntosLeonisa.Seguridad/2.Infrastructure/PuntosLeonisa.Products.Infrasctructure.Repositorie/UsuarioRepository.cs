using PuntosLeonisa.Infrasctructure.Core.Repository;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Seguridad.Domain.Interfaces;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;
using PuntosLeonisa.Seguridad.Domain.Service.Enum;

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

        var usuario = await _context.Set<Usuario>().FirstOrDefaultAsync(u => u.Correo == loginDto.Email);

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
}


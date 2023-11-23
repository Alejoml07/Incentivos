using PuntosLeonisa.Infrasctructure.Core.Repository;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Seguridad.Domain.Interfaces;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace PuntosLeonisa.Seguridad.Infrasctructure.Repositorie;
public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    internal SeguridadContext _context;
    private PasswordHasher passwordHasher = new PasswordHasher();

    public UsuarioRepository(SeguridadContext context) : base(context)
    {
        _context = context;
    }

    public async Task<LoginDto> Login(string correo, string contrasena)
    {
        // Buscar el usuario por correo
        
        Usuario usuario = await _context.Set<Usuario>().FirstOrDefaultAsync(u => u.Correo == correo);

        if (usuario == null)
        {
            // Usuario no encontrado
            return null;
        }

        // Verificar la contraseña
        PasswordVerificationResult result = passwordHasher.VerifyHashedPassword(usuario.Contrasena, contrasena);

        if (result != PasswordVerificationResult.Success)
        {
            // Contraseña incorrecta
            return null;
        }

        // Si todo es correcto, construir y devolver el DTO
        LoginDto loginDto = new LoginDto
        {
            // Asignar las propiedades necesarias al DTO
        };

        return loginDto;
    }
}


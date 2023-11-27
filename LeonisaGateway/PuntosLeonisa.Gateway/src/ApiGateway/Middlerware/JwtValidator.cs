using Jose;
using System.Text;

namespace ApiGateway.Middlerware
{
    public static class JwtValidator
    {
        public static bool ValidateToken(string token, string secretKey)
        {
            try
            {
                byte[] secretKeyBytes = Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes(secretKey))); // Clave secreta

                var payload = JWT.Decode(token, secretKeyBytes);
                // Aquí puedes añadir lógica adicional para validar los claims, expiración, etc.
                return true; // Si todo está correcto
            }
            catch (Exception)
            {
                // Si hay algún error en la validación, considerar el token como inválido
                return false;
            }
        }
    }
}

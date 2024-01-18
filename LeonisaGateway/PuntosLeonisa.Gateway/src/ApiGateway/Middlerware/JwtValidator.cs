using Jose;
using Newtonsoft.Json;
using System.Text;

namespace ApiGateway.Middlerware
{
    public static class JwtValidator
    {
        public static bool ValidateToken(string token, string secretKey,ref HttpContext context)
        {
            try
            {
                byte[] secretKeyBytes = Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes(secretKey))); // Clave secreta

                var payload =  JWT.Decode(token, secretKeyBytes);
                if// vallidar expiracion
                    (DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(JsonConvert.DeserializeObject<Dictionary<string, object>>(payload)["exp"])) < DateTimeOffset.Now)
                {
                    return false;
                }
                // Aquí puedes añadir lógica adicional para validar los claims, expiración, etc.
                var payLoadDeserialize = JsonConvert.DeserializeObject<Dictionary<string, object>>(payload);
                // añadi a los headers de la request el id del usuario
                var id = payLoadDeserialize["sub"];
                context.Request.Headers.Add("em", id.ToString());

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

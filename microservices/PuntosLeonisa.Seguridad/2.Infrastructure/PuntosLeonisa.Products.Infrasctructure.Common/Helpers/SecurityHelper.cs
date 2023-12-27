
using PuntosLeonisa.Seguridad.Domain.Service.Enum;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;
using System.Text;
using System.Security.Cryptography;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Products.Domain.Model;

namespace PuntosLeonisa.Seguridad.Infrasctructure.Common.Helpers
{
    public class SecurityHelper : ISecurityService
    {
        private readonly string key;
        public SecurityHelper(string key)
        {
            this.key = key;
        }

        public string HasPassword(string? password)
        {
            return this.GenerateHash(password ?? "");
        }

        public PasswordVerifyResult VerifyPassword(string pwdprm, string? passwordhas)
        {
            if (passwordhas == null)
            {
                throw new ArgumentNullException(nameof(passwordhas));
            }

            if (pwdprm == null)
            {
                throw new ArgumentNullException(nameof(pwdprm));
            }

            return this.GenerateHash(pwdprm) == passwordhas.ToString() ? PasswordVerifyResult.Success : PasswordVerifyResult.Failed;
        }

        private string GenerateHash(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Computar el hash del string de entrada
                byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Convertir el array de bytes en un string hexadecimal
                var stringBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    stringBuilder.Append(data[i].ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }

        public static string GetCode()
        {
            var random = new Random();
            var code = random.Next(100000, 999999);
            return code.ToString();
        }
        public static byte[] GenerateRandomSixDigitNumber()
        {

            Random random = new Random();
            var ran = random.Next(100000, 1000000);
            byte[] bytes = BitConverter.GetBytes(ran);
            return bytes;
        }

        // generate a 20 digit code
        public static byte[] GenerateWithLargeCode()
        {
            var random = new Random();
            var code = random.Next(100000000, 999999999);
            byte[] bytes = BitConverter.GetBytes(code);
            return bytes;
        }   

        public string GenerarHTML(string urlRestablecer)
        {
            var sb = new StringBuilder();

            sb.Append("<!DOCTYPE html><html lang=\"es\">");
            sb.Append("<head><meta charset=\"UTF-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"><title>Tabla para Redimir</title>");
            sb.Append(" <style>\r\n        /* Estilos opcionales para el cuerpo de la página */\r\n        body {\r\n            font-family: Arial, sans-serif;\r\n            background-color: #f4f4f4;\r\n            margin: 0;\r\n            padding: 0;\r\n        }\r\n        .container {\r\n            max-width: 600px;\r\n            margin: 20px auto;\r\n            padding: 20px;\r\n            background-color: #fff;\r\n            border: 1px solid #ddd;\r\n            border-radius: 10px;\r\n            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.05);\r\n        }\r\n        .header {\r\n            color: #333;\r\n            text-align: center;\r\n        }\r\n        .content {\r\n            line-height: 1.6;\r\n            color: #666;\r\n        }\r\n        .button {\r\n            display: block;\r\n            width: max-content;\r\n            background-color: #4CAF50;\r\n            color: #fff;\r\n            padding: 10px 20px;\r\n            text-align: center;\r\n            text-decoration: none;\r\n            border-radius: 5px;\r\n            margin: 20px auto;\r\n            font-weight: bold;\r\n        }\r\n        .footer {\r\n            text-align: center;\r\n            font-size: 0.8em;\r\n            color: #aaa;\r\n        }\r\n    </style>");
            sb.Append("</head>");
            sb.Append("<body>");

            // Contenido principal
            sb.Append("<div>");
            sb.Append("<!-- Aquí puedes agregar cualquier otro contenido HTML antes de la tabla -->");

            sb.Append("<div style=\"padding: 5%;\" class=\"\">");
            sb.Append("<img src=\"https://stgactincentivos.blob.core.windows.net/$web/img/mis%20sue%C3%B1os%20a%20un%20clic.svg?sp=r&st=2023-12-19T16:00:18Z&se=2023-12-20T00:00:18Z&spr=https&sv=2022-11-02&sr=b&sig=oZC6y4Uw3sH9%2FjTNNYgxzDqxQfKzqZAeS6clt92IS6Y%3D\" style=\"width: 100%; margin: auto; display: block;\">");
            sb.Append("</div>");
            sb.Append("<div class=\"content\">");
            sb.Append("<p>Hemos recibido una solicitud para restablecer tu contraseña. Si no has hecho esta solicitud, por favor ignora este correo. De lo contrario, puedes restablecer tu contraseña haciendo clic en el siguiente enlace:</p>");
            sb.Append($"<a href=\"{urlRestablecer}\">Restablecer mi contraseña</a>");
            sb.Append("<p>Este enlace solo será válido por las próximas 24 horas.<br> Si necesitas ayuda adicional, no dudes en contactar a nuestro equipo de soporte.</p>");
            sb.Append("  <p>Gracias,<br>" +
                "Equipo de Mis Sueños A Un Clic</p>");
            sb.Append("</div>");
            sb.Append("<div class=\"footer\">");
            sb.Append("<p>Este es un correo automático, por favor no respondas a este mensaje.</p>");
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("</body></html>");

            return sb.ToString();
        }
    }
}

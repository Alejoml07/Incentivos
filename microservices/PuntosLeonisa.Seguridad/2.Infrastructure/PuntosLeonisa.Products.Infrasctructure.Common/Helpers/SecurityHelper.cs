
using PuntosLeonisa.Seguridad.Domain.Service.Enum;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;
using System.Text;
using System.Security.Cryptography;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;

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
    }
}

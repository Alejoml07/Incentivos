

namespace Productos.FunctionHelper
{
    public class FunctionHelper
    {
        public string key;
        public FunctionHelper(string key)
        {
            this.key = key;
        }

        //public string GenerateToken(string email)
        //{
            //var tokenHandler = new JwtSecurityTokenHandler();
            //var key = Encoding.ASCII.GetBytes(this.key);
            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(new[]
            //    {
            //    new Claim(ClaimTypes.Email, email),
            //    // Agrega más claims si es necesario
            //}),
            //    Expires = DateTime.UtcNow.AddDays(7), // Expiración del token
            //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            //};

            //var token = tokenHandler.CreateToken(tokenDescriptor);
            //return tokenHandler.WriteToken(token);
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Helpers
{
    public static class FidelizacionHelper
    {
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

    }
}

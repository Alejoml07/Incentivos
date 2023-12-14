using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Fidelizacion.Domain.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.WishList;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Products.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie
{
    public class SmsRepository : Repository<SmsDto>, ISmsRepository
    {
        internal FidelizacionContext _context;
        private static readonly HttpClient httpClient = new HttpClient();
        public SmsRepository(FidelizacionContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> SendSmsAsync(SmsDto data)
        {

            var token = data.Codigo;
            var message = HttpUtility.UrlEncode(token);
            var url = $"https://api.masivapp.com/SmsHandlers/sendhandler.ashx?action=sendmessage&username=Api_7ZHR8&password=RM3MLI21TE&recipient={data.Usuario.Celular}&messagedata={token}&Message=false";

            try
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    // Opcional: Aquí puedes deserializar y manejar la respuesta si es necesario
                    // var responseObject = JsonConvert.DeserializeObject<TuObjeto>(responseContent);

                    return true;
                }
                else
                {
                    // Manejo de respuestas HTTP que no son de éxito
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error al enviar SMS: {ex.Message}");
                return false;
            }
        }

    }


}

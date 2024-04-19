using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion
{
    public class ResultDto
    {
        [JsonProperty("code")]
        public string code { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("sequentialGenerated")]
        public string sequentialGenerated { get; set; }

        [JsonProperty("operationType")]
        public string operationType { get; set; }
    }

    public class ResultNroPedidoOp
    {
        [JsonProperty("result")]
        public  string  result { get; set; }
        public ResultDto ParsedResult => JsonConvert.DeserializeObject<ResultDto>(result);

    }
}

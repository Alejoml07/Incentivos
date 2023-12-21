using PuntosLeonisa.Seguridad.Infrasctructure.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Infraestructure.Core.Agent.Interfaces
{
    public interface IHttpClientAgent
    {
        Task<bool> SendMail(EmailDTO email);
    }
}

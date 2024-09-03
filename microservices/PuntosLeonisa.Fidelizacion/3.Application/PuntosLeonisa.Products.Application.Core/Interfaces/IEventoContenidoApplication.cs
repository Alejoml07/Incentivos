using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Garantias;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Application.Core.Interfaces
{
    public interface IEventoContenidoApplication
    {
        Task<GenericResponse<bool>> AddEventoContenido(EventoContenido data);
        Task<GenericResponse<EventoContenido>> GetEventoContenidoByEvento(EventoContenido data);

    }
}

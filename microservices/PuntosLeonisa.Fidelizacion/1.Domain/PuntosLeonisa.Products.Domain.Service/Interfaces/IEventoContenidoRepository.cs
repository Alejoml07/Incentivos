﻿using PuntosLeonisa.Fidelizacion.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces
{
    public interface IEventoContenidoRepository: IRepository<EventoContenido>
    {
        Task<EventoContenido> GetEventoContenidoByEvento(EventoContenido data);
    }
}

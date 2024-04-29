using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Seguridad.Domain.Model;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Seguridad.Infrasctructure.Repositorie
{
    public class TratamientoDatosRepository : Repository<TratamientoDatosDto>, ITratamientoDatosRepository
    {
        internal SeguridadContext _context;
        public TratamientoDatosRepository(SeguridadContext context) : base(context)
        {
            _context = context;
        }
    }
}

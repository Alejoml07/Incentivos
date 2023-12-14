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
        public SmsRepository(FidelizacionContext context) : base(context)
        {
            _context = context;
        }


    }


}

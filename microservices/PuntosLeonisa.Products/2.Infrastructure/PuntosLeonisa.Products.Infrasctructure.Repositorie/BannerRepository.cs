using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Products.Domain.Interfaces;
using PuntosLeonisa.Products.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuntosLeonisa.Products.Domain.Service.DTO.Banners;
using PuntosLeonisa.Products.Domain.Service.Interfaces;

namespace PuntosLeonisa.Products.Infrasctructure.Repositorie
{
    public class BannerRepository : Repository<Banner>, IBannerRepository
    {
        private readonly ProductContext _context;
        public BannerRepository(ProductContext context) : base(context)
        {
            _context = context;
        }

        public Task<Banner> GetBannersByUserType(Banner data)
        {
            var banner = _context.Set<Banner>().Where(x => x.TipoUsuario == data.TipoUsuario).FirstOrDefault();
            return Task.FromResult(banner);
        }
    }
}

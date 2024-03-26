using PuntosLeonisa.Products.Domain.Service.DTO.Banners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Products.Domain.Service.Interfaces
{
    public interface IBannerRepository : IRepository<Banner>
    {
        Task<Banner> GetBannersByUserType(Banner data);
    }
}

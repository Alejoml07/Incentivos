using System;
using AutoMapper;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Products.Domain.Service.DTO.Genericos;
using PuntosLeonisa.Products.Domain.Service.DTO.Productos;

namespace PuntosLeonisa.Products.Domain.Service.Mapping
{
    public class AutoMappingProfile : Profile
    {
        public AutoMappingProfile()
        {
            // Se hace el mapeo doble pero birideccional

            //Producto
            CreateMap<Producto, ProductoDto>();
            CreateMap<ProductoDto, Producto>();
            CreateMap<PagedResult<Producto>, PagedResult<ProductoDto>>();
            CreateMap<Producto, FiltroDto>();
            CreateMap<FiltroDto, Producto>();
            CreateMap<ProductoRefence, Producto>();
            CreateMap<Producto, ProductoRefence>();
            CreateMap<Producto, ProductoDto>();
            CreateMap<ProductoDto, Producto>();
            CreateMap<ProveedorLite, ProveedorLiteDto>();
            CreateMap<ProveedorLiteDto, ProveedorLite>();
            CreateMap<ProductoDto, ProveedorLite>();
            CreateMap<ProveedorLite, ProductoDto>();



            //TODO: Hacer el de usuario
        }
    }
}


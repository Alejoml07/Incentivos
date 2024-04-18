using System;
using AutoMapper;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Seguridad.Domain.Model;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.PuntosDeVenta;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;

namespace PuntosLeonisa.Seguridad.Domain.Service.Mapping
{
    public class AutoMappingProfile : Profile
    {
        public AutoMappingProfile()
        {
            // Se hace el mapeo doble pero birideccional

            //Producto
            CreateMap<Usuario, UsuarioDto>();
            CreateMap<Usuario, UsuarioResponseLiteDto>();
            CreateMap<UsuarioDto, Usuario>();
            CreateMap<Proveedor, ProveedorDto>();
            CreateMap<ProveedorDto, Proveedor>();
            CreateMap<UsuarioBasicDto, Usuario>();
            CreateMap<Usuario, UsuarioBasicDto>();
            CreateMap<PuntoDeVenta, PuntoDeVentaDto>();
            CreateMap<PuntoDeVentaDto, PuntoDeVenta>();


            //TODO: Hacer el de usuario
        }
    }
}


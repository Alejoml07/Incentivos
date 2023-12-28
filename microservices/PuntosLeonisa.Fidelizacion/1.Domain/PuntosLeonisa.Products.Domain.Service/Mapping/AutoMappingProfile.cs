using AutoMapper;
using PuntosLeonisa.fidelizacion.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.fidelizacion.Domain.Service.DTO.PuntosManuales;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;
using UsuarioEnvio = PuntosLeonisa.Fidelizacion.Domain.Model.UsuarioEnvio;

namespace PuntosLeonisa.Seguridad.Domain.Service.Mapping
{
    public class AutoMappingProfile : Profile
    {
        public AutoMappingProfile()
        {
            // Se hace el mapeo doble pero birideccional

            //Producto
            CreateMap<Usuario, UsuarioDto>();
            CreateMap<UsuarioDto, Usuario>();
            CreateMap <PuntosManual,PuntosManualDto>();
            CreateMap<PuntosManualDto, PuntosManual>();
            CreateMap<UsuarioRedencion, OrdenDto>();
            CreateMap<ProductoRefence, ProductoCarritoLite>();
            CreateMap<Usuario, UsuarioDtoLite>();
            CreateMap<UsuarioEnvio, Fidelizacion.Domain.Service.DTO.Redencion.UsuarioEnvio>();

            CreateMap<OrdenDto, UsuarioRedencion>();
            //TODO: Hacer el de usuario
        }
    }
}


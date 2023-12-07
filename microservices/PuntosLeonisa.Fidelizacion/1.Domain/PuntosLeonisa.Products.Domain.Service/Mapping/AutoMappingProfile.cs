using AutoMapper;
using PuntosLeonisa.fidelizacion.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.fidelizacion.Domain.Service.DTO.PuntosManuales;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Model;

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
            //TODO: Hacer el de usuario
        }
    }
}


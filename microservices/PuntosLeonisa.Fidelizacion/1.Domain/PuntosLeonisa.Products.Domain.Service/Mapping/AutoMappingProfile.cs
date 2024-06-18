using AutoMapper;
using PuntosLeonisa.fidelizacion.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.fidelizacion.Domain.Service.DTO.PuntosManuales;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;
using UsuarioEnvio = PuntosLeonisa.Fidelizacion.Domain.Model.UsuarioEnvio;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Variables;
using PuntosLeonisa.Fidelizacion.Domain.Model.Carrito;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.PuntoDeVenta;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Scanner;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Garantias;

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
            CreateMap <MovimientoPuntos,PuntosManualDto>();
            CreateMap<PuntosManualDto, MovimientoPuntos>();
            CreateMap<UsuarioInfoPuntos, PuntosManualDto>();
            CreateMap<PuntosManualDto, UsuarioInfoPuntos>();
            CreateMap<UsuarioRedencion, OrdenDto>();
            CreateMap<ProductoRefence, ProductoCarritoLite>();
            CreateMap<ProductoCarritoLite, ProductoRefence>();
            CreateMap<ProveedorLite, ProveedorLite>();
            CreateMap<Usuario, UsuarioDtoLite>();
            CreateMap<UsuarioDtoLite, Usuario>();
            CreateMap<UsuarioEnvio, Fidelizacion.Domain.Service.DTO.Redencion.UsuarioEnvio>();
            CreateMap<Variable, VariableDto>();
            CreateMap<VariableDto, Variable>();
            CreateMap<PuntoDeVenta, PuntoDeVentaDto>();
            CreateMap<PuntoDeVentaDto, PuntoDeVenta>();
            CreateMap<PuntoVentaVar, PuntoVentaVarDto>();
            CreateMap<PuntoVentaVarDto, PuntoVentaVar>();
            CreateMap<Asignacion, AsignacionDto>();
            CreateMap<AsignacionDto, Asignacion>();
            CreateMap<PuntoVentaHistoria, PuntoVentaHistoriaDto>();
            CreateMap<PuntoVentaHistoriaDto, PuntoVentaHistoria>();
            CreateMap<UsuarioScanner, UsuarioScannerDto>();
            CreateMap<UsuarioScannerDto, UsuarioScanner>();
            CreateMap<DataCompradora, DataCompradoraDto>();
            CreateMap<DataCompradoraDto, DataCompradora>();
            CreateMap<OrdenDto, UsuarioRedencion>();
            CreateMap<Garantia, GarantiaDto>();
            CreateMap<GarantiaDto, Garantia>();
            //TODO: Hacer el de usuario
        }
    }
}


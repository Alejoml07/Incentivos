using System;
using AutoMapper;
using PuntosLeonisa.Products.Domain.Service.DTO.Productos;

namespace PuntosLeonisa.Products.Domain.Service.Mapping
{
    public class AutoMappingProfile:Profile
	{
		public AutoMappingProfile()
		{
			// Se hace el mapeo doble pero birideccional

			//Producto
			CreateMap<Producto, ProductoDto>();
			CreateMap<ProductoDto, Producto>();


            //TODO: Hacer el de usuario
        }
	}
}


using System;
using FluentValidation;
using PuntosLeonisa.Products.Domain.Service.DTO.Productos;

namespace PuntosLeonisa.Products.Domain.Service.Validators
{
    public class ProductoDtoValidators: AbstractValidator<ProductoDto>
    {
		public ProductoDtoValidators()
		{

            //TODO agregar las reglas
            //Ejemplo:
            RuleFor(m => m.Nombre).NotEmpty()
             .WithMessage("El campo Nombre es requerido");
        }
	}
}


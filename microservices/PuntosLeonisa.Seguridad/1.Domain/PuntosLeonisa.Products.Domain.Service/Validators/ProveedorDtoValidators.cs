using System;
using FluentValidation;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;

namespace PuntosLeonisa.Seguridad.Domain.Service.Validators
{
    public class ProveedorDtoValidators : AbstractValidator<ProveedorDto>
    {
        public ProveedorDtoValidators()
        {

            //TODO agregar las reglas
            //Ejemplo:
            RuleFor(m => m.Nit).NotEmpty()
             .WithMessage("El campo Nombre es requerido");
        }
    }
}

using System;
using FluentValidation;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;

namespace PuntosLeonisa.Seguridad.Domain.Service.Validators
{
    public class UsuarioDtoValidators : AbstractValidator<UsuarioDto>
    {
        public UsuarioDtoValidators()
        {

            //TODO agregar las reglas
            //Ejemplo:
            RuleFor(m => m.Nombres).NotEmpty()
             .WithMessage("El campo Nombre es requerido");
        }
    }
}


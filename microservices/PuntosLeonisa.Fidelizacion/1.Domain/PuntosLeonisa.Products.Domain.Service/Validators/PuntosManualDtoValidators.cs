using FluentValidation;
using PuntosLeonisa.fidelizacion.Domain.Service.DTO.PuntosManuales;


namespace PuntosLeonisa.Seguridad.Domain.Service.Validators
{
    public class PuntosManualDtoValidators : AbstractValidator<PuntosManualDto>
    {
        public PuntosManualDtoValidators()
        {

            //TODO agregar las reglas
            //Ejemplo:
            RuleFor(m => m.Cedula).NotEmpty()
             .WithMessage("El campo Nombre es requerido");
        }
    }
}

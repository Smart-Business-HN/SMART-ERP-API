using FluentValidation;

namespace SMART.ERP.Application.Features.MachineryFeature.Commands.CreateHourmeterCommand
{
    public class CreateHourmeterCommandValidator : AbstractValidator<CreateHourmeterCommand>
    {
        public CreateHourmeterCommandValidator()
        {
            RuleFor(x => x.Hourmeter)
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(x => x.MachineryId)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(x => x.CreationDate)
                .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}

using FluentValidation;

namespace SMART.ERP.Application.Features.TypeStatusFeature.Commands.UpdateTypeStatusCommand
{
    public class UpdateTypeStatusCommandValidator : AbstractValidator<UpdateTypeStatusCommand>
    {
        public UpdateTypeStatusCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}

using FluentValidation;

namespace SMART.ERP.Application.Features.PrefixFeature.Command.UpdatePrefixCommand
{
    public class UpdatePrefixCommandValidator : AbstractValidator<UpdatePrefixCommand>
    {
        public UpdatePrefixCommandValidator()
        {
            RuleFor(p => p.Id)
              .NotEmpty().WithMessage("{PropertyName} no puede ser nulo")
              .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
            RuleFor(p => p.Format)
              .NotEmpty().WithMessage("{PropertyName} no puede ser nulo")
              .MaximumLength(50).WithMessage("{PropertyName} no puede ser mayor a 50 caracteres");
            RuleFor(p => p.InternalDocumentId)
              .NotEmpty().WithMessage("{PropertyName} no puede ser nulo")
              .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
            RuleFor(p => p.ItIsTaken)
              .NotEmpty().WithMessage("{PropertyName} no puede ser nulo");
        }
    }
}

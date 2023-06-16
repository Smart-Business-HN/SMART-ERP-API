using FluentValidation;

namespace SMART.ERP.Application.Features.ProspectFeature.Commands.ConvertProspectCommand
{
    public class ConvertProspectCommandValidator : AbstractValidator<ConvertProspectCommand>
    {
        public ConvertProspectCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}

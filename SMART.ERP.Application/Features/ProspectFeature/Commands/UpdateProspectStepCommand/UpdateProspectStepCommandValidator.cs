using FluentValidation;

namespace SMART.ERP.Application.Features.ProspectFeature.Commands.UpdateProspectStepCommand
{
    public class UpdateProspectStepCommandValidator : AbstractValidator<UpdateProspectStepCommand>
    {
        public UpdateProspectStepCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage("{PropertyName} es requerido")
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.NotQualified)
                .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}

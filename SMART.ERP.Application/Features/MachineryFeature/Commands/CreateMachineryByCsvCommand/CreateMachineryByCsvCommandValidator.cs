using FluentValidation;

namespace SMART.ERP.Application.Features.MachineryFeature.Commands.CreateMachineryByCsvCommand
{
    public class CreateMachineryByCsvCommandValidator : AbstractValidator<CreateMachineryByCsvCommand>
    {
        public CreateMachineryByCsvCommandValidator()
        {
            RuleFor(x => x.ExcelFile)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo");
        }
    }
}

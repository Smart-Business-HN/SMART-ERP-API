using FluentValidation;

namespace SMART.ERP.Application.Features.FiscalPeriodFeature.Commands.CreateFiscalYearCommand
{
    public class CreateFiscalYearCommandValidator : AbstractValidator<CreateFiscalYearCommand>
    {
        public CreateFiscalYearCommandValidator()
        {
            RuleFor(x => x.Year)
                .InclusiveBetween(2000, 2100).WithMessage("El año del ejercicio debe estar entre 2000 y 2100");
        }
    }
}

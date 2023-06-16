using FluentValidation;

namespace SMART.ERP.Application.Features.CustomerFeature.Commands.ImportCustomerCommand
{
    public class ImportCustomerCommandValidator : AbstractValidator<ImportCustomerCommand>
    {
        public ImportCustomerCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}

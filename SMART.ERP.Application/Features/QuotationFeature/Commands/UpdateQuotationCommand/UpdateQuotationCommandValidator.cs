using FluentValidation;

namespace SMART.ERP.Application.Features.QuotationFeature.Commands.UpdateQuotationCommand
{
    public class UpdateQuotationCommandValidator : AbstractValidator<UpdateQuotationCommand>
    {
        public UpdateQuotationCommandValidator() {
            RuleFor(p => p.CustomerId)
                  .NotEmpty().WithMessage("{PropertyName} es requerido");
            RuleFor(p => p.BranchOfficeId)
                    .NotEmpty().WithMessage("{PropertyName} es requerido");
            RuleFor(p => p.CreationDate)
                    .NotNull().WithMessage("{PropertyName} no puede ser nulo");
            RuleFor(p => p.DueDate)
                  .NotEmpty().WithMessage("{PropertyName} es requerido");
            RuleFor(p => p.StatusId)
                    .NotNull().WithMessage("{PropertyName} no puede ser vacio");
            RuleFor(p => p.PrefixId)
                    .NotNull().WithMessage("{PropertyName} no puede ser vacio");
        }
    }
}

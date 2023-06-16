using FluentValidation;


namespace SMART.ERP.Application.Features.CustomerMachineryFeature.Commands.CreateCustomerMachineryCommand
{
    public class CreateCustomerMachineryCommandValidator : AbstractValidator<CreateCustomerMachineryCommand>
    {
        public CreateCustomerMachineryCommandValidator()
        {
            RuleFor(p => p.ProductId)
                .NotEmpty().WithMessage("El {PropertyName} no puede ser vacio")
                .NotNull().WithMessage("El {PropertyName} no puede ser nulo");

            RuleFor(p => p.CustomerId)
               .NotEmpty().WithMessage("El {PropertyName} no puede ser vacio")
               .NotNull().WithMessage("El {PropertyName} no puede ser nulo");

            RuleFor(p => p.BaseInfoId)
                .NotEmpty().WithMessage("El {PropertyName} no puede ser vacio")
                .NotNull().WithMessage("El {PropertyName} no puede ser nulo");

        }
    }
}

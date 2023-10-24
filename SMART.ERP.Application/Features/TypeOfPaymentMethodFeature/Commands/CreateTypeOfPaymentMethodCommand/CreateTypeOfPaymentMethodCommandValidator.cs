using FluentValidation;
using SMART.ERP.Application.Features.BrandFeature.Commands.CreateBrandCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.TypeOfPaymentMethodFeature.Commands.CreateTypeOfPaymentMethodCommand
{
    public class CreateTypeOfPaymentMethodCommandValidator : AbstractValidator<CreateTypeOfPaymentMethodCommand>
    {
        public CreateTypeOfPaymentMethodCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.IsActive)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo");
        }
    }
}

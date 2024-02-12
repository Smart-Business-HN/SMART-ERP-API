using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.PurchaseBillFeature.Commands.CreatePurchaseBillCommand
{
    public class CreatePurchaseBillCommandValidator : AbstractValidator<CreatePurchaseBillCommand>
    {
        public CreatePurchaseBillCommandValidator() {
            RuleFor(p => p.Cai)
                        .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                        .MaximumLength(37).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                        .MinimumLength(37).WithMessage("{PropertyName} no debe ser menor  {MinLength} caracteres");
            RuleFor(p => p.InvoiceNumber)
                    .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                    .MaximumLength(19).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                    .MinimumLength(19).WithMessage("{PropertyName} no debe ser menor  {MinLength} caracteres");
            RuleFor(p => p.InvoiceDate)
                    .NotNull().WithMessage("{PropertyName} no puede ser vacio");
            RuleFor(p => p.ProviderId)
                   .NotNull().WithMessage("{PropertyName} no puede ser vacio");
        }
    }
}

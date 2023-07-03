using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.TaxFeature.Commands.UpdateTaxCommand
{
    public class UpdateTaxCommandValidator : AbstractValidator<UpdateTaxCommand>
    {
        public UpdateTaxCommandValidator() {
            RuleFor(p => p.Id)
                    .NotNull().WithMessage("{PropertyName} no puede ser vacio");
            RuleFor(p => p.Name)
                     .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                     .MaximumLength(10).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
            RuleFor(p => p.TextForDocuments)
              .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
              .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
            RuleFor(p => p.Rate)
              .NotNull().WithMessage("{PropertyName} no puede ser null")
              .LessThan(0).WithMessage("\"{PropertyName} no puede ser menor a cero.");
        }
    }
}

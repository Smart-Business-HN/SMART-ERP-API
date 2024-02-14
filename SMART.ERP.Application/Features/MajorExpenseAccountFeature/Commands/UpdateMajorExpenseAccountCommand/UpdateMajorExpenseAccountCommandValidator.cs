using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.MajorExpenseAccountFeature.Commands.UpdateMajorExpenseAccountCommand
{
    public class UpdateMajorExpenseAccountCommandValidator : AbstractValidator<UpdateMajorExpenseAccountCommand>
    {
        public UpdateMajorExpenseAccountCommandValidator() {
            RuleFor(x => x.Id)
                  .NotEmpty().WithMessage("{PropertyName} es requerido")
                  .NotNull().WithMessage("{PropertyName es requerido}");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName es requerido}")
                .MaximumLength(100).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");
        }
    }
}

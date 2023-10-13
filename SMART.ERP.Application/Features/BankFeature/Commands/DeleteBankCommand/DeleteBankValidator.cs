using FluentValidation;
using SMART.ERP.Application.Features.CityFeature.Commands.DeleteCityCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.BankFeature.Commands.DeleteBankCommand
{
    public class DeleteBankValidator : AbstractValidator<DeleteBankCommand>
    {
        public DeleteBankValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

        }
    }
}

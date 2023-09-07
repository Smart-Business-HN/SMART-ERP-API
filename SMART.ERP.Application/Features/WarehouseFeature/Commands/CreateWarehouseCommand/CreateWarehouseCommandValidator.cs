using FluentValidation;
using SMART.ERP.Application.Features.WinReasonFeature.Commands.CreateWinReasonCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.WarehouseFeature.Commands.CreateWarehouseCommand
{
    public class CreateWarehouseCommandValidator : AbstractValidator<CreateWarehouseCommand>
    {
        public CreateWarehouseCommandValidator() {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .MinimumLength(3).WithMessage("{PropertyName} no debe ser menor de {MinLength} caracteres");
            RuleFor(p => p.BranchOfficeId)
                .NotNull().WithMessage("{PropertyName} no puede ser vacio");
             
            RuleFor(p => p.Address)
                .MaximumLength(400).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}

using FluentValidation;

namespace SMART.ERP.Application.Features.BranchOfficeFeature.Commands.DeleteBranchOfficeCommand
{
    public class DeleteBranchOfficeCommandValidator : AbstractValidator<DeleteBranchOfficeCommand>
    {
        public DeleteBranchOfficeCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}

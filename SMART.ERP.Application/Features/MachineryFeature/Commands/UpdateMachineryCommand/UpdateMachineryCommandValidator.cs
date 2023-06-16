using FluentValidation;

namespace SMART.ERP.Application.Features.MachineryFeature.Commands.UpdateMachineryCommand
{
    public class UpdateMachineryCommandValidator : AbstractValidator<UpdateMachineryCommand>
    {
        public UpdateMachineryCommandValidator()
        {
            RuleFor(p => p.DeviceName)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(150).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
            RuleFor(p => p.SerialNum)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
            RuleFor(x => x.MachineTypeName)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
            RuleFor(p => p.Country)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
            RuleFor(p => p.Province)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(100).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
            RuleFor(p => p.Interval)
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}

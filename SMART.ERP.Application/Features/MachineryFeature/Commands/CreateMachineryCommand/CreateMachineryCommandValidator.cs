using FluentValidation;

namespace SMART.ERP.Application.Features.MachineryFeature.Commands.CreateMachineryCommand
{
    public class CreateMachineryCommandValidator : AbstractValidator<CreateMachineryCommand>
    {
        public CreateMachineryCommandValidator()
        {
            RuleFor(a => a.SerialNum)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
            RuleFor(x => x.MachineTypeName)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
            RuleFor(a => a.Customer)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
            RuleFor(a => a.Country)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
            RuleFor(a => a.Province)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(100).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
            RuleFor(a => a.DeviceName)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(150).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
            RuleFor(a => a.Interval)
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
            RuleFor(a => a.InitialMaintenance)
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}

using FluentValidation;

namespace SMART.ERP.Application.Features.CaiFeature.Commands.UpdateCaiCommand
{
    public  class UpdateCaiCommandValidator : AbstractValidator<UpdateCaiCommand>
    {
        public UpdateCaiCommandValidator()
    {
        RuleFor(p => p.Identificator)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(37).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .MinimumLength(37).WithMessage("{PropertyName} no debe ser menor  {MinLength} caracteres");
        RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        RuleFor(p => p.StartCorrelative)
                .NotNull().WithMessage("{PropertyName} no puede ser vacio");
        RuleFor(p => p.EndCorrelative)
              .NotNull().WithMessage("{PropertyName} no puede ser vacio")
              .When(x => x.EndCorrelative < x.StartCorrelative).WithMessage("{PropertyName} no puede ser menor que el inicio del correlativo");
        RuleFor(p => p.ValidFrom)
                .NotNull().WithMessage("{PropertyName} no puede ser vacio");
        RuleFor(p => p.ValidUntil)
                .NotNull().WithMessage("{PropertyName} no puede ser vacio");
        RuleFor(p => p.IsGeneralCai)
                .NotNull().WithMessage("{PropertyName} no puede ser vacio");
        RuleFor(p => p.IsActive)
                .NotNull().WithMessage("{PropertyName} no puede ser vacio");

    }
}
}

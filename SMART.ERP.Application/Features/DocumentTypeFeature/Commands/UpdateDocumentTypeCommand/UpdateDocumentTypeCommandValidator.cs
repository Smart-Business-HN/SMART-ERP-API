using FluentValidation;

namespace SMART.ERP.Application.Features.DocumentTypeFeature.Commands.UpdateDocumentTypeCommand
{
    public class UpdateDocumentTypeCommandValidator : AbstractValidator<UpdateDocumentTypeCommand>
    {
        public UpdateDocumentTypeCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(100).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}

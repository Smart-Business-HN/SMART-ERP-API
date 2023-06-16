using FluentValidation;

namespace SMART.ERP.Application.Features.DocumentTypeFeature.Commands.CreateDocumentTypeCommand
{
    public class CreateDocumentTypeCommandValidator : AbstractValidator<CreateDocumentTypeCommand>
    {
        public CreateDocumentTypeCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(100).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}

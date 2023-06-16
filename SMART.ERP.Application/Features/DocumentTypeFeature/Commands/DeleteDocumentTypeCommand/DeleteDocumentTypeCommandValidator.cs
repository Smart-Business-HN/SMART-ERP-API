using FluentValidation;

namespace SMART.ERP.Application.Features.DocumentTypeFeature.Commands.DeleteDocumentTypeCommand
{
    public class DeleteDocumentTypeCommandValidator : AbstractValidator<DeleteDocumentTypeCommand>
    {
        public DeleteDocumentTypeCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}

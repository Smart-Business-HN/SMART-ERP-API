using FluentValidation;

namespace SMART.ERP.Application.Features.ProjectAttachmentCategoryFeature.Commands.CreateProjectAttachmentCategoryCommand
{
    public class CreateProjectAttachmentCategoryCommandValidator : AbstractValidator<CreateProjectAttachmentCategoryCommand>
    {
        public CreateProjectAttachmentCategoryCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(100).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}

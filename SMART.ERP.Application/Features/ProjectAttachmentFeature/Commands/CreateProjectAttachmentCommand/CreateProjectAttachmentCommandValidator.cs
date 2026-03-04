using FluentValidation;

namespace SMART.ERP.Application.Features.ProjectAttachmentFeature.Commands.CreateProjectAttachmentCommand
{
    public class CreateProjectAttachmentCommandValidator : AbstractValidator<CreateProjectAttachmentCommand>
    {
        public CreateProjectAttachmentCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(150).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.Url)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.ContentType)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(100).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.ProjectId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.ProjectAttachmentCategoryId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.UserId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}

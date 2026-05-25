using FluentValidation;

namespace SMART.ERP.Application.Features.JournalEntryFeature.Commands.UpdateJournalEntryCommand
{
    public class UpdateJournalEntryCommandValidator : AbstractValidator<UpdateJournalEntryCommand>
    {
        public UpdateJournalEntryCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
            RuleFor(x => x.EntryDate)
                .NotEmpty().WithMessage("La fecha del asiento es requerida");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("{PropertyName} (concepto) es requerido")
                .MaximumLength(300).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");
            RuleFor(x => x.Lines)
                .NotNull().WithMessage("El asiento debe tener líneas")
                .Must(l => l != null && l.Count >= 2).WithMessage("El asiento debe tener al menos dos líneas (partidas)");
        }
    }
}

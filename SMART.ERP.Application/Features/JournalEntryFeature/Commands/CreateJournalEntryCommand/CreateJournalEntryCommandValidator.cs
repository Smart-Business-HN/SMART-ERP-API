using FluentValidation;

namespace SMART.ERP.Application.Features.JournalEntryFeature.Commands.CreateJournalEntryCommand
{
    public class CreateJournalEntryCommandValidator : AbstractValidator<CreateJournalEntryCommand>
    {
        public CreateJournalEntryCommandValidator()
        {
            RuleFor(x => x.EntryDate)
                .NotEmpty().WithMessage("La fecha del asiento es requerida");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("{PropertyName} (concepto) es requerido")
                .MaximumLength(300).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");
            RuleFor(x => x.Lines)
                .NotNull().WithMessage("El asiento debe tener líneas")
                .Must(l => l != null && l.Count >= 2).WithMessage("El asiento debe tener al menos dos líneas (partidas)");
            RuleForEach(x => x.Lines).ChildRules(line =>
            {
                line.RuleFor(l => l.LedgerAccountId)
                    .NotEqual(0).WithMessage("Cada línea debe tener una cuenta contable");
            });
        }
    }
}

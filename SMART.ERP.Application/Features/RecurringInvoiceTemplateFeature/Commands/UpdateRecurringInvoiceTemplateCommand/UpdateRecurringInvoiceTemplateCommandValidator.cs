using FluentValidation;

namespace SMART.ERP.Application.Features.RecurringInvoiceTemplateFeature.Commands.UpdateRecurringInvoiceTemplateCommand
{
    public class UpdateRecurringInvoiceTemplateCommandValidator : AbstractValidator<UpdateRecurringInvoiceTemplateCommand>
    {
        public UpdateRecurringInvoiceTemplateCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El Id de la plantilla es requerido.");

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("El cliente es requerido.");

            RuleFor(x => x.BranchOfficeId)
                .GreaterThan(0).WithMessage("La sucursal es requerida.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("El usuario es requerido.");

            RuleFor(x => x.InvoicePaymentTypeId)
                .GreaterThan(0).WithMessage("El tipo de pago es requerido.");

            RuleFor(x => x.DayOfMonth)
                .Must(d => (d >= 1 && d <= 28) || d == -1)
                .WithMessage("El día del mes debe ser entre 1 y 28, o -1 para el último día del mes.");

            RuleFor(x => x.StatusId)
                .GreaterThan(0).WithMessage("El estado es requerido.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Debe incluir al menos un producto o servicio.");
        }
    }
}

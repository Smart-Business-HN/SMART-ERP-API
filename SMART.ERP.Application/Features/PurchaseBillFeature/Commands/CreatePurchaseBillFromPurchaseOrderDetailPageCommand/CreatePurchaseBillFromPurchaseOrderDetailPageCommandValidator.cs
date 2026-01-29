using FluentValidation;

namespace SMART.ERP.Application.Features.PurchaseBillFeature.Commands.CreatePurchaseBillFromPurchaseOrderDetailPageCommand
{
    public class CreatePurchaseBillFromPurchaseOrderDetailPageCommandValidator : AbstractValidator<CreatePurchaseBillFromPurchaseOrderDetailPageCommand>
    {
        public CreatePurchaseBillFromPurchaseOrderDetailPageCommandValidator() {
            RuleFor(p => p.Cai)
                        .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                        .MaximumLength(37).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                        .MinimumLength(37).WithMessage("{PropertyName} no debe ser menor  {MinLength} caracteres");
            RuleFor(p => p.InvoiceNumber)
                    .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                    .MaximumLength(19).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                    .MinimumLength(19).WithMessage("{PropertyName} no debe ser menor  {MinLength} caracteres");
            RuleFor(p => p.InvoiceDate)
                    .NotNull().WithMessage("{PropertyName} no puede ser vacio");
            RuleFor(p => p.PurchaseOrderOriginId)
                    .NotNull().WithMessage("{PropertyName} no puede ser vacio");
        }
    }
}

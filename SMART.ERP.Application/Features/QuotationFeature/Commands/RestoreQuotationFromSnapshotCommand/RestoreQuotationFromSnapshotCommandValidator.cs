using FluentValidation;

namespace SMART.ERP.Application.Features.QuotationFeature.Commands.RestoreQuotationFromSnapshotCommand
{
    public class RestoreQuotationFromSnapshotCommandValidator : AbstractValidator<RestoreQuotationFromSnapshotCommand>
    {
        public RestoreQuotationFromSnapshotCommandValidator()
        {
            RuleFor(p => p.QuotationId).NotEmpty().WithMessage("{PropertyName} es requerido");
            RuleFor(p => p.SnapshotId).NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}

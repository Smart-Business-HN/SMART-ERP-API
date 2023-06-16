using FluentValidation;

namespace SMART.ERP.Application.Features.ClientDeliveryDirectionFeature.Commands.DeleteDeliveryDirectionCommand
{
    public class DeleteDeliveryDirectionCommandValidator : AbstractValidator<DeleteDeliveryDirectionCommand>
    {
        public DeleteDeliveryDirectionCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}

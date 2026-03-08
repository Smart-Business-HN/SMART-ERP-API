using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.RecurringInvoiceTemplateFeature.Commands.ToggleRecurringInvoiceTemplateCommand
{
    public class ToggleRecurringInvoiceTemplateCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }

    public class ToggleRecurringInvoiceTemplateCommandHandler : IRequestHandler<ToggleRecurringInvoiceTemplateCommand, Response<string>>
    {
        private readonly IRepositoryAsync<RecurringInvoiceTemplate> _repositoryAsync;

        public ToggleRecurringInvoiceTemplateCommandHandler(IRepositoryAsync<RecurringInvoiceTemplate> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(ToggleRecurringInvoiceTemplateCommand request, CancellationToken cancellationToken)
        {
            var templateExist = await _repositoryAsync.GetByIdAsync(request.Id);
            if (templateExist == null)
                throw new ApiException($"No existe una plantilla de factura recurrente con el Id {request.Id}");

            templateExist.IsActive = request.IsActive;

            if (request.IsActive)
            {
                templateExist.NextGenerationDate = CreateRecurringInvoiceTemplateCommand.CreateRecurringInvoiceTemplateCommandHandler
                    .CalculateNextGenerationDate(templateExist.DayOfMonth, DateTime.UtcNow);
            }

            await _repositoryAsync.UpdateAsync(templateExist);
            await _repositoryAsync.SaveChangesAsync();

            var statusText = request.IsActive ? "activada" : "desactivada";
            return new Response<string>($"Plantilla de factura recurrente {statusText} exitosamente.");
        }
    }
}

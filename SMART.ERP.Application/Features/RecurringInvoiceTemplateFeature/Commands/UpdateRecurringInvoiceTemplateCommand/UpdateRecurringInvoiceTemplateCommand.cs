using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.RecurringInvoiceTemplate;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.RecurringInvoiceTemplateSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.RecurringInvoiceTemplateFeature.Commands.UpdateRecurringInvoiceTemplateCommand
{
    public class UpdateRecurringInvoiceTemplateCommand : IRequest<Response<RecurringInvoiceTemplateDto>>
    {
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        public int BranchOfficeId { get; set; }
        public Guid UserId { get; set; }
        public int InvoicePaymentTypeId { get; set; }
        public int DayOfMonth { get; set; }
        public string? Observations { get; set; }
        public string? TermsAndConditions { get; set; }
        public int StatusId { get; set; }
        public int? ProjectId { get; set; }
        public List<RecurringInvoiceTemplateItemDto> Items { get; set; } = new();
    }

    public class UpdateRecurringInvoiceTemplateCommandHandler : IRequestHandler<UpdateRecurringInvoiceTemplateCommand, Response<RecurringInvoiceTemplateDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<RecurringInvoiceTemplate> _repositoryAsync;
        private readonly IRepositoryAsync<RecurringInvoiceTemplateItem> _itemRepositoryAsync;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IRepositoryAsync<BranchOffices> _branchOfficeRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<Status> _statusRepositoryAsync;
        private readonly IRepositoryAsync<InvoicePaymentType> _invoicePaymentTypeRepositoryAsync;
        private readonly IRepositoryAsync<Tax> _taxRepositoryAsync;

        public UpdateRecurringInvoiceTemplateCommandHandler(
            IMapper mapper,
            IRepositoryAsync<RecurringInvoiceTemplate> repositoryAsync,
            IRepositoryAsync<RecurringInvoiceTemplateItem> itemRepositoryAsync,
            IRepositoryAsync<Customer> customerRepositoryAsync,
            IRepositoryAsync<BranchOffices> branchOfficeRepositoryAsync,
            IRepositoryAsync<User> userRepositoryAsync,
            IRepositoryAsync<Status> statusRepositoryAsync,
            IRepositoryAsync<InvoicePaymentType> invoicePaymentTypeRepositoryAsync,
            IRepositoryAsync<Tax> taxRepositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _itemRepositoryAsync = itemRepositoryAsync;
            _customerRepositoryAsync = customerRepositoryAsync;
            _branchOfficeRepositoryAsync = branchOfficeRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _statusRepositoryAsync = statusRepositoryAsync;
            _invoicePaymentTypeRepositoryAsync = invoicePaymentTypeRepositoryAsync;
            _taxRepositoryAsync = taxRepositoryAsync;
        }

        public async Task<Response<RecurringInvoiceTemplateDto>> Handle(UpdateRecurringInvoiceTemplateCommand request, CancellationToken cancellationToken)
        {
            var templateExist = await _repositoryAsync.FirstOrDefaultAsync(new FilterRecurringInvoiceTemplateByIdSpecification(request.Id));
            if (templateExist == null)
                throw new ApiException($"No existe una plantilla de factura recurrente con el Id {request.Id}");

            var customerExist = await _customerRepositoryAsync.GetByIdAsync(request.CustomerId);
            if (customerExist == null)
                throw new ApiException($"No existe un cliente con el Id {request.CustomerId}");

            var branchOfficeExist = await _branchOfficeRepositoryAsync.GetByIdAsync(request.BranchOfficeId);
            if (branchOfficeExist == null)
                throw new ApiException($"No existe una sucursal con el id {request.BranchOfficeId}");

            var userExist = await _userRepositoryAsync.GetByIdAsync(request.UserId);
            if (userExist == null)
                throw new ApiException($"No existe un usuario con el Id {request.UserId}");

            var statusExist = await _statusRepositoryAsync.GetByIdAsync(request.StatusId);
            if (statusExist == null)
                throw new ApiException($"No existe un Estado con el id {request.StatusId}");

            var invoicePaymentTypeExist = await _invoicePaymentTypeRepositoryAsync.GetByIdAsync(request.InvoicePaymentTypeId);
            if (invoicePaymentTypeExist == null)
                throw new ApiException($"No existe un tipo de pago con el id {request.InvoicePaymentTypeId}");

            var taxes = await _taxRepositoryAsync.ListAsync();
            foreach (var item in request.Items)
            {
                if (!taxes.Exists(x => x.Id == item.TaxId))
                    throw new ApiException($"No existe un impuesto con el id {item.TaxId}");
            }

            templateExist.CustomerId = request.CustomerId;
            templateExist.BranchOfficeId = request.BranchOfficeId;
            templateExist.UserId = request.UserId;
            templateExist.InvoicePaymentTypeId = request.InvoicePaymentTypeId;
            templateExist.DayOfMonth = request.DayOfMonth;
            templateExist.Observations = request.Observations;
            templateExist.TermsAndConditions = request.TermsAndConditions;
            templateExist.StatusId = request.StatusId;
            templateExist.ProjectId = request.ProjectId;
            templateExist.NextGenerationDate = CreateRecurringInvoiceTemplateCommand.CreateRecurringInvoiceTemplateCommandHandler
                .CalculateNextGenerationDate(request.DayOfMonth, DateTime.UtcNow);

            await _repositoryAsync.UpdateAsync(templateExist);
            await _repositoryAsync.SaveChangesAsync();

            // Remove old items and add new ones
            if (templateExist.Items != null)
            {
                foreach (var oldItem in templateExist.Items)
                {
                    await _itemRepositoryAsync.DeleteAsync(oldItem);
                }
                await _itemRepositoryAsync.SaveChangesAsync();
            }

            foreach (var item in request.Items)
            {
                var newItem = new RecurringInvoiceTemplateItem
                {
                    RecurringInvoiceTemplateId = templateExist.Id,
                    ProductId = item.ProductId,
                    ProductCode = item.ProductCode,
                    ProductDescription = item.ProductDescription,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    TaxId = item.TaxId
                };
                await _itemRepositoryAsync.AddAsync(newItem);
            }
            await _itemRepositoryAsync.SaveChangesAsync();

            var dto = _mapper.Map<RecurringInvoiceTemplateDto>(templateExist);
            dto.Items = request.Items;

            return new Response<RecurringInvoiceTemplateDto>(dto, "Plantilla de factura recurrente actualizada exitosamente.");
        }
    }
}

using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.RecurringInvoiceTemplate;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.RecurringInvoiceTemplateFeature.Commands.CreateRecurringInvoiceTemplateCommand
{
    public class CreateRecurringInvoiceTemplateCommand : IRequest<Response<RecurringInvoiceTemplateDto>>
    {
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

    public class CreateRecurringInvoiceTemplateCommandHandler : IRequestHandler<CreateRecurringInvoiceTemplateCommand, Response<RecurringInvoiceTemplateDto>>
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
        private readonly IJwtService _jwtService;

        public CreateRecurringInvoiceTemplateCommandHandler(
            IMapper mapper,
            IRepositoryAsync<RecurringInvoiceTemplate> repositoryAsync,
            IRepositoryAsync<RecurringInvoiceTemplateItem> itemRepositoryAsync,
            IRepositoryAsync<Customer> customerRepositoryAsync,
            IRepositoryAsync<BranchOffices> branchOfficeRepositoryAsync,
            IRepositoryAsync<User> userRepositoryAsync,
            IRepositoryAsync<Status> statusRepositoryAsync,
            IRepositoryAsync<InvoicePaymentType> invoicePaymentTypeRepositoryAsync,
            IRepositoryAsync<Tax> taxRepositoryAsync,
            IJwtService jwtService)
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
            _jwtService = jwtService;
        }

        public async Task<Response<RecurringInvoiceTemplateDto>> Handle(CreateRecurringInvoiceTemplateCommand request, CancellationToken cancellationToken)
        {
            var customerExist = await _customerRepositoryAsync.GetByIdAsync(request.CustomerId);
            if (customerExist == null)
                throw new ApiException($"No existe un cliente con el Id {request.CustomerId}");
            if (!customerExist.IsActive)
                throw new ApiException($"El cliente {customerExist.FullName} se encuentra inactivo");

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

            var newTemplate = new RecurringInvoiceTemplate
            {
                CustomerId = request.CustomerId,
                BranchOfficeId = request.BranchOfficeId,
                UserId = request.UserId,
                InvoicePaymentTypeId = request.InvoicePaymentTypeId,
                DayOfMonth = request.DayOfMonth,
                Observations = request.Observations,
                TermsAndConditions = request.TermsAndConditions,
                StatusId = request.StatusId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = _jwtService.GetSubjectToken(),
                NextGenerationDate = CalculateNextGenerationDate(request.DayOfMonth, DateTime.UtcNow),
                ProjectId = request.ProjectId
            };

            var templateResponse = await _repositoryAsync.AddAsync(newTemplate);
            await _repositoryAsync.SaveChangesAsync();

            foreach (var item in request.Items)
            {
                var newItem = new RecurringInvoiceTemplateItem
                {
                    RecurringInvoiceTemplateId = templateResponse.Id,
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

            var dto = _mapper.Map<RecurringInvoiceTemplateDto>(templateResponse);
            dto.Customer = _mapper.Map<DTOs.Customer.CustomerDto>(customerExist);
            dto.Items = request.Items;

            return new Response<RecurringInvoiceTemplateDto>(dto, "Plantilla de factura recurrente creada exitosamente.");
        }

        public static DateTime CalculateNextGenerationDate(int dayOfMonth, DateTime fromDate)
        {
            var targetDate = fromDate.Date;

            if (dayOfMonth == -1)
            {
                // If today is already the last day or past it, go to next month's last day
                var lastDayThisMonth = new DateTime(targetDate.Year, targetDate.Month, DateTime.DaysInMonth(targetDate.Year, targetDate.Month));
                if (targetDate >= lastDayThisMonth)
                {
                    var nextMonth = targetDate.AddMonths(1);
                    return new DateTime(nextMonth.Year, nextMonth.Month, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
                }
                return lastDayThisMonth;
            }
            else
            {
                var day = Math.Min(dayOfMonth, DateTime.DaysInMonth(targetDate.Year, targetDate.Month));
                var candidate = new DateTime(targetDate.Year, targetDate.Month, day);
                if (targetDate >= candidate)
                {
                    var nextMonth = targetDate.AddMonths(1);
                    day = Math.Min(dayOfMonth, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
                    return new DateTime(nextMonth.Year, nextMonth.Month, day);
                }
                return candidate;
            }
        }
    }
}

using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.BillPayment;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.AccountingPostingService;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BillPaymentFeature.Commands.CreateBillPaymentCommand
{
    public class CreateBillPaymentCommand : IRequest<Response<BillPaymentDto>>
    {
        public int InvoiceId { get; set; }
        public int TypeOfPaymentMethodId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int? InternalBankAccountId { get; set; }
        public string? Attachment { get; set; }
    }

    public class CreateBillPaymentCommandHandler : IRequestHandler<CreateBillPaymentCommand, Response<BillPaymentDto>>
    {
        private readonly IRepositoryAsync<BillPayment> _repositoryAsync;
        private readonly IRepositoryAsync<Invoice> _invoiceRepositoryAsync;
        private readonly IRepositoryAsync<InternalBankAccount> _internalBankAccountRepositoryAsync;
        private readonly IRepositoryAsync<TypeOfPaymentMethod> _typeOfPaymentMethodRepositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IAccountingPostingService _accountingPostingService;
        public CreateBillPaymentCommandHandler(IRepositoryAsync<BillPayment> repositoryAsync, IJwtService jwtService, IRepositoryAsync<Invoice> invoiceRepositoryAsync, IRepositoryAsync<InternalBankAccount> internalBankAccountRepositoryAsync, IRepositoryAsync<TypeOfPaymentMethod> typeOfPaymentMethodRepositoryAsync, IMapper mapper, IAccountingPostingService accountingPostingService)
        {
            _repositoryAsync = repositoryAsync;
            _invoiceRepositoryAsync = invoiceRepositoryAsync;
            _internalBankAccountRepositoryAsync = internalBankAccountRepositoryAsync;
            _typeOfPaymentMethodRepositoryAsync = typeOfPaymentMethodRepositoryAsync;
            _mapper = mapper;
            _jwtService = jwtService;
            _accountingPostingService = accountingPostingService;
        }

        public async Task<Response<BillPaymentDto>> Handle(CreateBillPaymentCommand request, CancellationToken cancellationToken)
        {
            var checkInvoice = await _invoiceRepositoryAsync.GetByIdAsync(request.InvoiceId);
            if (checkInvoice == null)
            {
                throw new KeyNotFoundException($"No se encontro una factura con id {request.InvoiceId}");
            }
            if (checkInvoice.StatusId == 17)
            {
                throw new ApiException("Esta factura ya se encuentra pagada.");
            }
            if (checkInvoice.Outstanding < request.Amount)
            {
                throw new ApiException("No se permiten pagos mayores al saldo adeudado.");
            }
            var checkTypeOfPaymentMethod = await _typeOfPaymentMethodRepositoryAsync.GetByIdAsync(request.TypeOfPaymentMethodId);
            if (checkTypeOfPaymentMethod == null)
            {
                throw new KeyNotFoundException($"No se encontro una forma de pago con id {request.TypeOfPaymentMethodId}");
            }
            if (checkTypeOfPaymentMethod.RequiresBankAccount && request.InternalBankAccountId == null)
            {
                throw new ApiException($"La forma de pago '{checkTypeOfPaymentMethod.Name}' requiere especificar la cuenta bancaria que recibió el pago.");
            }
            if (request.InternalBankAccountId != null)
            {
                var checkInternalBankAccount = await _internalBankAccountRepositoryAsync.GetByIdAsync((int)request.InternalBankAccountId);
                if (checkInternalBankAccount == null)
                {
                    throw new KeyNotFoundException($"No se encontro una la cuenta bancaria con id {request.InternalBankAccountId}");
                }
            }
            checkInvoice.Outstanding = checkInvoice.Outstanding - request.Amount;
            if (checkInvoice.Outstanding == 0)
            {
                checkInvoice.StatusId = 19;
            }

            var newRecord = _mapper.Map<BillPayment>(request);
            newRecord.CreationDate = DateTime.Now;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            var response = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            await _invoiceRepositoryAsync.UpdateAsync(checkInvoice);
            await _invoiceRepositoryAsync.SaveChangesAsync();

            await _accountingPostingService.PostBillPaymentAsync(response.Id, cancellationToken);

            var dto = _mapper.Map<BillPaymentDto>(response);
            return new Response<BillPaymentDto>(dto, $"Pago recibido exitosamente");
        }
    }
}

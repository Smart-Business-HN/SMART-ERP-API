using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Address;
using SMART.ERP.Application.DTOs.BillPayment;
using SMART.ERP.Application.DTOs.InternalBankAccount;
using SMART.ERP.Application.DTOs.Invoice;
using SMART.ERP.Application.DTOs.TypeOfPaymentMethod;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Features.CityFeature.Commands.UpdateCityCommand;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.BillPaymentFeature.Commands.UpdateBillPaymentCommand
{
    public class UpdateBillPaymentCommand : IRequest<Response<BillPaymentDto>>
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int TypeOfPaymentMethodId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int? InternalBankAccountId { get; set; }
        public string? Attachment { get; set; }
    }

    public class UpdateBillPaymentCommandHandler : IRequestHandler<UpdateBillPaymentCommand, Response<BillPaymentDto>>
    {
        private readonly IRepositoryAsync<BillPayment> _repositoryAsync;
        private readonly IRepositoryAsync<Invoice> _invoiceRepositoryAsync;
        private readonly IRepositoryAsync<InternalBankAccount> _internalBankAccountRepositoryAsync;
        private readonly IRepositoryAsync<TypeOfPaymentMethod> _typeOfPaymentMethodRepositoryAsync;
        private readonly IMapper _mapper;

        public UpdateBillPaymentCommandHandler(IRepositoryAsync<BillPayment> repositoryAsync, IRepositoryAsync<Invoice> invoiceRepositoryAsync, IRepositoryAsync<InternalBankAccount> internalBankAccountRepositoryAsync, IRepositoryAsync<TypeOfPaymentMethod> typeOfPaymentMethodRepositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _invoiceRepositoryAsync = invoiceRepositoryAsync;
            _internalBankAccountRepositoryAsync = internalBankAccountRepositoryAsync;
            _typeOfPaymentMethodRepositoryAsync = typeOfPaymentMethodRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<BillPaymentDto>> Handle(UpdateBillPaymentCommand request, CancellationToken cancellationToken)
        {
            var checkBillPaymen = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkBillPaymen == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun pago el con id {request.Id}");
            }
            var checkInvoice = await _invoiceRepositoryAsync.GetByIdAsync(request.InvoiceId);
            if (checkInvoice == null)
            {
                throw new KeyNotFoundException($"No se encontro una factura con id {request.InvoiceId}");
            }
            if (checkInvoice.StatusId == 19)
            {
                throw new ApiException("Esta factura ya se encuentra pagada. Por lo cual no puede modificar este pago.");
            }
            var checkTypeOfPaymentMethod = await _typeOfPaymentMethodRepositoryAsync.GetByIdAsync(request.TypeOfPaymentMethodId);
            if (checkTypeOfPaymentMethod == null)
            {
                throw new KeyNotFoundException($"No se encontro una forma de pago con id {request.TypeOfPaymentMethodId}");
            }
            if (request.InternalBankAccountId != null)
            {
                var checkInternalBankAccount = await _internalBankAccountRepositoryAsync.GetByIdAsync((int)request.InternalBankAccountId);
                if (checkInternalBankAccount == null)
                {
                    throw new KeyNotFoundException($"No se encontro una la cuenta bancaria con id {request.InternalBankAccountId}");
                }
            }
            decimal invoiceWithoutThisPayment = checkInvoice.Outstanding + checkBillPaymen.Amount;
            if(invoiceWithoutThisPayment < request.Amount)
            {
                throw new ApiException("Esta intentando pagar mas de lo debido en la factura de origen.");
            }
            checkInvoice.Outstanding = invoiceWithoutThisPayment-request.Amount;
            if(invoiceWithoutThisPayment - request.Amount == 0)
            {
                checkInvoice.StatusId = 17;
            }
            checkBillPaymen.Amount = request.Amount;
            checkBillPaymen.InvoiceId = request.InvoiceId;
            checkBillPaymen.TypeOfPaymentMethodId = request.TypeOfPaymentMethodId;
            checkBillPaymen.Date = request.Date;
            checkBillPaymen.Attachment = request.Attachment;
            checkBillPaymen.InternalBankAccountId = request.InternalBankAccountId;

            await _repositoryAsync.UpdateAsync(checkBillPaymen);
            await _repositoryAsync.SaveChangesAsync();
            await _invoiceRepositoryAsync.UpdateAsync(checkInvoice);
            await _invoiceRepositoryAsync.SaveChangesAsync();

            var dto = _mapper.Map<BillPaymentDto>(checkBillPaymen);
            return new Response<BillPaymentDto>(dto, $"Pago actualizado correctamente");
        }
    }
}

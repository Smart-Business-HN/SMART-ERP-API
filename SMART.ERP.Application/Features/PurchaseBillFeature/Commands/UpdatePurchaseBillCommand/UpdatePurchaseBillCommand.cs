using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using Quartz.Impl.AdoJobStore.Common;
using SMART.ERP.Application.DTOs.Provider;
using SMART.ERP.Application.DTOs.PurchaseBill;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PurchaseBillFeature.Commands.UpdatePurchaseBillCommand
{
    public class UpdatePurchaseBillCommand : IRequest<Response<PurchaseBillDto>>
    {
        public int Id { get; set; }
        public int? ProviderId { get; set; }
        public int? StatusId { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public DateTime InvoiceDate { get; set; }
        public string Cai { get; set; } = null!;
        public int? PurchaseOrderOriginId { get; set; }
        public decimal? Exempt { get; set; }
        public decimal? Exonerated { get; set; }
        public decimal? TaxedAt15Percent { get; set; }
        public decimal? TaxedAt18Percent { get; set; }
        public decimal? Taxes15Percent { get; set; }
        public decimal? Taxes18Percent { get; set; }
    }
    public class UpdatePurchaseBillCommandHandler : IRequestHandler<UpdatePurchaseBillCommand,Response<PurchaseBillDto>>
    {
        private readonly IRepositoryAsync<PurchaseBill> _repositoryAsync;
        private readonly IRepositoryAsync<PurchaseOrder> _purchaseOrderRepositoryAsync;
        private readonly IRepositoryAsync<Provider> _providerRepositoryAsync;
        private readonly IRepositoryAsync<Status> _statusRepositoryAsync;
        private readonly IMapper _mapper;
        public UpdatePurchaseBillCommandHandler(IRepositoryAsync<PurchaseBill> repositoryAsync, IRepositoryAsync<Status> statusRepositoryAsync, IRepositoryAsync<PurchaseOrder> purchaseOrderRepositoryAsync, IRepositoryAsync<Provider> providerRepositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _purchaseOrderRepositoryAsync = purchaseOrderRepositoryAsync;
            _providerRepositoryAsync = providerRepositoryAsync;
            _statusRepositoryAsync = statusRepositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<PurchaseBillDto>> Handle(UpdatePurchaseBillCommand request, CancellationToken cancellationToken)
        {
            var purchaseBillExist = await _repositoryAsync.GetByIdAsync(request.Id);
            if (purchaseBillExist == null)
            {
                throw new ApiException($"No existe una factura de compra con el Id {request.Id}");
            }
            if(request.ProviderId != null && request.ProviderId != 0)
            {
                var providerExist = await _providerRepositoryAsync.GetByIdAsync((int)request.ProviderId);
                if (providerExist == null)
                {
                    throw new ApiException($"No existe un proveedor con el Id {request.ProviderId}");
                }
            }
            if(request.StatusId != null && request.StatusId != 0)
            {
                var statusExist = await _statusRepositoryAsync.GetByIdAsync((int)request.StatusId);
                if (statusExist == null)
                {
                    throw new ApiException($"No existe un estaddo con el Id {request.StatusId}");
                }
            }
            if(request.PurchaseOrderOriginId!=null&&request.PurchaseOrderOriginId!=0)
            {
                var purchaseOrderExist = await _purchaseOrderRepositoryAsync.GetByIdAsync((int)request.PurchaseOrderOriginId);
                if (purchaseOrderExist == null)
                {
                    throw new ApiException($"No existe una orden de compra con el Id {request.PurchaseOrderOriginId}");
                }
            }
            if(purchaseBillExist.InvoiceDate != request.InvoiceDate)
            {
                purchaseBillExist.InvoiceDate = request.InvoiceDate;
            }
            if (purchaseBillExist.InvoiceNumber != request.InvoiceNumber)
            {
                purchaseBillExist.InvoiceNumber = request.InvoiceNumber;
            }
            if (purchaseBillExist.StatusId != request.StatusId)
            {
                purchaseBillExist.StatusId = (int)request.StatusId;
            }
            if (purchaseBillExist.ProviderId != request.ProviderId)
            {
                purchaseBillExist.ProviderId = (int)request.ProviderId;
            }
            if (purchaseBillExist.Exempt != request.Exempt)
            {
                purchaseBillExist.Exempt = (decimal)request.Exempt;
            }
            if (purchaseBillExist.Exonerated != request.Exonerated)
            {
                purchaseBillExist.Exonerated = (decimal)request.Exonerated;
            }
            if (purchaseBillExist.TaxedAt15Percent != request.TaxedAt15Percent)
            {
                purchaseBillExist.TaxedAt15Percent = (decimal)request.TaxedAt15Percent;
            }
            if (purchaseBillExist.Taxes15Percent != request.Taxes15Percent)
            {
                purchaseBillExist.Taxes15Percent = (decimal)request.Taxes15Percent;
            }
            if (purchaseBillExist.TaxedAt18Percent != request.TaxedAt18Percent)
            {
                purchaseBillExist.TaxedAt18Percent = (decimal)request.TaxedAt18Percent;
            }
            if (purchaseBillExist.Taxes18Percent != request.Taxes18Percent)
            {
                purchaseBillExist.Taxes18Percent = (decimal)request.Taxes18Percent;
            }
            if (purchaseBillExist.Cai != request.Cai)
            {
                purchaseBillExist.Cai = request.Cai;
            }
            purchaseBillExist.Total = (decimal)(request.Exempt + request.Exonerated + request.Taxes15Percent + request.Taxes18Percent + request.TaxedAt15Percent + request.TaxedAt18Percent);
            purchaseBillExist.Outstanding = purchaseBillExist.Total;
            await _repositoryAsync.UpdateAsync(purchaseBillExist);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<PurchaseBillDto>(purchaseBillExist);
            return new Response<PurchaseBillDto>(dto, message: $"{purchaseBillExist.Id} actualizado correctamente");
        }

    }
}

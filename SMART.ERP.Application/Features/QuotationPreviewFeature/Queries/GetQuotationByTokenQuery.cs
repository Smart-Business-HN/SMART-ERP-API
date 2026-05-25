using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.QuotationSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.QuotationPreviewFeature.Queries
{
    public class GetQuotationByTokenQuery : IRequest<Response<QuotationPreviewDto>>
    {
        public Guid AccessToken { get; set; }
    }

    public class GetQuotationByTokenQueryHandler : IRequestHandler<GetQuotationByTokenQuery, Response<QuotationPreviewDto>>
    {
        private readonly IRepositoryAsync<Quotation> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetQuotationByTokenQueryHandler(IRepositoryAsync<Quotation> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<QuotationPreviewDto>> Handle(GetQuotationByTokenQuery request, CancellationToken cancellationToken)
        {
            var quotation = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterQuotationByAccessTokenSpecification(request.AccessToken));

            if (quotation == null)
            {
                throw new ApiException("Cotización no encontrada o el enlace es inválido.");
            }

            var dto = new QuotationPreviewDto
            {
                Id = quotation.Id,
                QuotationCode = quotation.QuotationCode,
                CreationDate = quotation.CreationDate,
                DueDate = quotation.DueDate,
                StatusName = quotation.Status?.Name,
                CustomerName = quotation.Customer?.FullName,
                CustomerPhone = quotation.Customer?.PhoneNumber,
                CustomerRtn = quotation.Customer?.RTN,
                UserFullName = quotation.User?.FullName,
                Observations = quotation.Observations,
                TermsAndConditions = quotation.TermsAndConditions,
                SubTotal = quotation.SubTotal,
                Total = quotation.Total,
                TotalShippingCost = quotation.TotalShippingCost,
                SubTotalWithoutShipping = quotation.SubTotalWithoutShipping,
            };

            // Get customer address
            if (quotation.Customer?.DeliveryDirections != null && quotation.Customer.DeliveryDirections.Count > 0)
            {
                var favoriteAddress = quotation.Customer.DeliveryDirections.FirstOrDefault(x => x.IsFavorite);
                dto.CustomerAddress = favoriteAddress?.AdditionalInformation
                    ?? quotation.Customer.DeliveryDirections[0].AdditionalInformation;
            }

            // Map products
            dto.ProductsOffered = quotation.ProductsOffered?.Select(p =>
            {
                var isCombo = p.Product?.ProductType == ProductType.Combo;
                List<ComboComponentPreviewDto>? components = null;
                if (isCombo && p.Product?.Components != null)
                {
                    components = p.Product.Components
                        .Where(c => c.IsActive)
                        .Select(c => new ComboComponentPreviewDto
                        {
                            Name = c.Product?.Name ?? c.ProductName,
                            Quantity = c.Quantity,
                            UnitOfMeasurement = c.Product?.UnitOfMeasurement?.Abreviation,
                            DisplayOrder = c.Id
                        })
                        .OrderBy(c => c.DisplayOrder)
                        .ToList();
                }
                return new ProductOfferedPreviewDto
                {
                    Id = p.Id,
                    ProductCode = p.ProductCode,
                    ProductDescription = p.ProductDescription,
                    Quantity = p.Quantity,
                    UnitPrice = p.UnitPrice,
                    TaxId = p.TaxId,
                    TaxRate = p.Tax?.Rate ?? 0,
                    Taxes = p.Taxes,
                    TotalLine = p.TotalLine,
                    Observations = quotation.ItemObservations?
                        .Where(o => o.ProductOfferedId == p.Id)
                        .Select(o => _mapper.Map<QuotationItemObservationDto>(o))
                        .ToList(),
                    IsCombo = isCombo,
                    Components = components
                };
            }).ToList();

            // Map comments
            dto.Comments = quotation.Comments?
                .OrderBy(c => c.CreationDate)
                .Select(c =>
                {
                    var commentDto = _mapper.Map<QuotationCommentDto>(c);
                    if (!c.IsFromClient && c.User != null)
                    {
                        commentDto.UserFullName = c.User.FullName;
                    }
                    return commentDto;
                })
                .ToList();

            return new Response<QuotationPreviewDto>(dto);
        }
    }
}

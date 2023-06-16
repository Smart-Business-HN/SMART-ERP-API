using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.QuoteProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.QuoteProductFeature.Commands.CreateQuoteProductCommand
{
    public class CreateQuoteProductCommand : IRequest<Response<List<QuoteProductDto>>>
    {
        public List<CreateQuoteProductDto> QuoteProducts { get; set; } = null!;
    }

    public class CreateQuoteProductCommandHandler : IRequestHandler<CreateQuoteProductCommand, Response<List<QuoteProductDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<QuoteProduct> _repositoryAsync;
        private readonly IRepositoryAsync<Opportunity> _shoppingCartRepositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;

        public CreateQuoteProductCommandHandler(IMapper mapper, IRepositoryAsync<QuoteProduct> repositoryAsync,
            IRepositoryAsync<Opportunity> shoppingCartRepositoryAsync, IRepositoryAsync<Product> productRepositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _shoppingCartRepositoryAsync = shoppingCartRepositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
        }

        public async Task<Response<List<QuoteProductDto>>> Handle(CreateQuoteProductCommand request, CancellationToken cancellationToken)
        {
            var dtoList = new List<QuoteProductDto>();
            foreach (var quote in request.QuoteProducts)
            {
                var checkIfExist = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterIfExistsProductSpecification(quote.ProductId, quote.OpportunityId));
                if (checkIfExist == null)
                {
                    var product = await _productRepositoryAsync.GetByIdAsync(quote.ProductId);
                    var newRecord = _mapper.Map<QuoteProduct>(quote);
                    newRecord.IsActive = true;
                    newRecord.SalePrice = product!.RecomendedSalePrice;
                    var data = await _repositoryAsync.AddAsync(newRecord);
                    data.Product = product;
                    await _shoppingCartRepositoryAsync.SaveChangesAsync();
                    await UpdateShoppingCart(quote.OpportunityId, newRecord.SalePrice);
                    var dto = _mapper.Map<QuoteProductDto>(data);
                    dtoList.Add(dto);
                }

            }
            return new Response<List<QuoteProductDto>>(dtoList, message: $"Producto agregado exitosamente");
        }

        public async Task UpdateShoppingCart(int opportunityId, decimal recommendedSalePrice)
        {
            var opportunity = await _shoppingCartRepositoryAsync.GetByIdAsync(opportunityId);
            if (opportunity != null)
            {
                if (opportunity.QtyItems == 0)
                {
                    if (opportunity.Total == 0)
                    {
                        opportunity.Total = recommendedSalePrice;
                    }
                    else
                    {
                        opportunity.Total += recommendedSalePrice;
                    }
                    opportunity.QtyItems = 1;
                    await _shoppingCartRepositoryAsync.UpdateAsync(opportunity);
                    await _shoppingCartRepositoryAsync.SaveChangesAsync();
                }
                else
                {
                    if (opportunity.Total == 0)
                    {
                        opportunity.Total = recommendedSalePrice;
                    }
                    else
                    {
                        opportunity.Total += recommendedSalePrice;
                    }
                    opportunity.QtyItems += 1;
                    await _shoppingCartRepositoryAsync.UpdateAsync(opportunity);
                    await _shoppingCartRepositoryAsync.SaveChangesAsync();
                }
            }
        }
    }
}

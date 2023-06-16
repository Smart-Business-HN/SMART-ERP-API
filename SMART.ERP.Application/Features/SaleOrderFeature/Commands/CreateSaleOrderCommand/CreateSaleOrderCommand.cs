using MediatR;
using SMART.ERP.Application.DTOs.SaleOrder;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Specifications.SaleOrderSpecification;
using SMART.ERP.Application.Specifications.StatusSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.SaleOrderFeature.Commands.CreateSaleOrderCommand
{
    public class CreateSaleOrderCommand : IRequest<Response<string>>
    {
        public int OpportunityId { get; set; }
        public List<CreateSaleOrderProductDto> SaleOrderProducts { get; set; } = null!;
    }

    public class CreateSaleOrderCommandHandle : IRequestHandler<CreateSaleOrderCommand, Response<string>>
    {
        private readonly IRepositoryAsync<SaleOrder> _repositoryAsync;
        private readonly IRepositoryAsync<Opportunity> _shoppingCartRepositoryAsync;
        private readonly IRepositoryAsync<Status> _statusAsync;
        private readonly IJwtService _jwtService;

        public CreateSaleOrderCommandHandle(IRepositoryAsync<SaleOrder> repositoryAsync,
            IRepositoryAsync<Opportunity> shoppingCartRepositoryAsync,
            IRepositoryAsync<Status> statusAsync,
            IJwtService jwtService)
        {
            _repositoryAsync = repositoryAsync;
            _shoppingCartRepositoryAsync = shoppingCartRepositoryAsync;
            _statusAsync = statusAsync;
            _jwtService = jwtService;
        }
        public async Task<Response<string>> Handle(CreateSaleOrderCommand request, CancellationToken cancellationToken)
        {
            var shoppiongCart = await _shoppingCartRepositoryAsync.FirstOrDefaultAsync(
                new OpportunityIncludesSpecification(id: request.OpportunityId, customerId: null));
            var status = await _statusAsync.FirstOrDefaultAsync(
                new FilterStatusFromNameSpecification("Cotización"));
            if (shoppiongCart == null && status == null)
            {
                throw new ApiException($"No se encontro el carrito con el id {request.OpportunityId}");
            }
            var saleOrder = new SaleOrder
            {
                Code = await GenerateCode(DateTime.Now),
                OpportunityId = request.OpportunityId,
                CustomerId = shoppiongCart!.CustomerId,
                StatusId = status!.Id,
                CreatedBy = _jwtService.GetSubjectToken(),
                CreationDate = DateTime.Now,
                IsActive = true
            };
            var products = new List<SaleOrderProduct>();

            foreach (var item in request.SaleOrderProducts)
            {
                var product = new SaleOrderProduct
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };
                products.Add(product);
            }
            saleOrder.SaleOrderProducts = products;

            await _repositoryAsync.AddAsync(saleOrder);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>("Cotización creada", message: $"Cotización creada exitosamente con codigo: {saleOrder.Code}");
        }

        public async Task<string> GenerateCode(DateTime date)
        {
            var getLastRegister = await _repositoryAsync.ListAsync(new FilterLastSaleOrderSpecification());
            return CodeIdentity.SOPM + "-" + date.Year.ToString() + "-" + (getLastRegister.Count > 0 ? getLastRegister[0].Id + 1 : 1);
        }
    }
}

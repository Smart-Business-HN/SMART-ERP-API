using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.QuoteProductFeature.Commands.UpdateStatusQuoteProductCommand
{
    public class UpdateStatusQuoteProductCommand : IRequest<Response<QuoteProductDto>>
    {
        public int Id { get; set; }
    }

    public class UpdateStatusQuoteProductCommandHandler : IRequestHandler<UpdateStatusQuoteProductCommand, Response<QuoteProductDto>>
    {
        private readonly IRepositoryAsync<QuoteProduct> _repositoryAsync;
        private readonly IRepositoryAsync<Opportunity> _opportunityRepositoryAsync;
        private readonly IMapper _mapper;

        public UpdateStatusQuoteProductCommandHandler(IRepositoryAsync<QuoteProduct> repositoryAsync, IRepositoryAsync<Opportunity> opportunityRepositoryAsync
            , IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _opportunityRepositoryAsync = opportunityRepositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<QuoteProductDto>> Handle(UpdateStatusQuoteProductCommand request, CancellationToken cancellationToken)
        {
            var checkIfExist = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkIfExist == null)
            {
                throw new KeyNotFoundException($"No se encontro el producto cotizado con id {request.Id}");
            }
            var checkIfOpportunityExist = await _opportunityRepositoryAsync.GetByIdAsync(checkIfExist.OpportunityId);
            if (checkIfOpportunityExist == null)
            {
                throw new KeyNotFoundException("Ocurrio un error con la oportunidad de este producto");
            }
            checkIfExist.IsActive = false;
            checkIfOpportunityExist.Total -= checkIfExist.Quantity * checkIfExist.SalePrice;
            checkIfOpportunityExist.QtyItems -= checkIfExist.Quantity;
            await _repositoryAsync.UpdateAsync(checkIfExist);
            await _repositoryAsync.SaveChangesAsync();
            await _opportunityRepositoryAsync.UpdateAsync(checkIfOpportunityExist);
            await _opportunityRepositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<QuoteProductDto>(checkIfExist);
            return new Response<QuoteProductDto>(dto, "Producto eliminado con exito");
        }
    }
}

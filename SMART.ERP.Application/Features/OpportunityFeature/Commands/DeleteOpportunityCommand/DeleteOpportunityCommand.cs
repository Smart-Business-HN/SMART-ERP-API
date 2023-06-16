using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityFeature.Commands.DeleteOpportunityCommand
{
    public class DeleteOpportunityCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteShoppingCartCommandHandler : IRequestHandler<DeleteOpportunityCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;
        private readonly IRepositoryAsync<QuoteProduct> _quoteProductRepositoryAsync;

        public DeleteShoppingCartCommandHandler(IRepositoryAsync<Opportunity> repositoryAsync, IRepositoryAsync<QuoteProduct> quoteProductRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _quoteProductRepositoryAsync = quoteProductRepositoryAsync;
        }
        public async Task<Response<string>> Handle(DeleteOpportunityCommand request, CancellationToken cancellationToken)
        {
            var opportunity = await _repositoryAsync.FirstOrDefaultAsync(
                new OpportunityIncludesSpecification(request.Id, null));
            if (opportunity == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            if (opportunity.QuoteProducts != null)
            {
                foreach (var item in opportunity.QuoteProducts)
                {
                    await _quoteProductRepositoryAsync.DeleteAsync(item);
                    await _quoteProductRepositoryAsync.SaveChangesAsync();
                }
            }
            await _repositoryAsync.DeleteAsync(opportunity);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"Carrito con codigo {opportunity.Code} eliminado correctamente", "Eliminado correctamente");
        }
    }
}

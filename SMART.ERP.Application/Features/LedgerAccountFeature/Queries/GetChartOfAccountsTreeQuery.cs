using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.LedgerAccount;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.LedgerAccountSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.LedgerAccountFeature.Queries
{
    /// <summary>Devuelve el catálogo de cuentas como árbol jerárquico (ensamblado en memoria).</summary>
    public class GetChartOfAccountsTreeQuery : IRequest<Response<List<LedgerAccountTreeDto>>>
    {
        public class GetChartOfAccountsTreeQueryHandler : IRequestHandler<GetChartOfAccountsTreeQuery, Response<List<LedgerAccountTreeDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<LedgerAccount> _repositoryAsync;

            public GetChartOfAccountsTreeQueryHandler(IMapper mapper, IRepositoryAsync<LedgerAccount> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<Response<List<LedgerAccountTreeDto>>> Handle(GetChartOfAccountsTreeQuery request, CancellationToken cancellationToken)
            {
                var accounts = await _repositoryAsync.ListAsync(new AllLedgerAccountsOrderedSpecification(), cancellationToken);
                var nodes = accounts.Select(a => _mapper.Map<LedgerAccountTreeDto>(a)).ToList();
                var byId = nodes.ToDictionary(n => n.Id);

                var roots = new List<LedgerAccountTreeDto>();
                foreach (var node in nodes)
                {
                    if (node.ParentId.HasValue && byId.TryGetValue(node.ParentId.Value, out var parent))
                        parent.Children.Add(node);
                    else
                        roots.Add(node);
                }

                return new Response<List<LedgerAccountTreeDto>>(roots);
            }
        }
    }
}

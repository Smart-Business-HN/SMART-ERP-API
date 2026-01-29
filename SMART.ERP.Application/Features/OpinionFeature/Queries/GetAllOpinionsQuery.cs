using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpinionSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpinionFeature.Queries
{
    public class GetAllOpinionsQuery : IRequest<PagedResponse<List<OpinionDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public class GetAllOpinionsQueryHandler : IRequestHandler<GetAllOpinionsQuery, PagedResponse<List<OpinionDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Opinion> _repositoryAsync;

            public GetAllOpinionsQueryHandler(IMapper mapper, IRepositoryAsync<Opinion> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<OpinionDto>>> Handle(GetAllOpinionsQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var opinions = await _repositoryAsync.ListAsync(new FilterAndPaginationOpinionSpecification(request.Parameter,
                    request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<OpinionDto>>(opinions);
                return new PagedResponse<List<OpinionDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}

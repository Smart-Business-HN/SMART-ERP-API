using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.BranchOfficeSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BranchOfficeFeature.Queries
{
    public class GetAllBranchOfficesQuery : IRequest<PagedResponse<List<BranchOfficeDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllBranchOfficesQueryHandler : IRequestHandler<GetAllBranchOfficesQuery, PagedResponse<List<BranchOfficeDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<BranchOffices> _repositoryAsync;

            public GetAllBranchOfficesQueryHandler(IMapper mapper, IRepositoryAsync<BranchOffices> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<BranchOfficeDto>>> Handle(GetAllBranchOfficesQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var branchOffices = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationBranchOfficeSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<BranchOfficeDto>>(branchOffices);
                return new PagedResponse<List<BranchOfficeDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}

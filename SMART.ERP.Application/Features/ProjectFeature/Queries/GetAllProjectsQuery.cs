using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Project;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProjectSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProjectFeature.Queries
{
    public class GetAllProjectsQuery : IRequest<PagedResponse<List<ProjectDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, PagedResponse<List<ProjectDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Project> _repositoryAsync;

            public GetAllProjectsQueryHandler(IMapper mapper, IRepositoryAsync<Project> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<PagedResponse<List<ProjectDto>>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var projects = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationProjectSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<ProjectDto>>(projects);
                return new PagedResponse<List<ProjectDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}

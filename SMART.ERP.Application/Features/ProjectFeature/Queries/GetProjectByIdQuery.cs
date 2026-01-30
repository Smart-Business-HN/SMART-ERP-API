using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Project;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProjectSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProjectFeature.Queries
{
    public class GetProjectByIdQuery : IRequest<Response<ProjectDto>>
    {
        public int Id { get; set; }
    }

    public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, Response<ProjectDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Project> _repositoryAsync;

        public GetProjectByIdQueryHandler(IMapper mapper, IRepositoryAsync<Project> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<ProjectDto>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            var project = await _repositoryAsync.FirstOrDefaultAsync(new FilterProjectByIdSpecification(request.Id));
            if (project == null)
            {
                throw new KeyNotFoundException($"Proyecto no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<ProjectDto>(project);
            return new Response<ProjectDto>(dto);
        }
    }
}

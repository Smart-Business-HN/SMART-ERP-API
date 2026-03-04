using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.ProjectAttachment;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProjectAttachmentSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProjectAttachmentFeature.Queries
{
    public class GetProjectAttachmentByIdQuery : IRequest<Response<ProjectAttachmentDto>>
    {
        public int Id { get; set; }
    }

    public class GetProjectAttachmentByIdQueryHandler : IRequestHandler<GetProjectAttachmentByIdQuery, Response<ProjectAttachmentDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<ProjectAttachment> _repositoryAsync;

        public GetProjectAttachmentByIdQueryHandler(IMapper mapper, IRepositoryAsync<ProjectAttachment> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<ProjectAttachmentDto>> Handle(GetProjectAttachmentByIdQuery request, CancellationToken cancellationToken)
        {
            var attachment = await _repositoryAsync.FirstOrDefaultAsync(new ProjectAttachmentIncludesSpecification(request.Id));
            if (attachment == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<ProjectAttachmentDto>(attachment);
            return new Response<ProjectAttachmentDto>(dto);
        }
    }
}

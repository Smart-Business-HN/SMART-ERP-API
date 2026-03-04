using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.ProjectAttachment;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProjectAttachmentSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProjectAttachmentFeature.Queries
{
    public class GetAllProjectAttachmentsByProjectIdQuery : IRequest<PagedResponse<List<ProjectAttachmentDto>>>
    {
        public int ProjectId { get; set; }
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllProjectAttachmentsByProjectIdQueryHandler : IRequestHandler<GetAllProjectAttachmentsByProjectIdQuery, PagedResponse<List<ProjectAttachmentDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<ProjectAttachment> _repositoryAsync;

            public GetAllProjectAttachmentsByProjectIdQueryHandler(IMapper mapper, IRepositoryAsync<ProjectAttachment> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<PagedResponse<List<ProjectAttachmentDto>>> Handle(GetAllProjectAttachmentsByProjectIdQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var attachments = await _repositoryAsync.ListAsync(
                    new PaginationProjectAttachmentSpecification(request.ProjectId, request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<ProjectAttachmentDto>>(attachments);
                return new PagedResponse<List<ProjectAttachmentDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}

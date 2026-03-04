using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.ProjectAttachmentCategory;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProjectAttachmentCategorySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProjectAttachmentCategoryFeature.Queries
{
    public class GetAllProjectAttachmentCategoriesQuery : IRequest<PagedResponse<List<ProjectAttachmentCategoryDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllProjectAttachmentCategoriesQueryHandler : IRequestHandler<GetAllProjectAttachmentCategoriesQuery, PagedResponse<List<ProjectAttachmentCategoryDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<ProjectAttachmentCategory> _repositoryAsync;

            public GetAllProjectAttachmentCategoriesQueryHandler(IMapper mapper, IRepositoryAsync<ProjectAttachmentCategory> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<PagedResponse<List<ProjectAttachmentCategoryDto>>> Handle(GetAllProjectAttachmentCategoriesQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var records = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationProjectAttachmentCategorySpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<ProjectAttachmentCategoryDto>>(records);
                return new PagedResponse<List<ProjectAttachmentCategoryDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}

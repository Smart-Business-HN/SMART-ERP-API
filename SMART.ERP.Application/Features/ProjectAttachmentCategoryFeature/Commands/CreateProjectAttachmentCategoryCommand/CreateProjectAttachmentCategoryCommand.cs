using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.ProjectAttachmentCategory;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProjectAttachmentCategorySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProjectAttachmentCategoryFeature.Commands.CreateProjectAttachmentCategoryCommand
{
    public class CreateProjectAttachmentCategoryCommand : IRequest<Response<ProjectAttachmentCategoryDto>>
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class CreateProjectAttachmentCategoryCommandHandler : IRequestHandler<CreateProjectAttachmentCategoryCommand, Response<ProjectAttachmentCategoryDto>>
    {
        private readonly IRepositoryAsync<ProjectAttachmentCategory> _repositoryAsync;
        private readonly IMapper _mapper;

        public CreateProjectAttachmentCategoryCommandHandler(IRepositoryAsync<ProjectAttachmentCategory> repositoryAsync,
            IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<ProjectAttachmentCategoryDto>> Handle(CreateProjectAttachmentCategoryCommand request, CancellationToken cancellationToken)
        {
            var checkDuplicate = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterProjectAttachmentCategoryFromNameSpecification(request.Name));
            if (checkDuplicate != null)
                throw new ApiException($"Ya existe una categoria con el nombre: {request.Name}");

            var newRecord = _mapper.Map<ProjectAttachmentCategory>(request);
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();

            var dto = _mapper.Map<ProjectAttachmentCategoryDto>(data);
            return new Response<ProjectAttachmentCategoryDto>(dto, message: "Categoria creada exitosamente");
        }
    }
}

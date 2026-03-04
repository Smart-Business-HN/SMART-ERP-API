using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.ProjectAttachmentCategory;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProjectAttachmentCategoryFeature.Commands.UpdateProjectAttachmentCategoryCommand
{
    public class UpdateProjectAttachmentCategoryCommand : IRequest<Response<ProjectAttachmentCategoryDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class UpdateProjectAttachmentCategoryCommandHandler : IRequestHandler<UpdateProjectAttachmentCategoryCommand, Response<ProjectAttachmentCategoryDto>>
    {
        private readonly IRepositoryAsync<ProjectAttachmentCategory> _repositoryAsync;
        private readonly IMapper _mapper;

        public UpdateProjectAttachmentCategoryCommandHandler(IRepositoryAsync<ProjectAttachmentCategory> repositoryAsync,
            IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<ProjectAttachmentCategoryDto>> Handle(UpdateProjectAttachmentCategoryCommand request, CancellationToken cancellationToken)
        {
            var record = await _repositoryAsync.GetByIdAsync(request.Id);
            if (record == null)
            {
                throw new KeyNotFoundException($"No se encontro la categoria con id {request.Id}");
            }

            record.Name = request.Name;
            record.IsActive = request.IsActive;

            await _repositoryAsync.UpdateAsync(record);
            await _repositoryAsync.SaveChangesAsync();

            var dto = _mapper.Map<ProjectAttachmentCategoryDto>(record);
            return new Response<ProjectAttachmentCategoryDto>(dto, $"{request.Name} actualizado correctamente");
        }
    }
}

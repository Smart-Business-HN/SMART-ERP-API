using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.ProjectAttachment;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProjectAttachmentFeature.Commands.CreateProjectAttachmentCommand
{
    public class CreateProjectAttachmentCommand : IRequest<Response<ProjectAttachmentDto>>
    {
        public string Name { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public Guid UserId { get; set; }
        public int ProjectAttachmentCategoryId { get; set; }
        public int ProjectId { get; set; }
    }

    public class CreateProjectAttachmentCommandHandler : IRequestHandler<CreateProjectAttachmentCommand, Response<ProjectAttachmentDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<ProjectAttachment> _repositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<ProjectAttachmentCategory> _categoryRepositoryAsync;
        private readonly IRepositoryAsync<Project> _projectRepositoryAsync;
        private readonly IJwtService _jwtService;

        public CreateProjectAttachmentCommandHandler(
            IMapper mapper,
            IRepositoryAsync<ProjectAttachment> repositoryAsync,
            IRepositoryAsync<User> userRepositoryAsync,
            IRepositoryAsync<ProjectAttachmentCategory> categoryRepositoryAsync,
            IRepositoryAsync<Project> projectRepositoryAsync,
            IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _categoryRepositoryAsync = categoryRepositoryAsync;
            _projectRepositoryAsync = projectRepositoryAsync;
            _jwtService = jwtService;
        }

        public async Task<Response<ProjectAttachmentDto>> Handle(CreateProjectAttachmentCommand request, CancellationToken cancellationToken)
        {
            var getUser = await _userRepositoryAsync.GetByIdAsync(request.UserId);
            if (getUser == null)
                throw new ApiException($"No se encontro el usuario con el id: {request.UserId}");

            var getCategory = await _categoryRepositoryAsync.GetByIdAsync(request.ProjectAttachmentCategoryId);
            if (getCategory == null)
                throw new ApiException($"No se encontro la categoria con el id: {request.ProjectAttachmentCategoryId}");

            var getProject = await _projectRepositoryAsync.GetByIdAsync(request.ProjectId);
            if (getProject == null)
                throw new ApiException($"No se encontro el proyecto con el id: {request.ProjectId}");

            var newRecord = _mapper.Map<ProjectAttachment>(request);
            newRecord.CreationDate = DateTime.Now;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            var response = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            newRecord.ProjectAttachmentCategory = getCategory;
            newRecord.User = getUser;
            var dto = _mapper.Map<ProjectAttachmentDto>(response);
            return new Response<ProjectAttachmentDto>(dto, message: "Adjunto creado exitosamente");
        }
    }
}

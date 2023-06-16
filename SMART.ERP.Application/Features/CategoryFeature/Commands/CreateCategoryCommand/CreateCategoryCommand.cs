using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.CategorySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CategoryFeature.Commands.CreateCategoryCommand
{
    public class CreateCategoryCommand : IRequest<Response<CategoryDto>>
    {
        public string Name { get; set; } = null!;
        public string Image { get; set; } = null!;
        public bool IsPartCategory { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Response<CategoryDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Category> _repositoryAsync;
        private readonly IJwtService _jwtService;

        public CreateCategoryCommandHandler(
            IMapper mapper,
            IRepositoryAsync<Category> repositoryAsync,
            IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
        }

        public async Task<Response<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _repositoryAsync.FirstOrDefaultAsync(new FilterCategorySpecification(request.Name, null));
            if (category != null)
            {
                throw new ApiException($"Ya existe una categoria con el nombre {request.Name}");
            }
            var newRecord = _mapper.Map<Category>(request);
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            newRecord.CreationDate = DateTime.Now;

            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<CategoryDto>(data);
            return new Response<CategoryDto>(dto, message: $"{request.Name} creado exitosamente");
        }
    }
}

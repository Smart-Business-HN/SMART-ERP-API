using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.CategorySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CategoryFeature.Commands.UpdateCategoryCommand
{
    public class UpdateCategoryCommand : IRequest<Response<CategoryDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Image { get; set; } = null!;
        public bool IsPartCategory { get; set; }
        public bool IsActive { get; set; }
    }
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Response<CategoryDto>>
    {
        private readonly IRepositoryAsync<Category> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;

        public UpdateCategoryCommandHandler(IRepositoryAsync<Category> repositoryAsync, IMapper mapper,
            IJwtService jwtService)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _jwtService = jwtService;
        }
        public async Task<Response<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _repositoryAsync.GetByIdAsync(request.Id);
            if (category == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var filterByName = await _repositoryAsync.FirstOrDefaultAsync(
                    new FilterCategorySpecification(request.Name, request.Id));
            if (filterByName != null)
            {
                throw new ApiException($"Ya existe una categoria con el nombre {request.Name}");
            }
            else
            {
                category.Name = request.Name;
                category.Image = request.Image;
                category.IsPartCategory = request.IsPartCategory;
                category.IsActive = request.IsActive;
                category.ModificationDate = DateTime.Now;
                category.ModificatedBy = _jwtService.GetSubjectToken();
                await _repositoryAsync.UpdateAsync(category);
                await _repositoryAsync.SaveChangesAsync();
                var dto = _mapper.Map<CategoryDto>(category);
                return new Response<CategoryDto>(dto, message: $"{category.Name} actualizado correctamente");
            }
        }
    }
}

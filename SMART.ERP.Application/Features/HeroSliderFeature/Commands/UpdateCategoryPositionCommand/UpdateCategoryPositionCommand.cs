using AutoMapper;
using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CategorySpecification;
using SMART.ERP.Application.Specifications.HeroSliderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Company;

namespace SMART.ERP.Application.Features.HeroSliderFeature.Commands.UpdateCategoryPositionCommand
{
    public class UpdateCategoryPositionCommand : IRequest<Response<CategoryWithHeroSliderDto>>
    {
        public int Id { get; set; }
        public int Position { get; set; }

    }

    public class UpdateCategoryPositionCommandHandler : IRequestHandler<UpdateCategoryPositionCommand, Response<CategoryWithHeroSliderDto>>
    {
        private readonly IRepositoryAsync<Category> _repositoryAsync;
        private readonly IMapper _mapper;

        public UpdateCategoryPositionCommandHandler(IRepositoryAsync<Category> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<CategoryWithHeroSliderDto>> Handle(UpdateCategoryPositionCommand request, CancellationToken cancellationToken)
        {
            var category = await _repositoryAsync.GetByIdAsync(request.Id);
            if (category == null)
            {
                throw new KeyNotFoundException($"No se encontro la categoria con el id {request.Id}");
            }
            if (category.Position == 0)
            {
                var listCategory = await _repositoryAsync.ListAsync(new FilterCategoryPositionSpecification(null, null));
                if (listCategory.Count > 0)
                {
                    foreach (var item in listCategory)
                    {
                        item.Position += 1;
                    }
                    category.Position = 1;
                    listCategory.Add(category);
                    await _repositoryAsync.UpdateRangeAsync(listCategory);
                    await _repositoryAsync.SaveChangesAsync();
                }
                else
                {
                    category.Position = 1;
                    await _repositoryAsync.UpdateAsync(category);
                    await _repositoryAsync.SaveChangesAsync();
                }
            }
            else
            {
                var listCategory = await _repositoryAsync.ListAsync(new FilterCategoryPositionSpecification(category.Position, request.Position));
                if (listCategory.Count == 0)
                {
                    throw new ApiException("No es posible mover esta categoria, intenta asignar mas categorias.");
                }
                if (request.Position < category.Position)
                {
                    foreach (var item in listCategory)
                    {
                        item.Position += 1;
                    }
                    category.Position = request.Position;
                }
                else
                {
                    foreach (var item in listCategory)
                    {
                        item.Position -= 1;
                    }
                    category.Position = request.Position;
                }
                listCategory.Add(category);
                await _repositoryAsync.UpdateRangeAsync(listCategory);
                await _repositoryAsync.SaveChangesAsync();
            }
            var categorySlider = await _repositoryAsync.FirstOrDefaultAsync(new IncludeHeroSliderSpecification(request.Id));
            var dto = _mapper.Map<CategoryWithHeroSliderDto>(categorySlider);
            return new Response<CategoryWithHeroSliderDto>(dto, "Actualizado correctamente");
        }
    }
}

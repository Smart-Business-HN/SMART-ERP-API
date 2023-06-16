using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CategorySpecification;
using SMART.ERP.Application.Specifications.HeroSliderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.HeroSliderFeature.Commands.DeleteHeroSliderCommand
{
    public class DeleteHeroSliderCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteHeroSliderCommandHandler : IRequestHandler<DeleteHeroSliderCommand, Response<string>>
    {
        private readonly IRepositoryAsync<HeroSlider> _repositoryAsync;
        private readonly IRepositoryAsync<Category> _categoryRepositoryAsync;

        public DeleteHeroSliderCommandHandler(IRepositoryAsync<HeroSlider> repositoryAsync, IRepositoryAsync<Category> categoryRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _categoryRepositoryAsync = categoryRepositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteHeroSliderCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepositoryAsync.GetByIdAsync(request.Id);
            var listCategory = await _categoryRepositoryAsync.ListAsync(new FilterCategoryPositionSpecification(null, null));
            if (category == null)
            {
                throw new KeyNotFoundException($"No se encontro la categoria con el id {request.Id}");
            }
            for (int i = 0; i < listCategory.Count; i++)
            {
                if (listCategory[i].Position > category.Position)
                {
                    listCategory[i].Position -= 1;
                }
                if (listCategory[i].Id == category.Id)
                {
                    listCategory[i].Position = 0;
                }
            }
            await _categoryRepositoryAsync.UpdateRangeAsync(listCategory);
            await _categoryRepositoryAsync.SaveChangesAsync();
            var listProductSlider = await _repositoryAsync.ListAsync(new FilterHeroFromProductSpecification(0, request.Id));
            await _repositoryAsync.DeleteRangeAsync(listProductSlider);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>("Eliminado correctamente");
        }
    }
}

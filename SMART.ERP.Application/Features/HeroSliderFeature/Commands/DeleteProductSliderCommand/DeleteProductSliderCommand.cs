using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.HeroSliderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.HeroSliderFeature.Commands.DeleteProductSliderCommand
{
    public class DeleteProductSliderCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteProductSliderCommandHandler : IRequestHandler<DeleteProductSliderCommand, Response<string>>
    {
        private readonly IRepositoryAsync<HeroSlider> _repositoryAsync;

        public DeleteProductSliderCommandHandler(IRepositoryAsync<HeroSlider> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteProductSliderCommand request, CancellationToken cancellationToken)
        {
            var slider = await _repositoryAsync.FirstOrDefaultAsync(new FilterHeroFromProductSpecification(request.Id, 0));
            if (slider == null)
            {
                throw new KeyNotFoundException($"No se encontro el slider con el id {request.Id}");
            }
            var existsList = await _repositoryAsync.ListAsync(new FilterHeroFromProductSpecification(0, slider.CategoryId));
            if (existsList.Count > 0)
            {
                for (var i = 0; i < existsList.Count; i++)
                {
                    if (existsList[i].Position > slider.Position)
                    {
                        existsList[i].Position = existsList[i].Position - 1;
                    }
                    if (existsList[i].ProductId == slider.ProductId)
                    {
                        existsList.RemoveAt(i);
                    }
                }
                await _repositoryAsync.UpdateRangeAsync(existsList);
                await _repositoryAsync.SaveChangesAsync();
            }
            await _repositoryAsync.DeleteAsync(slider);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>("Eliminado correctamente");
        }
    }
}

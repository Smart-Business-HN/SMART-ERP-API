using AutoMapper;
using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.HeroSliderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Company;

namespace SMART.ERP.Application.Features.HeroSliderFeature.Commands.UpdateHeroSliderCommand
{
    public class UpdateHeroSliderCommand : IRequest<Response<HeroSliderDto>>
    {
        public int Id { get; set; }
        public int Position { get; set; }
    }

    public class UpdateHeroSliderCommandHandler : IRequestHandler<UpdateHeroSliderCommand, Response<HeroSliderDto>>
    {
        private readonly IRepositoryAsync<HeroSlider> _repositoryAsync;
        private readonly IMapper _mapper;

        public UpdateHeroSliderCommandHandler(IRepositoryAsync<HeroSlider> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<HeroSliderDto>> Handle(UpdateHeroSliderCommand request, CancellationToken cancellationToken)
        {
            var heroSlider = await _repositoryAsync.GetByIdAsync(request.Id);
            if (heroSlider == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }

            var listHero = await _repositoryAsync.ListAsync(new FilterHeroFromPositionSpecification(heroSlider.Position, request.Position, heroSlider.CategoryId));
            if (listHero.Count == 0)
            {
                throw new ApiException($"No es posible actualizar la posicion de este hero slider, intenta agregar mas productos.");
            }
            foreach (var item in listHero)
            {
                if (request.Position > heroSlider.Position)
                {
                    item.Position -= 1;
                }
                else
                {
                    item.Position += 1;
                }
            }
            heroSlider.Position = request.Position;
            listHero.Add(heroSlider);
            await _repositoryAsync.UpdateRangeAsync(listHero);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<HeroSliderDto>(heroSlider);
            return new Response<HeroSliderDto>(dto, message: $"Actualizado correctamente");
        }
    }
}

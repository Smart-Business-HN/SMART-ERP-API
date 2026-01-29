using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.HeroSliderFeature.Commands.CreateHeroSliderCommand
{
    public class CreateHeroSliderCommand : IRequest<Response<HeroSliderDto>>
    {
        public int Position { get; set; }
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
    }

    public class CreateHeroSliderCommandHandler : IRequestHandler<CreateHeroSliderCommand, Response<HeroSliderDto>>
    {
        private readonly IRepositoryAsync<HeroSlider> _repositoryAsync;
        private readonly IRepositoryAsync<Category> _categoryRepositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;
        private readonly IMapper _mapper;

        public CreateHeroSliderCommandHandler(IRepositoryAsync<HeroSlider> repositoryAsync, IMapper mapper,
            IRepositoryAsync<Category> categoryRepositoryAsync, IRepositoryAsync<Product> productRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _categoryRepositoryAsync = categoryRepositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
        }

        public async Task<Response<HeroSliderDto>> Handle(CreateHeroSliderCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepositoryAsync.GetByIdAsync(request.CategoryId);
            if (category == null)
                throw new ApiException($"No existe ninguna categoria con el Id {request.CategoryId}");
            var product = await _productRepositoryAsync.FirstOrDefaultAsync(new ProductIncludesSpecification(request.ProductId, slug: null));
            if (product == null)
                throw new ApiException($"No existe ningun producto con el Id {request.ProductId}");
            if (category != null && product != null)
            {
                if (product.SubCategory != null)
                {
                    if (product.SubCategory!.CategoryId != category.Id)
                    {
                        throw new ApiException($"El producto {product.Name} no pertenece a la categoria {category.Name}");
                    }
                }
            }

            var newRecord = _mapper.Map<HeroSlider>(request);
            var response = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<HeroSliderDto>(response);

            return new Response<HeroSliderDto>(dto, message: $"Producto asignado correctamente");
        }
    }
}

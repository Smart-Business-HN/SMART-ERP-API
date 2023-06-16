using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.ProductFeatureSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProductFtrFeature.Commands.UpdateProductFtrCommand
{
    public class UpdateProductFtrCommand : IRequest<Response<ProductFeatureDto>>
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class UpdateProductFtrCommandHandler : IRequestHandler<UpdateProductFtrCommand, Response<ProductFeatureDto>>
    {
        private readonly IRepositoryAsync<ProductFeature> _repositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public UpdateProductFtrCommandHandler(IMapper mapper, IRepositoryAsync<ProductFeature> repositoryAsync,
            IJwtService jwtService)
        {
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
            _mapper = mapper;
        }
        public async Task<Response<ProductFeatureDto>> Handle(UpdateProductFtrCommand request, CancellationToken cancellationToken)
        {
            var productFeature = await _repositoryAsync.GetByIdAsync(request.Id);
            if (productFeature == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var filterByName = await _repositoryAsync.FirstOrDefaultAsync(new FilterProductFeatureFromTitleSpecification(request.Title, productFeature.ProductId));
            if (filterByName != null && filterByName.Id != request.Id)
            {
                throw new ApiException($"Ya existe un registro con el titulo {request.Title}");
            }
            productFeature.Title = request.Title;
            productFeature.Description = request.Description;
            productFeature.IsActive = request.IsActive;
            productFeature.ModificatedBy = _jwtService.GetSubjectToken();
            productFeature.ModificationDate = DateTime.Now;
            await _repositoryAsync.UpdateAsync(productFeature);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<ProductFeatureDto>(productFeature);
            return new Response<ProductFeatureDto>(dto, message: $"{productFeature.Title} actualizado correctamente");
        }
    }
}

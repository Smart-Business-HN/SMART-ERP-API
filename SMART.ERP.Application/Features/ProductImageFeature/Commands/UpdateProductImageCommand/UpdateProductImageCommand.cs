using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProductImageSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProductImageFeature.Commands.UpdateProductImageCommand
{
    public class UpdateProductImageCommand : IRequest<Response<ProductImageDto>>
    {
        public int Id { get; set; }
        public string FileName { get; set; } = null!;
        public string Url { get; set; } = null!;
        public int ProductId { get; set; }
    }

    public class UpdateProductImageCommandHandler : IRequestHandler<UpdateProductImageCommand, Response<ProductImageDto>>
    {
        private readonly IRepositoryAsync<ProductImage> _repositoryAsync;
        private readonly IMapper _mapper;

        public UpdateProductImageCommandHandler(IMapper mapper, IRepositoryAsync<ProductImage> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<ProductImageDto>> Handle(UpdateProductImageCommand request, CancellationToken cancellationToken)
        {
            var productImage = await _repositoryAsync.GetByIdAsync(request.Id);
            if (productImage == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var filterByName = await _repositoryAsync.FirstOrDefaultAsync(new FilterProductImageFromFilenameSpecification(request.FileName));
            if (filterByName != null && filterByName.Id != request.Id)
            {
                throw new ApiException($"Ya existe un registro con el titulo {request.FileName}");
            }
            productImage.FileName = request.FileName;
            productImage.Url = request.Url;
            await _repositoryAsync.UpdateAsync(productImage);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<ProductImageDto>(productImage);
            return new Response<ProductImageDto>(dto, message: $"{productImage.FileName} actualizado correctamente");
        }
    }
}

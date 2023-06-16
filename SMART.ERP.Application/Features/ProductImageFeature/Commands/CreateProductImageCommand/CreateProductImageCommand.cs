using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProductImageSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProductImageFeature.Commands.CreateProductImageCommand
{
    public class CreateProductImageCommand : IRequest<Response<ProductImageDto>>
    {
        public string FileName { get; set; } = null!;
        public string Url { get; set; } = null!;
        public int ProductId { get; set; }
    }

    public class CreateProductImageCommandHandler : IRequestHandler<CreateProductImageCommand, Response<ProductImageDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<ProductImage> _repositoryAsync;

        public CreateProductImageCommandHandler(IMapper mapper, IRepositoryAsync<ProductImage> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<ProductImageDto>> Handle(CreateProductImageCommand request, CancellationToken cancellationToken)
        {
            var checkIfExist = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterProductImageFromFilenameSpecification(request.FileName));
            if (checkIfExist != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.FileName}");
            }
            var newRecord = _mapper.Map<ProductImage>(request);
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<ProductImageDto>(data);

            return new Response<ProductImageDto>(dto, message: $"{request.FileName} creado exitosamente");
        }
    }
}

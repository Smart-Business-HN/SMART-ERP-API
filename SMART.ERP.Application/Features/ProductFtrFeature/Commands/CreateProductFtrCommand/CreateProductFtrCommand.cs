using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.ProductFeatureSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProductFtrFeature.Commands.CreateProductFtrCommand
{
    public class CreateProductFtrCommand : IRequest<Response<ProductFeatureDto>>
    {
        public string Title { get; set; } = null!;
        public int ProductId { get; set; }
        public string Description { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class CreateProductFtrCommandHandler : IRequestHandler<CreateProductFtrCommand, Response<ProductFeatureDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<ProductFeature> _repositoryAsync;
        private readonly IJwtService _jwtService;

        public CreateProductFtrCommandHandler(IMapper mapper, IRepositoryAsync<ProductFeature> repositoryAsync,
            IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
        }

        public async Task<Response<ProductFeatureDto>> Handle(CreateProductFtrCommand request, CancellationToken cancellationToken)
        {
            var checkIfExist = await _repositoryAsync.FirstOrDefaultAsync(new FilterProductFeatureFromTitleSpecification(request.Title, request.ProductId));
            if (checkIfExist != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Title}");
            }
            var newRecord = _mapper.Map<ProductFeature>(request);
            newRecord.CreationDate = DateTime.Now;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<ProductFeatureDto>(data);

            return new Response<ProductFeatureDto>(dto, message: $"{request.Title} creado exitosamente");
        }
    }
}

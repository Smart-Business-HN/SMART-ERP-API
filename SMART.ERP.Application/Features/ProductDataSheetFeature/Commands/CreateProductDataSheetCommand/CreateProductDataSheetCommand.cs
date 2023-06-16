using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.ProductDataSheetSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProductDataSheetFeature.Commands.CreateProductDataSheetCommand
{
    public class CreateProductDataSheetCommand : IRequest<Response<ProductDataSheetDto>>
    {
        public string Title { get; set; } = null!;
        public int ProductId { get; set; }
        public int DataSheetId { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateProductDataSheetCommandHandler : IRequestHandler<CreateProductDataSheetCommand, Response<ProductDataSheetDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<ProductDataSheet> _repositoryAsync;
        private readonly IRepositoryAsync<DataSheet> _dataSheetRepositoryAsync;
        private readonly IJwtService _jwtService;

        public CreateProductDataSheetCommandHandler(IMapper mapper, IRepositoryAsync<ProductDataSheet> repositoryAsync,
            IRepositoryAsync<DataSheet> dataSheetRepositoryAsync, IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _dataSheetRepositoryAsync = dataSheetRepositoryAsync;
            _jwtService = jwtService;
        }

        public async Task<Response<ProductDataSheetDto>> Handle(CreateProductDataSheetCommand request, CancellationToken cancellationToken)
        {
            var checkIfExist = await _repositoryAsync.FirstOrDefaultAsync(new FilterProductDataSheetFromTitleSpecification(request.Title));
            if (checkIfExist != null && checkIfExist.ProductId == request.ProductId)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Title}");
            }
            else
            {
                var newRecord = _mapper.Map<ProductDataSheet>(request);

                var getDataSheet = await _dataSheetRepositoryAsync.GetByIdAsync(request.DataSheetId);
                if (getDataSheet == null)
                {
                    throw new ApiException($"No se encontro ninguna caracteristica con el id {request.DataSheetId}");
                }
                newRecord.CreatedBy = _jwtService.GetSubjectToken();
                newRecord.CreationDate = DateTime.Now;
                var data = await _repositoryAsync.AddAsync(newRecord);
                await _repositoryAsync.SaveChangesAsync();
                var dto = _mapper.Map<ProductDataSheetDto>(data);

                return new Response<ProductDataSheetDto>(dto, message: $"{request.Title} creado exitosamente");
            }
        }
    }
}

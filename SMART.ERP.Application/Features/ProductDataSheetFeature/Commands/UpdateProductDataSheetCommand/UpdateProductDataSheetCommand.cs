using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.ProductDataSheetSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProductDataSheetFeature.Commands.UpdateProductDataSheetCommand
{
    public class UpdateProductDataSheetCommand : IRequest<Response<ProductDataSheetDto>>
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int ProductId { get; set; }
        public int DataSheetId { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateProductDataSheetCommandHandler : IRequestHandler<UpdateProductDataSheetCommand, Response<ProductDataSheetDto>>
    {
        private readonly IRepositoryAsync<ProductDataSheet> _repositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public UpdateProductDataSheetCommandHandler(IMapper mapper, IRepositoryAsync<ProductDataSheet> repositoryAsync,
            IJwtService jwtService)
        {
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
            _mapper = mapper;
        }
        public async Task<Response<ProductDataSheetDto>> Handle(UpdateProductDataSheetCommand request, CancellationToken cancellationToken)
        {
            var productDataSheet = await _repositoryAsync.GetByIdAsync(request.Id);
            if (productDataSheet == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var filterByName = await _repositoryAsync.FirstOrDefaultAsync(new FilterProductDataSheetFromTitleSpecification(request.Title));
            if (filterByName != null && filterByName.Id != request.Id && productDataSheet.ProductId == request.ProductId)
            {
                throw new ApiException($"Ya existe un registro con el titulo {request.Title}");
            }
            productDataSheet.Title = request.Title;
            productDataSheet.IsActive = request.IsActive;
            productDataSheet.ModificationDate = DateTime.Now;
            productDataSheet.ModificatedBy = _jwtService.GetSubjectToken();
            await _repositoryAsync.UpdateAsync(productDataSheet);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<ProductDataSheetDto>(productDataSheet);
            return new Response<ProductDataSheetDto>(dto, message: $"{productDataSheet.Title} actualizado correctamente");
        }
    }
}

using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.Discount;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.DiscountSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DiscountFeature.Commands.UpdateDiscountCommand
{
    public class UpdateDiscountCommand : IRequest<Response<DiscountDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Rate { get; set; }
    }
    public class UpdateDiscountCommandHandler : IRequestHandler<UpdateDiscountCommand, Response<DiscountDto>>
    {
        private readonly IRepositoryAsync<Discount> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IOutputCacheStore _outputCacheStored;
        public UpdateDiscountCommandHandler(IRepositoryAsync<Discount> repositoryAsync, IMapper mapper, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _outputCacheStored = outputCacheStored;
        }
        public async Task<Response<DiscountDto>> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
        {
            var discount = await _repositoryAsync.GetByIdAsync(request.Id);
            if (discount == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var filterByName = await _repositoryAsync.FirstOrDefaultAsync(new FilterDiscountByNameSpecification(request.Name, request.Id));
            if (filterByName != null)
            {
                throw new ApiException($"Ya existe una impuesto con el nombre {request.Name}");
            }
            discount.Name = request.Name;
            discount.Rate = request.Rate;
            await _repositoryAsync.UpdateAsync(discount);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_discounts", cancellationToken);
            var dto = _mapper.Map<DiscountDto>(discount);
            return new Response<DiscountDto>(dto, message: $"{discount.Name} actualizado correctamente");
        }
    }
}
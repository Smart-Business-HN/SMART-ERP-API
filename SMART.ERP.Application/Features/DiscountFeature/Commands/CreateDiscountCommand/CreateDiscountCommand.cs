using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.Discount;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.DiscountSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DiscountFeature.Commands.CreateDiscountCommand
{
    public class CreateDiscountCommand : IRequest<Response<DiscountDto>>
    {
        public string Name { get; set; } = null!;
        public int Rate { get; set; }
    }
    public class CreateDiscountCommandHandler : IRequestHandler<CreateDiscountCommand, Response<DiscountDto>>
    {
        private readonly IRepositoryAsync<Discount> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IOutputCacheStore _outputCacheStored;
        public CreateDiscountCommandHandler(IRepositoryAsync<Discount> repositoryAsync, IMapper mapper, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _outputCacheStored = outputCacheStored;
        }
        public async Task<Response<DiscountDto>> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
        {
            var checkDiscount = await _repositoryAsync.FirstOrDefaultAsync(new FilterDiscountByNameSpecification(request.Name,null));
            if (checkDiscount != null)
            {
                throw new ApiException($"Ya existe un descuento con el nombre {request.Name}");
            }
            var newRecord = _mapper.Map<Discount>(request);
            var response = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_discounts", cancellationToken);
            var dto = _mapper.Map<DiscountDto>(response);
            return new Response<DiscountDto>(dto, $"Descuento {request.Name} creado exitosamente.");
        }
    }
}


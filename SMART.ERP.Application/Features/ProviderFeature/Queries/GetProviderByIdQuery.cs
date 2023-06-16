using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Product;

namespace SMART.ERP.Application.Features.ProviderFeature.Queries
{
    public class GetProviderByIdQuery : IRequest<Response<ProviderDto>>
    {
        public int Id { get; set; }
    }
    public class GetProviderByIdQueryHandler : IRequestHandler<GetProviderByIdQuery, Response<ProviderDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Provider> _repositoryAsync;

        public GetProviderByIdQueryHandler(IMapper mapper, IRepositoryAsync<Provider> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<ProviderDto>> Handle(GetProviderByIdQuery request, CancellationToken cancellationToken)
        {
            var provider = await _repositoryAsync.GetByIdAsync(request.Id);
            if (provider == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<ProviderDto>(provider);
            return new Response<ProviderDto>(dto);
        }
    }
}

using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Cai;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CaiFeature.Queries
{
    public class GetCaiByIdQuery : IRequest<Response<CaiDto>>
    {
        public int Id { get; set; }

        public class GetCaiByIdQueryHandler : IRequestHandler<GetCaiByIdQuery,Response<CaiDto>> {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Cai> _repositoryAsync;
            public GetCaiByIdQueryHandler(IMapper mapper, IRepositoryAsync<Cai> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<Response<CaiDto>> Handle(GetCaiByIdQuery request, CancellationToken cancellationToken)
            {
                var cai = await _repositoryAsync.GetByIdAsync(request.Id);
                if(cai == null)
                {
                    throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
                }
                var dto = _mapper.Map<CaiDto>(cai);
                return new Response<CaiDto>(dto);
            }
        }
    }
}

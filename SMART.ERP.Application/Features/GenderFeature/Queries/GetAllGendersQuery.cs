using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.User;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.GenderFeature.Queries
{
    public class GetAllGendersQuery : IRequest<Response<List<GenderDto>>>
    {
        public class GetAllGendersQueryHandler : IRequestHandler<GetAllGendersQuery, Response<List<GenderDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Gender> _repositoryAsync;

            public GetAllGendersQueryHandler(IMapper mapper, IRepositoryAsync<Gender> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<Response<List<GenderDto>>> Handle(GetAllGendersQuery request, CancellationToken cancellationToken)
            {
                var genders = await _repositoryAsync.ListAsync();
                var dto = _mapper.Map<List<GenderDto>>(genders);
                return new Response<List<GenderDto>>(dto);
            }
        }
    }
}

using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.User;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.MASTER.Domain.Entities;

namespace SMART.ERP.Application.Features.GenderFeature.Queries
{
    public class GetAllClientGenderQuery : IRequest<Response<List<GenderDto>>>
    {
        public class GetAllClientGenderQueryHandler : IRequestHandler<GetAllClientGenderQuery, Response<List<GenderDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryHNAsync<ClientGender> _repositoryAsync;

            public GetAllClientGenderQueryHandler(IMapper mapper, IRepositoryHNAsync<ClientGender> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<Response<List<GenderDto>>> Handle(GetAllClientGenderQuery request, CancellationToken cancellationToken)
            {
                var genders = await _repositoryAsync.ListAsync();
                var dto = _mapper.Map<List<GenderDto>>(genders);
                return new Response<List<GenderDto>>(dto);
            }
        }
    }
}

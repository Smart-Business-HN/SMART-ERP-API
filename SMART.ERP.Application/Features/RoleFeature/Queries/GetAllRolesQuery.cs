using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.RoleFeature.Queries
{
    public class GetAllRolesQuery : IRequest<Response<List<Role>>>
    {
        public class GetAllRolesQueryHanlder : IRequestHandler<GetAllRolesQuery, Response<List<Role>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Role> _repositoryAsync;

            public GetAllRolesQueryHanlder(IMapper mapper, IRepositoryAsync<Role> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<Response<List<Role>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
            {
                var roles = await _repositoryAsync.ListAsync();
                return new Response<List<Role>>(roles);
            }
        }
    }
}

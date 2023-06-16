using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.User;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.UserFeature.Queries
{
    public class GetAdvisorsByBranchQuery : IRequest<Response<List<UserDto>>>
    {
        public int BranchId { get; set; }
    }

    public class GetAdvisorsByBranchQueryHandler : IRequestHandler<GetAdvisorsByBranchQuery, Response<List<UserDto>>>
    {
        private readonly IRepositoryAsync<User> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAdvisorsByBranchQueryHandler(IRepositoryAsync<User> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<List<UserDto>>> Handle(GetAdvisorsByBranchQuery request, CancellationToken cancellationToken)
        {
            List<User> saleAdvisors;
            saleAdvisors = await _repositoryAsync.ListAsync(new FilterSalesAdvisorsByBranchSpecification(request.BranchId));
            var dto = _mapper.Map<List<UserDto>>(saleAdvisors);
            return new Response<List<UserDto>>(dto);
        }
    }
}

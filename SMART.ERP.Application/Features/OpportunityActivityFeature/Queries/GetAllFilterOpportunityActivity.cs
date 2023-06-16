using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.OpportunityActivitySpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityActivityFeature.Queries
{
    public class GetAllFilterOpportunityActivity : IRequest<Response<List<OpportunityActivityDto>>>
    {
        public Guid? UserId { get; set; }
        public int? BranchOfficeId { get; set; }
        public DateTime InitDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class GetAllFilterOpportunityActivityHandler : IRequestHandler<GetAllFilterOpportunityActivity, Response<List<OpportunityActivityDto>>>
    {
        private readonly IRepositoryAsync<OpportunityActivity> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<BranchOffices> _branchRepositoryAsync;
        private readonly IJwtService _jwtService;

        public GetAllFilterOpportunityActivityHandler(IRepositoryAsync<OpportunityActivity> repositoryAsync, IMapper mapper, IRepositoryAsync<User> userRepositoryAsync,
            IRepositoryAsync<BranchOffices> branchRepositoryAsync, IJwtService jwtService)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _userRepositoryAsync = userRepositoryAsync;
            _branchRepositoryAsync = branchRepositoryAsync;
            _jwtService = jwtService;
        }
        public async Task<Response<List<OpportunityActivityDto>>> Handle(GetAllFilterOpportunityActivity request, CancellationToken cancellationToken)
        {
            var guid = _jwtService.GetUidToken();
            var requestUser = await _userRepositoryAsync.FirstOrDefaultAsync(new UserIncludesSpecification(guid, null));
            if (requestUser == null)
            {
                throw new ApiException("Usuario invalido");
            }
            if (request.UserId != null)
            {
                var checkUser = await _userRepositoryAsync.GetByIdAsync((Guid)request.UserId);
                if (checkUser == null)
                {
                    throw new KeyNotFoundException($"No se encontro el usuario con id {request.UserId}");
                }
            }
            if (requestUser!.Role!.Name == "Sales Advisor")
            {
                request.UserId = requestUser.Id;
            }
            if (request.BranchOfficeId != null && request.BranchOfficeId > 0)
            {
                var checkBranch = await _branchRepositoryAsync.GetByIdAsync((int)request.BranchOfficeId);
                if (checkBranch == null)
                {
                    throw new KeyNotFoundException($"No se encontro la sucursal con id {request.BranchOfficeId}");
                }
            }
            var activities = await _repositoryAsync.ListAsync(new FilterOpportunityActivityByAdvisorBranchSpecification
                (request.UserId, request.BranchOfficeId, request.InitDate, request.EndDate));
            var dto = _mapper.Map<List<OpportunityActivityDto>>(activities);
            return new Response<List<OpportunityActivityDto>>(dto);
        }
    }
}

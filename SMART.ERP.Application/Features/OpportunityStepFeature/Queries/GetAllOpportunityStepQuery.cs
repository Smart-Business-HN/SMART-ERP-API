using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityStepFeature.Queries
{
    public class GetAllOpportunityStepQuery : IRequest<Response<List<OpportunityStepDto>>>
    {
        public int BranchOfficeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class GetAllOpportunityStepQueryHandler : IRequestHandler<GetAllOpportunityStepQuery, Response<List<OpportunityStepDto>>>
    {
        private readonly IRepositoryAsync<OpportunityStep> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Opportunity> _opportunityRepositoryAsync;
        private readonly IRepositoryAsync<BranchOffices> _branchRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IJwtService _jwtService;

        public GetAllOpportunityStepQueryHandler(IRepositoryAsync<OpportunityStep> repositoryAsync, IMapper mapper, IRepositoryAsync<Opportunity> opportunityRepositoryAsync,
            IRepositoryAsync<BranchOffices> branchRepositoryAsync, IRepositoryAsync<User> userRepositoryAsync, IJwtService jwtService)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _opportunityRepositoryAsync = opportunityRepositoryAsync;
            _branchRepositoryAsync = branchRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _jwtService = jwtService;
        }

        public async Task<Response<List<OpportunityStepDto>>> Handle(GetAllOpportunityStepQuery request, CancellationToken cancellationToken)
        {
            var opportunityList = new List<Opportunity>();
            var guid = _jwtService.GetUidToken();
            var checkUser = await _userRepositoryAsync.FirstOrDefaultAsync(new UserIncludesSpecification(guid, null));
            if (checkUser == null)
            {
                throw new ApiException("Usuario Invalido");
            }
            if (checkUser!.Role!.Selector == "SalesAdvisor")
            {
                var opportunityStepList = await _repositoryAsync.ListAsync();
                opportunityList = await _opportunityRepositoryAsync.ListAsync(new FilterOpportunityByUserInDateSpecification(request.StartDate, request.EndDate, checkUser!.Id));

                var dto = _mapper.Map<List<OpportunityStepDto>>(opportunityStepList);
                for (int i = 0; i < dto.Count(); i++)
                {
                    var opportunities = opportunityList.FindAll(x => x.OpportunityStepId == dto[i].Id);

                    for (int x = 0; x < opportunities.Count(); x++)
                    {
                        dto[i].Total += opportunities[x].Total;
                    }
                }
                return new Response<List<OpportunityStepDto>>(dto);
            }
            else
            {
                if (request.BranchOfficeId != 0)
                {
                    var checkBranch = await _branchRepositoryAsync.GetByIdAsync(request.BranchOfficeId);
                    if (checkBranch == null)
                    {
                        throw new KeyNotFoundException($"No se encontro la sucursal con id {request.BranchOfficeId}");
                    }
                    var opportunityStepList = await _repositoryAsync.ListAsync();
                    opportunityList = await _opportunityRepositoryAsync.ListAsync(new FilterOpportunitiesinDatesSpecification(request.StartDate, request.EndDate, request.BranchOfficeId));

                    var dto = _mapper.Map<List<OpportunityStepDto>>(opportunityStepList);
                    for (int i = 0; i < dto.Count(); i++)
                    {
                        var opportunities = opportunityList.FindAll(x => x.OpportunityStepId == dto[i].Id && x.User!.BranchOfficeId == request.BranchOfficeId);
                        if (request.StartDate != null && request.EndDate != null)
                        {
                            opportunities = opportunities.FindAll(x => x.CreationDate >= request.StartDate && x.CreationDate <= request.EndDate);
                        }
                        for (int x = 0; x < opportunities.Count(); x++)
                        {
                            dto[i].Total += opportunities[x].Total;
                        }
                    }
                    return new Response<List<OpportunityStepDto>>(dto);
                }
                else
                {
                    var opportunityStepList = await _repositoryAsync.ListAsync();
                    var dto = _mapper.Map<List<OpportunityStepDto>>(opportunityStepList);
                    return new Response<List<OpportunityStepDto>>(dto);
                }
            }
        }
    }
}

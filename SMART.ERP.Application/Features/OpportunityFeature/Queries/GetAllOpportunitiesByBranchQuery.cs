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

namespace SMART.ERP.Application.Features.OpportunityFeature.Queries
{
    public class GetAllOpportunitiesByBranchQuery : IRequest<Response<List<OpportunityDto>>>
    {
        public int BranchOfficeId { get; set; }
        public DateTime? StartDate { get; set; }
        public string? Parameter { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class GetAllOpportunitiesByBranchQueryHandler : IRequestHandler<GetAllOpportunitiesByBranchQuery, Response<List<OpportunityDto>>>
    {
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Customer> _clientRepositoryAsync;

        public GetAllOpportunitiesByBranchQueryHandler(IRepositoryAsync<Opportunity> repositoryAsync, IJwtService jwtService,
            IRepositoryAsync<User> userRepositoryAsync, IMapper mapper, IRepositoryAsync<Customer> clientRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
            _userRepositoryAsync = userRepositoryAsync;
            _mapper = mapper;
            _clientRepositoryAsync = clientRepositoryAsync;
        }

        public async Task<Response<List<OpportunityDto>>> Handle(GetAllOpportunitiesByBranchQuery request, CancellationToken cancellationToken)
        {
            var opportunities = new List<Opportunity>();
            var guid = _jwtService.GetUidToken();
            var checkUser = await _userRepositoryAsync.FirstOrDefaultAsync(new UserIncludesSpecification(guid, null));
            if (checkUser == null)
            {
                throw new ApiException("Usuario Invalido");
            }
            if (checkUser!.Role!.Selector == "SalesAdvisor")
            {
                opportunities = await _repositoryAsync.ListAsync(new FilterOpportunityByUserInDateSpecification(request.StartDate, request.EndDate, checkUser!.Id));
            }
            else
            {
                opportunities = await _repositoryAsync.ListAsync(new FilterOpportunitiesinDatesSpecification(request.StartDate, request.EndDate, request.BranchOfficeId));
            }
            var dto = _mapper.Map<List<OpportunityDto>>(opportunities);
            var clients = await _clientRepositoryAsync.ListAsync();
            for (int index = 0; index < dto.Count; index++)
            {
                var client = clients.Find(x => x.Id == dto[index].Customer!.MotorsId);
                if (client != null)
                {
                    dto[index].Customer!.FullName = client.FullName;
                    dto[index].Customer!.PhoneNumber = client.PhoneNumber;
                    dto[index].Customer!.Email = client.Email;
                }
            }
            if (checkUser.Role.Selector == "SalesAdvisor")
            {
                if (request.Parameter != null)
                {
                    dto = dto.FindAll(x => x.Code.ToLower().Contains(request.Parameter.ToLower()) || x.Customer.FullName.ToLower().Contains(request.Parameter.ToLower()));
                }
            }
            else
            {
                if (request.Parameter != null)
                {
                    dto = dto.FindAll(x => x.Code.ToLower().Contains(request.Parameter.ToLower()) || x.Customer.FullName.ToLower().Contains(request.Parameter.ToLower()) || x.User.FullName.ToLower().Contains(request.Parameter.ToLower()));
                }
            }

            return new Response<List<OpportunityDto>>(dto);

        }
    }
}

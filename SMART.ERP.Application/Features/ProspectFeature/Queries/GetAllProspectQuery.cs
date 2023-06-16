using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Prospect;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.ProspectSpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProspectFeature.Queries
{
    public class GetAllProspectQuery : IRequest<PagedResponse<List<ProspectDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }

    public class GetAllProspectQueryHandler : IRequestHandler<GetAllProspectQuery, PagedResponse<List<ProspectDto>>>
    {
        private readonly IRepositoryAsync<Prospect> _repositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public GetAllProspectQueryHandler(IRepositoryAsync<Prospect> repositoryAsync, IRepositoryAsync<User> userRepositoryAsync, IJwtService jwtService, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        public async Task<PagedResponse<List<ProspectDto>>> Handle(GetAllProspectQuery request, CancellationToken cancellationToken)
        {
            var uid = _jwtService.GetUidToken();
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = await _repositoryAsync.CountAsync();
            }
            var checkUser = await _userRepositoryAsync.FirstOrDefaultAsync(new UserIncludesSpecification(uid, null));
            if (checkUser == null)
            {
                throw new ApiException("Usuario invalido");
            }

            var prospects = await _repositoryAsync.ListAsync(new FilterAndPaginationProspectSpecification(request.Parameter, request.Order, request.Column));
            if (string.IsNullOrEmpty(request.Order) && string.IsNullOrEmpty(request.Column))
            {
                prospects = prospects.OrderByDescending(x => x.CreationDate).ToList();
            }
            if (checkUser!.Role!.Name == "Sales Advisor")
            {
                prospects = prospects.FindAll(x => x.UserId == uid);
            }
            var response = prospects.Skip(request.PageNumber * request.PageSize).Take(request.PageSize);
            var dto = _mapper.Map<List<ProspectDto>>(response);
            return new PagedResponse<List<ProspectDto>>(dto, request.PageNumber, request.PageSize, prospects.Count);

        }
    }
}

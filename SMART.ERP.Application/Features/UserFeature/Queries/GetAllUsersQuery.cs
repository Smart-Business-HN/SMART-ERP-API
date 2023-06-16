using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.User;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.UserFeature.Queries
{
    public class GetAllUsersQuery : IRequest<PagedResponse<List<UserDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PagedResponse<List<UserDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<User> _repositoryAsync;

            public GetAllUsersQueryHandler(IMapper mapper, IRepositoryAsync<User> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var users = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationUserSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<UserDto>>(users);
                return new PagedResponse<List<UserDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}

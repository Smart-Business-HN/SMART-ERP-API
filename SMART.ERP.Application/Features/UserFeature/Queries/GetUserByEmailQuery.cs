using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.User;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.UserFeature.Queries
{
    public class GetUserByEmailQuery : IRequest<Response<UserDto>>
    {
        public string Email { get; set; } = null!;
    }

    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, Response<UserDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<User> _repositoryAsync;

        public GetUserByEmailQueryHandler(IMapper mapper, IRepositoryAsync<User> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<UserDto>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            var user = await _repositoryAsync.FirstOrDefaultAsync(new UserIncludesSpecification(id: null, email: request.Email));
            if (user == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el correo {request.Email}");
            }
            var dto = _mapper.Map<UserDto>(user);
            return new Response<UserDto>(dto);
        }
    }
}

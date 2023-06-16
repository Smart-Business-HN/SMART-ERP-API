using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.User;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.UserFeature.Queries
{
    public class GetUserByIdQuery : IRequest<Response<UserDto>>
    {
        public Guid Id { get; set; }
    }

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Response<UserDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<User> _repositoryAsync;

        public GetUserByIdQueryHandler(IMapper mapper, IRepositoryAsync<User> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _repositoryAsync.FirstOrDefaultAsync(new UserIncludesSpecification(id: request.Id, email: null));
            if (user == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<UserDto>(user);
            return new Response<UserDto>(dto);
        }
    }
}

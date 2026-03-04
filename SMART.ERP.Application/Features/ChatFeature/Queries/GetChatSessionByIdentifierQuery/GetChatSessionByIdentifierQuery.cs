using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Chat;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ChatSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ChatFeature.Queries.GetChatSessionByIdentifierQuery
{
    public class GetChatSessionByIdentifierQuery : IRequest<Response<ChatSessionDto?>>
    {
        public string SessionIdentifier { get; set; } = null!;
    }

    public class GetChatSessionByIdentifierQueryHandler : IRequestHandler<GetChatSessionByIdentifierQuery, Response<ChatSessionDto?>>
    {
        private readonly IRepositoryAsync<ChatSession> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetChatSessionByIdentifierQueryHandler(IRepositoryAsync<ChatSession> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<ChatSessionDto?>> Handle(GetChatSessionByIdentifierQuery request, CancellationToken cancellationToken)
        {
            var session = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterChatSessionByIdentifierSpecification(request.SessionIdentifier));
            var dto = session != null ? _mapper.Map<ChatSessionDto>(session) : null;
            return new Response<ChatSessionDto?>(dto);
        }
    }
}

using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Chat;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ChatSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ChatFeature.Queries.GetActiveChatSessionsQuery
{
    public class GetActiveChatSessionsQuery : IRequest<Response<List<ChatSessionDto>>>
    {
    }

    public class GetActiveChatSessionsQueryHandler : IRequestHandler<GetActiveChatSessionsQuery, Response<List<ChatSessionDto>>>
    {
        private readonly IRepositoryAsync<ChatSession> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetActiveChatSessionsQueryHandler(IRepositoryAsync<ChatSession> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<List<ChatSessionDto>>> Handle(GetActiveChatSessionsQuery request, CancellationToken cancellationToken)
        {
            var sessions = await _repositoryAsync.ListAsync(new FilterActiveChatSessionsSpecification());
            var dto = _mapper.Map<List<ChatSessionDto>>(sessions);
            return new Response<List<ChatSessionDto>>(dto);
        }
    }
}

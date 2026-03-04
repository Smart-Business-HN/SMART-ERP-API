using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Chat;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ChatSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ChatFeature.Queries.GetChatMessagesBySessionQuery
{
    public class GetChatMessagesBySessionQuery : IRequest<Response<List<ChatMessageDto>>>
    {
        public int ChatSessionId { get; set; }
    }

    public class GetChatMessagesBySessionQueryHandler : IRequestHandler<GetChatMessagesBySessionQuery, Response<List<ChatMessageDto>>>
    {
        private readonly IRepositoryAsync<ChatMessage> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetChatMessagesBySessionQueryHandler(IRepositoryAsync<ChatMessage> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<List<ChatMessageDto>>> Handle(GetChatMessagesBySessionQuery request, CancellationToken cancellationToken)
        {
            var messages = await _repositoryAsync.ListAsync(
                new FilterChatMessagesBySessionIdSpecification(request.ChatSessionId));
            var dto = _mapper.Map<List<ChatMessageDto>>(messages);
            return new Response<List<ChatMessageDto>>(dto);
        }
    }
}

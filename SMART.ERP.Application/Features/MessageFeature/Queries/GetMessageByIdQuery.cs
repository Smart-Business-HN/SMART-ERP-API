using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MessageFeature.Queries
{
    public class GetMessageByIdQuery : IRequest<Response<Message>>
    {
        public int Id { get; set; }
    }

    public class GetMessageByIdQueryHandler : IRequestHandler<GetMessageByIdQuery, Response<Message>>
    {
        private readonly IRepositoryAsync<Message> _repositoryAsync;

        public GetMessageByIdQueryHandler(IRepositoryAsync<Message> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<Message>> Handle(GetMessageByIdQuery request, CancellationToken cancellationToken)
        {
            var message = await _repositoryAsync.GetByIdAsync(request.Id);
            if (message == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            return new Response<Message>(message);
        }
    }
}

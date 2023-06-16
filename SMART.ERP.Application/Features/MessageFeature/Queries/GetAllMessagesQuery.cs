using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.MessageSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System.Drawing.Printing;

namespace SMART.ERP.Application.Features.MessageFeature.Queries
{
    public class GetAllMessagesQuery : IRequest<PagedResponse<List<Message>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public class GetAllMessagesQueryHandler : IRequestHandler<GetAllMessagesQuery, PagedResponse<List<Message>>>
        {
            private readonly IRepositoryAsync<Message> _repositoryAsync;

            public GetAllMessagesQueryHandler(IRepositoryAsync<Message> repositoryAsync)
            {
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<Message>>> Handle(GetAllMessagesQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var messages = await _repositoryAsync.ListAsync(new FilterAndPaginationMessageSpecification(request.Parameter, request.Order, request.Column));
                var response = messages.Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
                return new PagedResponse<List<Message>>(response, request.PageNumber, request.PageSize, request.Parameter == null ? request.All ? request.PageSize : await _repositoryAsync.CountAsync() : messages.Count);
            }
        }
    }
}

using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CountryFeature.Queries
{
    public class GetCountryByIdQuery : IRequest<Response<Country>>
    {
        public int Id { get; set; }
    }

    public class GetCountryByIdQueryHandler : IRequestHandler<GetCountryByIdQuery, Response<Country>>
    {
        private readonly IRepositoryAsync<Country> _repositoryAsync;

        public GetCountryByIdQueryHandler(IRepositoryAsync<Country> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<Country>> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _repositoryAsync.GetByIdAsync(request.Id);
            if (result == null)
                throw new ApiException($"No se encontro un pais con el id: {request.Id}");
            return new Response<Country>(result);
        }
    }
}

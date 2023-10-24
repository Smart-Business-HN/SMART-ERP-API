using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.TypeOfPaymentMethod;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.TypeOfPaymentMethodSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.TypeOfPaymentMethodFeature.Queries
{
    public class GetAllTypesOfPaymentMethodQuery : IRequest<PagedResponse<List<TypeOfPaymentMethodDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllTypesOfPaymentMethodQueryHandler : IRequestHandler<GetAllTypesOfPaymentMethodQuery, PagedResponse<List<TypeOfPaymentMethodDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<TypeOfPaymentMethod> _repositoryAsync;
            public GetAllTypesOfPaymentMethodQueryHandler(IMapper mapper, IRepositoryAsync<TypeOfPaymentMethod> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<TypeOfPaymentMethodDto>>> Handle(GetAllTypesOfPaymentMethodQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }
                var cities = await _repositoryAsync.ListAsync(new FilterAndPaginationTypeOfPaymentMethodSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<TypeOfPaymentMethodDto>>(cities);
                return new PagedResponse<List<TypeOfPaymentMethodDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());

            }
        }
    }
}

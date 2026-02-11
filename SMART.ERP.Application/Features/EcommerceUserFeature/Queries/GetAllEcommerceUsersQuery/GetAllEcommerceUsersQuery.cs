using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.EcommerceUser;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.EcommerceUserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.EcommerceUserFeature.Queries.GetAllEcommerceUsersQuery
{
    public class GetAllEcommerceUsersQuery : IRequest<PagedResponse<List<EcommerceUserDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public bool? IsActive { get; set; }
        public int? CustomerTypeId { get; set; }
        public int? DepartmentId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public class GetAllEcommerceUsersQueryHandler : IRequestHandler<GetAllEcommerceUsersQuery, PagedResponse<List<EcommerceUserDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<EcommerceUser> _repositoryAsync;

            public GetAllEcommerceUsersQueryHandler(IMapper mapper, IRepositoryAsync<EcommerceUser> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<PagedResponse<List<EcommerceUserDto>>> Handle(GetAllEcommerceUsersQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync(cancellationToken);
                }

                var users = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationEcommerceUserSpecification(
                        request.Parameter, request.PageNumber, request.PageSize,
                        request.Order, request.Column, request.IsActive,
                        request.CustomerTypeId, request.DepartmentId,
                        request.DateFrom, request.DateTo), cancellationToken);

                var dto = _mapper.Map<List<EcommerceUserDto>>(users);
                return new PagedResponse<List<EcommerceUserDto>>(dto, request.PageNumber, request.PageSize,
                    request.All ? request.PageSize : await _repositoryAsync.CountAsync(cancellationToken));
            }
        }
    }
}

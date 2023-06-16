using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.AdvisorDepartment;

namespace SMART.ERP.Application.Features.AdvisorDepartmentFeature.Queries
{
    public class GetAllAdvisorDepartmentQuery : IRequest<Response<List<AdvisorDepartmentDto>>>
    {
    }

    public class GetAllAdvisorDepartmentQueryHandler : IRequestHandler<GetAllAdvisorDepartmentQuery, Response<List<AdvisorDepartmentDto>>>
    {
        private readonly IRepositoryAsync<AdvisorDepartment> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllAdvisorDepartmentQueryHandler(IRepositoryAsync<AdvisorDepartment> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<List<AdvisorDepartmentDto>>> Handle(GetAllAdvisorDepartmentQuery request, CancellationToken cancellationToken)
        {
            var advisorDepartments = await _repositoryAsync.ListAsync();
            var dto = _mapper.Map<List<AdvisorDepartmentDto>>(advisorDepartments);
            return new Response<List<AdvisorDepartmentDto>>(dto);
        }
    }
}

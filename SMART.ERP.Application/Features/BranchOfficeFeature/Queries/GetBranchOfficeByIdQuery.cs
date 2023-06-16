using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Company;

namespace SMART.ERP.Application.Features.BranchOfficeFeature.Queries
{
    public class GetBranchOfficeByIdQuery : IRequest<Response<BranchOfficeDto>>
    {
        public int Id { get; set; }
    }
    public class GetBranchOfficeByIdQueryHandler : IRequestHandler<GetBranchOfficeByIdQuery, Response<BranchOfficeDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<BranchOffices> _repositoryAsync;

        public GetBranchOfficeByIdQueryHandler(IMapper mapper, IRepositoryAsync<BranchOffices> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<BranchOfficeDto>> Handle(GetBranchOfficeByIdQuery request, CancellationToken cancellationToken)
        {
            var branchOffice = await _repositoryAsync.GetByIdAsync(request.Id);
            if (branchOffice == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<BranchOfficeDto>(branchOffice);
            return new Response<BranchOfficeDto>(dto);
        }
    }

}

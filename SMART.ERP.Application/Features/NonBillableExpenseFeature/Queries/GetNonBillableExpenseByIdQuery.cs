using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.NonBilllableExpense;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.NonBillableExpenseSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;


namespace SMART.ERP.Application.Features.NonBillableExpenseFeature.Queries
{
    public class GetNonBillableExpenseByIdQuery : IRequest<Response<NonBillableExpenseDto>>
    {
        public int Id { get; set; }
    }
    public class GetNonBillableExpenseByIdQueryHandler : IRequestHandler<GetNonBillableExpenseByIdQuery, Response<NonBillableExpenseDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<NonBillableExpense> _repositoryAsync;

        public GetNonBillableExpenseByIdQueryHandler(IMapper mapper, IRepositoryAsync<NonBillableExpense> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<NonBillableExpenseDto>> Handle(GetNonBillableExpenseByIdQuery request, CancellationToken cancellationToken)
        {
            var nonBillableExpense = await _repositoryAsync.FirstOrDefaultAsync(new FilterNonBillableExpenseByIdSpecification(request.Id));
            if (nonBillableExpense == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<NonBillableExpenseDto>(nonBillableExpense);
            return new Response<NonBillableExpenseDto>(dto);
        }
    }
}

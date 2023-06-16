using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.SubcategoryFeature.Queries
{
    public class GetSubcategoryByIdQuery : IRequest<Response<SubcategoryDto>>
    {
        public int Id { get; set; }
    }

    public class GetSubcategoryByIdQueryHandler : IRequestHandler<GetSubcategoryByIdQuery, Response<SubcategoryDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Subcategory> _repositoryAsync;

        public GetSubcategoryByIdQueryHandler(IMapper mapper, IRepositoryAsync<Subcategory> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<SubcategoryDto>> Handle(GetSubcategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var subcategory = await _repositoryAsync.GetByIdAsync(request.Id);
            if (subcategory == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<SubcategoryDto>(subcategory);
            return new Response<SubcategoryDto>(dto);
        }
    }
}

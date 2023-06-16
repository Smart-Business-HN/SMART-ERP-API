using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CategoryFeature.Queries
{
    public class GetCategoryByIdQuery : IRequest<Response<CategoryDto>>
    {
        public int Id { get; set; }
    }
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Response<CategoryDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Category> _repositoryAsync;

        public GetCategoryByIdQueryHandler(IMapper mapper, IRepositoryAsync<Category> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _repositoryAsync.GetByIdAsync(request.Id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<CategoryDto>(category);
            return new Response<CategoryDto>(dto);
        }
    }

}

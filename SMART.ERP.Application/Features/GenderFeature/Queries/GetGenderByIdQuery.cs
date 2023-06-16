using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.User;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.GenderFeature.Queries
{
    public class GetGenderByIdQuery : IRequest<Response<GenderDto>>
    {
        public int Id { get; set; }
    }

    public class GetGenderByIdQueryHandler : IRequestHandler<GetGenderByIdQuery, Response<GenderDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Gender> _repositoryAsync;

        public GetGenderByIdQueryHandler(IMapper mapper, IRepositoryAsync<Gender> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<GenderDto>> Handle(GetGenderByIdQuery request, CancellationToken cancellationToken)
        {
            var gender = await _repositoryAsync.GetByIdAsync(request.Id);
            if (gender == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<GenderDto>(gender);
            return new Response<GenderDto>(dto);
        }
    }
}

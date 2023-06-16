using AutoMapper;
using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpinionSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Company;

namespace SMART.ERP.Application.Features.OpinionFeature.Commands.UpdateOpinionCommand
{
    public class UpdateOpinionCommand : IRequest<Response<OpinionDto>>
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Image { get; set; }
        public string Observation { get; set; } = null!;
        public string Customer { get; set; } = null!;
        public string Charge { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class UpdateOpinionCommandHandler : IRequestHandler<UpdateOpinionCommand, Response<OpinionDto>>
    {
        private readonly IRepositoryAsync<Opinion> _repositoryAsync;
        private readonly IMapper _mapper;

        public UpdateOpinionCommandHandler(IMapper mapper, IRepositoryAsync<Opinion> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<OpinionDto>> Handle(UpdateOpinionCommand request, CancellationToken cancellationToken)
        {
            var opinion = await _repositoryAsync.GetByIdAsync(request.Id);
            if (opinion == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var filterByName = await _repositoryAsync.FirstOrDefaultAsync(new FilterOpinionSpecification(request.Customer, request.Id));
            if (filterByName != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Customer}");
            }
            opinion.Title = request.Title;
            opinion.Observation = request.Observation;
            opinion.Image = request.Image;
            opinion.Customer = request.Customer;
            opinion.Charge = request.Charge;
            opinion.IsActive = request.IsActive;
            await _repositoryAsync.UpdateAsync(opinion);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<OpinionDto>(opinion);
            return new Response<OpinionDto>(dto, message: $"Opinion de {opinion.Customer} actualizado correctamente");
        }
    }
}

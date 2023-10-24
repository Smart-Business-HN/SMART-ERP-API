using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.TypeOfPaymentMethod;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;


namespace SMART.ERP.Application.Features.TypeOfPaymentMethodFeature.Commands.UpdateTypeOfPaymentMethodCommand
{
    public class UpdateTypeOfPaymentMethodCommand : IRequest<Response<TypeOfPaymentMethodDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool ItIsNationBank { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateTypeOfPaymentMethodCommandHandler : IRequestHandler<UpdateTypeOfPaymentMethodCommand, Response<TypeOfPaymentMethodDto>>
    {
        private readonly IRepositoryAsync<TypeOfPaymentMethod> _repositoryAsync;
        private readonly IMapper _mapper;

        public UpdateTypeOfPaymentMethodCommandHandler(IRepositoryAsync<TypeOfPaymentMethod> repositoryAsync,
            IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<TypeOfPaymentMethodDto>> Handle(UpdateTypeOfPaymentMethodCommand request, CancellationToken cancellationToken)
        {
            var checkTypeOfPaymentMethod = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkTypeOfPaymentMethod == null)
            {
                throw new KeyNotFoundException($"No se encontro el Tipo de Pago con id {request.Id}");
            }

            checkTypeOfPaymentMethod.Name = request.Name;
            checkTypeOfPaymentMethod.IsActive = request.IsActive;

            await _repositoryAsync.UpdateAsync(checkTypeOfPaymentMethod);
            await _repositoryAsync.SaveChangesAsync();

            var dto = _mapper.Map<TypeOfPaymentMethodDto>(checkTypeOfPaymentMethod);
            return new Response<TypeOfPaymentMethodDto>(dto, $"{request.Name} actualizado correctamente");
        }
    }
}

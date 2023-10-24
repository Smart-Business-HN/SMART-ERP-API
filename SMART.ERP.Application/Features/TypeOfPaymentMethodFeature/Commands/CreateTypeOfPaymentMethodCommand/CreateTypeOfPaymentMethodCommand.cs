using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.TypeOfPaymentMethod;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.TypeOfPaymentMethodSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.TypeOfPaymentMethodFeature.Commands.CreateTypeOfPaymentMethodCommand
{
    public class CreateTypeOfPaymentMethodCommand: IRequest<Response<TypeOfPaymentMethodDto>>
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }

public class CreateTypeOfPaymentMethodCommandHandler : IRequestHandler<CreateTypeOfPaymentMethodCommand, Response<TypeOfPaymentMethodDto>>
{
    private readonly IRepositoryAsync<TypeOfPaymentMethod> _repositoryAsync;
    private readonly IMapper _mapper;

     public CreateTypeOfPaymentMethodCommandHandler(IRepositoryAsync<TypeOfPaymentMethod> repositoryAsync,
        IMapper mapper)
    {
        _repositoryAsync = repositoryAsync;
        _mapper = mapper;
    }

    public async Task<Response<TypeOfPaymentMethodDto>> Handle(CreateTypeOfPaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var checkbyName = await _repositoryAsync.FirstOrDefaultAsync(new FilterTypeOfPaymentMethodFromNameSpecification(request.Name));
        if (checkbyName != null)
        {
            throw new ApiException($"Ya existe un banco con el nombre {request.Name}");
        }

        var newRecord = _mapper.Map<TypeOfPaymentMethod>(request);
        var response = await _repositoryAsync.AddAsync(newRecord);
        await _repositoryAsync.SaveChangesAsync();
        var dto = _mapper.Map<TypeOfPaymentMethodDto>(response);
        return new Response<TypeOfPaymentMethodDto>(dto, $"{request.Name} agregado correctamente");
    }
}
}

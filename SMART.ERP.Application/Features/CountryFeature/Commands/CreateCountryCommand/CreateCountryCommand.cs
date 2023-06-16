using AutoMapper;
using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CountrySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CountryFeature.Commands.CreateCountryCommand
{
    public class CreateCountryCommand : IRequest<Response<Country>>
    {
        public string Name { get; set; } = null!;
        public string Abbreviation { get; set; } = null!;
        public string PhoneNumberCode { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommand, Response<Country>>
    {
        private readonly IRepositoryAsync<Country> _repositoryAsync;
        private readonly IMapper _mapper;

        public CreateCountryCommandHandler(IRepositoryAsync<Country> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<Country>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
        {
            var checkIfExistByName = await _repositoryAsync.FirstOrDefaultAsync(new FindCountrySpecification(request.Name, null, null, null));
            if (checkIfExistByName != null)
                throw new ApiException($"Ya existe un pais con el nombre: {request.Name}");

            var checkIfExistByAbbre = await _repositoryAsync.FirstOrDefaultAsync(new FindCountrySpecification(null, request.Abbreviation, null, null));
            if (checkIfExistByAbbre != null)
                throw new ApiException($"Ya existe un pais con la abreviacion: {request.Abbreviation}");

            var checkIfExistByPCode = await _repositoryAsync.FirstOrDefaultAsync(new FindCountrySpecification(null, null, request.PhoneNumberCode, null));
            if (checkIfExistByPCode != null)
                throw new ApiException($"Ya existe un pais con el codigo de numero: {request.PhoneNumberCode}");

            var newRecord = _mapper.Map<Country>(request);
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();

            return new Response<Country>(data, "Pais creado exitosamente");
        }
    }
}

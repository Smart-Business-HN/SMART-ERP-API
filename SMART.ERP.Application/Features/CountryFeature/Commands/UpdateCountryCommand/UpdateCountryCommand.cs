using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CountrySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CountryFeature.Commands.UpdateCountryCommand
{
    public class UpdateCountryCommand : IRequest<Response<Country>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Abbreviation { get; set; } = null!;
        public string PhoneNumberCode { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand, Response<Country>>
    {
        private readonly IRepositoryAsync<Country> _repositoryAsync;

        public UpdateCountryCommandHandler(IRepositoryAsync<Country> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<Country>> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _repositoryAsync.GetByIdAsync(request.Id);
            if (country == null)
                throw new ApiException($"No se encontro ningun registro con el id: {request.Name}");

            var checkIfExistByName = await _repositoryAsync.FirstOrDefaultAsync(new FindCountrySpecification(request.Name, null, null, request.Id));
            if (checkIfExistByName != null)
                throw new ApiException($"Ya existe un pais con el nombre: {request.Name}");

            var checkIfExistByAbbre = await _repositoryAsync.FirstOrDefaultAsync(new FindCountrySpecification(null, request.Abbreviation, null, request.Id));
            if (checkIfExistByAbbre != null)
                throw new ApiException($"Ya existe un pais con la abreviacion: {request.Abbreviation}");

            var checkIfExistByPCode = await _repositoryAsync.FirstOrDefaultAsync(new FindCountrySpecification(null, null, request.PhoneNumberCode, request.Id));
            if (checkIfExistByPCode != null)
                throw new ApiException($"Ya existe un pais con el codigo de numero: {request.PhoneNumberCode}");

            country.Name = request.Name;
            country.IsActive = request.IsActive;
            country.Abbreviation = request.Abbreviation;
            country.PhoneNumberCode = request.PhoneNumberCode;

            await _repositoryAsync.UpdateAsync(country, cancellationToken);
            await _repositoryAsync.SaveChangesAsync();

            return new Response<Country>(country, "Pais creado exitosamente");
        }
    }
}

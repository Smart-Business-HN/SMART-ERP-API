using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.QuotationSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.QuotationPreviewFeature.Commands.CreateQuotationItemObservationCommand
{
    public class CreateQuotationItemObservationCommand : IRequest<Response<QuotationItemObservationDto>>
    {
        public Guid AccessToken { get; set; }
        public int ProductOfferedId { get; set; }
        public string AuthorName { get; set; } = null!;
        public string Observation { get; set; } = null!;
    }

    public class CreateQuotationItemObservationCommandHandler : IRequestHandler<CreateQuotationItemObservationCommand, Response<QuotationItemObservationDto>>
    {
        private readonly IRepositoryAsync<Quotation> _quotationRepository;
        private readonly IRepositoryAsync<QuotationItemObservation> _observationRepository;
        private readonly IMapper _mapper;

        public CreateQuotationItemObservationCommandHandler(
            IRepositoryAsync<Quotation> quotationRepository,
            IRepositoryAsync<QuotationItemObservation> observationRepository,
            IMapper mapper)
        {
            _quotationRepository = quotationRepository;
            _observationRepository = observationRepository;
            _mapper = mapper;
        }

        public async Task<Response<QuotationItemObservationDto>> Handle(CreateQuotationItemObservationCommand request, CancellationToken cancellationToken)
        {
            var quotation = await _quotationRepository.FirstOrDefaultAsync(
                new FilterQuotationByAccessTokenSpecification(request.AccessToken));

            if (quotation == null)
            {
                throw new ApiException("Cotización no encontrada o el enlace es inválido.");
            }

            // Validate that the ProductOffered belongs to this quotation
            var productExists = quotation.ProductsOffered?.Any(p => p.Id == request.ProductOfferedId) ?? false;
            if (!productExists)
            {
                throw new ApiException("El producto especificado no pertenece a esta cotización.");
            }

            var newObservation = new QuotationItemObservation
            {
                ProductOfferedId = request.ProductOfferedId,
                QuotationId = quotation.Id,
                AuthorName = request.AuthorName,
                Observation = request.Observation,
                CreationDate = DateTime.Now
            };

            var data = await _observationRepository.AddAsync(newObservation);
            await _observationRepository.SaveChangesAsync();

            var dto = _mapper.Map<QuotationItemObservationDto>(data);
            return new Response<QuotationItemObservationDto>(dto, "Observación agregada exitosamente.");
        }
    }
}

using AutoMapper;
using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.BannerSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BannerFeature.Commands.CreateBannerCommand
{
    public class CreateBannerCommand : IRequest<Response<BannerDto>>
    {
        public string Url { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
    }

    public class CreateBannerCommandHandler : IRequestHandler<CreateBannerCommand, Response<BannerDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Banner> _repositoryAsync;

        public CreateBannerCommandHandler(IMapper mapper, IRepositoryAsync<Banner> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<BannerDto>> Handle(CreateBannerCommand request, CancellationToken cancellationToken)
        {
            var result = await _repositoryAsync.FirstOrDefaultAsync(new FilterBannerFromNameSpecification(request.FileName));
            if (result != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.FileName}");
            }
            var newRecord = _mapper.Map<Banner>(request);
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<BannerDto>(data);
            return new Response<BannerDto>(dto, message: $"{request.FileName} creado exitosamente");
        }
    }
}

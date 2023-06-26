using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Cai;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.BrandSpecification;
using SMART.ERP.Application.Specifications.CaiSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.CaiFeature.Commands.UpdateCaiCommand
{
    public class UpdateCaiCommand : IRequest<Response<CaiDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int? BranchOfficeId { get; set; }
        public string Identificator { get; set; } = null!;
        public int StartCorrelative { get; set; }
        public int EndCorrelative { get; set; }
        public int CurrentCorrelative { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool IsGeneralCai { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateCaiCommandHandler : IRequestHandler<UpdateCaiCommand, Response<CaiDto>>
    {
        private readonly IRepositoryAsync<Cai> _repositoryAsync;
        private readonly IRepositoryAsync<BranchOffices> _branchOfficeRepositoryAsync;
        private readonly IMapper _mapper;
        public UpdateCaiCommandHandler(IRepositoryAsync<Cai> repositoryAsync, IRepositoryAsync<BranchOffices> branchOfficeRepositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _branchOfficeRepositoryAsync = branchOfficeRepositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<CaiDto>> Handle(UpdateCaiCommand request, CancellationToken cancellationToken)
        {
            var cai = await _repositoryAsync.GetByIdAsync(request.Id);
            if(cai== null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            if(request.BranchOfficeId != null)
            {
                var branchOffice = await _branchOfficeRepositoryAsync.GetByIdAsync((int)request.BranchOfficeId);
                if (branchOffice == null)
                {
                    throw new KeyNotFoundException($"No se encontro ninguna sucursal con el id {request.Id}");
                }
                cai.BranchOfficeId = request.BranchOfficeId;
            }
            
            var checkIfExist = await _repositoryAsync.FirstOrDefaultAsync(
                    new FilterCaiByIdentificatorSpecification(request.Identificator));
            if(checkIfExist != null)
            {
                if(checkIfExist.Id != request.Id)
                {
                    throw new ApiException($"Ya existe un CAI con el identificador {request.Identificator}");
                }
            }
            cai.Name = request.Name;
            cai.AvailableInvoices = request.EndCorrelative - request.CurrentCorrelative;
            cai.IsGeneralCai = request.IsGeneralCai;
            cai.IsActive = request.IsActive;
            cai.Identificator = request.Identificator;
            cai.ValidFrom= request.ValidFrom;
            cai.ValidUntil = request.ValidUntil;
            cai.CurrentCorrelative = request.CurrentCorrelative;
            cai.StartCorrelative= request.StartCorrelative;
            cai.EndCorrelative= request.EndCorrelative;
            await _repositoryAsync.UpdateAsync(cai);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<CaiDto>(cai);
            return new Response<CaiDto>(dto, message: $"{cai.Name} actualizado correctamente");
        }
    }
}

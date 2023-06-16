using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Status;
using SMART.ERP.Application.DTOs.WishList;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CustomerSpecification;
using SMART.ERP.Application.Specifications.WishListSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.WishListFeature.Commands.UpdateWishListCommand
{
    public class UpdateWishListCommand : IRequest<Response<WishListDto>>
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public int CantItems { get; set; }
        public Guid CustomerId { get; set; }
        public int StatusId { get; set; }
        public StatusDto? Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime ModificationDate { get; set; }
    }
    public class UpdateWishListCommandHandler : IRequestHandler<UpdateWishListCommand, Response<WishListDto>>
    {
        private readonly IRepositoryAsync<WishList> _repositoryAsync;
        private readonly IRepositoryAsync<Status> _statusRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        public UpdateWishListCommandHandler(IRepositoryAsync<WishList> repositoryAsync,
            IRepositoryAsync<Status> statusRepositoryAsync,
            IRepositoryAsync<Customer> customerRepositoryAsync,
            IMapper mapper)
        {
            _mapper = mapper;
            _customerRepositoryAsync = customerRepositoryAsync;
            _repositoryAsync = repositoryAsync;
            _statusRepositoryAsync = statusRepositoryAsync;
        }
        public async Task<Response<WishListDto>> Handle(UpdateWishListCommand request, CancellationToken cancellationToken)
        {
            var existWishList = await _repositoryAsync.GetByIdAsync(request.Id);
            if (existWishList == null)
            {
                throw new KeyNotFoundException($"No se encontro una lista de deseo con el id {request.Id}");
            }
            var existCustomer = await _customerRepositoryAsync.FirstOrDefaultAsync(new FilterCustomerByMasterIdSpecification(request.CustomerId));
            if (existCustomer == null)
            {
                throw new KeyNotFoundException($"No se encontro un usuario con el id {request.CustomerId}");
            }
            var existStatus = await _statusRepositoryAsync.GetByIdAsync(request.StatusId);
            if (existStatus == null)
            {
                throw new KeyNotFoundException($"No se encontro el estado con el id {request.StatusId}");
            }

            existWishList.StatusId = existStatus!.Id;
            existWishList.CustomerId = request.CustomerId;
            existWishList.Code = request.Code;
            existWishList.IsActive = request.IsActive;
            existWishList.ModificationDate = DateTime.Now;
            await _repositoryAsync.UpdateAsync(existWishList);
            await _repositoryAsync.SaveChangesAsync();
            var data = await _repositoryAsync.FirstOrDefaultAsync(new WishListIncludesSpecification(request.Id, null, null));
            var dto = _mapper.Map<WishListDto>(data);
            return new Response<WishListDto>(dto, $"{dto.Code} actualizado correctamente");

        }
    }
}


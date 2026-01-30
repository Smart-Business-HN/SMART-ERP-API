using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.Project;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProjectFeature.Commands.UpdateProjectCommand
{
    public class UpdateProjectCommand : IRequest<Response<ProjectDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal ExecutionBudget { get; set; }
        public Guid CustomerId { get; set; }
        public int StatusId { get; set; }
    }

    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Response<ProjectDto>>
    {
        private readonly IRepositoryAsync<Project> _repositoryAsync;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IOutputCacheStore _outputCacheStored;

        public UpdateProjectCommandHandler(
            IRepositoryAsync<Project> repositoryAsync,
            IRepositoryAsync<Customer> customerRepositoryAsync,
            IMapper mapper,
            IJwtService jwtService,
            IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _customerRepositoryAsync = customerRepositoryAsync;
            _mapper = mapper;
            _jwtService = jwtService;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<ProjectDto>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _repositoryAsync.GetByIdAsync(request.Id);
            if (project == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun proyecto con el id {request.Id}");
            }
            var customerExist = await _customerRepositoryAsync.GetByIdAsync(request.CustomerId);
            if (customerExist == null)
            {
                throw new ApiException($"No existe un cliente con el ID: {request.CustomerId}");
            }

            project.Name = request.Name;
            project.Description = request.Description;
            project.StartDate = request.StartDate;
            project.EndDate = request.EndDate;
            project.ExecutionBudget = request.ExecutionBudget;
            project.CustomerId = request.CustomerId;
            project.StatusId = request.StatusId;
            project.ModificatedBy = _jwtService.GetSubjectToken();
            project.ModificationDate = DateTime.Now;

            await _repositoryAsync.UpdateAsync(project);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_project", cancellationToken);
            var dto = _mapper.Map<ProjectDto>(project);
            return new Response<ProjectDto>(dto, message: "Proyecto actualizado correctamente.");
        }
    }
}

using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.Project;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProjectFeature.Commands.CreateProjectCommand
{
    public class CreateProjectCommand : IRequest<Response<ProjectDto>>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal ExecutionBudget { get; set; }
        public Guid CustomerId { get; set; }
        public int PrefixId { get; set; }
    }

    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Response<ProjectDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Project> _repositoryAsync;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IRepositoryAsync<Prefix> _prefixRepositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IOutputCacheStore _outputCacheStored;

        public CreateProjectCommandHandler(
            IMapper mapper,
            IRepositoryAsync<Project> repositoryAsync,
            IRepositoryAsync<Customer> customerRepositoryAsync,
            IRepositoryAsync<Prefix> prefixRepositoryAsync,
            IJwtService jwtService,
            IOutputCacheStore outputCacheStored)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _customerRepositoryAsync = customerRepositoryAsync;
            _prefixRepositoryAsync = prefixRepositoryAsync;
            _jwtService = jwtService;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<ProjectDto>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var customerExist = await _customerRepositoryAsync.GetByIdAsync(request.CustomerId);
            if (customerExist == null)
            {
                throw new ApiException($"No existe un cliente con el ID: {request.CustomerId}");
            }
            var prefixExist = await _prefixRepositoryAsync.GetByIdAsync(request.PrefixId);
            if (prefixExist == null)
            {
                throw new ApiException($"No existe un prefijo con el ID: {request.PrefixId}");
            }
            var currentProjects = await _repositoryAsync.ListAsync();
            if (currentProjects.Count() == 0)
            {
                var firstProject = new Project { Id = 0 };
                currentProjects = [firstProject];
            }

            var newRecord = _mapper.Map<Project>(request);
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            newRecord.InsertedDate = DateTime.Now;
            newRecord.StatusId = 29;
            newRecord.ProjectCode = CreateProjectCode(prefixExist, currentProjects.Last());
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_project", cancellationToken);
            data.Prefix = prefixExist;
            data.Customer = customerExist;
            var dto = _mapper.Map<ProjectDto>(data);

            return new Response<ProjectDto>(dto, message: "Nuevo proyecto creado exitosamente");
        }

        public static string CreateProjectCode(Prefix prefix, Project lastProject)
        {
            var numberOfCharacters = prefix.Format.ToCharArray().Length;
            var numberOfCharactersInId = (lastProject.Id + 1).ToString().ToCharArray().Length;
            var code = "";
            if (numberOfCharacters + numberOfCharactersInId < 8)
            {
                int characterOffset = 8 - (numberOfCharacters + numberOfCharactersInId);
                code = prefix.Format + new string('0', characterOffset) + (lastProject.Id + 1).ToString();
            }
            else
            {
                code = prefix.Format + (lastProject.Id + 1).ToString();
            }
            return code;
        }
    }
}

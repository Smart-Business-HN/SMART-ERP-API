using MediatR;
using Microsoft.AspNetCore.Http;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.BlobStorageService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.EcommerceUserFeature.Commands.UpdateEcommerceUserProfileePhotoCommand;

public class UpdateEcommerceUserProfilePhotoCommand : IRequest<Response<string>>
{
    public Guid Id { get; set; }
    public IFormFile File { get; set; }
}
public class UpdateEcommerceUserProfilePhotoCommandHandler : IRequestHandler<UpdateEcommerceUserProfilePhotoCommand, Response<string>>
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly IRepositoryAsync<EcommerceUser> _repositoryAsync;
    private readonly IRepositoryAsync<LogEcommerceUser> _logRepositoryAsync;
    public UpdateEcommerceUserProfilePhotoCommandHandler(
        IBlobStorageService blobStorageService,
        IRepositoryAsync<EcommerceUser> repositoryAsync,
        IRepositoryAsync<LogEcommerceUser> logRepositoryAsync)
    {
        _blobStorageService = blobStorageService;
        _repositoryAsync = repositoryAsync;
        _logRepositoryAsync = logRepositoryAsync;
    }
    public async Task<Response<string>> Handle(UpdateEcommerceUserProfilePhotoCommand request, CancellationToken cancellationToken)
    {
        var user = await _repositoryAsync.GetByIdAsync(request.Id);
        if (user == null)
        {
            throw new KeyNotFoundException($"Usuario no encontrado con el id {request.Id}");
        }
        await _blobStorageService.UploadFileAsync(request.File);
        var getUrl = _blobStorageService.GetFile(request.File.FileName);
        user.Photo = getUrl;
        await _repositoryAsync.UpdateAsync(user);
        await _repositoryAsync.SaveChangesAsync();

        await _logRepositoryAsync.AddAsync(new LogEcommerceUser
        {
            EcommerceUserId = user.Id,
            ActionType = (int)EcommerceUserActionType.ProfilePhotoUpdate,
            Details = getUrl,
            CreationDate = DateTime.UtcNow
        }, cancellationToken);
        await _logRepositoryAsync.SaveChangesAsync(cancellationToken);

        return new Response<string>(getUrl, message: "Foto de perfil actualizada correctamente");
    }
}

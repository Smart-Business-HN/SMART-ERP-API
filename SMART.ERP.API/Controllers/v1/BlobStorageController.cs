using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.BlobStorageFeature.Commands.DeleteBlobStorageCommand;
using SMART.ERP.Application.Features.BlobStorageFeature.Commands.UploadBlobStorageCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class BlobStorageController : BaseApiController
    {
        [HttpPost("Upload")]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] UploadBlobStorageCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{fileName}")]
        [Authorize]
        public async Task<IActionResult> Delete(string fileName)
        {
            return Ok(await Mediator.Send(new DeleteBlobStorageCommand { FileName = fileName }));
        }
    }
}

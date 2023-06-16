using SMART.ERP.Application.DTOs.Meta.MetaInteractiveReply.MetaInteractiveButtonReply;
using SMART.ERP.Application.DTOs.Meta.MetaInteractiveReply.MetaInteractiveListReply;
using SMART.ERP.Application.DTOs.Meta.MetaTextReply;

namespace SMART.ERP.Application.Services.MetaPostService
{
    public interface IMetaPostService
    {
        public Task<bool> SendInteractiveList(MetaInteractive meta);
        public Task<bool> SendInteractiveButton(MetaInteractiveButtonRoot meta);
        public Task<bool> SendText(MetaResponse response);
    }
}

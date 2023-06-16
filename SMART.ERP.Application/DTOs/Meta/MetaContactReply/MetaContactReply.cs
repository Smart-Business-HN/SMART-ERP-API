using System.Numerics;

namespace SMART.ERP.Application.DTOs.Meta.MetaContactReply
{
    public class MetaContactReply
    {
        public MetaContactReplyName name { get; set; } = new MetaContactReplyName();
        public List<MetaContactReplyPhone> phones { get; set; } = new List<MetaContactReplyPhone>();
    }
}

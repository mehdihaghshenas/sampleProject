using MAction.BaseClasses;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAction.BaseServices.ViewModel
{
    public partial class EventHistoryOutputModel<T, Tkey> where Tkey : new()
    {
        [Key]
        public Tkey EventHistoryId { get; set; }
        public EventHistoryTypeEnum EventHistoryTypeId { get; set; }
        public Tkey PrimeryKeyId { get; set; }
        public T ChangesContent { get; set; }
#nullable enable
        public Tkey? UserId { get; set; }
#nullable disable
        public DateTimeOffset? Time { get; set; }
        public string UserIp { get; set; }
        public string UserTimeZone { get; set; }
        public int? RequestId { get; set; }
        //HttpContext.TraceIdentifier

    }
    public enum EventHistoryTypeEnum
    {
        Added = 1,
        Modified = 2,
        Deleted = 3
    }
}

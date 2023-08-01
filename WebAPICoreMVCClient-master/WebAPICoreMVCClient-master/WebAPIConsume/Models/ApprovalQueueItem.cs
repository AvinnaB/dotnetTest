using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace WebAPIConsume.Models
{
    public class ApprovalQueueItem
    {
        public int RequestId { get; set; }
        public int ProductId { get; set; }
        public string RequestType { get; set; }
        public string RequestReason { get; set; }
        public DateTime RequestDate { get; set; }
    }
}

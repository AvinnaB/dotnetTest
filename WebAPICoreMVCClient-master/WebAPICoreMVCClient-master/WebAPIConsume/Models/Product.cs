using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebAPIConsume.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public DateTime PostedDate { get; set; }
        public int IsActive { get; set; }
        public string RequestType { get; set; }
        public DateTime RequestDate { get; set; }
        public decimal RequestPrice { get; set; }

    }

}

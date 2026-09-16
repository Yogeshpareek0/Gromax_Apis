using System;
using System.Collections.Generic;

#nullable disable

namespace GromaxMobileApis.Models
{
    public partial class SalesTargetCountMaster
    {
        public Guid? Id { get; set; }
        public string DealerCode { get; set; }
        public string UserName { get; set; }
        public string Target { get; set; }
        public string Achieve { get; set; }
        public string Remaining { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace GromaxMobileApis.Models
{
    public partial class LeadsFollowUpCountMaster
    {
        public Guid? Id { get; set; }
        public string DealerCode { get; set; }
        public string TotalLeads { get; set; }
        public string OverDueTarget { get; set; }
        public string TodayLeads { get; set; }
        public string HotLeads { get; set; }
        public string WarmLeads { get; set; }
        public string ColdLeads { get; set; }
        public DateTime? CreateDate { get; set; }
    }

    public class MultiLeadsFollowUp
    {
        public IEnumerable<dynamic> LeadsTable { get; set; }
        public IEnumerable<dynamic> TargetTable { get; set; }

    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace GromaxMobileApis.Models
{
    public partial class HistoryEnquiry
    {
        public Guid Id { get; set; }
        public string EnquiryId { get; set; }
        public string MobileNo { get; set; }
        public string UserName { get; set; }
        public string CallingStartTime { get; set; }
        public string CallingEndTime { get; set; }
        public DateTime CreateDate { get; set; }
        public string CallStatus { get; set; }
        public string Remark { get; set; }
        public string CallLeads { get; set; }
        public string NextFollowUpDate { get; set; }
        public string ExpectedDeliveryDate { get; set; }
        public string FinanceStatus { get; set; }
        public string ReturnRequestStatus { get; set; }
        public string SalesEnquiryMasterId { get; set; }
        //closureReason,subClosureReason,followenquiryStatus
        public string closureReason { get; set; }
        public string subClosureReason { get; set; }
        public string followenquiryStatus { get; set; }
    }
}

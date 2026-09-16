using Microsoft.AspNetCore.Http;
using System.IO;

namespace GromaxMobileApis.Models
{
    public class SalesmanMaster
    {
        public string SalesmanName { get; set; }
        public string MobileNo { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string Status { get; set; }

    }

    public class SalesmanGetdto
    {
        public string ShMail { get; set; }
        public string AmMail { get; set; }
        public string TmMail { get; set; }
        public string DealerMail { get; set; }
        public string Status { get; set; }
        public string Download { get; set; }

    }

    public class NotificationRequest
    {
        public string DeviceToken { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }

    public class SalesmanStatusdto
    {
        public string Id { get; set; }
        public string ActiveStatus { get; set; }
    }

    public class SalesmanMasterv1
    {
        public string SalesmanName { get; set; }
        public string MobileNo { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string Status { get; set; }
        public string kycstatus { get; set; }
        public IFormFile file1 { get; set; }
        public IFormFile file2 { get; set; }
        public IFormFile file3 { get; set; }
        public string insertORupdt { get; set; }
        public string Id { get; set; }
        public string dateofjoin { get; set; }
        public string dateofseperation { get; set; }

    }

    public class ApprovalSalesmanKyc 
    {
    public int BatchId { get; set; }
    public string Status { get; set; }
    public string Remark { get; set; }
    public string Id { get; set; }
    public string Dealercode { get; set; }
    }
}

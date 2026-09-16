using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

#nullable disable

namespace GromaxMobileApis.Models
{
    public partial class ReturnRequestMaster
    {
        public Guid? Id { get; set; }
        public string SalesEnquiryMasterId { get; set; }
        public string ReturnDate { get; set; }
        public string Reason { get; set; }
        public string Remarks { get; set; }
        public string TerritoryManager { get; set; }
        public string StateHead { get; set; }
        public string NationalSalesHead { get; set; }
        public DateTime? CreateDate { get; set; }
    }
    public partial class GetReturnRequestMasterv1
    {
        public string DealerCode { get; set; }
        public string SearchType { get; set; }
        public string SearchValue { get; set; }
        public string ReturnType { get; set; }
    }
    public partial class GenerateReturnRequest
    {
        public string SalesEnquiryMasterId { get; set; }
        public string ReturnDate { get; set; }
        public string Reason { get; set; }
        public string Remarks { get; set; }

        public string TerritoryManager { get; set; }
        public string StateHead { get; set; }
        public string NationalSalesHead { get; set; }
        public string CreateDate { get; set; }

        public string TmStatus { get; set; }
        public string TmStatusDate { get; set; }
        public string TmRemarks { get; set; }
        public string TmMobile { get; set; }

        public string AreaManager { get; set; }
        public string AmStatus { get; set; }
        public string AmStatusDate { get; set; }
        public string AmRemarks { get; set; }
        public string AmMobile { get; set; }

        public string StateHeadStatus { get; set; }
        public string StateHeadStatusDate { get; set; }
        public string StateHeadRemarks { get; set; }
        public string StateHeadMobile { get; set; }

        public string NationalsalesHeadStatus { get; set; }
        public string NationalsalesHeadStatusDate { get; set; }
        public string NationalsalesHeadRemarks { get; set; }
        public string NationalsalesHeadMobile { get; set; }

        public string ReturnStatus { get; set; }
        public string ChasisNumber { get; set; }
        public string CustomerName { get; set; }
        public string Mobile { get; set; }
        public string DealerCode { get; set; }
        public string Model { get; set; }
        public string SaleDate { get; set; }
        public string SoldBy { get; set; }
    }
    public class GenerateReturnRequestv1
    {
        public string CustomerName { get; set; }
        public string ChasisNumber { get; set; }
        public string Mobile { get; set; }
        public string SaleDate { get; set; }
        public string SoldBy { get; set; }
        public string Reason { get; set; }
        public string Remarks { get; set; }
        public string SalesEnquiryMasterId { get; set; }
        public string DealerCode { get; set; }
        public string Modeln { get; set; }
        public List<IFormFile> FileNames { get; set; }
        public List<string> FileNamesurl { get; set; }
    }

    public class ReturnRequestApproval
    {
        public string Position { get; set; }

        public string Status { get; set; }
        public string Remarks
        {
            get;set;
        }
        public string ReturnRequestId
        {
            get; set;
        }


    }

    public class SuperHotEnquiry
    {
        public int RowStart { get; set; }
        public int PageSize { get; set; }
        public string Download { get; set; } = "No";
    }


}

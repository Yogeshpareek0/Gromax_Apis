using System;

namespace GromaxMobileApis.Models
{
    public class StockMasterdto
    {
        public string TmName { get; set; }
        public string TmStatus { get; set; }
        public string TmStatusDate { get; set; }
        public string TmRemark { get; set; }

        public string SHName { get; set; }
        public string SHStatus { get; set; }
        public string SHStatusDate { get; set; }
        public string SHRemark { get; set; }

        public string DlrToName { get; set; }
        public string DlrToStatus { get; set; }
        public string DlrToStatusDate { get; set; }
        public string DlrToRemark { get; set; }

        public string ApprovalStatus { get; set; }
        public string ApprovalStatusDate { get; set; }

        public string DealerCodeFrom { get; set; }
        public string DealerCodeTo { get; set; }

        public string ChassisNo { get; set; }
        public string StockId { get; set; }
    }

    public class StockTrfAprovalDto
    {
        public string Position { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string Remarks { get; set; }
        public string DlrToDlrStkTrfMasterId { get; set; }


    }
    public class GetStockForStkTrfdto
    {
        public string SearchBy { get; set; }
    }

    public class Pagignation
    {
        public int Skip { get; set; }
        public string GlobalFilter { get; set; }

    }

    public class ReturnBillingModel
    {
        public string Id { get; set; }

        public string DealerCode { get; set; }

        public string ChassisNo { get; set; }

        public string ModelCode { get; set; }

        public string BillDate { get; set; }

        public string ReturnDate { get; set; }
    }
}

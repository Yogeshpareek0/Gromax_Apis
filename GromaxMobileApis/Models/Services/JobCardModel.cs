using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace GromaxMobileApis.Models.Services
{
    public class SparesPartMaster
    {
        public int Id { get; set; }

        public string Material { get; set; } = string.Empty;

        public string Desc { get; set; } = string.Empty;

        public string HsnCode { get; set; } = string.Empty;

        public decimal GstRate { get; set; }

        public decimal Mrp { get; set; }

        public decimal Ndp { get; set; }

        public decimal Zpmr { get; set; }

        public decimal Zs00 { get; set; }
    }


    public class JobCardMaster
    {
        public string Id { get; set; }
        public string JobCardNo { get; set; }
        public Guid SalesMasterId { get; set; }
        public string ChassiNumber { get; set; }
        public DateTime JobCardDate { get; set; }
        public string JobCardType { get; set; }
        public string Fuel { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string TractorSlNo { get; set; }
        public string RegnNo { get; set; }
        public DateTime DateOfSale { get; set; }
        public string WorkDoneBy { get; set; }
        public string MobileNo { get; set; }
        public string AlternateMobileNo { get; set; }

        public decimal? FrontTyrePressureLeft { get; set; }
        public decimal? FrontTyrePressureRight { get; set; }
        public decimal? RearTyrePressureLeft { get; set; }
        public decimal? RearTyrePressureRight { get; set; }

        public decimal? Hours { get; set; }
        public decimal? TimeEstimate { get; set; }
        public decimal? TimeActual { get; set; }
        public decimal? CostEstimate { get; set; }
        public decimal? CostActual { get; set; }

        public decimal SpareTotal { get; set; }
        public decimal LocalTotal { get; set; }
        public decimal SubletTotal { get; set; }
        public string SubletDescription { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal? TotalAmountPaid { get; set; }
        public decimal? LabourTotal { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string PlatformType { get; set; }

        // Child Collections
        public List<JobCardComplaint> Complaints { get; set; } = new();
        public List<JobCardMissingPart> MissingParts { get; set; } = new();
        public List<JobCardSparePart> SpareParts { get; set; } = new();
        public List<JobCardLocalPart> LocalParts { get; set; } = new();
    }

    public class JobCardComplaint
    {
        public string Id { get; set; }
        public Guid JobCardID { get; set; }
        public string Complaint { get; set; }
        public string ActionTaken { get; set; }
        public string Remark { get; set; }
    }

    public class JobCardMissingPart
    {
        public string Id { get; set; }
        public Guid JobCardID { get; set; }
        public string PartDescription { get; set; }
    }

    public class JobCardSparePart
    {
        public string Id { get; set; }
        public Guid JobCardID { get; set; }

        public string PartNumber { get; set; }
        public string PartName { get; set; }

        public decimal? Qty { get; set; }
        public decimal? Labour { get; set; }
        public string Remark { get; set; }

        public decimal? GstAmount { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? BasePrice { get; set; }
        public decimal? GstRate { get; set; }
    }

    public class JobCardLocalPart
    {
        public string Id { get; set; }
        public Guid JobCardID { get; set; }

        public string PartName { get; set; }
        public string PartNumber { get; set; }

        public decimal? Qty { get; set; }
        public decimal? Price { get; set; }
        public decimal? GSTRate { get; set; }
        public decimal? Labour { get; set; }
        public string Remark { get; set; }
    }


    public class JobCardDataTables
    {
        public DataTable Complaints { get; set; }
        public DataTable MissingParts { get; set; }
        public DataTable SpareParts { get; set; }
        public DataTable LocalParts { get; set; }
    }


    public class AddJobCardRequest
    {
        public Guid SalesMasterId { get; set; }
        public string ChassiNumber { get; set; }
        public DateTime JobCardDate { get; set; }
        public string JobCardType { get; set; }
        public string Fuel { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string TractorSlNo { get; set; }
        public string RegnNo { get; set; }
        public DateTime DateOfSale { get; set; }
        public string WorkDoneBy { get; set; }
        public string MobileNo { get; set; }
        public string AlternateMobileNo { get; set; }

        public decimal? FrontTyrePressureLeft { get; set; }
        public decimal? FrontTyrePressureRight { get; set; }
        public decimal? RearTyrePressureLeft { get; set; }
        public decimal? RearTyrePressureRight { get; set; }

        public decimal? Hours { get; set; }
        public decimal? TimeEstimate { get; set; }
        public decimal? TimeActual { get; set; }
        public decimal? CostEstimate { get; set; }
        public decimal? CostActual { get; set; }

        public decimal SpareTotal { get; set; }
        public decimal LocalTotal { get; set; }
        public decimal SubletTotal { get; set; }
        public string SubletDescription { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal? TotalAmountPaid { get; set; }
        public decimal? LabourTotal { get; set; }

        public JobCardDataTables Tables { get; set; }

    }

    public class JobCardMasterReportRequest
    {
        public string DealerMail { get; set; } = string.Empty;
        public string StateName { get; set; } = "All";

        // This Month, This Quarter, Last Quarter, This FY
        public string Duration { get; set; } = "This Month";

        public DateTime? StDate { get; set; }
        public DateTime? EnDate { get; set; }

        public string JobType { get; set; } = "All";
    }

    public class ResponseJobCardMaster
    {
        public List<JobCardMaster> resJobCardMaster { get; set; }
        public List<JobCardComplaint> resJobCardComplaint { get; set; }
        public List<JobCardMissingPart> resJobCardMissingPart { get; set; }
        public List<JobCardSparePart> resJobCardSparePart { get; set; }
        public List<JobCardLocalPart> resJobCardLocalPart { get; set; }

    }


    public class UpdateJobCardRequest
    {
        [Required(ErrorMessage = "JobCardMasterId is required")]
        public Guid? JobCardMasterId { get; set; }
        public Guid SalesMasterId { get; set; }
        public string ChassiNumber { get; set; }
        public DateTime JobCardDate { get; set; }
        public string JobCardType { get; set; }
        public string Fuel { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string TractorSlNo { get; set; }
        public string RegnNo { get; set; }
        public DateTime DateOfSale { get; set; }
        public string WorkDoneBy { get; set; }
        public string MobileNo { get; set; }
        public string AlternateMobileNo { get; set; }

        public decimal? FrontTyrePressureLeft { get; set; }
        public decimal? FrontTyrePressureRight { get; set; }
        public decimal? RearTyrePressureLeft { get; set; }
        public decimal? RearTyrePressureRight { get; set; }

        public decimal? Hours { get; set; }
        public decimal? TimeEstimate { get; set; }
        public decimal? TimeActual { get; set; }
        public decimal? CostEstimate { get; set; }
        public decimal? CostActual { get; set; }

        public decimal SpareTotal { get; set; }
        public decimal LocalTotal { get; set; }
        public decimal SubletTotal { get; set; }
        public string SubletDescription { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal? TotalAmountPaid { get; set; }
        public decimal? LabourTotal { get; set; }

        public JobCardDataTables Tables { get; set; }

    }



}

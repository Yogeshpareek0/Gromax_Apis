using GromaxMobileApis.Models.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GromaxMobileApis.Models.Services
{
    /// <summary>
    /// PDF ke liye final invoice model.
    /// Header + totals fields = ServiceInvoiceMaster table se (DB-bound, Dapper se aayega).
    /// Items list = ServiceInvoiceItemDetails (+ ItemMaster join) se (DB-bound).
    /// Company/Consignee fields abhi STATIC hain — StaticInvoiceDefaults class se fill honge.
    /// Jab ye DB-driven karne ho, sirf ApplyStaticDefaults() ki jagah DB-fetch call laga dena,
    /// baaki sab (PDF generator, mapper) waisa hi rahega.
    /// </summary>
    public class ServiceInvoiceModel
    {
        // ================= DB-bound: ServiceInvoiceMaster =================
        public Guid Id { get; set; }
        public Guid ServiceMasterId { get; set; }
        public string DealerCode { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }

        public decimal TotalGSTPercentage { get; set; }
        public decimal TotalTaxableValue { get; set; }
        public decimal TotalCGSTAmt { get; set; }
        public decimal TotalSGSTAmt { get; set; }
        public decimal TotalIGSTAmt { get; set; }
        public decimal TotalTaxAmt { get; set; }
        public decimal GrandTotal { get; set; }

        public string PlatformType { get; set; }

        // ================= DB-bound: ServiceInvoiceItemDetails (+ ItemMaster) =================
        public List<ServiceInvoiceItemModel> Items { get; set; } = new List<ServiceInvoiceItemModel>();

        // ================= STATIC (abhi fixed hai, aage dynamic ho sakta hai) =================
        // Company / header info
        public string CompanyName { get; set; }
        public string AuthorisedTsd { get; set; }
        public string CompanyAddress { get; set; }

        // Consignee (Bill To / Ship To — dono same maan rahe hain, static)
        public string ConsigneeName { get; set; }
        public string ConsigneeAddress { get; set; }
        public string ConsigneePinCode { get; set; }
        public string ConsigneeState { get; set; }
        public string ConsigneeStateCode { get; set; }
        public string ConsigneeGstn { get; set; }
        public string ConsigneePhone { get; set; }

        public string TaxPayableOnReverseCharge { get; set; }
        public string Note { get; set; }
        public int StateCode { get; set; }
        public string AoName { get; set; }
        public string DealerLocation { get; set; }
        public string GstHeader { get; set; }
        public string Claims { get; set; }

        /// <summary>
        /// Abhi ke liye static company/consignee details is model me bhar deta hai.
        /// Baad me dynamic karna ho to is method ki jagah DB/repository call laga dena —
        /// baaki poora flow (mapper, PDF generator) bina change kiye chalega.
        /// </summary>
        //public void ApplyStaticDefaults()
        //{
        //    CompanyName = StaticInvoiceDefaults.CompanyName;
        //    AuthorisedTsd = StaticInvoiceDefaults.AuthorisedTsd;
        //    CompanyAddress = StaticInvoiceDefaults.CompanyAddress;

        //    ConsigneeName = StaticInvoiceDefaults.ConsigneeName;
        //    ConsigneeAddress = StaticInvoiceDefaults.ConsigneeAddress;
        //    ConsigneePinCode = StaticInvoiceDefaults.ConsigneePinCode;
        //    ConsigneeState = StaticInvoiceDefaults.ConsigneeState;
        //    ConsigneeStateCode = StaticInvoiceDefaults.ConsigneeStateCode;
        //    ConsigneeGstn = StaticInvoiceDefaults.ConsigneeGstn;
        //    ConsigneePhone = StaticInvoiceDefaults.ConsigneePhone;

        //    TaxPayableOnReverseCharge = StaticInvoiceDefaults.TaxPayableOnReverseCharge;
        //    Note = StaticInvoiceDefaults.Note;
        //}
    }

    /// <summary>
    /// Ek item-line. ServiceInvoiceItemDetails table ke fields + ItemMaster se join
    /// karke ItemName/HSNCode.
    /// </summary>
    public class ServiceInvoiceItemModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string HSNCode { get; set; }

        public decimal Qty { get; set; }
        public decimal Rate { get; set; }
        public decimal TaxableValue { get; set; }

        public decimal CGSTPercentage { get; set; }
        public decimal CGSTAmt { get; set; }

        public decimal SGSTPercentage { get; set; }
        public decimal SGSTAmt { get; set; }

        public decimal IGSTPercentage { get; set; }
        public decimal IGSTAmt { get; set; }

        public decimal Amount { get; set; }



    }

    public class ChassisItemModel
    {
        public string ChassisNumber { get; set; }
        public DateTime? DateOfSales { get; set; }
        public string Model { get; set; }
        public string EngineNumber { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }


    public class ChassisServiceHoursModel
    {
        public string ChassiNumber { get; set; }

        public int? FirstServiceHours { get; set; }
        public string FirstServiceDate { get; set; }

        public int? SecondServiceHours { get; set; }
        public string SecondServiceDate { get; set; }

        public int? ThirdServiceHours { get; set; }
        public string ThirdServiceDate { get; set; }

        public int? FourthServiceHours { get; set; }
        public string FourthServiceDate { get; set; }

        public int? FifthServiceHours { get; set; }
        public string FifthServiceDate { get; set; }

        public int? SixthServiceHours { get; set; }
        public string SixthServiceDate { get; set; }
    }

    public class ChassisServiceDateModel
    {
        public string ChassiNumber { get; set; }

        public string FirstService { get; set; }
        public string SecondService { get; set; }
        public string ThirdService { get; set; }
        public string FourthService { get; set; }
        public string FifthService { get; set; }
        public string SixthService { get; set; }
    }

    public class ServiceInvoiceResponse
    {
        public ServiceInvoiceModel Master { get; set; }

        public List<ServiceInvoiceItemModel> Items { get; set; }
            = new List<ServiceInvoiceItemModel>();
        public List<ChassisServiceHoursModel> Hours { get; set; }
           = new List<ChassisServiceHoursModel>();
    }

    public class ServiceInvoiceListResp
    {
        public DateTime? stDate { get; set; }
        public DateTime? enDate { get; set; }
        [Required(ErrorMessage = "Duration is required.")]
        public string duration { get; set; }

    }


    public class UpdateReimbursementStatusRequest
    {
        public Guid InvoiceId { get; set; }
    }


    public class ReimbursementScoreMasterModel
    {
        public Guid Id { get; set; }
        public string GroupItem { get; set; }
        public string GroupName { get; set; }
        public string Units { get; set; }
        public string CatA { get; set; }
        public string CatB { get; set; }
        public int? MaxScore { get; set; }
        public bool? PhotoRequired { get; set; }
        public int? MinPhoto { get; set; }
        public int? MaxPhoto { get; set; }
        public string AllowedScores { get; set; }
    }


}


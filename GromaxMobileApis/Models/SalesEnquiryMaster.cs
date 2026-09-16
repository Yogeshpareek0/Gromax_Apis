using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

#nullable disable

namespace GromaxMobileApis.Models
{
    public partial class SalesEnquiryMaster
    {

        public Guid Id { get; set; }

        public string DealerCode { get; set; }
        public string DealershipName { get; set; }
        public string Mobile { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string AOName { get; set; }
        public string AMName { get; set; }
        public string TMName { get; set; }
        public string EnquiryNumber { get; set; }
        public string EnquiryName { get; set; }
        public string EnquiryFor { get; set; }
        public string ProspectDistrict { get; set; }
        public string ProspectTehsil { get; set; }
        public string ProspectVillage { get; set; }
        public string ProspectAddress { get; set; }
        public string SalesmanName { get; set; }
        public string SalesmanNumber { get; set; }
        public string ProspectType { get; set; }
        public string EnquiryDate { get; set; }
        public string EnquiryGeneratedBy { get; set; }
        public string EnquirySource { get; set; }
        public string EnquirySubSource { get; set; }
        public string ReferalCustomerName { get; set; }
        public string ReferalCustomerNumber { get; set; }
        public string ReferalCustomerId { get; set; }
        public string ExpectedPurchaseDate { get; set; }
        public string EnquiryStatus { get; set; }
        public string EnquiryType { get; set; }
        public string InterestedModel { get; set; }
        public string ProductUse { get; set; }
        public string LandHolding { get; set; }
        public string ProposedModel { get; set; }
        public string PurchaseType { get; set; }
        public string ExchangeMake { get; set; }
        public string ExchangeModel { get; set; }
        public string ExchangeMfgYear { get; set; }
        public string ExpectedExchangeValue { get; set; }
        public string OfferedExchangeValue { get; set; }
        public string FinalExchangeValue { get; set; }
        public string FinalQuotation { get; set; }
        public string PaymentType { get; set; }
        public string DPAmount { get; set; }
        public string LoanAmount { get; set; }
        public string FinancerName { get; set; }
        public string FinanceStatus { get; set; }
        public string NextFollowUpDate { get; set; }
        public string FollowUpStatus { get; set; }
        public string Username { get; set; }
        public string VarientOrBOMCode { get; set; }
        public string CustomerType { get; set; }
        public string ExpectedDeliveryDate { get; set; }
        public string ProspectPinCode { get; set; }
        public string ActionPlanned { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CalledStatus { get; set; }
        public string LastCallTime { get; set; }
        public string ProspectName { get; set; }
        public string ProspectMobile { get; set; }
        public string FatherName { get; set; }
        public string HPCategory { get; set; }
        public string DriveType { get; set; }
        public string SubSubSource { get; set; }
        public string customAction { get; set; }


        public string StateCode { get; set; }
        public string DistrictCode { get; set; }
        public string TehsilCode { get; set; }
        public string VillageCode { get; set; }
        public string Surname { get; set; }
        public string Remark { get; set; }
        public string RowStart { get; set; }
        public string PageSize { get; set; }
    }

    public class SalesEnquiryMasterdto
    {
        public string id { get; set; }
        public string ProspectMobile { get; set; }
        public string SalesEnquiryId { get; set; }
    }


    public class SalesEnquiryMasterFiltered
    {
        [Required(AllowEmptyStrings = true, ErrorMessage = "AmMail is required")]
        public string AmMail { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "ShMail is required")]
        public string ShMail { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "TmMail is required")]
        public string TmMail { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "DealerMail is required")]
        public string DealerMail { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "FollowenquiryStatus is required")]
        public string FollowenquiryStatus { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "Startdate is required")]
        public string Startdate { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "Enddate is required")]
        public string Enddate { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "Leads is required")]
        public string Leads { get; set; }
        //[Required(AllowEmptyStrings = true, ErrorMessage = "PageSize is required")]
        public string RowStart { get; set; }
        public string PageSize { get; set; }
        public string Source { get; set; }
        public string subSource { get; set; }
        public string Location { get; set; }
        public string StateCode { get; set; }

        //public string? AmMail { get; set; }
        //public string? ShMail { get; set; }
        //public string? TmMail { get; set; }
        //public string? DealerMail { get; set; }
        //public string? FollowenquiryStatus { get; set; }
        //public string? Startdate { get; set; }
        //public string? Enddate { get; set; }
        //public string? Leads { get; set; }

    }
    public class StockMaster
    {

        public string DealerCode { get; set; }
        public string Filter { get; set; }

    }
    public class FilterModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string loginas { get; set; }
        public string LoginMail { get; set; }
        public string AmMail { get; set; }
        public string TmMail { get; set; }
        public string DealerMail { get; set; }
    }


    public class StockFilterModel
    {
        [Required(AllowEmptyStrings = true, ErrorMessage = "StartDate is required")]
        public string StartDate { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "EndDate is required")]
        public string EndDate { get; set; }

        //[Required(AllowEmptyStrings = true, ErrorMessage = "loginas is required")]
        //public string loginas { get; set; }

        //[Required(AllowEmptyStrings = true, ErrorMessage = "LoginMail is required")]
        //public string LoginMail { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "AmMail is required")]
        public string AmMail { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "ShMail is required")]
        public string ShMail { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "TmMail is required")]
        public string TmMail { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "DealerMail is required")]
        public string DealerMail { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "AgingStartDay is required")]
        public string AgingStartDay { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "AgingEndDay is required")]
        public string AgingEndDay { get; set; }
        public string RowStart { get; set; }
        public string PageSize { get; set; }
        public string StockStatus { get; set; }
        public string location { get; set; }
        public string StateCode { get; set; }
        //public string StartDate { get; set; }
        //public string EndDate { get; set; }
        //public string loginas { get; set; }
        //public string LoginMail { get; set; }
        //public string AmMail { get; set; }
        //public string ShMail { get; set; }
        //public string TmMail { get; set; }
        //public string DealerMail { get; set; }
        //public string AgingStartDay { get; set; }
        //public string AgingEndDay { get; set; }
    }

    public class BussinessPerformance
    {
        [Required(AllowEmptyStrings = true, ErrorMessage = "StartDate is required")]
        public string Startdate { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "EndDate is required")]
        public string Enddate { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "AmMail is required")]
        public string AmMail { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "ShMail is required")]
        public string ShMail { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "TmMail is required")]
        public string TmMail { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "DealerMail is required")]
        public string DealerMail { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "BoxFilter is required")]
        public string BoxFilter { get; set; }
        public string RowStart { get; set; }
        public string PageSize { get; set; }
        public string Source { get; set; }
        public string subSource { get; set; }
        public string StateCode { get; set; }
        public string Location { get; set; }



    }
    public class BussinessPerformanceResult
    {
        public List<dynamic> GetBoxes { get; set; }

        public List<dynamic> GetList { get; set; }

    }

    public class MarkStockSoldModel
    {
        public string CustomerId { get; set; }
        public string SaleDate { get; set; }
        public string SoldBy { get; set; }
        public string StockMasterId { get; set; }
        public string SalesEnquiryMasterId { get; set; }
        public string DeliveryDate { get; set; }
        public string IsRetailedSales { get; set; }

    }

    public class SalesEnquirySearchdto
    {
        public string ProspectMobileNumber { get; set; }
    }

    public class SearchInventoryReportDto
    {
        public string ChassisNo { get; set; }
    }

    public class RetailedSalesFilter
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AmMail { get; set; }
        public string ShMail { get; set; }
        public string TmMail { get; set; }
        public string DealerMail { get; set; }
        public int PageSize { get; set; }
        public int RowStart { get; set; }
        public string SearchText { get; set; }
        public string selectedCategory { get; set; }
        public string location { get; set; }
        public string StateCode { get; set; }
    }

    public class RetailSaleRequest
    {
        public string PaymentMode { get; set; }
        public decimal DpAmount { get; set; }
        public decimal LoanAmount { get; set; }
        public string FinancerName { get; set; }
        public string FinanceStatus { get; set; }
        public string IsRetailedSales { get; set; }
        public decimal DisburseAmount { get; set; }
        public string LoanType { get; set; } // still not in use
        public string SalesId { get; set; }
        public DateTime RetailedDate { get; set; }
        public decimal FinalSellingPrice { get; set; }
    }
    public class RetailSaleRequestv1
    {

        public string SalesId { get; set; }

        public DateTime? RetailedDate { get; set; }

        public string Username { get; set; }

        public string PlatformType { get; set; }

        public string ProspectType { get; set; }

        public string ExchangeMake { get; set; }

        public string ExchangeHpCategory { get; set; }

        public string ExchangeModel { get; set; }

        public string MfgYear { get; set; }

        public decimal? CustomerAskExchange { get; set; }

        public decimal? MktValueExchange { get; set; }

        public decimal? FinalPriceExchange { get; set; }

        public string ExchangeStockEntry { get; set; }

        public string ExchangeStockEntryValue { get; set; }

        public string PaymentType { get; set; }

        public decimal? DueAmount { get; set; }

        public decimal? LoanRequired { get; set; }
        public decimal? AdditionalCash { get; set; }

        public string FinancerName { get; set; }

        public string ManualFinancerName { get; set; }

        public string LoanType { get; set; }

        public string FinanceStatus { get; set; }

        public string FinanceStatusDetail { get; set; }

        public decimal? DisbursedAmount { get; set; }

        public string SalesEnquiryId { get; set; }

        public string FinanceMasterId { get; set; }
        public DateTime expectedRetailDate { get; set; }
    }

    public class EnquiryDetails
    {
        public DateTime enquiryDate { get; set; }
        public string enquirySource { get; set; }
        public string enquirySubSource { get; set; }

        public string prospectName { get; set; }
        public string prospectMobile { get; set; }
        public string prospectDistrict { get; set; }
        public string prospectDistrictCode { get; set; }
        public string prospectTehsil { get; set; }
        public string prospectTehsilCode { get; set; }
        public string prospectVillage { get; set; }
        public string prospectVillageCode { get; set; }
        public string otherVillageName { get; set; }
        public string prospectPINCode { get; set; }
        public string stateCode { get; set; }

        public string hpCategory { get; set; }
        public string driveType { get; set; }
        public string interestedModel { get; set; }
        public string variant { get; set; }

        public string enquiryType { get; set; }
        public string enquiryCurrentStatus { get; set; }
        public DateTime? nextFollowupDate { get; set; }
        public DateTime? expDeliveryDate { get; set; }
        public DateTime? bookingDate { get; set; }
        public decimal? bookingAmount { get; set; }
        public string enquiryStatus { get; set; }


    }

    public class FinanceDetails
    {
        public string paymentType { get; set; }
        public decimal? finalSalePrice { get; set; }
        public decimal? dpAmount { get; set; }
        public decimal? dueAmount { get; set; }

        public decimal loanRequired { get; set; }
        public string financerName { get; set; }
        public string manualFinancerName { get; set; }
        public string loanType { get; set; }

        public string financeStatus { get; set; }
        public string financeStatusDetail { get; set; }
        public decimal? disbursedAmount { get; set; }
        public decimal? customerDues { get; set; }
        public decimal? AdditionalCash { get; set; }
    }

    public class SaleDetails
    {
        public string chassisNumber { get; set; }

        public string prospectType { get; set; }
        public DateTime? deliveryDate { get; set; }
        public DateTime? expectedRetailDate { get; set; }
        public string exchangeMake { get; set; }
        public string exchangeHpCategory { get; set; }
        public string exchangeModel { get; set; }
        public string mfgYear { get; set; }
        public decimal? customerAskExchange { get; set; }
        public decimal? mktValueExchange { get; set; }
        public decimal? finalPriceExchange { get; set; }
        public string exchangeStockEntry { get; set; }
        public string exchangeStockEntryValue { get; set; }
        //public IFormFile excFile { get; set; }
    }


    public class CloseDetails
    {

        public string closedDroppedStatus { get; set; }
        public string saleLostReason { get; set; }
    }
    public class EnquiryMainModel
    {
        // User / Dealer Mapping
        public string stateHead { get; set; }
        public string nameStateHead { get; set; }
        public string areaManager { get; set; }
        public string nameAm { get; set; }
        public string territoryManager { get; set; }
        public string nameTm { get; set; }

        public string dealer { get; set; }
        public string dealerName { get; set; }
        public string dealerMail { get; set; }

        public string dealershipCode { get; set; }
        public string dealershipName { get; set; }
        public string dealershipLocation { get; set; }

        public string salesmenName { get; set; }
        public string salesmenNumber { get; set; }

        // Stage wise models
        public EnquiryDetails Enquiry { get; set; }
        public FinanceDetails Finance { get; set; }
        public SaleDetails Sale { get; set; }
        public CloseDetails Close { get; set; }

        // Extra
        public string conversionChallenge { get; set; }
        public string actionPlanned { get; set; }
    }



    public class UpdateEnquiryMainModel
    {
        public string SalesEnquiryId { get; set; }
        public string FinanceMasterId { get; set; }
        public string callstatus { get; set; }
        public string Remarks { get; set; }
        public EnquiryMainModel enquiryMainModel { get; set; }

    }
    public class Rc
    {
        public string RcStatus { get; set; }
        public string ChassisNo { get; set; }
        public string RegistrationNumber { get; set; }
        public string Id { get; set; }

    }

    public class salesEnquiryIdDto
    {
        public string Id { get; set; }
    }

    public class getExchangeStock
    {
        public string Status { get; set; }
        public string AmMail { get; set; }
        public string ShMail { get; set; }
        public string TmMail { get; set; }
        public string DealerMail { get; set; }
        public int PageSize { get; set; }
        public int RowStart { get; set; }
        public string SearchText { get; set; }
        public string selectedCategory { get; set; }
        public string location { get; set; }
        public string StateCode { get; set; }

    }

    public class SoldExchStock
    {

        public decimal SellingPrice { get; set; }

        public string CustomerName { get; set; }

        public string CustomerMobile { get; set; }

        public string District { get; set; }

        public string Tehsil { get; set; }

        public string Village { get; set; }

        public DateTime? LiquidationDate { get; set; }
        public string SalesId { get; set; }


    }

    public class oldEnquiryUpdate
    {
        public decimal? FinalSellingPrice { get; set; }
        public decimal? DpAmount { get; set; }
        public string SalesId { get; set; }
    }

    public class sourceSubSourceMaster
    {
        //public string PositionName { get; set; }
        public string SourceName { get; set; }
        public string SubSourceName { get; set; }
    }

    public class EnquiryFilterRequest
    {
        public string EnquirySource { get; set; }
        public string EnquirySubSource { get; set; }
        public string InterestedModel { get; set; }
        public string EnquiryStatus { get; set; }
        public string EnquiryType { get; set; }

        public string PageSize { get; set; }
        public string RowStart { get; set; }

        public string DealerCode { get; set; }
        public string AMName { get; set; }
        public string TMName { get; set; }
        public string SHName { get; set; }
        public string DealerCategory { get; set; }
        public string location { get; set; }
        public string StateCode { get; set; }
    }
}


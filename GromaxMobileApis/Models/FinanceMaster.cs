using System;
using System.Collections.Generic;

#nullable disable

namespace GromaxMobileApis.Models
{
    public partial class FinanceMaster
    {
        public Guid Id { get; set; }
        public string SalesEnquiryMasterId { get; set; }
        public string HistoryId { get; set; }
        public string ExchangeRequired { get; set; }
        public string FinalOn_RoadPrice { get; set; }
        public string OtherExpenses { get; set; }
        public string ProductSupport { get; set; }
        public string NdpOfVarient { get; set; }
        public string NetMargin { get; set; }
        public string CustomerExpecValueForExchangemodel { get; set; }
        public string DealerEstmtdCostOfExchangetractor { get; set; }
        public string FinalValOfferedForExchangeModel { get; set; }
        public string PaymentMode { get; set; }
        public DateTime CreateDate { get; set; }
        public string DpAmount { get; set; }
        public string LoanAmount { get; set; }
        public string FinancerName { get; set; }
        public string FinanceStatus { get; set; }
        public string KycCollected { get; set; }
        public string CibilChecked { get; set; }
        public string CibilScore { get; set; }
        public string FileStatus { get; set; }
        public string LoanType { get; set; }
        public string TotalAmount { get; set; }
        public string ExchangeBrand { get; set; }
        public string ExchangeModel { get; set; }
        public string ExchangeHPCategory { get; set; }
        public string ExchangeModelName { get; set; }
        public string FinalLiquidationPrice { get; set; }
        public string FinanceFI { get; set; }
        public string SanctionDone { get; set; }
        public string StockAvailable { get; set; }
        public string TabType { get; set; }
    }

    public class FinanceMastersalesenqiddto 
    {
        public string SalesEnquiryMasterId { get; set; }
    }
}

using System;

namespace GromaxMobileApis.Models
{
    public class NDAFormModel
    {
        public string SHMail { get; set; }
        public string AmMail { get; set; }
        public string TmMail { get; set; }
        public string TmName { get; set; }
        public string AmName { get; set; }
        public string SHName { get; set; }
        public string EnquirySource { get; set; }
        public string EnquirySubSource { get; set; }
        public string NDAProspectName { get; set; }
        public string MobileNo { get; set; }
        public string EnquiryCurrentStatus { get; set; }
        public string DistrictName { get; set; }
        public int DistrictCode { get; set; }
        public string TehsilName { get; set; }
        public string CityName { get; set; }
        public string CurrentBussiness { get; set; }
        public string IndustrySize { get; set; }
        public string InvestPlan { get; set; }
        public string ActionPlan { get; set; }
        public string FollowupRemarks { get; set; }
        public DateTime? NextFollowDate { get; set; }
        public DateTime expectedConversionDate { get; set; }
        public string CloseEnquiryRemark { get; set; }
        public string Remarks { get; set; }


    }

    public class GetDistrict
    {
        public string Position { get; set; } = null;
        public string userName { get; set; } = null;
    }

    public class GetTehsil
    {
        public int DistrictCode { get; set; }
    }
    public class GetCity
    {
        public int TehsilCode { get; set; }
    }

    public class NDAEnquiryModel
    {

        public DateTime? NextFollowUpDate { get; set; }

        public string SHInterviewStatus { get; set; }
        public DateTime? SHInterviewDate { get; set; }
        public string SHRemarks { get; set; }
        public string SHRejectionRemark { get; set; }

        public string HOInterviewStatus { get; set; }
        public DateTime? HOInterviewDate { get; set; }
        public string HORemarks { get; set; }
        public string HORejectionRemark { get; set; }

        public string SDReceivedStatus { get; set; }
        public DateTime? SDReceivingDate { get; set; }

        public string GSTStatus { get; set; }
        public DateTime? GSTRegistrationDate { get; set; }

        public string FundsReceivedStatus { get; set; }
        public DateTime? FundTransferDate { get; set; }

        public string BGReceivedStatus { get; set; }
        public DateTime? BGSubmissionDate { get; set; }

        public string CodeOpenedStatus { get; set; }
        public DateTime? LOIDate { get; set; }

        public string Id { get; set; }
        public string FollowupRemarks { get; set; }
        public string CloseEnquiryRemark { get; set; }
        public string EnquiryCurrentStatus { get; set; }
        public string ActionPlan { get; set; }
        public DateTime? expectedConversionDate { get; set; }
        public string Remarks { get; set; }


    }
    public class GetNDAByIDModel
    {
        public string Id { get; set; }
    }

    public class whatsappUserDetail
    {
        public string Name { get; set; }
        public string MobileNumber { get; set; }
    }

    public class reqForUpdateConversion
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class RequestGetNDA
    {
        public string Status { get; set; }
        public int PageNo { get; set; }
        public int? StateCode { get; set; }
        public string Duration { get; set; }
        public DateTime? StDate { get; set; }
        public DateTime? EnDate { get; set; }

        public string SelectedMobileNumber { get; set; }
        public string selectedEnquirySubSource { get; set; }
        public string selectedEnquirySource { get; set; }
        public string selectedOverdueFilter { get; set; }
    }

}

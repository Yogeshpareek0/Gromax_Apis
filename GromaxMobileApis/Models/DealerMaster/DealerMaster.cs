using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GromaxMobileApis.Models.DealerMaster
{
    public class DealerMaster
    {
        public Guid Id { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string StateHead { get; set; }
        public string Am { get; set; }
        public string Tm { get; set; }
        public string SAPCode { get; set; }
        public DateTime create_date { get; set; }
        public string Ao { get; set; }
        public string StateCode { get; set; }
        public string StateHeadMail { get; set; }
        public string StateHeadMobile { get; set; }
        public string AMMail { get; set; }
        public string AMMobile { get; set; }
        public string TMMail { get; set; }
        public string TMMobile { get; set; }
        public string District { get; set; }
        public string DistrictCode { get; set; }
        public string DealerMobile { get; set; }
        public string DealerEmail { get; set; }
        public string TehsilForDealer { get; set; }
        public string AreaCode { get; set; }
        public string DealerAlternateMobile { get; set; }
        public string ActiveStatus { get; set; }
        public string Zone { get; set; }
        public string DisplayStateName { get; set; }
        public string GstNo { get; set; }
        public string PanNo { get; set; }
        public DateTime? DateOfAppointment { get; set; }
        public decimal? openingStock { get; set; }
        public DateTime? openingStockDate { get; set; }
        public decimal? openingAdvances { get; set; }
        public DateTime? openingAdvDate { get; set; }
        public string service_CcmName { get; set; }
        public string service_CcmEmail { get; set; }
        public string service_CcmMobile { get; set; }
    }

    public class DealerRequestModel
    {
        public string dealerCode { get; set; } = null;
        public string dealerName { get; set; } = null;
        public string gstNo { get; set; } = null;
        public string panNo { get; set; } = null;
        public string dealerMobile { get; set; } = null;
        public string dealerAlternateMobile { get; set; } = null;
        public string dealerEmail { get; set; } = null;
        public string address { get; set; } = null;
        public string stateCode { get; set; } = null;
        public string stateName { get; set; } = null;
        public string districtCode { get; set; } = null;
        public string district { get; set; } = null;
        public string city { get; set; } = null;
        public List<string> tehsilCodes { get; set; } = null;
        public List<string> tehsils { get; set; } = null;
        public string stateHeadMail { get; set; } = null;
        public string stateHeadName { get; set; } = null;
        public string stateHeadMobile { get; set; } = null;
        public string amMail { get; set; } = null;
        public string amName { get; set; } = null;
        public string amMobile { get; set; } = null;
        public string tmMail { get; set; } = null;
        public string tmName { get; set; } = null;
        public string tmMobile { get; set; } = null;
        public string serviceCcmName { get; set; } = null;
        public string serviceCcmEmail { get; set; } = null;
        public string serviceCcmMobile { get; set; } = null;
        public DateTime dateOfAppointment { get; set; }
        public string activeStatus { get; set; } = null;
    }

    public class UpdateDealerFormRequest
    {
        [Required(ErrorMessage = "Dealer ID is required")]
        public string DealerId { get; set; }

        public string StateHeadName { get; set; }

        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile number must be 10 digits")]
        public string StateHeadMobile { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string StateHeadMail { get; set; }

        public string AmName { get; set; }

        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile number must be 10 digits")]
        public string AmMobile { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string AmMail { get; set; }

        public string TmName { get; set; }

        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile number must be 10 digits")]
        public string TmMobile { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string TmMail { get; set; }

        public string ServiceCcmName { get; set; }

        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile number must be 10 digits")]
        public string ServiceCcmMobile { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string ServiceCcmEmail { get; set; }
    }


   
}

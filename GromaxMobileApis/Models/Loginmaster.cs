using System;
using System.Collections.Generic;

#nullable disable

namespace GromaxMobileApis.Models
{
    public partial class Loginmaster
    {
        public Guid? Id { get; set; }
        public string DealerCode { get; set; }
        public string UserName { get; set; }
        public string PossitionId { get; set; }
        public string MobileNo { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Name { get; set; }
        public string PlatformType { get; set; }
        public string ActiveStatus { get; set; }
        public string SalesPermission { get; set; }
        public string EnquiryGeneration { get; set; }
        public string SalesFollowUp { get; set; }
        public string Installation { get; set; }
        public DateTime? CreateDate { get; set; }
        public string ModelDevice { get; set; }
        public string DeviceBrand { get; set; }
        public string DeviceId { get; set; }
        public string StateCode { get; set; }
        public string FCMToken { get; set; }
    }

    public class Loginmasterdto
    {
        public string MobileNo { get; set; }
        public string DeviceId { get; set; }
        public string ModelDevice { get; set; }
        public string DeviceBrand { get; set; }
        public string PlatformType { get; set; }
        public string Password { get; set; }
    }
    public class GetLoginmasterweb
    {
        public Guid? Id { get; set; }
        public string DealerCode { get; set; }
        public string UserName { get; set; }
        public string PossitionId { get; set; }
        public string MobileNo { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Name { get; set; }
        public string PlatformType { get; set; }
        public string ActiveStatus { get; set; }
        public string SalesPermission { get; set; }
        public string EnquiryGeneration { get; set; }
        public string SalesFollowUp { get; set; }
        public string Installation { get; set; }
        public DateTime? CreateDate { get; set; }
        public string ModelDevice { get; set; }
        public string DeviceBrand { get; set; }
        public string DeviceId { get; set; }
        public string StateCode { get; set; }
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
        public string misstatus { get; set; }
    }


    public class DealerUpdate 
    {
    public string DealerCode { get; set; }
    public string DealerStatus { get; set; }
    }
}

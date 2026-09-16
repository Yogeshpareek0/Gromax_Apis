using System;

namespace GromaxMobileApis.Models.EmployeeMaster
{
    public class RequestEmployeeMaster
    {
        public int stateCode { get; set; }
        public string position { get; set; }
    }

    public class ResponseEmployeeMaster
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string MobileNo { get; set; }
        public string Position { get; set; }
        public int? ManagingId { get; set; }   // nullable, kyunki NULL aa sakta hai (jaisa data mein dikha)
        public bool IsActive { get; set; }
        public DateTime Create_Date { get; set; }
        public string StateCode { get; set; }
    }

    public class ResponseEmployeeByPosition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Position { get; set; }
        public int? BaseStateCode { get; set; }
        public int? BaseDistrictCode { get; set; }
        public int? BaseTehsilCode { get; set; }
        public int? BaseVillageCode { get; set; }
        public string OtherLocation { get; set; }
    }
}

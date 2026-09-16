using System;

namespace GromaxMobileApis.Models
{
    public class ModelMaster
    {
        public Guid? Id { get; set; }
        public string ModelName { get; set; }
        public string ModelCode { get; set; }
        public string DriveType { get; set; }
        public string Colour { get; set; }
    }

    public class FcmTokendto 
    {
    public string MobileNumber { get; set; }
    public string FCMToken { get; set; }
    }
}

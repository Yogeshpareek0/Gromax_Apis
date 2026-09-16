using System;
using System.Collections.Generic;

namespace GromaxMobileApis.Models.Services
{

    public class NTIRRequest
    {
        public Guid? id { get; set; }
        public Guid stockMasterId { get; set; }
        public string runningHrs { get; set; }
        public string other { get; set; }
        public string doneBy { get; set; } = null;
        public string engineNo { get; set; } = null;
        public List<NTIRField> fields { get; set; }

    }

    public class NTIRField
    {
        public Guid? id { get; set; }
        public string name { get; set; }
        public string section { get; set; }
        public string status { get; set; }
        public string remark { get; set; }
        public string photo1 { get; set; }
        public string photo2 { get; set; }
        public string serial { get; set; }
        public string brand { get; set; }
    }

    public class NTIRImageRequest
    {
        public string FileName { get; set; } = string.Empty;
    }
    public class NTIRListResponse
    {
        public int page { get; set; } = 1;
        public string searchQuery { get; set; } = null;
    }


    public class NTIRResponse
    {
        public Guid Id { get; set; }
        public string runningHrs { get; set; }
        public string other { get; set; }
        public string doneBy { get; set; }
        public string engineNo { get; set; }

    }

    public class NTIRFieldResponse
    {
        public Guid Id { get; set; }
        public string name { get; set; }
        public string section { get; set; }
        public string status { get; set; }
        public string remark { get; set; }
        public string photo1 { get; set; }
        public string photo2 { get; set; }
        public string serial { get; set; }
        public string brand { get; set; }
    }

    public class NTIRMasterReponse
    {
        public NTIRResponse NtirMaster { get; set; }
        public List<NTIRFieldResponse> NtirFieldsMaster { get; set; }
    }

}

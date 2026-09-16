using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace GromaxMobileApis.Models.Services
{
    public class PdiRequest
    {
        public Guid stockMasterId { get; set; }
        public string runningHrs { get; set; }
        public string other { get; set; }
        public List<PdiField> fields { get; set; }

    }

    public class PdiField
    {
        public string name { get; set; }
        public string section { get; set; }
        public string status { get; set; }
        public string remark { get; set; }
        public string photo1 { get; set; }
        public string photo2 { get; set; }
        public string serial { get; set; }
        public string brand { get; set; }
    }

    public class RemoveImageRequest
    {
        public string FileName { get; set; } = string.Empty;
    }
    public class PDIListResponse
    {
        public int page { get; set; } = 1;
        public string searchQuery { get; set; } = null;
    }
    public class EligibleChassisListRequest
    {
        public int page { get; set; } = 1;
        public string searchQuery { get; set; }
        public string searchBy { get; set; }
    }
}

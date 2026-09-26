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

    public class PdiReportReqModel
    {
        public string state { get; set; }
        public string dealerCode { get; set; }
        public string modelName { get; set; }
        public string chasisNo { get; set; }
        public string duration { get; set; }
        public DateTime? stDate { get; set; }
        public DateTime? enDate { get; set; }
        public int pageSize { get; set; }
        public int rowStart { get; set; }
        public Boolean isDownload { get; set; } = false;
    }
    public class PdiCountResponse
    {
        public int totalTractorsCount { get; set; }
        public int pdiCompletedCount { get; set; }
        public int pdiPendingCount { get; set; }
        public int defectFoundCount { get; set; }
    }

    public class PdiReportResponse
    {
        public IEnumerable<dynamic> pdiReport { get; set; }
        public PdiCountResponse countResponse { get; set; }


    }
}

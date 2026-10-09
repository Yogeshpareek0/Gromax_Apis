using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GromaxMobileApis.Models
{
    public class CircularModel
    {
        public class CircularListRequest
        {
            public int RowSkip { get; set; } = 0;
            public string CircularName { get; set; }
            public string FinancialYear { get; set; }
            public string CircularType { get; set; }
        }

        public class AddCircularRequest
        {
            [Required] public string CircularName { get; set; }
            public string NewCircularName { get; set; }
            [Required] public string CircularDate { get; set; }
            [Required] public string Description { get; set; }
            [Required] public string FinancialYear { get; set; }
            [Required] public string CircularType { get; set; }
            public List<IFormFile> Files { get; set; }
        }

        public class CircularFileModel
        {
            public string FileUrl { get; set; }
            public string FileName { get; set; }
        }

        public class AddCircularModel
        {
            public string CircularName { get; set; }
            public string? NewCircularName { get; set; }
            public string CircularDate { get; set; }
            public string Description { get; set; }
            public string FinancialYear { get; set; }
            public string CircularType { get; set; }
            public List<CircularFileModel> FileUrls { get; set; } = new();
        }

        public class GetCircularDetailRequest
        {
            [Required] public string CircularName { get; set; }
        }

        public class UpdateCircularSentStatusRequest
        {
            [Required] public int CircularId { get; set; }
            [Required] public string Status { get; set; }
            [Required] public string Type { get; set; }   // DPs / Company
        }

        public class GetMsgSentReportRequest
        {
            public int RowSkip { get; set; } = 0;
            [Required] public int CircularId { get; set; }
            [Required] public string Type { get; set; }
            public string Download { get; set; } = "No";
        }

        public class GetCircularDetailAppRequest
        {
            public string FY { get; set; }
            public string CircularType { get; set; }
        }

        public class CircularDetailsAppResponse
        {
            public int Id { get; set; }
            public string FY { get; set; }
            public string CircularType { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public DateTime CircularDate { get; set; }
            public List<CircularPdfApp> PdfUrls { get; set; }
        }

        public class CircularPdfApp
        {
            public string PdfName { get; set; }
            public string PdfUrl { get; set; }
        }

        public class CircularDropdownResponse
        {
            public List<string> FyYear { get; set; } = new();
            public List<string> CircularType { get; set; } = new();
        }
    }
}

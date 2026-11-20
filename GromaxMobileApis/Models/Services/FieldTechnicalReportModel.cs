using System;
using System.ComponentModel.DataAnnotations;

namespace GromaxMobileApis.Models.Services
{
    public class FieldTechnicalReportModel
    {
        public class GetChassisForFTRResponseModel
        {
            public Guid MasterId { get; set; }
            public Guid JobCardId { get; set; }
            public string ModelCode { get; set; }
            public string ChasisNo { get; set; }
            public string ModelName { get; set; }

            public string DealerName { get; set; }
            public string DealerCode { get; set; }
            public string DealerAddress { get; set; }

            public DateTime? InstallationDate { get; set; }

            public string DriveType { get; set; }
            public string Colour { get; set; }
            public string Status { get; set; }

            public string CustomerName { get; set; }
            public string CustomerAddress { get; set; }
            public string CustomerMobile { get; set; }

            public string Village { get; set; }
            public string PostOffice { get; set; }
            public string District { get; set; }
            public int TotalCount { get; set; }
        }

        public class AddFieldTechRequestModel
        {
            [MaxLength(50)]
            public string SSEName { get; set; }

            [MaxLength(50)]
            public string DealerCode { get; set; }

            [MaxLength(100)]
            public string TractorModelBOMCode { get; set; }

            [MaxLength(100)]
            public string TractorSerialNo { get; set; }

            public DateTime? FailureDate { get; set; }

            public decimal? HoursWorked { get; set; }

            public DateTime? RepairDate { get; set; }

            [MaxLength(100)]
            public string NatureOfWorkDone { get; set; }

            public Guid? SalesCustomerMasterId { get; set; }

            public Guid? StockMasterId { get; set; }

            [MaxLength(200)]
            public string CustomerName { get; set; }

            [MaxLength(500)]
            public string CustomerAddress { get; set; }

            [MaxLength(15)]
            public string MobileNo { get; set; }

            [MaxLength(150)]
            public string Village { get; set; }

            [MaxLength(150)]
            public string Post { get; set; }

            [MaxLength(150)]
            public string District { get; set; }

            [MaxLength(100)]
            public string ImplementTrolleySize { get; set; }

            [MaxLength(500)]
            public string CustomerComplaints { get; set; }

            [MaxLength(500)]
            public string ProblemDefinition { get; set; }
            [Required]
            public string StockStatus { get; set; }
            [MaxLength(100)]
            public string CreatedBy { get; set; }

            [MaxLength(50)]
            public string PositionName { get; set; }
        }

        public class UpdateFTRRequestModel
        {
            [Required]
            public string Id { get; set; }

            [Required]
            [MaxLength(50)]
            public string SSEName { get; set; }

            public DateTime? FailureDate { get; set; }

            public decimal? HoursWorked { get; set; }

            public DateTime? RepairDate { get; set; }

            [MaxLength(100)]
            public string NatureOfWorkDone { get; set; }

            [MaxLength(100)]
            public string ImplementTrolleySize { get; set; }

            [MaxLength(500)]
            public string CustomerComplaints { get; set; }

            [MaxLength(500)]
            public string ProblemDefinition { get; set; }

            [MaxLength(100)]
            public string ModifiedBy { get; set; }

            [MaxLength(50)]
            public string PositionName { get; set; }

            [Required]
            [MaxLength(10)]
            public string StockStatus { get; set; }
        }

        public class GetFTRAllDataRequestModel
        {
            public int pageNo { get; set; } = 1;
            [Required]
            public DateTime stDate { get; set; }
            [Required]
            public DateTime enDate { get; set; }
            public string chassisNo { get; set; } = null;
            public string stockStatus { get; set; } = "IN";

            public int? stateId { get; set; } = null;
            public bool isDownload { get; set; } = false;
            public string duration { get; set; } = "This Month";
        }
    }
}

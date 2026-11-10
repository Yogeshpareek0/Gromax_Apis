using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GromaxMobileApis.Models.Services
{
    public class WarrantyClaimModel
    {
        public class ClosedJobCardListRequest
        {
            public string JobCardNo { get; set; }
            public string ChassisNumber { get; set; }
            public string JobCardType { get; set; }

            public string Duration { get; set; } = "this Month";
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }

            public string Username { get; set; }
            public string PositionName { get; set; }
            public int Offset { get; set; }
            public int PageSize { get; set; }

        }

        public class WarrantyClaimRequest
        {
            [Required(ErrorMessage = "Job Card ID is required.")]
            public Guid JobCardId { get; set; }

            public string ClaimRemarks { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Labour charge cannot be negative.")]
            public decimal LabourCharge { get; set; }

            [Required(ErrorMessage = "At least one spare part is required.")]
            [MinLength(1, ErrorMessage = "At least one spare part is required.")]
            public List<WarrantyClaimPartRequest> Parts { get; set; } = new();
        }

        public class WarrantyClaimPartRequest
        {
            [Required(ErrorMessage = "Spare Part ID is required.")]
            public Guid SparePartId { get; set; }

            public string PartCode { get; set; }

            [Range(typeof(decimal), "0.01", "9999999999999999",ErrorMessage = "Claim quantity must be greater than zero.")]
            public decimal ClaimQty { get; set; }

            [Range(typeof(decimal), "0", "9999999999999999",ErrorMessage = "Rate cannot be negative.")]
            public decimal Rate { get; set; }

            [Range(typeof(decimal), "0", "100",ErrorMessage = "GST percentage must be between 0 and 100.")]
            public decimal GSTPercentage { get; set; }
        }

        public class WarrantyClaimApprovalRequest
        {
            public Guid WarrantyClaimId { get; set; }
            public string PositionName { get; set; } = string.Empty;
            public int Action { get; set; } // 1 = Approve, -1 = Reject
            public string Remarks { get; set; }
        }



        public class UpdateWarrantyClaimRequest
        {
            [Required(ErrorMessage = "Job Card ID is required.")]
            public Guid WarrantyClaimId { get; set; }

            public string ClaimRemarks { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Labour charge cannot be negative.")]
            public decimal LabourCharge { get; set; }

            [Required(ErrorMessage = "At least one spare part is required.")]
            [MinLength(1, ErrorMessage = "At least one spare part is required.")]
            public List<WarrantyClaimPartRequest> Parts { get; set; } = new();
        }

        public class UpdateWarrantyClaimPartRequest
        {
            [Required(ErrorMessage = "Spare Part ID is required.")]
            public Guid SparePartId { get; set; }

            public string PartCode { get; set; }

            [Range(typeof(decimal), "0.01", "9999999999999999", ErrorMessage = "Claim quantity must be greater than zero.")]
            public decimal ClaimQty { get; set; }

            [Range(typeof(decimal), "0", "9999999999999999", ErrorMessage = "Rate cannot be negative.")]
            public decimal Rate { get; set; }

            [Range(typeof(decimal), "0", "100", ErrorMessage = "GST percentage must be between 0 and 100.")]
            public decimal GSTPercentage { get; set; }
        }



    }


}

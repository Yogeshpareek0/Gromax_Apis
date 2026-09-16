using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GromaxMobileApis.Models.Services
{
    public class MechanicMaster
    {
        public Guid MechanicID { get; set; }

        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string StateName { get; set; }
        public string Location { get; set; }
        public string MechanicName { get; set; }
        public string ContactNo { get; set; }
        public string AadharCardNo { get; set; }
        public string TypeOfMechanic { get; set; }

        public string Education { get; set; }
        public int? ExperienceInGromax { get; set; }
        public string PriorExperience { get; set; }
        public int? PriorExperienceYears { get; set; }
        public int? TotalExperience { get; set; }

        public int PhysicalTrainingCount { get; set; }
        public int VirtualTrainingCount { get; set; }

        public string InstallationAttendance { get; set; }
        public string HydraulicAttendance { get; set; }
        public string EngineAttendance { get; set; }
        public string CompleteTractorTraining { get; set; }
        public string SystemAndProcess { get; set; }

        public string ApprovalStatus { get; set; }

        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public string RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public string RejectedRemark { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
    }

    public class MechanicCreateRequest
    {
        public string DealerCode { get; set; }
        public string MechanicName { get; set; }
        public string ContactNo { get; set; }
        public string AadharCardNo { get; set; }
        public string TypeOfMechanic { get; set; }

        public string Education { get; set; }
        public int? ExperienceInGromax { get; set; }
        public string PriorExperience { get; set; }
        public int? PriorExperienceYears { get; set; }
        public int? TotalExperience { get; set; }

        public int PhysicalTrainingCount { get; set; }
        public int VirtualTrainingCount { get; set; }

        public string InstallationAttendance { get; set; }
        public string HydraulicAttendance { get; set; }
        public string EngineAttendance { get; set; }
        public string CompleteTractorTraining { get; set; }
        public string SystemAndProcess { get; set; }

        public string CreatedBy { get; set; }
    }

    public class MechanicApprovalRequest
    {
        [Required]
        public Guid MechanicId { get; set; }

        [RegularExpression("^(Approved|Rejected)$", ErrorMessage = "Status must be Pending, Approved, or Rejected.")]
        public string Status { get; set; }

        [StringLength(100, ErrorMessage = "Remark cannot exceed 100 characters.")]
        public string Remark { get; set; }
        public string UserName { get; set; }
        public string LoginAs { get; set; }

    }


    public class MechanicApprovalFilterRequest
    {
        public DateTime? StDate { get; set; }

        public DateTime? EnDate { get; set; }

        public string Status { get; set; }
    }

    public class MechanicUpdateRequest : IValidatableObject
    {
        [Required]
        public Guid MechanicId { get; set; }
        public string MechanicName { get; set; }
        public string ContactNo { get; set; }
        public string AadharCardNo { get; set; }
        public string TypeOfMechanic { get; set; }

        public string Education { get; set; }
        public int? ExperienceInGromax { get; set; }
        public string PriorExperience { get; set; }
        public int? PriorExperienceYears { get; set; }
        public int? TotalExperience { get; set; }

        public int PhysicalTrainingCount { get; set; }
        public int VirtualTrainingCount { get; set; }

        public string InstallationAttendance { get; set; }
        public string HydraulicAttendance { get; set; }
        public string EngineAttendance { get; set; }
        public string CompleteTractorTraining { get; set; }
        public string SystemAndProcess { get; set; }
        public string CurrentStatus { get; set; }
        public DateTime? DeletedStatusDate { get; set; }

        public string CreatedBy { get; set; }
        public string UserName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.Equals(CurrentStatus, "Inactive", StringComparison.OrdinalIgnoreCase)
                && !DeletedStatusDate.HasValue)
            {
                yield return new ValidationResult(
                    "Deleted Status Date is required when Current Status is Inactive.",
                    new[] { nameof(DeletedStatusDate) }
                );
            }
        }

    }
}

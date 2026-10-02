using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace GromaxMobileApis.Models
{
    public partial class InstallationMaster
    {
        public Guid? Id { get; set; }
        public string InstallationPersonName { get; set; }
        public string MobileNo { get; set; }
        public string Status { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? CreateDate { get; set; }
    }
    //public partial class InstallationImg
    //{
    //    public Guid? Id { get; set; }
    //    public string InstallationMasterId { get; set; }
    //    public string ImgUrl { get; set; }
    //    public DateTime? CreateDate { get; set; }
    //}
    public partial class InstallationImg
    {
        public Guid? Id { get; set; }
        public string InstallationMasterId { get; set; }
        public List<IFormFile> ImgUrl { get; set; }
        //public string ImgUrl { get; set; }
        //public DateTime? CreateDate { get; set; }
    }

    public class InstallationImageData
    {
        public string InstallationMasterId { get; set; }
        public string ImgUrl { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
        public string TagName { get; set; }
    }


    public class InstallationMasterdto
    {
        public string LoginPosition { get; set; }
        public string ShMail { get; set; }
        public string AmMail { get; set; }
        public string TmMail { get; set; }
        public string DealerMail { get; set; }
        public string LoginMail { get; set; }
        public string RowStart { get; set; }
        public string PageSize { get; set; }
        public string Status { get; set; }
        public string StateName { get; set; }
        public string Dealership { get; set; }
        public string ChassisNo { get; set; }
        public string IsDownload { get; set; } = "No";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }

    public class InstallationImage
    {
        public string Id { get; set; }
    }

    public class installationReq
    {
        public string InstallationMasterId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
        public string WorkingHrs { get; set; }
        public string MobileNo { get; set; } = null;
        public List<installationImage> installationImages { get; set; }

    }

    public class installationImage
    {
        public string TagName { get; set; }
        public IFormFile Image { get; set; }


    }


    public class InstallationApprovalRequest : IValidatableObject
    {
        [Required]
        public string InstallationId { get; set; }
        [RegularExpression("^-1$|^1$", ErrorMessage = "ApprovalStatus must be either Approved or Rejected.")]
        public int ApprovalStatus { get; set; }
        [MaxLength(100, ErrorMessage = "Remark cannot exceed 100 characters.")]
        public string Remark { get; set; }

        public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
        {
            if (ApprovalStatus == -1 && string.IsNullOrWhiteSpace(Remark))
            {
                yield return new ValidationResult(
                    "Remark is required when the installation is rejected.",
                    new[] { nameof(Remark) }
                );
            }
        }

    }
}

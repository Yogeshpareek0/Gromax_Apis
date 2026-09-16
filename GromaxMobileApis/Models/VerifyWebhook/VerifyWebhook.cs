using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GromaxMobileApis.Models.VerifyWebhook
{
    public class VerifyWebhook
    {
        [JsonPropertyName("name")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [JsonPropertyName("mobile")]
        [Required(ErrorMessage = "Mobile is required")]
        public string Mobile { get; set; }

        [JsonPropertyName("city")]
        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [JsonPropertyName("state_province")]
        [Required(ErrorMessage = "State Name is required")]
        public string StateProvince { get; set; }

        [JsonPropertyName("model")]
        [Required(ErrorMessage = "Model is required")]

        public string Model { get; set; }

        [JsonPropertyName("hp_category")]
        [Required(ErrorMessage = "Hp Category is required")]
        public string HpCategory { get; set; }

        [JsonPropertyName("terms_of_service")]
        [Required(ErrorMessage = "Term Of Services is required")]

        public string TermsOfService { get; set; }
    }

    public class MetaLeadRequest
    {
        [Required]
        public string LeadId { get; set; }
        [Required]
        public DateTime LeadCreatedTime { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string CampaignName { get; set; }

        // Optional - Only for B2B leads
        public string Location { get; set; }
    }

}

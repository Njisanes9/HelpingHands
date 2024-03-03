using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HelpingHands.Models
{
    public class HelpingHandsBusiness
    {
        public int businessId { get; set; }

        [DisplayName("Organisation Name")]
        [Required(ErrorMessage ="Please provide organisation name")]
        public string orgName { get; set; }

        [DisplayName("NPO Number")]
        [Required(ErrorMessage = "Please provide NPO number")]
        public string npoNumber { get; set; }

        [DisplayName("Address")]
        [Required(ErrorMessage = "Please provide address")]
        public string address { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "Please provide email")]
        public string Email { get; set; }

        [DisplayName("Business Picture")]
        public byte[]? picture { get; set; }

        [DisplayName("Contact Number")]
        [Required(ErrorMessage = "Please provide organisation contact number")]
        public string contactNumber { get; set; }

        [DisplayName("Operating Hours")]
        [Required(ErrorMessage = "Please provide operating hours")]
        public string operatingHours { get; set; }

    }
}

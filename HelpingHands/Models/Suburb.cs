using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HelpingHands.Models
{
    public class Suburb
    {
        public int SuburbId { get; set; }

        [Required(ErrorMessage ="Please provide suburb name!")]
        [DisplayName("Suburb Name")]
        [StringLength(100)]
        public string SuburbName { get; set;}

        [DisplayName("Postal Code")]
        [StringLength(100)]
        [Required(ErrorMessage = "Please provide postal code!")]
        public string PostalCode { get; set; }

        [DisplayName("City")]
        public int CityId { get; set; }

        public string? CityName { get; set; }
    }
   
}

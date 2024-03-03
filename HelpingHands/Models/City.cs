using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HelpingHands.Models
{
    public class City
    {
        public int CityId { get; set; }

        
        [DisplayName("City Name")]
        [Required(ErrorMessage ="Please provide city name")]
        public string CityName { get; set; }

        
        [DisplayName("City Abbreviation")]
        [Required(ErrorMessage = "Please provide abbreaviation")]
        public string Abbreviation { get; set; }
    }
    public class GetSuburb : SelectListItem
    {
        public string? SuburbName { get; set; }
    }
}

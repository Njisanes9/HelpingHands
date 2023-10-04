using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HelpingHands.Models
{
    public class City
    {
        public int CityId { get; set; }

        [StringLength(100)]
        [DisplayName("City Name")]
        public string CityName { get; set; }

        [StringLength(100)]
        [DisplayName("City Abbreviation")]
        public string Abbreviation { get; set; }
    }
}

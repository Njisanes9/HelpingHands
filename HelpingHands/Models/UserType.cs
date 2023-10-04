using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HelpingHands.Models
{
    public class UserType
    {
        public int UserTypeId { get; set; }

        [DisplayName("User Type Name")]
        [StringLength(50)]
        public string UserTypeDescription { get; set; }

        [DisplayName("User Type Abbreviation")]
        [StringLength(10)]
        public string Abbreviation { get; set;}
    }
}

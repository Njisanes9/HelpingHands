using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HelpingHands.Models
{
    public class ChronicCondition
    {
        public int ConditionId { get; set; }

       
        [Required(ErrorMessage = "Please provide condition name!")]
        [DisplayName("Condition Name")]
        [StringLength(100)]
        public string ConditionName { get; set; }


        [Required(ErrorMessage = "Please provide condition name!")]
        [DisplayName("Condition Description")]
        [StringLength(500)]
        public string ConditionDescr { get; set; }

    }
}

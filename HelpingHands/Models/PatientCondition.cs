using System.ComponentModel;
using System.Web.Mvc;

namespace HelpingHands.Models
{
    public class PatientCondition 

    {
        public int PatientConditionId { get; set; }

        public int PatientId { get; set; }

        [DisplayName("Select Condition")]
        public int ConditionId { get; set; }

        public string? ConditionName { get; set; }

        public string? ConditionDescr { get; set; }

    }
    public class ConditionDescription : SelectListItem
    {
        public string? ConditionDescr { get; set; }
    }
}

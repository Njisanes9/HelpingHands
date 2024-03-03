using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HelpingHands.ViewModels
{
    public class VisitDetailsVM
    {

        public int VisitId { get; set; }

        [DisplayName("Visit Arrival Time")]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? VisitArrivalTime { get; set; }


        [DisplayName("Visit Depart Time")]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? VisitDepartTime { get; set; }

                
        [StringLength(200)]
        public string? WoundCondition { get; set; }


        [StringLength(200)]
        public string? Notes { get; set; }
    }
}

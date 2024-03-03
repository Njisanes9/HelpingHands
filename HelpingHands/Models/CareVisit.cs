using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HelpingHands.Models
{
    public class CareVisit
    {
        public int VisitId { get; set; }

        public int ContractId { get; set; }

        [Required]
        [DisplayName("Visit Date")]        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime VisitDate { get; set; }


        [Required]
        [DisplayName("Approximate Arrival Time")]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? ApproxArriveTime { get; set; }
        


        
        [DisplayName("Visit Arrival Time")]        
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? VisitArrivalTime { get; set; } 

        
        [DisplayName("Visit Depart Time")]            
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? VisitDepartTime { get; set; } 


        [Required]
        [StringLength(500)]
        public string? WoundCondition { get; set; }

        
        [StringLength(500)]
        public string? Notes { get; set; }
    }
}

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
        [StringLength(8)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime VisitDate { get; set; }


        [Required]
        [DisplayName("Approximate Arrival Time")]
        [StringLength(7)]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime ApproxArriveTime { get; set; }


        [Required]
        [DisplayName("Visit Arrival Time")]
        [StringLength(7)]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime VisitArrivalTime { get; set; }

        [Required]
        [DisplayName("Visit Depart Time")]
        [StringLength(7)]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime VisitDepartTime { get; set; }


        [Required]
        [StringLength(500)]
        public string WoundCondition { get; set; }

        [Required]
        [StringLength(500)]
        public string Notes { get; set; }
    }
}

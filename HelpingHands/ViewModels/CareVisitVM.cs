using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HelpingHands.ViewModels
{
    public class CareVisitVM
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
        public DateTime ApproxArriveTime { get; set; }
    }
}

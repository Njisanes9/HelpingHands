using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HelpingHands.ViewModels
{
    public class ContractVM
    {
        public int ContractId { get; set; }

        public int PatientId { get; set; }


        [Required]
        [DisplayName("Contract Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ContractDate { get; set; }



        [StringLength(100)]
        [DisplayName("Address Line 1")]
        public string? AddressLine1 { get; set; }


        [StringLength(100)]
        [DisplayName("Address Line 2")]
        public string? AddressLine2 { get; set; }


        [Required]
        [DisplayName("Suburb")]
        public int SuburbId { get; set; }



        [DisplayName("Wound Description")]
        public string? WoundDescription { get; set; }

        public string? Status { get; set; }

        public string? Gender { get; set; }
    }
}

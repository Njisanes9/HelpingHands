using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HelpingHands.Models
{
    public class CareContact
    {
        public int ContractId { get; set; }

        public int PatientId { get; set; }


        [Required]
        [StringLength(8)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ContractDate { get; set; }


        [Required]
        [StringLength(100)]
        [DisplayName("Address Line 1")]
        public string AddressLine1 { get; set; }


       
        [StringLength(100)]
        [DisplayName("Address Line 2")]
        public string AddressLine2 { get; set; }


        [Required]
        [DisplayName("Suburb")]
        public string Suburb { get; set; }


        
        [DisplayName("Wound Description")]
        public string WoundDescription { get; set; }


        [Required]
        [StringLength(8)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }


        [Required]
        [StringLength(8)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        public int NurseId { get; set; }
    }
}

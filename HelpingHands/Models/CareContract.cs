using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HelpingHands.Models
{
    public class CareContract
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
               
        
        
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }
                
       
        
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; } 

        public int NurseId { get; set; }

        public string? Status { get; set; }

        public string? Gender { get; set; }

        public string? SuburbName { get; set; }

        public string? Name { get; set; }

        public string? Surname { get; set; }


        public string? Fullname
        {
            get
            {
                return $"{Surname} {Name}";
            }
        }
    }
}

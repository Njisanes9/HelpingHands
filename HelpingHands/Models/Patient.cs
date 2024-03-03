using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HelpingHands.Models
{
    public class Patient
    {
        public int PatientId { get; set; }
        public int UserId { get; set; }

        [Required(ErrorMessage ="Please provide name!")]
        [DisplayName("Name")]
        [StringLength(100)]
        public string? Name { get; set;}


        [Required(ErrorMessage = "Please provide surname!")]
        [DisplayName("Surname")]
        [StringLength(100)]
        public string? Surname { get; set; }

        [Required(ErrorMessage = "Please select gender!")]
        [DisplayName("Gender")]
        [StringLength(100)]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Please provide number!")]
        [DisplayName("ID Number")]
        [StringLength(13)]
        public string IDNumber { get; set; }


        //[Required(ErrorMessage = "Please select date!")]
        [DisplayName("Date Of Birth")]
             
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DoB { get; set; }

        
        [DisplayName("Emergency Contact Person")]
        [StringLength(100)]
        public string ContactPerson { get; set; }

        
        [DisplayName("Emergency Number")]
        [StringLength(13)]
        public string ContactPersonNumber { get; set; }

        [DisplayName("Additional Information")]
        
        public string AdditionalInform { get; set; }

        public byte[] Picture { get; set; }

        public string UserName { get; set; }

        public string Fullname
        {
            get
            {
                return $"{Name} {Surname}";
            }
        }

    }
 
}

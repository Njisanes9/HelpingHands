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
        public string Name { get; set;}


        [Required(ErrorMessage = "Please provide surname!")]
        [DisplayName("Surname")]
        [StringLength(100)]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Please select gender!")]
        [DisplayName("Gender")]
        [StringLength(100)]
        public GenderEnum Gender { get; set; }

        [Required(ErrorMessage = "Please provide number!")]
        [DisplayName("ID Number")]
        [StringLength(13)]
        public string IDNumber { get; set; }


        //[Required(ErrorMessage = "Please select date!")]
        [DisplayName("Date Of Birth")]
        [StringLength(8)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DoB { get; set; }

        [Required(ErrorMessage = "Please provide contact person name!")]
        [DisplayName("Emergency Contact Person")]
        [StringLength(100)]
        public string ContactPerson { get; set; }

        [Required(ErrorMessage = "Please provide contact person name!")]
        [DisplayName("Emergency Contact Person")]
        [StringLength(13)]
        public string ContactPersonNumber { get; set; }


        
        [DisplayName("Additional Information")]
        [StringLength(500)]
        public string AdditionalInform { get; set; }

    }
    public enum GenderEnum 
    {
        Female,
        Male
    
    }

}

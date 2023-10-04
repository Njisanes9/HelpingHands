using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HelpingHands.Models
{
    public class Nurse
    {
        public int NurseId { get; set; }
        public int UserId { get; set; }

        [Required(ErrorMessage = "Please provide name!")]
        [DisplayName("Name")]
        [StringLength(100)]
        public string Name { get; set; }


        [Required(ErrorMessage = "Please provide surname!")]
        [DisplayName("Surname")]
        [StringLength(100)]
        public string Surname { get; set; }

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
        [StringLength(8)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DoB { get; set; }
    }
}

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
        public string? Name { get; set; }

        public string? UserName { get; set; }


        [Required(ErrorMessage = "Please provide surname!")]
        [DisplayName("Surname")]
        [StringLength(100)]
        public string? Surname { get; set; }

        [Required(ErrorMessage = "Please select gender!")]
        [DisplayName("Gender")]
        [StringLength(10)]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Please provide number!")]
        [DisplayName("ID Number")]
        [StringLength(13)]
        public string? IDNumber { get; set; }

        public byte[] Picture { get; set; }


        public string? FullName
        {
            get
            {
                return $"{Name} {Surname}";
            }
        }

    }
}

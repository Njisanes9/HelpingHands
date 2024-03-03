using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HelpingHands.ViewModels
{
    public class UserVM
    {
        [Key]
        public int UserId { get; set; }

        [RegularExpression(@"^(([A-za-z]+[\s]{1}[A-za-z]+)|([A-Za-z]+))$")]
        [Required(ErrorMessage = "Please provide a username!")]
        [DisplayName("Username")]
        [StringLength(100)]
        public string? UserName { get; set; }


        
        [DisplayName("Email")]
        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^\S+@\S+\.\S+$", ErrorMessage = "Please enter a valid email address")]

        public string? Email { get; set; }

              


        [Required(ErrorMessage = "Please provide a number!")]
        [DisplayName("Contact Number")]
        [StringLength(10)]
        public string? ContactNumber { get; set; }



        [DisplayName("User Type")]
        public int UserTypeId { get; set; }

        public string? UserTypeDescription { get; set; }

        [DisplayName("Status")]
        [StringLength(50)]
        public string? Status { get; set; }
    }
}

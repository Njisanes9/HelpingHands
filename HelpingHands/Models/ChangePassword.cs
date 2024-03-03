using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HelpingHands.Models
{
    public class ChangePassword
    {
        [Key]
        public int UserId { get; set; }

        public string? UserName { get; set; }
               

        public string? Email { get; set; }


        [Required(ErrorMessage = "Please provide a password!")]
        [DisplayName("Password")]
        [StringLength(15, ErrorMessage = "Password \"{0}\" must have {2} character", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^([a-zA-Z0-9@*#]{8,15})$", ErrorMessage = "Password must contain: Minimum 8 characters atleast 1 UpperCase Alphabet, 1 LowerCase " +
            "Alphabet, 1 Number and 1 Special Character")]
        public string Password { get; set; }


       
        public string? ContactNumber { get; set; }



        
        public int UserTypeId { get; set; }

        public string? UserTypeDescription { get; set; }

        
        public string? Status { get; set; }
    }
}

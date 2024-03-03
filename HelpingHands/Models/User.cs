using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HelpingHands.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [RegularExpression(@"^(([A-za-z]+[\s]{1}[A-za-z]+)|([A-Za-z]+))$")]
        [Required(ErrorMessage ="Please provide a username!")]
        [DisplayName("Username")]
        [StringLength(100)]
        public string UserName { get; set; }


        [Required(ErrorMessage = "Please provide email!")]
        [DisplayName("Email")]
        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                           ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Please provide a password!")]
        [DisplayName("Password")]
        [StringLength(50, ErrorMessage = "Password \"{0}\" must have {2} character", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^([a-zA-Z0-9@*#]{8,15})$", ErrorMessage = "Password must contain: Minimum 8 characters atleast 1 UpperCase Alphabet, 1 LowerCase " +
            "Alphabet, 1 Number and 1 Special Character")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Please provide a number!")]
        [DisplayName("Contact Number")]
        [StringLength(10)]  
        public string ContactNumber { get; set; }


       
        [DisplayName("User Type")]
        public int UserTypeId { get; set; }

        
        [DisplayName("Status")]
        [StringLength(50)]
        public string Status { get; set; }

    }
   

}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HelpingHands.Models
{
	public class Login
	{
		[Key]
		public int UserId { get; set; }

		
		[Required(ErrorMessage = "Please provide a username!")]
		[DisplayName("Username")]
		[StringLength(100)]
		public string UserName { get; set; }
		
		[Required(ErrorMessage = "Please provide a password!")]
		[DisplayName("Password")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}

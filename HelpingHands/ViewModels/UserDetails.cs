using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HelpingHands.ViewModels
{
    public class UserDetails
    {
        public int UserId { get; set; } 
        public string? Name { get; set; }


        public string? Surname { get; set; }
        public byte[]? Picture { get; set; }
    }
}

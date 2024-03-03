using System.ComponentModel;

namespace HelpingHands.Models
{
    public class PrefferedSuburb
    {
        public int PrefSuburbId { get; set; }

        public int NurseId { get;}

        [DisplayName("Suburb")]
        public int SuburbId { get;}
    }
}

namespace HelpingHands.ViewModels
{
    public class CareContractVM
    {
        public int ContractId { get; set; }
       
        public DateTime? StartDate { get; set; }
                
        public int NurseId { get; set; }
        public string Status { get; set; }
    }
}

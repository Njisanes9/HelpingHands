namespace HelpingHands.ViewModels
{
    public class PatientConditionVM
    {
        public int PatientConditionId { get; set; }

        public int PatientId { get; set; }
        
        public string Name { get; set; }
        
        public string Surname { get; set; }

        public string? ConditionName { get; set; }

        public string? ConditionDescr { get; set; }
    }
}

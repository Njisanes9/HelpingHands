namespace HelpingHands.ViewModels
{
	public class CareVisitDetailsVM
	{

		public string Name { get; set; }

		public string Surname { get; set; }

		public string WoundCondition { get; set; }

		public string Notes { get; set; }

		public byte[] Picture { get; set; }

		public DateTime ContractDate { get; set; }

		public DateTime VisitDate { get; set; }

		public DateTime DepartTime { get; set; }

		public DateTime ApproxArriveTime { get; set; }

		public DateTime ArriveTime { get; set; }

		public string Fullname
        {
            get
            {
                return $"{Name} {Surname}";
            }
        }
    }
}

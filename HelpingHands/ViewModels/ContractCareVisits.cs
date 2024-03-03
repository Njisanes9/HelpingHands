namespace HelpingHands.ViewModels
{
	public class ContractCareVisits
	{
		public string Surname { get; set; }

		public string Firstname { get; set; }

		public string AddressLine1 { get; set; }

		public DateTime ContractDate { get; set; }

		public DateTime VisitDate { get; set; }

		public TimeSpan ApproxArrival { get; set; }

		public TimeSpan ArrivalTime { get; set; }
		public TimeSpan DepartTime { get; set; }

		public string WoundCondition { get; set; }

		public string Fullname
		{
			get
			{
				return $"{Firstname} {Surname}";
			}
		}
	}
}

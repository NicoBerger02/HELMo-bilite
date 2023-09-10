namespace Helmobilite.Models.ViewModels
{
	public class DeliveriesPerWeekModel
	{
		public int WeekOffset { get; set; }

		public IDictionary<DateTime, IList<Delivery>> DeliveriesPerDate { get; set; }
	}
}

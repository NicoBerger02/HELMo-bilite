using System.Linq;

namespace Helmobilite.Models.ViewModels
{
    public class StatisticsViewModel
    {
		public readonly List<DeliveryViewModel> Deliveries;

        public int CountDeliveries => Deliveries.Count;
        public int CountDoneDeliveries => Deliveries.Count(d => d.Status == Status.DONE);
		public int CountFailedDeliveries => Deliveries.Count(d => d.Status == Status.FAILED);
		public decimal DoneDeliveriesPercent => CountDeliveries == 0 ? 0 : (decimal)CountDoneDeliveries / (decimal)CountDeliveries * 100;
		public decimal FailedDeliveriesPercent => CountDeliveries == 0 ? 0 : (decimal)CountFailedDeliveries / (decimal)CountDeliveries * 100;

		public StatisticsViewModel(List<DeliveryViewModel> deliveries)
        {
            Deliveries = deliveries;
        }
    }
}

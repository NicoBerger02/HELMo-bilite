using Microsoft.AspNetCore.SignalR;

namespace Helmobilite.Models
{
	public class DeliveryHub : Hub
	{
		public async Task NotifyNewDeliveryEncoded()
		{
			await Clients.All.SendAsync("NewDeliveryEncoded");
		}
	}
}

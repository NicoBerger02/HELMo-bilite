namespace Helmobilite.Models
{
	public class ScheduleCalculator
	{
		public List<DateTime> CalculateDatesOfWeekByWeekOffset(int weekOffset) => GetDatesForWeekStartingAt(GetFirstDayOfWeekWithOffset(weekOffset));

		private List<DateTime> GetDatesForWeekStartingAt(DateTime firstDayOfWeek)
		{
			var dates = new List<DateTime>();
			for (int i = 0; i < 7; i++)
			{
				dates.Add(firstDayOfWeek.AddDays(i));
			}
			return dates;
		}

		private DateTime GetFirstDayOfWeekWithOffset(int weekOffset)
		{
			return DateTime.Today.AddDays(-((int)DateTime.Today.DayOfWeek - 1) + (weekOffset * 7));
		}

	}
}

using Helmobilite.Models;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Helmobilite.Models
{
	public static class CustomExtensions
	{
		public static string GetEnumDisplayName(this Enum enumType)
		{
			return enumType.GetType().GetMember(enumType.ToString())
						   .First()
						   .GetCustomAttribute<DisplayAttribute>()
						   ?.GetName() ?? enumType.ToString();
		}
	}
}

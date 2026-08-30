using System;
using System.Reflection;

namespace Billard3;

public class CompileInfo
{
	public static DateTime DateCompiled
	{
		get
		{
			DateTime dateTime = DateTime.Parse("1/1/2000");
			Version version = Assembly.GetExecutingAssembly().GetName().Version;
			dateTime = dateTime.AddDays(version.Build).AddSeconds(version.Revision * 2);
			if (TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now))
			{
				dateTime = dateTime.AddHours(1.0);
			}
			return dateTime;
		}
	}
}

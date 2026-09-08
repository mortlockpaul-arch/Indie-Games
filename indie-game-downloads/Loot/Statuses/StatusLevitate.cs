using System;
using System.IO;

namespace Loot.Statuses;

public class StatusLevitate : StatusDuration
{
	public StatusLevitate(TimeSpan duration)
		: base(duration)
	{
	}

	public StatusLevitate(BinaryReader reader)
		: base(reader)
	{
	}
}

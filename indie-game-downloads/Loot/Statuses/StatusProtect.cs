using System;
using System.IO;

namespace Loot.Statuses;

public class StatusProtect : StatusDuration
{
	public StatusProtect(TimeSpan duration)
		: base(duration)
	{
	}

	public StatusProtect(BinaryReader reader)
		: base(reader)
	{
	}
}

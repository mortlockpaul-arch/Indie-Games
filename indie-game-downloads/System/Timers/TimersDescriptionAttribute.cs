using System.ComponentModel;

namespace System.Timers;

[AttributeUsage(AttributeTargets.All)]
public class TimersDescriptionAttribute : DescriptionAttribute
{
	private bool _replaced;

	public override string Description
	{
		get
		{
			if (!_replaced)
			{
				_replaced = true;
				base.DescriptionValue = string.Format(base.Description, default(ReadOnlySpan<object>));
			}
			return base.Description;
		}
	}

	public TimersDescriptionAttribute(string description)
		: base(description)
	{
	}

	internal TimersDescriptionAttribute(TimersDescriptionStringId id)
		: base(GetResourceString(id))
	{
	}

	private static string GetResourceString(TimersDescriptionStringId id)
	{
		return id switch
		{
			TimersDescriptionStringId.TimerAutoReset => System.SR.TimerAutoReset, 
			TimersDescriptionStringId.TimerEnabled => System.SR.TimerEnabled, 
			TimersDescriptionStringId.TimerInterval => System.SR.TimerInterval, 
			TimersDescriptionStringId.TimerIntervalElapsed => System.SR.TimerIntervalElapsed, 
			TimersDescriptionStringId.TimerSynchronizingObject => System.SR.TimerSynchronizingObject, 
			_ => "", 
		};
	}
}

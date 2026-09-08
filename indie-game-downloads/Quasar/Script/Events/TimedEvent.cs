using System.Xml.Linq;
using Quasar.Global;

namespace Quasar.Script.Events;

internal class TimedEvent : ScriptEvent
{
	private long activationTime;

	private bool executed;

	public TimedEvent(ScriptInstance instance)
		: base(instance)
	{
	}

	public override void ParseXml(XElement xe)
	{
		base.ParseXml(xe);
		activationTime = XDocHelper.ParseLongAttribute(xe, "time");
	}

	public override void Update()
	{
		if (!executed && base.Instance.Timer.TotalTime >= activationTime)
		{
			Execute();
			executed = true;
		}
	}
}

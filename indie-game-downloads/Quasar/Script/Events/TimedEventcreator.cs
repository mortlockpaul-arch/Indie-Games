using System.Xml.Linq;

namespace Quasar.Script.Events;

public class TimedEventcreator : IScriptEventCreator
{
	public string Name => "Timed";

	public ScriptEvent Parse(ScriptInstance instance, XElement xe)
	{
		Quasar.Script.Events.TimedEvent timedEvent = new Quasar.Script.Events.TimedEvent(instance);
		timedEvent.ParseXml(xe);
		return timedEvent;
	}
}

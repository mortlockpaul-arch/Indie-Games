using System.Xml.Linq;

namespace Quasar.Script.Events;

public class OneShotEventcreator : IScriptEventCreator
{
	public string Name => "OneShot";

	public ScriptEvent Parse(ScriptInstance instance, XElement xe)
	{
		Quasar.Script.Events.OneShotEvent oneShotEvent = new Quasar.Script.Events.OneShotEvent(instance);
		oneShotEvent.ParseXml(xe);
		return oneShotEvent;
	}
}

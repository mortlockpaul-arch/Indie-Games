using System.Xml.Linq;

namespace Quasar.Script;

public interface IScriptEventCreator
{
	string Name { get; }

	ScriptEvent Parse(ScriptInstance instance, XElement xe);
}

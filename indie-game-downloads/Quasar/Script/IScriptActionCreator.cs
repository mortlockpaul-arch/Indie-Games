using System.Xml.Linq;

namespace Quasar.Script;

public interface IScriptActionCreator
{
	string Name { get; }

	ScriptAction Parse(ScriptEvent scriptEvent, XElement xe);
}

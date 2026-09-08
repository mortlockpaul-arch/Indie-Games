using System.Collections.Generic;
using System.Xml.Linq;

namespace Quasar.Script;

public class ScriptManager
{
	private Dictionary<string, IScriptActionCreator> actionCreators = new Dictionary<string, IScriptActionCreator>();

	private Dictionary<string, IScriptEventCreator> eventCreators = new Dictionary<string, IScriptEventCreator>();

	public ScriptManager()
	{
		Initialize();
	}

	private void Initialize()
	{
	}

	public void AddScriptActionCreator(string name, IScriptActionCreator creator)
	{
		actionCreators.Add(name, creator);
	}

	public void AddScriptEventCreator(string name, IScriptEventCreator creator)
	{
		eventCreators.Add(name, creator);
	}

	public ScriptEvent ParseScriptEvent(ScriptInstance instance, XElement xe)
	{
		if (eventCreators.TryGetValue(xe.Name.LocalName, out var value))
		{
			return value.Parse(instance, xe);
		}
		return null;
	}

	public ScriptAction ParseScriptAction(ScriptEvent scriptEvent, XElement xe)
	{
		if (actionCreators.TryGetValue(xe.Name.LocalName, out var value))
		{
			return value.Parse(scriptEvent, xe);
		}
		return null;
	}
}

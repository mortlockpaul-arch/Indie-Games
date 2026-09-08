using System.Collections.Generic;
using System.Xml.Linq;

namespace Quasar.Script;

public abstract class ScriptEvent
{
	private ScriptInstance instance;

	private List<ScriptAction> actions = new List<ScriptAction>();

	public ScriptInstance Instance => instance;

	public ScriptEvent(ScriptInstance instance)
	{
		this.instance = instance;
	}

	public void AddAction(ScriptAction action)
	{
		actions.Add(action);
	}

	public virtual void ParseXml(XElement xe)
	{
		foreach (XElement item in xe.Elements())
		{
			ScriptAction scriptAction = instance.Manager.ParseScriptAction(this, item);
			if (scriptAction != null)
			{
				actions.Add(scriptAction);
			}
		}
	}

	public abstract void Update();

	public virtual void Execute()
	{
		foreach (ScriptAction action in actions)
		{
			action.Execute();
			if (!action.IsInstantAction)
			{
				instance.AddScriptAction(action);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Quasar.ContentPipeline;
using Quasar.Global;

namespace Quasar.Script;

public class ScriptInstance
{
	private Timer timer = new Timer();

	private ScriptManager manager;

	private List<ScriptEvent> scriptEvents = new List<ScriptEvent>();

	private List<ScriptAction> activeActions = new List<ScriptAction>();

	public Timer Timer => timer;

	public ScriptManager Manager => manager;

	public ScriptInstance(string filename, ScriptManager manager)
	{
		this.manager = manager;
		LoadXml(filename);
	}

	private void LoadXml(string filename)
	{
		try
		{
			XmlSource xmlSource = Engine.ContentManager.Load<XmlSource>(filename);
			XDocument xDocument = XDocument.Parse(xmlSource.XmlCode);
			foreach (XElement item in xDocument.Root.Elements())
			{
				ScriptEvent scriptEvent = manager.ParseScriptEvent(this, item);
				if (scriptEvent != null)
				{
					scriptEvents.Add(scriptEvent);
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public void AddScriptAction(ScriptAction action)
	{
		activeActions.Add(action);
	}

	public virtual void Update()
	{
		timer.Clock();
		foreach (ScriptEvent scriptEvent in scriptEvents)
		{
			scriptEvent.Update();
		}
		for (int i = 0; i < activeActions.Count; i++)
		{
			ScriptAction scriptAction = activeActions[i];
			scriptAction.Update();
			if (scriptAction.IsFinished)
			{
				activeActions.RemoveAt(i);
			}
		}
	}
}

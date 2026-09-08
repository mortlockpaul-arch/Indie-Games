using System.Xml.Linq;

namespace Quasar.Script;

public abstract class ScriptAction
{
	private ScriptEvent scriptEvent;

	protected bool isInstantAction;

	protected bool isFinished;

	public ScriptEvent Event => scriptEvent;

	public bool IsInstantAction => isInstantAction;

	public bool IsFinished => isFinished;

	public ScriptAction(ScriptEvent scriptEvent, bool isInstantAction)
	{
		this.scriptEvent = scriptEvent;
		this.isInstantAction = isInstantAction;
	}

	public virtual void ParseXml(XElement xe)
	{
	}

	public abstract void Execute();

	public virtual void DoUpdate()
	{
	}

	public void Update()
	{
		if (!isFinished)
		{
			DoUpdate();
		}
	}
}

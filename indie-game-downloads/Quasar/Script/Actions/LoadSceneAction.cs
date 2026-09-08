using System.Xml.Linq;
using Quasar.Global;
using Quasar.Xml;

namespace Quasar.Script.Actions;

public class LoadSceneAction : ScriptAction
{
	private string filename;

	public LoadSceneAction(ScriptEvent scriptEvent)
		: base(scriptEvent, isInstantAction: true)
	{
	}

	public override void ParseXml(XElement xe)
	{
		base.ParseXml(xe);
		filename = XDocHelper.GetAttribute(xe, "filename");
	}

	public override void Execute()
	{
		SceneScriptInstance sceneScriptInstance = base.Event.Instance as SceneScriptInstance;
		Element e = ElementLoader.Load(filename);
		sceneScriptInstance.Scene.Add(e);
	}
}

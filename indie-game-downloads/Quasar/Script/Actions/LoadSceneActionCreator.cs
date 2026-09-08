using System.Xml.Linq;

namespace Quasar.Script.Actions;

public class LoadSceneActionCreator : IScriptActionCreator
{
	public string Name => "LoadScene";

	public ScriptAction Parse(ScriptEvent scriptEvent, XElement xe)
	{
		LoadSceneAction loadSceneAction = new LoadSceneAction(scriptEvent);
		loadSceneAction.ParseXml(xe);
		return loadSceneAction;
	}
}

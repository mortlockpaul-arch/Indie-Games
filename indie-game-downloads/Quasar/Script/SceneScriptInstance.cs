namespace Quasar.Script;

public class SceneScriptInstance : ScriptInstance
{
	private Scene scene;

	public Scene Scene => scene;

	public SceneScriptInstance(string filename, ScriptManager manager)
		: base(filename, manager)
	{
		scene = new Scene();
	}

	public override void Update()
	{
		base.Update();
		scene.Update();
	}
}

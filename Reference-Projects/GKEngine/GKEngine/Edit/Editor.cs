using GKEngine.Scenes;
using Microsoft.Xna.Framework;

namespace GKEngine.Edit;

public class Editor
{
	public Scene scene;

	public Editor(object oScene)
	{
	}

	public virtual void Init()
	{
	}

	public virtual void Render(GameTime oGameTime)
	{
	}

	public virtual void Load()
	{
	}

	public virtual void Activate()
	{
	}

	public virtual void Deactivate()
	{
	}

	public virtual void Update(GameTime oGameTime)
	{
	}
}

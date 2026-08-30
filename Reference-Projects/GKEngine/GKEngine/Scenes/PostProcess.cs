using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GKEngine.Scenes;

public class PostProcess
{
	public EntityStack entityStack;

	public Effect effect;

	public Scene scene;

	public bool active;

	public float amount = 1f;

	public Vector2 position;

	public PostProcess(EntityStack oEntityStack)
	{
		entityStack = oEntityStack;
		entityStack.Add(this);
		position = new Vector2(0f, 0f);
		scene = entityStack.scene;
	}

	public virtual void Load()
	{
	}

	public virtual void Unload()
	{
		entityStack = null;
		scene = null;
		effect = null;
		scene = null;
	}

	public virtual void Init()
	{
	}

	public virtual void Execute(GraphicsDevice oDevice, GameTime oGameTime)
	{
	}
}

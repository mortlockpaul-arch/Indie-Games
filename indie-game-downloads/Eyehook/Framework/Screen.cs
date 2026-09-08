using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Eyehook.Framework;

public abstract class Screen
{
	public readonly bool IsModal;

	protected SpriteBatch spriteBatch => MC.SpriteBatch;

	protected Rectangle viewportRect => MC.ScreenManager.viewportRect;

	public bool IsBackground { get; set; }

	protected Screen(bool modal)
	{
		IsModal = modal;
	}

	public virtual void initialize()
	{
	}

	public virtual void loadContent(ContentManager content)
	{
	}

	public virtual void transitionOn()
	{
	}

	public virtual void transitionOff()
	{
	}

	public virtual void unloadContent()
	{
	}

	public virtual void update(GameTime gameTime)
	{
	}

	public virtual void draw(GameTime gameTime)
	{
	}
}

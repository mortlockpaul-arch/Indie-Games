using Microsoft.Xna.Framework;

namespace OluXNA;

public class BaseComponent : DrawableGameComponent
{
	public BaseComponent(Game game)
		: base(game)
	{
	}

	public override void Initialize()
	{
		((DrawableGameComponent)this).Initialize();
	}

	public override void Update(GameTime gameTime)
	{
		BaseGame.Get().Update(gameTime);
		((GameComponent)this).Update(gameTime);
	}

	public override void Draw(GameTime gameTime)
	{
		BaseGame.Get().Draw(gameTime);
		((DrawableGameComponent)this).Draw(gameTime);
	}
}

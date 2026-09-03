using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OluXNA;

public class InstructionComponent : DrawableGameComponent
{
	private Texture2D screen;

	public InstructionComponent(Game game)
		: base(game)
	{
	}

	public override void Initialize()
	{
		screen = BaseGame.Get().content.Load<Texture2D>("Content\\instructions");
		((DrawableGameComponent)this).Initialize();
	}

	public override void Update(GameTime gameTime)
	{
		BaseGame.Get().input.Update();
		if (BaseGame.Get().input.PadPressed((Buttons)16) || BaseGame.Get().input.KeyPressed((Keys)13))
		{
			((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Add((IGameComponent)(object)new BaseComponent(((GameComponent)this).Game));
			((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Remove((IGameComponent)(object)this);
		}
		((GameComponent)this).Update(gameTime);
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().DrawFullscreenQuad(screen, BaseGame.WIDTH, BaseGame.HEIGHT, null, Color.White);
		((DrawableGameComponent)this).Draw(gameTime);
	}
}

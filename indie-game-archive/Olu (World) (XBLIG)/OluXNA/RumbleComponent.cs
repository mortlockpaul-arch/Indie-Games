using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace OluXNA;

internal class RumbleComponent : GameComponent
{
	public RumbleComponent(Game game)
		: base(game)
	{
	}

	public override void Update(GameTime gameTime)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (BaseGame.Get().paused || !BaseGame.Get().rumble)
		{
			GamePad.SetVibration(BaseGame.Get().input.ActivePlayerIndex, 0f, 0f);
		}
		else
		{
			GamePad.SetVibration(BaseGame.Get().input.ActivePlayerIndex, BaseGame.Get().channels[28], BaseGame.Get().channels[29]);
		}
		((GameComponent)this).Update(gameTime);
	}
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace OluXNA;

public class TrialModeCheck : GameComponent
{
	private bool IsTrialMode;

	public TrialModeCheck(Game game)
		: base(game)
	{
	}

	public override void Initialize()
	{
		IsTrialMode = Guide.IsTrialMode;
		((GameComponent)this).Initialize();
	}

	public override void Update(GameTime gameTime)
	{
		if (IsTrialMode != Guide.IsTrialMode)
		{
			BaseGame.Get().TrialModeSettings(Guide.IsTrialMode);
			IsTrialMode = Guide.IsTrialMode;
		}
		((GameComponent)this).Update(gameTime);
	}
}

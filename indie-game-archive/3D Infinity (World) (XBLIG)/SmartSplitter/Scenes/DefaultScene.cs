using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaLibrary;
using XnaLibrary.Input;

namespace SmartSplitter.Scenes;

public class DefaultScene : GameScene
{
	public DefaultScene(Game game)
		: base(game)
	{
		base.update += SceneUpdate;
		base.draw += SceneDraw;
	}

	public override void Initialize()
	{
		base.Initialize();
	}

	public override void Dispose()
	{
		base.Content.Unload();
		base.Dispose();
	}

	private void SceneUpdate(object sender, GameTime gameTime)
	{
		if (fadePhase != FadePhase.In)
		{
			if (fadePhase == FadePhase.Main)
			{
				UpdateMain(gameTime);
				return;
			}
			_ = fadePhase;
			_ = 2;
		}
	}

	private void UpdateMain(GameTime gameTime)
	{
		VirtualPadState virtualPadState = base.Input[(PlayerIndex)0];
		_ = virtualPadState.Buttons;
		_ = virtualPadState.ThumbSticks.Left;
		_ = virtualPadState.DPad;
	}

	private void SceneDraw(object sender, GameTime gameTime, SpriteBatch spriteBatch)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		base.GraphicsDevice.Clear(Color.CornflowerBlue);
	}
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaLibrary;
using XnaLibrary.Input;

namespace Infinity.Scenes;

public class DebugSelect : GameScene
{
	private GameScene[] nextScenes;

	private int selectIndex;

	private SpriteFont font;

	public DebugSelect(Game game)
		: base(game)
	{
		base.update += SceneUpdate;
		base.draw += SceneDraw;
	}

	public override void Initialize()
	{
		font = base.Content.Load<SpriteFont>("Fonts/DefaultFont");
		nextScenes = new GameScene[4]
		{
			new Logo(base.Game),
			new Title(base.Game),
			new MainGameLoader(base.Game),
			new TrialDemo(base.Game)
		};
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
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		VirtualPadState virtualPadState = base.Input[Global.CurrentPlayer];
		VirtualPadButtons buttons = virtualPadState.Buttons;
		VirtualPadDPad left = virtualPadState.ThumbSticks.Left;
		VirtualPadDPad dPad = virtualPadState.DPad;
		if (InputState.IsPush(left.Up) || InputState.IsPush(dPad.Up))
		{
			selectIndex = (selectIndex + (nextScenes.Length - 1)) % nextScenes.Length;
		}
		if (InputState.IsPush(left.Down) || InputState.IsPush(dPad.Down))
		{
			selectIndex = (selectIndex + 1) % nextScenes.Length;
		}
		if (InputState.IsPush(buttons.A))
		{
			base.SceneManager.AddScene(nextScenes[selectIndex]);
			FadeOut();
		}
	}

	private void SceneDraw(object sender, GameTime gameTime, SpriteBatch spriteBatch)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		base.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin();
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(100f, 100f);
		for (int i = 0; i < nextScenes.Length; i++)
		{
			Color val2 = ((i == selectIndex) ? Color.Red : Color.White);
			spriteBatch.DrawString(font, nextScenes[i].ToString(), val, val2);
			val.Y += (float)font.LineSpacing;
		}
		spriteBatch.End();
	}
}

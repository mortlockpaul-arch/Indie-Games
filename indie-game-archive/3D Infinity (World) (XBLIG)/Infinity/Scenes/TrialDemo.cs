using System;
using System.Collections.ObjectModel;
using Infinity.Messages;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using XnaLibrary;
using XnaLibrary.Blade;
using XnaLibrary.Input;

namespace Infinity.Scenes;

public class TrialDemo : AnaglyphScene
{
	private Texture2D image;

	private SpriteBatch spriteBatch;

	private MessageBoxComponent messageBox;

	public TrialDemo(Game game)
		: base(game)
	{
		base.update += SceneUpdate;
		base.draw += SceneDraw;
	}

	public override void Initialize()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		image = base.Content.Load<Texture2D>("Textures/image_locked");
		messageBox = new MessageBoxComponent(base.Game);
		((Collection<IGameComponent>)(object)base.Game.Components).Add((IGameComponent)(object)messageBox);
		spriteBatch = new SpriteBatch(base.Game.GraphicsDevice);
		base.Initialize();
	}

	public override void Dispose()
	{
		base.Content.Unload();
		((GameComponent)messageBox).Dispose();
		spriteBatch.Dispose();
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
		//IL_006a: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		VirtualPadState virtualPadState = base.Input[Global.CurrentPlayer];
		VirtualPadButtons buttons = virtualPadState.Buttons;
		_ = virtualPadState.ThumbSticks.Left;
		_ = virtualPadState.DPad;
		if (!Guide.IsTrialMode)
		{
			base.SceneManager.AddScene(new Title(base.Game, Title.Phase.SelectMenu));
			FadeOut();
			return;
		}
		if (InputState.IsPush(buttons.X))
		{
			try
			{
				Guide.ShowMarketplace(Global.CurrentPlayer);
				return;
			}
			catch (GamerPrivilegeException ex)
			{
				GamerPrivilegeException ex2 = ex;
				messageBox.ShowMessageBox(Global.CurrentPlayer, " ", ((Exception)(object)ex2).Message, new string[1] { UIMessage.Yes }, 0, (MessageBoxIcon)1);
				return;
			}
		}
		if (InputState.IsPush(buttons.A) || InputState.IsPush(buttons.Start))
		{
			base.SceneManager.AddScene(new Title(base.Game, Title.Phase.SelectMenu));
			FadeOut();
		}
	}

	protected override void FadeInFinished(object sender, EventArgs e)
	{
		base.FadeInFinished(sender, e);
	}

	private void SceneDraw(object sender, GameTime gameTime, SpriteBatch spriteBatch)
	{
		anaglyphRender.Draw(gameTime, base.SASData);
	}

	protected override void DrawScene(GameTime gameTime)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		spriteBatch.Begin();
		spriteBatch.Draw(image, Vector2.Zero, Color.White);
		spriteBatch.End();
		base.DrawScene(gameTime);
	}
}

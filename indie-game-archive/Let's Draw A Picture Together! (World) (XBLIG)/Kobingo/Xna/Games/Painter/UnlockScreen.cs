using System;
using Kobingo.Xna.Library.Common;
using Kobingo.Xna.Library.Game;
using Kobingo.Xna.Library.Graphics;
using Kobingo.Xna.Library.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Kobingo.Xna.Games.Painter;

internal class UnlockScreen : MessageScreen
{
	private TickTimer UnlockTimer;

	public bool UnlockVisible { get; set; }

	public UnlockScreen(ScreenManager screenManager)
		: base(screenManager, "Draw together with friends over Xbox LIVE", "Save pictures to gallery for viewing and loading", "Get access to all the colors available", "Unlock the very useful function to undo/redo")
	{
		UnlockTimer = new TickTimer(TimeSpan.FromSeconds(0.4000000059604645));
		TickTimer unlockTimer = UnlockTimer;
		EventHandler value = delegate
		{
			UnlockVisible = !UnlockVisible;
		};
		unlockTimer.Tick += value;
	}

	public override void HandleInput()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (ScreenInput.Back || ScreenInput.Start)
		{
			Close();
		}
		if (GameManager.ActiveGamer.Privileges.AllowPurchaseContent && GamepadManager.IsButtonPressed((Buttons)16384))
		{
			Guide.ShowMarketplace(GameManager.ActiveGamer.PlayerIndex);
		}
		base.HandleInput();
	}

	public override void Update(GameTime gameTime, bool active)
	{
		UnlockTimer.Update(gameTime);
		if (!Guide.IsTrialMode)
		{
			Close();
		}
		base.Update(gameTime, active);
	}

	public override void Draw(GameTime gameTime, float transition)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		base.ScreenManager.SpriteBatch.Begin();
		base.ScreenManager.SpriteBatch.DrawAligned(Graphics.UnlockBack, base.ScreenManager.ScreenCenter, Align.Center, Color.White);
		base.ScreenManager.SpriteBatch.DrawAlignedString(Fonts.HeaderFont, "Unlock Full Version", GameManager.ScreenManager.ScreenCenter - new Vector2(0f, 230f), Align.Center, Color.Black);
		if (GameManager.ActiveGamer.Privileges.AllowPurchaseContent)
		{
			base.ScreenManager.SpriteBatch.DrawAligned(Graphics.ButtonX, base.ScreenManager.ScreenCenter + new Vector2(-110f, 228f), Align.Center, Color.White);
			base.ScreenManager.SpriteBatch.DrawAlignedString(Fonts.DefaultFont, "Press      to unlock full version now", GameManager.ScreenManager.ScreenCenter + new Vector2(0f, 230f), Align.Center, Color.Black);
		}
		else
		{
			base.ScreenManager.SpriteBatch.DrawAlignedString(Fonts.DefaultFont, "Gamer profile is not allowed to purchase content", GameManager.ScreenManager.ScreenCenter + new Vector2(0f, 230f), Align.Center, Color.Black);
		}
		base.ScreenManager.SpriteBatch.End();
		base.Draw(gameTime, transition);
	}
}

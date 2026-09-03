using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OluXNA;

internal class SignBackInComponent : DrawableGameComponent
{
	private string text;

	private Vector2 textPos;

	private string buttonXText;

	private string buttonYText;

	private Vector2 buttonXPos;

	private Vector2 buttonYPos;

	private Vector2 buttonXOrig;

	private Vector2 buttonYOrig;

	private Vector2 buttonOffset;

	public static string origGamerTag = "";

	public SignBackInComponent(Game game)
		: base(game)
	{
	}

	public SignBackInComponent(Game game, string gamertag)
		: this(game, gamertag, "You have signed out.  If you do not sign back in, you will lose your progress.")
	{
	}

	public SignBackInComponent(Game game, string gamertag, string _text)
		: this(game)
	{
		origGamerTag = gamertag;
		text = _text;
	}

	public override void Initialize()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		text = BaseGame.WrapString(text, 0.8f * (float)BaseGame.WIDTH, 1f, BaseGame.Get().hud.HUDfont);
		textPos = new Vector2((float)BaseGame.WIDTH / 2f, (float)BaseGame.HEIGHT / 2f);
		textPos -= BaseGame.Get().hud.HUDfont.MeasureString(text) / 2f;
		buttonOffset = new Vector2(0f, BaseGame.Get().hud.HUDfont.MeasureString("Sign InQuit").Y / 2f);
		buttonXText = BaseGame.Get().hud.KeyMap[(Buttons)16384];
		buttonXPos = new Vector2((float)BaseGame.WIDTH * 0.3f, (float)BaseGame.HEIGHT * 0.7f);
		buttonXOrig = BaseGame.Get().hud.ControllerFont.MeasureString(buttonXText);
		buttonXOrig.X /= 2f;
		buttonXOrig.Y /= 2f;
		buttonYText = BaseGame.Get().hud.KeyMap[(Buttons)32768];
		buttonYPos = new Vector2((float)BaseGame.WIDTH * 0.3f, (float)BaseGame.HEIGHT * 0.8f);
		buttonYOrig = BaseGame.Get().hud.ControllerFont.MeasureString(buttonYText);
		buttonYOrig.X /= 2f;
		buttonYOrig.Y /= 2f;
		((DrawableGameComponent)this).Initialize();
	}

	public override void Update(GameTime gameTime)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		BaseGame.Get().input.Update();
		if (((GameComponent)this).Game.IsActive)
		{
			if (BaseGame.Get().input.PadPressed((Buttons)16384))
			{
				Guide.ShowSignIn(1, false);
			}
			else if (BaseGame.Get().input.PadPressed((Buttons)32768))
			{
				BaseGame.Get().continueWithoutSaving = true;
				foreach (GameComponent item in (Collection<IGameComponent>)(object)((GameComponent)this).Game.Components)
				{
					GameComponent val = item;
					if (!(val is GamerServicesComponent))
					{
						val.Enabled = true;
					}
				}
				for (int num = ((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Count - 1; num >= 0; num--)
				{
					if (((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components)[num] is SignBackInComponent)
					{
						((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).RemoveAt(num);
					}
				}
			}
		}
		((GameComponent)this).Update(gameTime);
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		SpriteBatch spriteBatch = BaseGame.Get().spriteBatch;
		spriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)3, (SaveStateMode)0);
		spriteBatch.Draw(BaseGame.Get().hud.blackTex, new Rectangle(0, 0, BaseGame.WIDTH, BaseGame.HEIGHT), (Rectangle?)null, new Color(1f, 1f, 1f, 0.6f), 0f, Vector2.Zero, (SpriteEffects)0, 0.01f);
		spriteBatch.DrawString(BaseGame.Get().hud.HUDfont, text, textPos, Color.White);
		spriteBatch.DrawString(BaseGame.Get().hud.HUDfont, "Sign in", new Vector2((float)BaseGame.WIDTH * 0.35f, (float)BaseGame.HEIGHT * 0.7f), Color.White, 0f, buttonOffset, 1f, (SpriteEffects)0, 0f);
		spriteBatch.DrawString(BaseGame.Get().hud.HUDfont, "Continue without saving", new Vector2((float)BaseGame.WIDTH * 0.35f, (float)BaseGame.HEIGHT * 0.8f), Color.White, 0f, buttonOffset, 1f, (SpriteEffects)0, 0f);
		spriteBatch.DrawString(BaseGame.Get().hud.ControllerFont, buttonXText, buttonXPos, Color.White, 0f, buttonXOrig, HUD.textScale, (SpriteEffects)0, 0f);
		spriteBatch.DrawString(BaseGame.Get().hud.ControllerFont, buttonYText, buttonYPos, Color.White, 0f, buttonYOrig, HUD.textScale, (SpriteEffects)0, 0f);
		spriteBatch.End();
		((DrawableGameComponent)this).Draw(gameTime);
	}
}

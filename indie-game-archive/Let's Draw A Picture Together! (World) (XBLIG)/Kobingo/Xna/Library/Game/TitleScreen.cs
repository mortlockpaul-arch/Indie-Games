using System;
using Kobingo.Xna.Library.Data;
using Kobingo.Xna.Library.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Kobingo.Xna.Library.Game;

public class TitleScreen : GameScreen
{
	public MainMenu MainMenu { get; set; }

	public LoadingScreen LoadingScreen { get; set; }

	private bool IsSigningIn { get; set; }

	public TitleScreen(ScreenManager screenManager)
		: base(screenManager)
	{
		MainMenu = new MainMenu(screenManager);
		LoadingScreen = new LoadingScreen(screenManager);
		LoadingScreen loadingScreen = LoadingScreen;
		EventHandler value = delegate
		{
			OnLoadingCompleted();
		};
		loadingScreen.Completed += value;
	}

	public override void HandleInput()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Invalid comparison between Unknown and I4
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if ((ScreenInput.Start || ScreenInput.Select) && base.ScreenManager.ActiveScreen == this)
		{
			StorageManager.Reset();
			GameManager.ActiveGamer = null;
			for (PlayerIndex val = (PlayerIndex)0; (int)val < 3; val = (PlayerIndex)(val + 1))
			{
				if (!ScreenInput.IsStart(val) && !ScreenInput.IsSelect(val))
				{
					continue;
				}
				GamerCollectionEnumerator<SignedInGamer> enumerator = ((GamerCollection<SignedInGamer>)(object)Gamer.SignedInGamers).GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						SignedInGamer current = enumerator.Current;
						if (current.PlayerIndex == val)
						{
							GameManager.ActiveGamer = current;
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
				}
			}
			if (GameManager.ActiveGamer == null)
			{
				IsSigningIn = true;
				Guide.ShowSignIn(1, false);
			}
			else
			{
				OnLoading();
			}
		}
		base.HandleInput();
	}

	protected virtual void OnLoading()
	{
		if (LoadingScreen != null)
		{
			LoadingScreen.Show();
		}
	}

	protected virtual void OnLoadingCompleted()
	{
		MainMenu.Show();
		LoadingScreen.Close();
	}

	public override void Draw(GameTime gameTime, float transition)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		base.ScreenManager.SpriteBatch.Begin((SpriteBlendMode)1);
		if (GameManager.Font != null)
		{
			base.ScreenManager.SpriteBatch.DrawAlignedString(GameManager.Font, GameManager.Title, base.ScreenManager.ScreenCenter, Align.Center, Color.White);
		}
		base.ScreenManager.SpriteBatch.End();
		base.Draw(gameTime, transition);
	}

	public override void Close()
	{
	}
}

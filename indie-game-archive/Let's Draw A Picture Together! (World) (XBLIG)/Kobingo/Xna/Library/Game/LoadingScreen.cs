using System;
using Kobingo.Xna.Library.Data;
using Kobingo.Xna.Library.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;

namespace Kobingo.Xna.Library.Game;

public class LoadingScreen : GameScreen
{
	public static SpriteFont Font { get; set; }

	public bool LoadPlayerStorage { get; set; }

	private LoadingEventArgs LoadingEventArgs { get; set; }

	public event EventHandler Completed;

	public event EventHandler<LoadingEventArgs> Loading;

	public LoadingScreen(ScreenManager screenManager)
		: base(screenManager)
	{
		LoadingEventArgs = new LoadingEventArgs();
	}

	public override void Draw(GameTime gameTime, float transition)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		base.Draw(gameTime, transition);
		SpriteFont font = GameManager.Font;
		if (Font != null)
		{
			font = Font;
		}
		if (font != null)
		{
			GameManager.ScreenManager.SpriteBatch.Begin((SpriteBlendMode)1);
			GameManager.ScreenManager.SpriteBatch.DrawAlignedString(font, "Loading", GameManager.ScreenManager.ScreenCenter, Align.Center, Color.Black);
			GameManager.ScreenManager.SpriteBatch.End();
		}
	}

	public override void Show()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (LoadPlayerStorage)
		{
			StorageManager.PerformOperation(GameManager.ActiveGamer.PlayerIndex, OnLoading);
		}
		else
		{
			StorageManager.PerformOperation(OnLoading);
		}
		base.Show();
	}

	private void OnLoading(StorageContainer container)
	{
		if (container == null)
		{
			Close();
			return;
		}
		DoLoading(container);
		if (Loading != null)
		{
			LoadingEventArgs.Container = container;
			Loading(this, LoadingEventArgs);
		}
		OnCompleted();
	}

	protected virtual void DoLoading(StorageContainer container)
	{
		GameManager.Settings.Load(container);
		GameManager.Highscores.Load(container);
	}

	protected virtual void OnCompleted()
	{
		if (Completed != null)
		{
			Completed(this, EventArgs.Empty);
		}
	}
}

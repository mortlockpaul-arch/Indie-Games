using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kobingo.Xna.Library.Game;

public class ScreenManager : DrawableGameComponent
{
	public const float TITLESAFE_PERCENT = 0.8f;

	[CompilerGenerated]
	private Vector2 _003CScreenCenter_003Ek__BackingField;

	[CompilerGenerated]
	private Rectangle _003CTitleSafeArea_003Ek__BackingField;

	protected List<GameScreen> Screens { get; set; }

	public GameScreen ActiveScreen
	{
		get
		{
			if (Screens.Count == 0)
			{
				return null;
			}
			return Screens[Screens.Count - 1];
		}
	}

	public SpriteBatch SpriteBatch { get; private set; }

	public Vector2 ScreenCenter
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CScreenCenter_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CScreenCenter_003Ek__BackingField = value;
		}
	}

	public Rectangle TitleSafeArea
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CTitleSafeArea_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CTitleSafeArea_003Ek__BackingField = value;
		}
	}

	public ScreenManager(Game game)
		: base(game)
	{
		Screens = new List<GameScreen>();
	}

	protected override void LoadContent()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		SpriteBatch = new SpriteBatch(((GameComponent)this).Game.GraphicsDevice);
		Viewport viewport = ((GameComponent)this).Game.GraphicsDevice.Viewport;
		ScreenCenter = new Vector2((float)(((Viewport)(ref viewport)).Width / 2), (float)(((Viewport)(ref viewport)).Height / 2));
		Point val = default(Point);
		((Point)(ref val))._002Ector((int)((float)((Viewport)(ref viewport)).Width * 0.8f), (int)((float)((Viewport)(ref viewport)).Height * 0.8f));
		TitleSafeArea = new Rectangle((int)ScreenCenter.X - val.X / 2, (int)ScreenCenter.Y - val.Y / 2, val.X, val.Y);
		((DrawableGameComponent)this).LoadContent();
	}

	public override void Update(GameTime gameTime)
	{
		GameScreen[] array = Screens.ToArray();
		bool flag = ((GameComponent)this).Game.IsActive;
		for (int num = array.Length - 1; num >= 0; num--)
		{
			if (flag)
			{
				array[num].HandleInput();
			}
			array[num].Update(gameTime, flag);
			flag = false;
		}
		((GameComponent)this).Update(gameTime);
	}

	public override void Draw(GameTime gameTime)
	{
		for (int i = 0; i < Screens.Count; i++)
		{
			if (i < Screens.Count - 1 && Screens[i + 1].IsPopup)
			{
				Screens[i].Draw(gameTime, 1f);
			}
			if (i == Screens.Count - 1)
			{
				Screens[i].Draw(gameTime, 1f);
			}
		}
		((DrawableGameComponent)this).Draw(gameTime);
	}

	public void Add(GameScreen screen)
	{
		Screens.Add(screen);
	}

	public void Remove(GameScreen screen)
	{
		Screens.Remove(screen);
	}

	public void CloseAll()
	{
		for (int num = Screens.Count - 1; num >= 0; num--)
		{
			Screens[num].Close();
		}
	}
}

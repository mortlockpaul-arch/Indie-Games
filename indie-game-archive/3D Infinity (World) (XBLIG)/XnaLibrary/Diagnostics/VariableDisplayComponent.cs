using System;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XnaLibrary.Diagnostics;

public class VariableDisplayComponent : DrawableGameComponent
{
	private ContentManager contentManager;

	private SpriteBatch spriteBatch;

	private Texture2D texture;

	private SpriteFont font;

	private readonly Vector2[] ShadowTable;

	private int workFps;

	private TimeSpan workFpsTime;

	private StringBuilder workString;

	[CompilerGenerated]
	private Color _003CBackgroundColor_003Ek__BackingField;

	[CompilerGenerated]
	private Color _003CKeyColor_003Ek__BackingField;

	[CompilerGenerated]
	private Color _003CValueColor_003Ek__BackingField;

	[CompilerGenerated]
	private Color _003CShadowColor_003Ek__BackingField;

	[CompilerGenerated]
	private Vector2 _003CPosition_003Ek__BackingField;

	public Color BackgroundColor
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CBackgroundColor_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CBackgroundColor_003Ek__BackingField = value;
		}
	}

	public Color KeyColor
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CKeyColor_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CKeyColor_003Ek__BackingField = value;
		}
	}

	public Color ValueColor
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CValueColor_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CValueColor_003Ek__BackingField = value;
		}
	}

	public Color ShadowColor
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CShadowColor_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CShadowColor_003Ek__BackingField = value;
		}
	}

	public Vector2 Position
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CPosition_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CPosition_003Ek__BackingField = value;
		}
	}

	public StringBuilder Text { get; private set; }

	public int FPS { get; private set; }

	public VariableDisplayComponent(Game game)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		ShadowTable = (Vector2[])(object)new Vector2[4]
		{
			new Vector2(0f, -1f),
			new Vector2(0f, 1f),
			new Vector2(-1f, 0f),
			new Vector2(1f, 0f)
		};
		workString = new StringBuilder();
		((DrawableGameComponent)this)._002Ector(game);
		contentManager = new ContentManager((IServiceProvider)game.Services);
		contentManager.RootDirectory = "Content";
		Text = new StringBuilder();
		Position = new Vector2(64f, 36f);
		BackgroundColor = new Color((byte)0, (byte)0, (byte)0, (byte)64);
		KeyColor = Color.White;
		ValueColor = Color.White;
		ShadowColor = Color.Black;
		workFps = 0;
		workFpsTime = TimeSpan.Zero;
	}

	public override void Initialize()
	{
		((DrawableGameComponent)this).Initialize();
	}

	protected override void LoadContent()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		spriteBatch = new SpriteBatch(((DrawableGameComponent)this).GraphicsDevice);
		texture = new Texture2D(((DrawableGameComponent)this).GraphicsDevice, 1, 1, 1, (TextureUsage)0, (SurfaceFormat)1);
		texture.SetData<Color>((Color[])(object)new Color[1] { Color.White });
		font = contentManager.Load<SpriteFont>("Debug/DebugFont");
		((DrawableGameComponent)this).LoadContent();
	}

	protected override void UnloadContent()
	{
		spriteBatch.Dispose();
		((GraphicsResource)texture).Dispose();
		contentManager.Unload();
		contentManager.Dispose();
		((DrawableGameComponent)this).UnloadContent();
	}

	public override void Update(GameTime gameTime)
	{
		((GameComponent)this).Update(gameTime);
	}

	private void UpdateFPS(GameTime gameTime)
	{
		workFpsTime += gameTime.ElapsedGameTime;
		workFps++;
		if (workFpsTime.TotalSeconds >= 1.0)
		{
			FPS = workFps;
			workFps = 0;
			workFpsTime = TimeSpan.Zero;
		}
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		UpdateFPS(gameTime);
		spriteBatch.Begin();
		workString.Remove(0, workString.Length);
		workString.AppendFormat("FPS:{0}", new object[1] { FPS });
		if (Text.Length > 0)
		{
			workString.AppendFormat("\n{0}", new object[1] { Text });
		}
		Vector2 val = font.MeasureString(workString);
		spriteBatch.Draw(texture, new Rectangle
		{
			X = (int)Position.X,
			Y = (int)Position.Y,
			Width = (int)val.X,
			Height = (int)val.Y
		}, BackgroundColor);
		Vector2[] shadowTable = ShadowTable;
		foreach (Vector2 val2 in shadowTable)
		{
			spriteBatch.DrawString(font, workString, Position + val2, ShadowColor);
		}
		spriteBatch.DrawString(font, workString, Position, KeyColor);
		spriteBatch.End();
		((DrawableGameComponent)this).Draw(gameTime);
	}

	public void ClearText()
	{
		Text.Remove(0, Text.Length);
	}
}

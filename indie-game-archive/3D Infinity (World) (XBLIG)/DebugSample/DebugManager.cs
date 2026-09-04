using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DebugSample;

public class DebugManager : DrawableGameComponent
{
	public ContentManager Content { get; private set; }

	public SpriteBatch SpriteBatch { get; private set; }

	public Texture2D WhiteTexture { get; private set; }

	public SpriteFont DebugFont { get; private set; }

	public DebugManager(Game game)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		((DrawableGameComponent)this)._002Ector(game);
		((GameComponent)this).Game.Services.AddService(typeof(DebugManager), (object)this);
		Content = new ContentManager((IServiceProvider)game.Services);
		Content.RootDirectory = "Content/Debug";
		((GameComponent)this).Enabled = false;
		((DrawableGameComponent)this).Visible = false;
	}

	protected override void LoadContent()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		SpriteBatch = new SpriteBatch(((DrawableGameComponent)this).GraphicsDevice);
		DebugFont = Content.Load<SpriteFont>("DebugFont");
		WhiteTexture = new Texture2D(((DrawableGameComponent)this).GraphicsDevice, 1, 1);
		Color[] data = (Color[])(object)new Color[1] { Color.White };
		WhiteTexture.SetData<Color>(data);
		((DrawableGameComponent)this).LoadContent();
	}
}

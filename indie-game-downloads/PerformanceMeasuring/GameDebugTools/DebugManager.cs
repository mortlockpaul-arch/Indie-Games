using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PerformanceMeasuring.GameDebugTools;

public class DebugManager : DrawableGameComponent
{
	private string debugFont;

	public SpriteBatch SpriteBatch { get; private set; }

	public Texture2D WhiteTexture { get; private set; }

	public SpriteFont DebugFont { get; private set; }

	public DebugManager(Game game, string debugFont)
		: base(game)
	{
		base.Game.Services.AddService(typeof(DebugManager), this);
		this.debugFont = debugFont;
		base.Enabled = false;
		base.Visible = false;
	}

	protected override void LoadContent()
	{
		SpriteBatch = new SpriteBatch(base.GraphicsDevice);
		DebugFont = base.Game.Content.Load<SpriteFont>(debugFont);
		WhiteTexture = new Texture2D(base.GraphicsDevice, 1, 1);
		Color[] data = new Color[1] { Color.White };
		WhiteTexture.SetData(data);
		base.LoadContent();
	}
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus.DebugTools;

public class DebugManager : DrawableGameComponent
{
	public SpriteFont debugFont;

	private SpriteBatch overridenSpriteBatch;

	public SpriteBatch SpriteBatch { get; private set; }

	public Texture2D WhiteTexture { get; private set; }

	public void SBBegin()
	{
		if (overridenSpriteBatch == null)
		{
			SpriteBatch.Begin();
		}
	}

	public void SBEnd()
	{
		if (overridenSpriteBatch == null)
		{
			SpriteBatch.End();
		}
	}

	public DebugManager(Game game, SpriteFont font, SpriteBatch SB)
		: base(game)
	{
		base.Game.Services.AddService(typeof(DebugManager), this);
		debugFont = font;
		base.Enabled = false;
		base.Visible = false;
		overridenSpriteBatch = SB;
	}

	protected override void LoadContent()
	{
		SpriteBatch = ((overridenSpriteBatch == null) ? new SpriteBatch(base.GraphicsDevice) : overridenSpriteBatch);
		WhiteTexture = new Texture2D(base.GraphicsDevice, 1, 1);
		Color[] data = new Color[1] { Color.White };
		WhiteTexture.SetData(data);
		base.LoadContent();
	}
}

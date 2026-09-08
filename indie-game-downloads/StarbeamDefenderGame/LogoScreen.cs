using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace StarbeamDefenderGame;

public class LogoScreen
{
	private int timeout;

	private Texture2D background;

	public void LoadContent(ContentManager content, string background)
	{
		this.background = content.Load<Texture2D>(background);
		timeout = 0;
	}

	public int DoGameUpdate(int timems)
	{
		timeout += timems;
		if (timeout > 2000)
		{
			return 1;
		}
		return 0;
	}

	public void Draw(SpriteBatch spritebatch)
	{
		Rectangle value = new Rectangle(0, 0, 1280, 720);
		spritebatch.Draw(destinationRectangle: new Rectangle(0, 0, 1280, 720), texture: background, sourceRectangle: value, color: Color.White);
	}
}

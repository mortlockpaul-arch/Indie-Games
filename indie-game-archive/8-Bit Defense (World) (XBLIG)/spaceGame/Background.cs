using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace spaceGame;

internal class Background : Sprite
{
	private Texture2D whiteRectangle;

	private Game1 theGame;

	public Background(Game1 getGame)
	{
		theGame = getGame;
		Position = new Vector2(384f, 0f);
	}

	public void LoadContent(ContentManager theContentManager)
	{
		whiteRectangle = new Texture2D(theGame.GraphicsDevice, 1, 1);
		whiteRectangle.SetData(new Color[1] { Color.White });
		LoadContent(theContentManager, "Background1");
		base.Scale = 1f;
	}

	public void UnloadContent()
	{
		whiteRectangle.Dispose();
	}

	public override void Draw(SpriteBatch theSpriteBatch)
	{
		base.Draw(theSpriteBatch);
	}
}

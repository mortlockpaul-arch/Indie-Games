using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace spaceGame;

public class Barricade : Sprite
{
	private Game1 theGame;

	private bool bDestroyed;

	private int blockCount;

	public BarricadeBlock[] BlockArray = new BarricadeBlock[8];

	public Barricade(Game1 getGame, Vector2 position)
	{
		theGame = getGame;
		Position = position;
		bDestroyed = false;
		blockCount = 8;
		int i = 0;
		Vector2 position2 = Position;
		Vector2 pos = position2;
		for (; i != blockCount; i++)
		{
			BlockArray[i] = new BarricadeBlock(theGame, pos);
			pos += new Vector2(16f, 0f);
			if (i == 3)
			{
				pos.X = position2.X;
				pos.Y += 16f;
			}
		}
	}

	public void LoadContent(ContentManager theContentManager)
	{
		BarricadeBlock[] blockArray = BlockArray;
		foreach (BarricadeBlock barricadeBlock in blockArray)
		{
			barricadeBlock.LoadContent(theContentManager);
		}
	}

	public void UnloadContent()
	{
	}

	public void Restore()
	{
		BarricadeBlock[] blockArray = BlockArray;
		foreach (BarricadeBlock barricadeBlock in blockArray)
		{
			barricadeBlock.RestoreHealth();
		}
	}

	public void Destroy()
	{
		bDestroyed = true;
	}

	public bool GetDestroyed()
	{
		return bDestroyed;
	}

	public override void Draw(SpriteBatch theSpriteBatch)
	{
		BarricadeBlock[] blockArray = BlockArray;
		foreach (BarricadeBlock barricadeBlock in blockArray)
		{
			barricadeBlock.Draw(theSpriteBatch);
		}
	}
}

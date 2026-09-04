using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace spaceGame;

public class BarricadeBlock : AnimatedSprite
{
	private Game1 theGame;

	private Color myColor;

	private int iHealth;

	private int iMaxHealth;

	private bool bDestroyed;

	public BarricadeBlock(Game1 getGame, Vector2 pos)
	{
		theGame = getGame;
		myColor = Color.White;
		Position = pos;
		iHealth = 3;
		iMaxHealth = iHealth;
		bDestroyed = false;
	}

	public void LoadContent(ContentManager theContentManager)
	{
		LoadContent(theContentManager, "BarricadeBlock");
		SetupAnimatedSprite(1, 4, 3, 4, 1f);
	}

	public void UnloadContent()
	{
	}

	public void Destroy()
	{
		bDestroyed = true;
	}

	public void RestoreHealth()
	{
		iHealth = iMaxHealth;
		ChangeHealth(0);
		bDestroyed = false;
	}

	public void ChangeHealth(int diff)
	{
		iHealth += diff;
		SetFrame(iHealth);
		if (iHealth <= 0)
		{
			Destroy();
		}
	}

	public int GetHealth()
	{
		return iHealth;
	}

	public bool GetDestroyed()
	{
		return bDestroyed;
	}

	public void Draw(SpriteBatch theSpriteBatch)
	{
		if (!GetDestroyed())
		{
			base.Draw(theSpriteBatch, myColor);
		}
	}
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace spaceGame;

public class Sprite
{
	public string AssetName;

	public Rectangle Size;

	private float mScale = 1f;

	public Vector2 Position = new Vector2(0f, 0f);

	private Texture2D mSpriteTexture;

	public float Scale
	{
		get
		{
			return mScale;
		}
		set
		{
			mScale = value;
			Size = new Rectangle(0, 0, (int)((float)mSpriteTexture.Width * Scale), (int)((float)mSpriteTexture.Height * Scale));
		}
	}

	public void LoadContent(ContentManager theContentManager, string theASsetName)
	{
		mSpriteTexture = theContentManager.Load<Texture2D>(theASsetName);
		AssetName = theASsetName;
		Size = new Rectangle(0, 0, (int)((float)mSpriteTexture.Width * Scale), (int)((float)mSpriteTexture.Height * Scale));
	}

	public void Update(GameTime theGameTime, Vector2 velocity)
	{
		Position += velocity * (float)theGameTime.ElapsedGameTime.TotalSeconds;
	}

	public virtual void Draw(SpriteBatch theSpriteBatch)
	{
		Vector2 position = Position;
		position.X -= 2f;
		position.Y -= 2f;
		theSpriteBatch.Draw(mSpriteTexture, position, new Rectangle(0, 0, mSpriteTexture.Width, mSpriteTexture.Height), Color.Gray, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
		theSpriteBatch.Draw(mSpriteTexture, Position, new Rectangle(0, 0, mSpriteTexture.Width, mSpriteTexture.Height), Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
	}

	public virtual void Draw(SpriteBatch theSpriteBatch, Color theColor)
	{
		theSpriteBatch.Draw(mSpriteTexture, Position, new Rectangle(-2, -2, mSpriteTexture.Width, mSpriteTexture.Height), Color.Gray, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
		theSpriteBatch.Draw(mSpriteTexture, Position, new Rectangle(0, 0, mSpriteTexture.Width, mSpriteTexture.Height), theColor, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
	}
}

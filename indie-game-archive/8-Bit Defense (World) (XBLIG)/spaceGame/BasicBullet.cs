using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace spaceGame;

public class BasicBullet : Sprite
{
	private int maxDistance = 640;

	private float XdistanceTraveled = 0f;

	private float YdistanceTraveled = 0f;

	public bool Visible = false;

	public bool bActive = false;

	private int myOwner;

	private int penetration;

	public Vector2 vVelocity;

	private Game1 theGame;

	public BasicBullet(ref Game1 myRef)
	{
		theGame = myRef;
	}

	public void LoadContent(ContentManager theContentManager)
	{
		LoadContent(theContentManager, "EnemyBullet");
		base.Scale = 1f;
	}

	public void LoadContent2(ContentManager theContentManager)
	{
		LoadContent(theContentManager, "PlayerBullet");
		base.Scale = 1f;
	}

	public void Update(GameTime theGameTime)
	{
		if (bActive)
		{
			Update(theGameTime, vVelocity);
			Position += vVelocity;
			XdistanceTraveled += vVelocity.X;
			YdistanceTraveled += vVelocity.Y;
			if (Math.Abs(XdistanceTraveled) >= (float)maxDistance || Math.Abs(YdistanceTraveled) >= (float)maxDistance)
			{
				Despawn();
			}
			if (Position.X <= 384f || Position.X >= 896f)
			{
				Despawn();
			}
			if (Position.Y > 544f && Position.Y < 640f)
			{
				CheckBarricadeCollision();
			}
			if (myOwner == 0)
			{
				CheckPlayerCollision();
			}
			else
			{
				CheckEnemyCollision();
			}
		}
	}

	public override void Draw(SpriteBatch theSpriteBatch)
	{
		if (GetVisible())
		{
			base.Draw(theSpriteBatch);
		}
	}

	public void Spawn(Vector2 pos, Vector2 vel)
	{
		bActive = true;
		Visible = true;
		Position = pos;
		vVelocity = vel;
		XdistanceTraveled = 0f;
		YdistanceTraveled = 0f;
		penetration = 0;
	}

	public void Despawn()
	{
		bActive = false;
		Visible = false;
		Position.X = -4f;
		Position.Y = -4f;
	}

	public void SetOwner(int x)
	{
		myOwner = x;
	}

	public void SetPenetration(int pen)
	{
		penetration = pen;
	}

	public bool GetActive()
	{
		return bActive;
	}

	public bool GetVisible()
	{
		return Visible;
	}

	public void CheckEnemyCollision()
	{
		int i = 0;
		for (int maxEnemies = theGame.AIC.GetMaxEnemies(); i < maxEnemies; i++)
		{
			Parent_Enemy enemyFromArray = theGame.AIC.GetEnemyFromArray(i);
			if (enemyFromArray.GetActive() && Position.X + (float)(Size.Width / 2) >= enemyFromArray.Position.X && Position.X + (float)(Size.Width / 2) <= enemyFromArray.Position.X + (float)enemyFromArray.Size.Width && Position.Y + (float)(Size.Height / 2) >= enemyFromArray.Position.Y && Position.Y + (float)(Size.Height / 2) <= enemyFromArray.Position.Y + (float)enemyFromArray.Size.Height && enemyFromArray.GetImmunityTimer() <= 0)
			{
				enemyFromArray.ChangeHealth(-1, myOwner);
				if (penetration > 0)
				{
					penetration--;
				}
				else
				{
					Despawn();
				}
			}
		}
		i = 0;
		for (int maxEnemies = theGame.AIC.GetMaxBombers(); i < maxEnemies; i++)
		{
			Bomber_Enemy bomberFromArray = theGame.AIC.GetBomberFromArray(i);
			if (bomberFromArray.GetActive() && Position.X + (float)(Size.Width / 2) >= bomberFromArray.Position.X && Position.X + (float)(Size.Width / 2) <= bomberFromArray.Position.X + (float)bomberFromArray.Size.Width && Position.Y + (float)(Size.Height / 2) >= bomberFromArray.Position.Y && Position.Y + (float)(Size.Height / 2) <= bomberFromArray.Position.Y + (float)bomberFromArray.Size.Height && bomberFromArray.GetImmunityTimer() <= 0)
			{
				bomberFromArray.ChangeHealth(-1, myOwner);
				if (penetration > 0)
				{
					penetration--;
				}
				else
				{
					Despawn();
				}
			}
		}
	}

	public void CheckPlayerCollision()
	{
		MainShip mMainShipSprite = theGame.mMainShipSprite;
		if (mMainShipSprite.GetAlive() && Position.X + (float)(Size.Width / 2) >= mMainShipSprite.Position.X && Position.X + (float)(Size.Width / 2) <= mMainShipSprite.Position.X + (float)mMainShipSprite.Size.Width && Position.Y + (float)(Size.Height / 2) >= mMainShipSprite.Position.Y && Position.Y + (float)(Size.Height / 2) <= mMainShipSprite.Position.Y + (float)mMainShipSprite.Size.Height)
		{
			mMainShipSprite.ChangeHealth(-1);
			Despawn();
		}
	}

	public void CheckBarricadeCollision()
	{
		BarricadeBlock[] blockArray = theGame.Barricade1.BlockArray;
		foreach (BarricadeBlock barricadeBlock in blockArray)
		{
			if (!barricadeBlock.GetDestroyed() && Position.X + (float)(Size.Width / 2) >= barricadeBlock.Position.X && Position.X + (float)(Size.Width / 2) <= barricadeBlock.Position.X + (float)barricadeBlock.Size.Width && Position.Y + (float)(Size.Height / 2) >= barricadeBlock.Position.Y && Position.Y + (float)(Size.Height / 2) <= barricadeBlock.Position.Y + (float)barricadeBlock.Size.Height)
			{
				barricadeBlock.ChangeHealth(-1);
				Despawn();
			}
		}
		blockArray = theGame.Barricade2.BlockArray;
		foreach (BarricadeBlock barricadeBlock in blockArray)
		{
			if (!barricadeBlock.GetDestroyed() && Position.X + (float)(Size.Width / 2) >= barricadeBlock.Position.X && Position.X + (float)(Size.Width / 2) <= barricadeBlock.Position.X + (float)barricadeBlock.Size.Width && Position.Y + (float)(Size.Height / 2) >= barricadeBlock.Position.Y && Position.Y + (float)(Size.Height / 2) <= barricadeBlock.Position.Y + (float)barricadeBlock.Size.Height)
			{
				barricadeBlock.ChangeHealth(-1);
				Despawn();
			}
		}
		blockArray = theGame.Barricade3.BlockArray;
		foreach (BarricadeBlock barricadeBlock in blockArray)
		{
			if (!barricadeBlock.GetDestroyed() && Position.X + (float)(Size.Width / 2) >= barricadeBlock.Position.X && Position.X + (float)(Size.Width / 2) <= barricadeBlock.Position.X + (float)barricadeBlock.Size.Width && Position.Y + (float)(Size.Height / 2) >= barricadeBlock.Position.Y && Position.Y + (float)(Size.Height / 2) <= barricadeBlock.Position.Y + (float)barricadeBlock.Size.Height)
			{
				barricadeBlock.ChangeHealth(-1);
				Despawn();
			}
		}
		blockArray = theGame.Barricade4.BlockArray;
		foreach (BarricadeBlock barricadeBlock in blockArray)
		{
			if (!barricadeBlock.GetDestroyed() && Position.X + (float)(Size.Width / 2) >= barricadeBlock.Position.X && Position.X + (float)(Size.Width / 2) <= barricadeBlock.Position.X + (float)barricadeBlock.Size.Width && Position.Y + (float)(Size.Height / 2) >= barricadeBlock.Position.Y && Position.Y + (float)(Size.Height / 2) <= barricadeBlock.Position.Y + (float)barricadeBlock.Size.Height)
			{
				barricadeBlock.ChangeHealth(-1);
				Despawn();
			}
		}
	}
}

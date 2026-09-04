using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace spaceGame;

public class Parent_Enemy : AnimatedSprite
{
	public int iHealth;

	public int rateOfFire;

	public int fireTimer;

	public int immunityTimer;

	public int whoKilledMe;

	public bool isElite;

	public int eliteFlashTimer;

	public bool isDefender;

	public bool canShoot;

	public bool enemyDying;

	public int deathTimer;

	public int deathFrame;

	public int iPointValue;

	public int myRow;

	public int myColumn;

	public bool Visible = false;

	public bool bActive = false;

	public Vector2 vVelocity;

	public BasicBullet myBullet;

	public BasicBullet myBullet2;

	public BasicBullet myBullet3;

	public Game1 theGame;

	public Parent_Enemy()
	{
	}

	public Parent_Enemy(ref Game1 myRef, int rdm)
	{
		theGame = myRef;
		iHealth = 1;
		enemyDying = false;
		deathTimer = 7;
		deathFrame = 0;
		iPointValue = 5;
		immunityTimer = 0;
		isElite = false;
		eliteFlashTimer = 30;
		isDefender = false;
		canShoot = false;
		rateOfFire = 600;
		fireTimer = rdm;
		myBullet = new BasicBullet(ref myRef);
		myBullet2 = new BasicBullet(ref myRef);
		myBullet3 = new BasicBullet(ref myRef);
		myRow = 0;
		myColumn = 0;
	}

	public void LoadContent(ContentManager theContentManager)
	{
		LoadContent(theContentManager, "Enemy1");
		SetupAnimatedSprite(1, 2, 0, 2, 1f);
		myBullet.LoadContent(theContentManager);
		myBullet2.LoadContent(theContentManager);
		myBullet3.LoadContent(theContentManager);
	}

	public void Update(GameTime theGameTime)
	{
		if (bActive)
		{
			if (Position.Y > 596f)
			{
				theGame.iGameState = 3;
			}
			if (!canShoot && !isDefender && isElite)
			{
				if (fireTimer <= 0)
				{
					Parent_Enemy[] enemyArray = theGame.AIC.EnemyArray;
					foreach (Parent_Enemy parent_Enemy in enemyArray)
					{
						if (parent_Enemy.myRow == myRow && parent_Enemy.GetActive())
						{
							parent_Enemy.ChangeHealth(1, 0);
						}
					}
					fireTimer = rateOfFire;
				}
				else
				{
					fireTimer--;
				}
			}
			if (!canShoot)
			{
				return;
			}
			if (fireTimer <= 0)
			{
				theGame.SoundEnemyLaser();
				if (!isElite)
				{
					myBullet.Spawn(Position + new Vector2(Size.Width / 2, Size.Height / 2), new Vector2(0f, 4f));
				}
				else
				{
					myBullet.Spawn(Position + new Vector2(Size.Width / 2, Size.Height / 2), new Vector2(0f, 4f));
					myBullet2.Spawn(Position + new Vector2(Size.Width / 2, Size.Height / 2), new Vector2(1f, 4f));
					myBullet3.Spawn(Position + new Vector2(Size.Width / 2, Size.Height / 2), new Vector2(-1f, 4f));
				}
				fireTimer = rateOfFire;
			}
			else
			{
				fireTimer--;
			}
		}
		else
		{
			if (!enemyDying)
			{
				return;
			}
			if (deathTimer == 0)
			{
				if (deathFrame == 1)
				{
					Despawn();
					return;
				}
				deathFrame++;
				Update();
				deathTimer = 7;
			}
			else
			{
				deathTimer--;
			}
		}
	}

	public void UpdateBullet(GameTime theGameTime)
	{
		myBullet.Update(theGameTime);
		myBullet2.Update(theGameTime);
		myBullet3.Update(theGameTime);
	}

	public void Draw(SpriteBatch theSpriteBatch)
	{
		if (Visible)
		{
			int num;
			for (num = iHealth; num > 10; num -= 10)
			{
			}
			if (eliteFlashTimer > 0)
			{
				eliteFlashTimer--;
			}
			else
			{
				eliteFlashTimer = 45;
			}
			if (isElite && eliteFlashTimer > 30)
			{
				base.Draw(theSpriteBatch, Color.Red);
			}
			else
			{
				switch (num)
				{
				case 0:
					base.Draw(theSpriteBatch, Color.White);
					break;
				case 1:
					base.Draw(theSpriteBatch, Color.White);
					break;
				case 2:
					base.Draw(theSpriteBatch, Color.LightBlue);
					break;
				case 3:
					base.Draw(theSpriteBatch, Color.MediumBlue);
					break;
				case 4:
					base.Draw(theSpriteBatch, Color.LightGreen);
					break;
				case 5:
					base.Draw(theSpriteBatch, Color.MediumSeaGreen);
					break;
				case 6:
					base.Draw(theSpriteBatch, Color.GreenYellow);
					break;
				case 7:
					base.Draw(theSpriteBatch, Color.LightGoldenrodYellow);
					break;
				case 8:
					base.Draw(theSpriteBatch, Color.Yellow);
					break;
				case 9:
					base.Draw(theSpriteBatch, Color.Orange);
					break;
				case 10:
					base.Draw(theSpriteBatch, Color.Gold);
					break;
				}
			}
		}
		if (myBullet.GetVisible())
		{
			myBullet.Draw(theSpriteBatch);
		}
		if (myBullet2.GetVisible())
		{
			myBullet2.Draw(theSpriteBatch);
		}
		if (myBullet3.GetVisible())
		{
			myBullet3.Draw(theSpriteBatch);
		}
	}

	public void Spawn(Vector2 pos, Vector2 vel)
	{
		bActive = true;
		Visible = true;
		iHealth = 1;
		Position = pos;
		vVelocity = vel;
		LoadContent(theGame.Content, "Enemy1");
		SetupAnimatedSprite(1, 2, 0, 2, 1f);
	}

	public void Despawn()
	{
		enemyDying = false;
		bActive = false;
		Visible = false;
		isElite = false;
		Position.X = -4f;
		Position.Y = -4f;
		canShoot = false;
		isDefender = false;
	}

	public bool Move(Vector2 vel)
	{
		if (bActive)
		{
			Update();
			Position += vel;
			if (Position.Y >= 512f)
			{
				CheckBarricadeCollision();
			}
			if (Position.X >= 860f)
			{
				return true;
			}
			if (Position.X <= 388f)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public void ChangeHealth(int diff, int whoHitMe)
	{
		iHealth += diff;
		immunityTimer = 5;
		whoKilledMe = whoHitMe;
		if (diff < 0)
		{
			theGame.SoundImpact();
			if (whoKilledMe == 1)
			{
				theGame.mMainShipSprite.ChangePoints(iPointValue);
			}
		}
		if (iHealth <= 0)
		{
			EnemyDeath();
			if (whoKilledMe == 1)
			{
				theGame.mMainShipSprite.ChangePoints(iPointValue);
			}
		}
	}

	public void EnableShooting()
	{
		canShoot = true;
		isDefender = false;
		LoadContent(theGame.Content, "ShootingEnemy");
	}

	public void EnableDefending()
	{
		isDefender = true;
		canShoot = false;
		ChangeHealth(2, 0);
		LoadContent(theGame.Content, "DefenderEnemy");
	}

	public void EnableElite()
	{
		isElite = true;
		ChangeHealth(5, 0);
	}

	public bool GetActive()
	{
		return bActive;
	}

	public int GetImmunityTimer()
	{
		return immunityTimer;
	}

	public void EnemyDeath()
	{
		enemyDying = true;
		bActive = false;
		deathFrame = 0;
		deathTimer = 7;
		LoadContent(theGame.Content, "DeathAnimation");
		SetupAnimatedSprite(1, 2, 0, 2, 1f);
		theGame.SoundEnemyDestroy();
	}

	public void CheckBarricadeCollision()
	{
		BarricadeBlock[] blockArray = theGame.Barricade1.BlockArray;
		foreach (BarricadeBlock barricadeBlock in blockArray)
		{
			if (barricadeBlock.GetHealth() > 0 && Position.X + (float)(Size.Width / 2) >= barricadeBlock.Position.X && Position.X + (float)(Size.Width / 2) <= barricadeBlock.Position.X + (float)barricadeBlock.Size.Width && Position.Y + (float)(Size.Height / 2) >= barricadeBlock.Position.Y && Position.Y + (float)(Size.Height / 2) <= barricadeBlock.Position.Y + (float)barricadeBlock.Size.Height)
			{
				barricadeBlock.ChangeHealth(-1);
			}
		}
		blockArray = theGame.Barricade2.BlockArray;
		foreach (BarricadeBlock barricadeBlock in blockArray)
		{
			if (barricadeBlock.GetHealth() > 0 && Position.X + (float)(Size.Width / 2) >= barricadeBlock.Position.X && Position.X + (float)(Size.Width / 2) <= barricadeBlock.Position.X + (float)barricadeBlock.Size.Width && Position.Y + (float)(Size.Height / 2) >= barricadeBlock.Position.Y && Position.Y + (float)(Size.Height / 2) <= barricadeBlock.Position.Y + (float)barricadeBlock.Size.Height)
			{
				barricadeBlock.ChangeHealth(-1);
			}
		}
		blockArray = theGame.Barricade3.BlockArray;
		foreach (BarricadeBlock barricadeBlock in blockArray)
		{
			if (barricadeBlock.GetHealth() > 0 && Position.X + (float)(Size.Width / 2) >= barricadeBlock.Position.X && Position.X + (float)(Size.Width / 2) <= barricadeBlock.Position.X + (float)barricadeBlock.Size.Width && Position.Y + (float)(Size.Height / 2) >= barricadeBlock.Position.Y && Position.Y + (float)(Size.Height / 2) <= barricadeBlock.Position.Y + (float)barricadeBlock.Size.Height)
			{
				barricadeBlock.ChangeHealth(-1);
			}
		}
		blockArray = theGame.Barricade4.BlockArray;
		foreach (BarricadeBlock barricadeBlock in blockArray)
		{
			if (barricadeBlock.GetHealth() > 0 && Position.X + (float)(Size.Width / 2) >= barricadeBlock.Position.X && Position.X + (float)(Size.Width / 2) <= barricadeBlock.Position.X + (float)barricadeBlock.Size.Width && Position.Y + (float)(Size.Height / 2) >= barricadeBlock.Position.Y && Position.Y + (float)(Size.Height / 2) <= barricadeBlock.Position.Y + (float)barricadeBlock.Size.Height)
			{
				barricadeBlock.ChangeHealth(-1);
			}
		}
	}
}

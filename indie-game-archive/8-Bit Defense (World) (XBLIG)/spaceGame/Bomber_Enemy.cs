using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace spaceGame;

public class Bomber_Enemy : Parent_Enemy
{
	private bool enteredTheBattle;

	private int animationTimer;

	private float helix1;

	private float helix2;

	private bool helix1Right;

	private bool helix2Right;

	public Bomber_Enemy(ref Game1 myRef, int rdm)
	{
		theGame = myRef;
		iHealth = 1;
		enemyDying = false;
		deathTimer = 7;
		eliteFlashTimer = 30;
		deathFrame = 0;
		iPointValue = 10;
		immunityTimer = 0;
		animationTimer = 60;
		canShoot = true;
		rateOfFire = 300;
		fireTimer = rdm;
		myBullet = new BasicBullet(ref myRef);
		myBullet2 = new BasicBullet(ref myRef);
		myBullet3 = new BasicBullet(ref myRef);
		helix1 = 0f;
		helix1Right = false;
		helix2Right = true;
		helix2 = 0f;
		myRow = 0;
		myColumn = 0;
		enteredTheBattle = false;
	}

	public new void LoadContent(ContentManager theContentManager)
	{
		LoadContent(theContentManager, "Enemy2");
		SetupAnimatedSprite(1, 2, 0, 2, 1f);
		myBullet.LoadContent(theContentManager);
		myBullet2.LoadContent(theContentManager);
		myBullet3.LoadContent(theContentManager);
	}

	public new void Update(GameTime theGameTime)
	{
		if (animationTimer <= 0)
		{
			Update();
			animationTimer = 60;
		}
		else
		{
			animationTimer--;
		}
		if (enteredTheBattle)
		{
			if (bActive)
			{
				Position += vVelocity;
				if (Position.X >= 864f)
				{
					vVelocity = new Vector2(-1f, 0f);
				}
				else if (Position.X <= 384f)
				{
					vVelocity = new Vector2(1f, 0f);
				}
				if (!canShoot)
				{
					return;
				}
				if (fireTimer <= 0)
				{
					float num = Math.Abs(theGame.mMainShipSprite.Position.X - Position.X);
					if (num <= 16f)
					{
						theGame.SoundEnemyLaser();
						myBullet.Spawn(Position + new Vector2(Size.Width / 2, Size.Height / 2), new Vector2(0f, 3f));
						myBullet2.Spawn(Position + new Vector2(Size.Width / 2, Size.Height / 2), new Vector2(0f, 3f));
						myBullet3.Spawn(Position + new Vector2(Size.Width / 2, Size.Height / 2), new Vector2(0f, 3f));
						if (isElite)
						{
							fireTimer = rateOfFire / 2;
						}
						else
						{
							fireTimer = rateOfFire;
						}
					}
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
		else
		{
			Position += vVelocity;
			if (Position.Y >= 48f)
			{
				enteredTheBattle = true;
			}
		}
	}

	public new void Spawn(Vector2 pos, Vector2 vel)
	{
		bActive = true;
		Visible = true;
		canShoot = true;
		iHealth = 1;
		Position = pos;
		vVelocity = vel;
		LoadContent(theGame.Content, "Enemy2");
		SetupAnimatedSprite(1, 2, 0, 2, 1f);
		enteredTheBattle = false;
	}

	public new void UpdateBullet(GameTime theGameTime)
	{
		myBullet.Update(theGameTime);
		if (helix1Right)
		{
			helix1 += 0.05f;
			if ((double)helix1 >= 1.3)
			{
				helix1Right = false;
			}
		}
		else
		{
			helix1 -= 0.05f;
			if ((double)helix1 <= -1.3)
			{
				helix1Right = true;
			}
		}
		if (helix2Right)
		{
			helix2 += 0.05f;
			if ((double)helix2 >= 1.3)
			{
				helix2Right = false;
			}
		}
		else
		{
			helix2 -= 0.05f;
			if ((double)helix2 <= -1.3)
			{
				helix2Right = true;
			}
		}
		myBullet2.vVelocity.X = helix1;
		myBullet3.vVelocity.X = helix2;
		myBullet2.Update(theGameTime);
		myBullet3.Update(theGameTime);
	}
}

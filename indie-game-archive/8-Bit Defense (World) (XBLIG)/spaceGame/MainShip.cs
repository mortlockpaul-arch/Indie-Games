using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace spaceGame;

public class MainShip : AnimatedSprite
{
	private enum State
	{
		Moving
	}

	private const string MAINSHIP_ASSETNAME = "Tank2";

	private const int START_POSITION_X = 624;

	private const int START_POSITION_Y = 612;

	private const short MAX_BULLETS = 50;

	private bool playerDying;

	private int deathTimer;

	private int deathFrame;

	private int playerNumber;

	private bool bAlive;

	private bool bVisible;

	private int iHealth;

	private int iLives;

	private int nextLife;

	private int iTotalPoints;

	private int iPoints;

	private Vector2 vVelocity;

	private int rateOfFire;

	private int fireTimer;

	private int machineGunLevel;

	private int spreadShotLevel;

	private int penetrationLevel;

	public int immunityTimer;

	private BasicBullet[] BulletArray = new BasicBullet[50];

	private Game1 theGame;

	private ContentManager mContentManager;

	private KeyboardState mPreviousKeyboardState;

	private GamePadState mPreviousGamePadState;

	private State mCurrentState = State.Moving;

	public MainShip(Game1 gameRef, int player)
	{
		theGame = gameRef;
		playerDying = false;
		deathTimer = 15;
		deathFrame = 0;
		playerNumber = player;
		bAlive = true;
		bVisible = true;
		iHealth = 1;
		iLives = 2;
		nextLife = 2000;
		Position = new Vector2(624f, 612f);
		rateOfFire = 90;
		fireTimer = rateOfFire;
		machineGunLevel = 0;
		spreadShotLevel = 0;
		penetrationLevel = 0;
		immunityTimer = 60;
		for (short num = 0; num != 50; num++)
		{
			BulletArray[num] = new BasicBullet(ref gameRef);
			BulletArray[num].SetOwner(playerNumber);
		}
	}

	public void LoadContent(ContentManager theContentManager)
	{
		mContentManager = theContentManager;
		BasicBullet[] bulletArray = BulletArray;
		foreach (BasicBullet basicBullet in bulletArray)
		{
			basicBullet.LoadContent2(theContentManager);
		}
		LoadContent(theContentManager, "Tank2");
		SetupAnimatedSprite(1, 1, 0, 1, 1f);
	}

	public void Update(GameTime theGameTime)
	{
		KeyboardState state = Keyboard.GetState();
		GamePadState state2 = GamePad.GetState(theGame.ThePlayer);
		if (immunityTimer > 0)
		{
			immunityTimer--;
		}
		if (bAlive)
		{
			if (!theGame.theUpgradeMenu.GetActive())
			{
				UpdateMovement(state, state2);
				UpdateWeapon(theGameTime, state, state2);
			}
			else
			{
				vVelocity = new Vector2(0f, 0f);
			}
			mPreviousKeyboardState = state;
			mPreviousGamePadState = state2;
			if (fireTimer > 0)
			{
				fireTimer--;
			}
		}
		else if (playerDying)
		{
			if (deathTimer == 0)
			{
				if (deathFrame == 1)
				{
					Respawn();
				}
				else
				{
					deathFrame++;
					Update();
					deathTimer = 10;
				}
			}
			else
			{
				deathTimer--;
			}
		}
		for (short num = 0; num != 50; num++)
		{
			BulletArray[num].Update(theGameTime);
		}
	}

	private void UpdateWeapon(GameTime theGameTime, KeyboardState aCurrentKeyboardState, GamePadState currentGamePadState)
	{
		if (aCurrentKeyboardState.IsKeyDown(Keys.Space))
		{
			Shoot();
		}
		else if (currentGamePadState.Buttons.A == ButtonState.Pressed)
		{
			Shoot();
		}
	}

	private void Shoot()
	{
		if (fireTimer > 0)
		{
			return;
		}
		fireTimer = rateOfFire - machineGunLevel * 20;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = spreadShotLevel + 1;
		short num7 = 0;
		short num8 = 0;
		while (num7 != num6)
		{
			if (!BulletArray[num8].bActive)
			{
				switch (num7)
				{
				case 0:
					num = num8;
					break;
				case 1:
					num2 = num8;
					break;
				case 2:
					num3 = num8;
					break;
				case 3:
					num4 = num8;
					break;
				case 4:
					num5 = num8;
					break;
				}
				num7++;
			}
			num8++;
			if (num8 >= 50)
			{
				break;
			}
		}
		if (num7 == num6)
		{
			switch (num7)
			{
			case 1:
				BulletArray[num].Spawn(Position + new Vector2(Size.Width / 2 - 2, Size.Height / 2), new Vector2(0f, -8f));
				BulletArray[num].SetPenetration(penetrationLevel);
				theGame.laser();
				break;
			case 2:
				BulletArray[num].Spawn(Position + new Vector2(Size.Width / 2 + 4, Size.Height / 2), new Vector2(0.3f, -8f));
				BulletArray[num2].Spawn(Position + new Vector2(Size.Width / 2 - 8, Size.Height / 2), new Vector2(-0.3f, -8f));
				BulletArray[num].SetPenetration(penetrationLevel);
				BulletArray[num2].SetPenetration(penetrationLevel);
				theGame.laser();
				break;
			case 3:
				BulletArray[num].Spawn(Position + new Vector2(Size.Width / 2 - 2, Size.Height / 2), new Vector2(0f, -8f));
				BulletArray[num2].Spawn(Position + new Vector2(Size.Width / 2 - 2, Size.Height / 2), new Vector2(-1f, -8f));
				BulletArray[num3].Spawn(Position + new Vector2(Size.Width / 2 - 2, Size.Height / 2), new Vector2(1f, -8f));
				BulletArray[num].SetPenetration(penetrationLevel);
				BulletArray[num2].SetPenetration(penetrationLevel);
				BulletArray[num3].SetPenetration(penetrationLevel);
				theGame.laser();
				break;
			case 4:
				BulletArray[num].Spawn(Position + new Vector2(Size.Width / 2 - 2, Size.Height / 2), new Vector2(-1.5f, -8f));
				BulletArray[num2].Spawn(Position + new Vector2(Size.Width / 2 - 2, Size.Height / 2), new Vector2(-0.5f, -8f));
				BulletArray[num3].Spawn(Position + new Vector2(Size.Width / 2 - 2, Size.Height / 2), new Vector2(0.5f, -8f));
				BulletArray[num4].Spawn(Position + new Vector2(Size.Width / 2 - 2, Size.Height / 2), new Vector2(1.5f, -8f));
				BulletArray[num].SetPenetration(penetrationLevel);
				BulletArray[num2].SetPenetration(penetrationLevel);
				BulletArray[num3].SetPenetration(penetrationLevel);
				BulletArray[num4].SetPenetration(penetrationLevel);
				theGame.laser();
				break;
			case 5:
				BulletArray[num].Spawn(Position + new Vector2(Size.Width / 2 - 2, Size.Height / 2), new Vector2(-2f, -8f));
				BulletArray[num2].Spawn(Position + new Vector2(Size.Width / 2 - 2, Size.Height / 2), new Vector2(-1f, -8f));
				BulletArray[num3].Spawn(Position + new Vector2(Size.Width / 2 - 2, Size.Height / 2), new Vector2(0f, -8f));
				BulletArray[num4].Spawn(Position + new Vector2(Size.Width / 2 - 2, Size.Height / 2), new Vector2(1f, -8f));
				BulletArray[num5].Spawn(Position + new Vector2(Size.Width / 2 - 2, Size.Height / 2), new Vector2(2f, -8f));
				BulletArray[num].SetPenetration(penetrationLevel);
				BulletArray[num2].SetPenetration(penetrationLevel);
				BulletArray[num3].SetPenetration(penetrationLevel);
				BulletArray[num4].SetPenetration(penetrationLevel);
				BulletArray[num5].SetPenetration(penetrationLevel);
				theGame.laser();
				break;
			}
		}
	}

	private void UpdateMovement(KeyboardState aCurrentKeyboardState, GamePadState currentGamePadState)
	{
		if (mCurrentState != State.Moving)
		{
			return;
		}
		Position += vVelocity;
		vVelocity = new Vector2(0f, 0f);
		if (aCurrentKeyboardState.IsKeyDown(Keys.Left) && Position.X > 384f)
		{
			vVelocity += new Vector2(-3f, 0f);
			if (Position.X < 384f)
			{
				Position.X = 384f;
			}
		}
		if (aCurrentKeyboardState.IsKeyDown(Keys.Right) && Position.X < 864f)
		{
			vVelocity += new Vector2(3f, 0f);
			if (Position.X > 864f)
			{
				Position.X = 864f;
			}
		}
		if (currentGamePadState.ThumbSticks.Left.X < 0f && Position.X > 384f)
		{
			vVelocity += new Vector2(-3f, 0f);
			if (Position.X < 384f)
			{
				Position.X = 384f;
			}
		}
		if (currentGamePadState.ThumbSticks.Left.X > 0f && Position.X < 864f)
		{
			vVelocity += new Vector2(3f, 0f);
			if (Position.X > 864f)
			{
				Position.X = 864f;
			}
		}
	}

	public void ChangeHealth(int diff)
	{
		if (immunityTimer != 0)
		{
			return;
		}
		iHealth += diff;
		if (iHealth <= 0)
		{
			theGame.SoundPlayerDeath();
			if (GetLives() > 0)
			{
				PlayerDeath();
			}
			else
			{
				Kill();
			}
		}
	}

	public void ChangePoints(int diff)
	{
		iPoints += diff;
		if (diff > 0)
		{
			iTotalPoints += diff;
		}
		if (iTotalPoints > nextLife)
		{
			iLives++;
			theGame.SoundExtraLife();
			nextLife *= 2;
		}
	}

	public void ChangeLives(int diff)
	{
		iLives += diff;
	}

	public void ChangeMachineGunLevel()
	{
		if (machineGunLevel < 5)
		{
			machineGunLevel++;
		}
	}

	public void ChangeSpreadShotLevel()
	{
		if (spreadShotLevel < 5)
		{
			spreadShotLevel++;
		}
	}

	public void ChangePenetrationLevel()
	{
		if (penetrationLevel < 5)
		{
			penetrationLevel++;
		}
	}

	public int GetMachineGunLevel()
	{
		return machineGunLevel;
	}

	public int GetSpreadShotLevel()
	{
		return spreadShotLevel;
	}

	public int GetPenetrationLevel()
	{
		return penetrationLevel;
	}

	public void Kill()
	{
		bAlive = false;
	}

	public bool GetAlive()
	{
		return bAlive;
	}

	public int GetLives()
	{
		return iLives;
	}

	public int GetPoints()
	{
		return iPoints;
	}

	public int SetPoints()
	{
		return iPoints = (iPoints -= 1000);
	}

	public int GetTotalPoints()
	{
		return iTotalPoints;
	}

	public void Draw(SpriteBatch theSpriteBatch)
	{
		BasicBullet[] bulletArray = BulletArray;
		foreach (BasicBullet basicBullet in bulletArray)
		{
			basicBullet.Draw(theSpriteBatch);
		}
		if (bVisible)
		{
			base.Draw(theSpriteBatch, Color.White);
		}
	}

	public void PlayerDeath()
	{
		playerDying = true;
		bAlive = false;
		deathFrame = 0;
		deathTimer = 15;
		LoadContent(theGame.Content, "MainShipDeathAnimation");
		SetupAnimatedSprite(1, 2, 0, 2, 1f);
	}

	public void Respawn()
	{
		ChangeLives(-1);
		bAlive = true;
		playerDying = false;
		immunityTimer = 60;
		Position.X = 624f;
		Position.Y = 612f;
		LoadContent(theGame.Content, "Tank2");
		SetupAnimatedSprite(1, 1, 0, 1, 1f);
	}
}

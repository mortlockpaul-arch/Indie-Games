using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace spaceGame;

public class AI_Controller : Sprite
{
	private const int MAX_ENEMIES = 48;

	private const int MAX_BOMBERS = 3;

	private int iBombersToSpawn;

	private int iElitesToSpawn;

	private int rateOfSpawning;

	private int waveNumber;

	private int waveScheme;

	private int enemyRows;

	private int boostRow;

	private int boostAll;

	private int lastEnemySpawned;

	private bool enemiesGoingRight;

	private bool enemiesWillDropDown;

	private bool bWaveStarted = false;

	public bool bWaveEnded = false;

	private double iEnemiesLeft;

	private double iUpdateDelay;

	private float spawnTimer;

	private Vector2 spawnPosition;

	private Game1 theGame;

	private Random RNG;

	public Parent_Enemy[] EnemyArray = new Parent_Enemy[48];

	public Bomber_Enemy[] BomberArray = new Bomber_Enemy[3];

	public AI_Controller(Game1 myRef)
	{
		RNG = new Random();
		theGame = myRef;
		iEnemiesLeft = 48.0;
		iBombersToSpawn = 1;
		iElitesToSpawn = 0;
		enemyRows = 6;
		waveNumber = 1;
		waveScheme = 0;
		boostRow = 0;
		boostAll = 0;
		lastEnemySpawned = 0;
		enemiesGoingRight = true;
		enemiesWillDropDown = false;
		iUpdateDelay = iEnemiesLeft;
		rateOfSpawning = 600;
		spawnTimer = rateOfSpawning;
		spawnPosition = new Vector2(492f, 96f);
		for (short num = 0; num != 48; num++)
		{
			EnemyArray[num] = new Parent_Enemy(ref myRef, RNG.Next(1200) + 60);
		}
		for (short num = 0; num != 3; num++)
		{
			BomberArray[num] = new Bomber_Enemy(ref myRef, RNG.Next(600) + 60);
		}
	}

	public void LoadContent(ContentManager theContentManager)
	{
		Parent_Enemy[] enemyArray = EnemyArray;
		foreach (Parent_Enemy parent_Enemy in enemyArray)
		{
			parent_Enemy.LoadContent(theContentManager);
		}
		Bomber_Enemy[] bomberArray = BomberArray;
		foreach (Bomber_Enemy bomber_Enemy in bomberArray)
		{
			bomber_Enemy.LoadContent(theContentManager);
		}
	}

	public void Update(GameTime theGameTime)
	{
		if (!bWaveStarted)
		{
			bWaveStarted = true;
			bWaveEnded = false;
			iBombersToSpawn = 1;
			spawnTimer = rateOfSpawning;
			int i = 0;
			int j = 0;
			for (; i < 6; i++)
			{
				for (; j < 8; j++)
				{
					SpawnBasicEnemy(spawnPosition);
					EnemyArray[lastEnemySpawned].myRow = i;
					EnemyArray[lastEnemySpawned].myColumn = j;
					if (boostRow > i)
					{
						EnemyArray[lastEnemySpawned].ChangeHealth(1, 0);
					}
					switch (waveScheme)
					{
					case 0:
						if (i == 1 || i == 4)
						{
							EnemyArray[lastEnemySpawned].EnableShooting();
						}
						break;
					case 1:
						if ((i == 0 || i == 1 || i == 4) && (j == 0 || j == 1 || j == 6 || j == 7))
						{
							EnemyArray[lastEnemySpawned].EnableShooting();
						}
						if (i == 5 || i == 3)
						{
							EnemyArray[lastEnemySpawned].EnableDefending();
						}
						break;
					case 2:
						if (i == 0)
						{
							EnemyArray[lastEnemySpawned].EnableShooting();
						}
						if (i == 2)
						{
							EnemyArray[lastEnemySpawned].EnableDefending();
						}
						if (i == 3)
						{
							EnemyArray[lastEnemySpawned].EnableShooting();
						}
						if (i == 5)
						{
							EnemyArray[lastEnemySpawned].EnableDefending();
						}
						break;
					case 3:
						if (j == 0 || j == 1 || j == 6 || j == 7)
						{
							EnemyArray[lastEnemySpawned].EnableShooting();
						}
						if (i == 5)
						{
							EnemyArray[lastEnemySpawned].EnableDefending();
						}
						break;
					case 4:
						if (i == 1)
						{
							EnemyArray[lastEnemySpawned].EnableShooting();
						}
						if (i == 4)
						{
							EnemyArray[lastEnemySpawned].EnableShooting();
						}
						if (i == 5)
						{
							EnemyArray[lastEnemySpawned].EnableDefending();
						}
						if (j == 0 || j == 7)
						{
							EnemyArray[lastEnemySpawned].EnableDefending();
						}
						break;
					}
					EnemyArray[lastEnemySpawned].ChangeHealth(boostAll, 0);
					if (RNG.Next(0, 20) == 1 && iElitesToSpawn > 0)
					{
						EnemyArray[lastEnemySpawned].EnableElite();
						iElitesToSpawn--;
					}
					spawnPosition.X += 36f;
				}
				spawnPosition.X = 492f;
				spawnPosition.Y += 48f;
				j = 0;
			}
		}
		else if (bWaveStarted)
		{
			if (spawnTimer <= 0f && GetEnemiesLeft() > 0.0)
			{
				if (iBombersToSpawn > 0)
				{
					SpawnBomberEnemy(new Vector2(384f, -64f));
					if (RNG.Next(0, 25) == 1 && iElitesToSpawn > 0)
					{
						BomberArray[0].EnableElite();
						iElitesToSpawn--;
					}
				}
			}
			else
			{
				spawnTimer--;
			}
			if (iUpdateDelay <= 0.0)
			{
				if (!bWaveEnded)
				{
					theGame.SoundEnemyMove();
				}
				iUpdateDelay = GetEnemiesLeft() + 8.0;
				if (enemiesGoingRight)
				{
					Parent_Enemy[] enemyArray = EnemyArray;
					foreach (Parent_Enemy parent_Enemy in enemyArray)
					{
						if (parent_Enemy.Move(new Vector2(8f, 0f)))
						{
							enemiesGoingRight = false;
							enemiesWillDropDown = true;
						}
					}
				}
				else
				{
					Parent_Enemy[] enemyArray = EnemyArray;
					foreach (Parent_Enemy parent_Enemy in enemyArray)
					{
						if (parent_Enemy.Move(new Vector2(-8f, 0f)))
						{
							enemiesGoingRight = true;
							enemiesWillDropDown = true;
						}
					}
				}
				if (enemiesWillDropDown)
				{
					Parent_Enemy[] enemyArray = EnemyArray;
					foreach (Parent_Enemy parent_Enemy in enemyArray)
					{
						parent_Enemy.Move(new Vector2(0f, 32f));
						enemiesWillDropDown = false;
					}
				}
			}
			else
			{
				iUpdateDelay--;
			}
		}
		for (short num = 0; num != 48; num++)
		{
			EnemyArray[num].UpdateBullet(theGameTime);
			EnemyArray[num].Update(theGameTime);
			if (EnemyArray[num].GetImmunityTimer() > 0)
			{
				EnemyArray[num].immunityTimer--;
			}
		}
		for (short num = 0; num != 3; num++)
		{
			BomberArray[num].UpdateBullet(theGameTime);
			BomberArray[num].Update(theGameTime);
			if (BomberArray[num].GetImmunityTimer() > 0)
			{
				BomberArray[num].immunityTimer--;
			}
		}
	}

	public void SpawnBasicEnemy(Vector2 pos)
	{
		bool flag = false;
		short num = 0;
		while (!flag)
		{
			if (!EnemyArray[num].bActive)
			{
				flag = true;
			}
			else
			{
				num++;
			}
			if (num >= 48)
			{
				break;
			}
		}
		if (flag)
		{
			EnemyArray[num].Spawn(pos, new Vector2(1f, 0f));
			lastEnemySpawned = num;
		}
	}

	public void SpawnBomberEnemy(Vector2 pos)
	{
		spawnTimer = rateOfSpawning;
		iBombersToSpawn--;
		bool flag = false;
		short num = 0;
		while (!flag)
		{
			if (!BomberArray[num].bActive)
			{
				flag = true;
			}
			else
			{
				num++;
			}
			if (num >= 3)
			{
				break;
			}
		}
		if (flag)
		{
			BomberArray[num].Spawn(pos, new Vector2(0f, 1f));
			BomberArray[num].ChangeHealth(boostAll, 0);
		}
	}

	public void NextWave()
	{
		waveNumber++;
		spawnPosition = new Vector2(492f, 96f);
		bWaveStarted = false;
		boostRow += 2;
		waveScheme++;
		if (waveScheme > 4)
		{
			waveScheme = 0;
		}
		if (boostRow >= enemyRows)
		{
			boostRow = 0;
			boostAll++;
			iElitesToSpawn++;
		}
	}

	public override void Draw(SpriteBatch theSpriteBatch)
	{
		Parent_Enemy[] enemyArray = EnemyArray;
		foreach (Parent_Enemy parent_Enemy in enemyArray)
		{
			parent_Enemy.Draw(theSpriteBatch);
		}
		Bomber_Enemy[] bomberArray = BomberArray;
		foreach (Bomber_Enemy bomber_Enemy in bomberArray)
		{
			bomber_Enemy.Draw(theSpriteBatch);
		}
	}

	public Parent_Enemy GetEnemyFromArray(int x)
	{
		return EnemyArray[x];
	}

	public Bomber_Enemy GetBomberFromArray(int x)
	{
		return BomberArray[x];
	}

	public int GetMaxEnemies()
	{
		return 48;
	}

	public int GetMaxBombers()
	{
		return 3;
	}

	public int GetWaveNumber()
	{
		return waveNumber;
	}

	public double GetEnemiesLeft()
	{
		iEnemiesLeft = 0.0;
		Parent_Enemy[] enemyArray = EnemyArray;
		foreach (Parent_Enemy parent_Enemy in enemyArray)
		{
			if (parent_Enemy.GetActive())
			{
				iEnemiesLeft++;
			}
		}
		Bomber_Enemy[] bomberArray = BomberArray;
		foreach (Bomber_Enemy bomber_Enemy in bomberArray)
		{
			if (bomber_Enemy.GetActive())
			{
				iEnemiesLeft++;
			}
		}
		return iEnemiesLeft;
	}
}

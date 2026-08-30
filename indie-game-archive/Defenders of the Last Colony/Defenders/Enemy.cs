using System;
using System.Collections.Generic;
using Hammer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

internal class Enemy
{
	public Texture2D texture;

	public Texture2D textureSpawning;

	public Texture2D textureSpawning2;

	private Texture2D txArrow;

	public EnemState state = EnemState.normal;

	private StateControl stateControl = new StateControl();

	public Vector2 position;

	public Vector2 destiny;

	public Vector2 OLDposition;

	public List<Vector2> posReg = new List<Vector2>(10);

	public Vector2 direction;

	private Vector2 positionRandom;

	public int enemyType;

	public float speed;

	public float frozen = 0f;

	public float topSpeed;

	public Color col;

	public float angle;

	private float destinyAngle;

	private float movingAngle;

	private float cadence;

	public float drawingAngle;

	private float targetWeight;

	private float delay;

	private float dist;

	private uint change;

	private float speedSaved;

	private Vector2[] location = new Vector2[8];

	private int lastLocation = 7;

	public ushort jump = 0;

	private SpriteEffects se = SpriteEffects.None;

	public float Health;

	public float maximunHealth;

	public float Damage;

	public float shootingDamage = 1f;

	public int Energy;

	public int experience;

	public int score;

	public float scale;

	private float topScale;

	public int pnumber;

	public bool isShooting;

	public bool Active;

	public float spawning;

	public int spawnRatio;

	public uint life = 0u;

	public int scion = 0;

	public List<Enemy> followers = new List<Enemy>(10);

	private Random random;

	public int Width => texture.Width;

	public int Height => texture.Height;

	public float size()
	{
		if (enemyType != 99)
		{
			return ((float)Width + (float)Height) / 2f * scale;
		}
		return 40f;
	}

	public Enemy()
	{
	}

	public Enemy(Texture2D texture, Texture2D textureSpawning, Texture2D textureSpawning2, Vector2 position, Vector2 direction, float speed, int enemyType, int target, Texture2D txArrow, int randomseed, float health)
	{
		Initialize(texture, textureSpawning, textureSpawning2, position, direction, speed, enemyType, target, txArrow, randomseed, health);
	}

	public void Initialize(Texture2D texture, Texture2D textureSpawning, Texture2D textureSpawning2, Vector2 position, int enemyType, int target, Texture2D txArrow, int randomseed)
	{
		random = new Random(randomseed);
		Initialize(texture, textureSpawning, textureSpawning2, position, Vector2.Zero, (float)random.Next(650, 700) / 100f, enemyType, target, txArrow, randomseed, 3f);
	}

	public void Initialize(Texture2D texture, Texture2D textureSpawning, Texture2D textureSpawning2, Vector2 position, Vector2 direction, float speed, int enemyType, int target, Texture2D txArrow, int randomseed)
	{
		Initialize(texture, textureSpawning, textureSpawning2, position, direction, speed, enemyType, target, txArrow, randomseed, 3f);
	}

	public void Initialize(Texture2D texture, Texture2D textureSpawning, Texture2D textureSpawning2, Vector2 position, Vector2 direction, float speed, int enemyType, int target, Texture2D txArrow, int randomseed, float health)
	{
		random = new Random(randomseed);
		this.texture = texture;
		this.textureSpawning = textureSpawning;
		this.textureSpawning2 = textureSpawning2;
		this.position = position;
		OLDposition = position;
		this.direction = direction;
		this.speed = speed;
		this.enemyType = enemyType;
		this.txArrow = txArrow;
		Health = health;
		isShooting = false;
		positionRandom = new Vector2(random.Next(-560, 1800), random.Next(-300, 1100));
		col = new Color(255, 50, 5);
		int num = 1280;
		int num2 = 720;
		ref Vector2 reference = ref location[0];
		reference = new Vector2(num / -4, num2 / -4);
		ref Vector2 reference2 = ref location[1];
		reference2 = new Vector2(num / 2, num2 / -4);
		ref Vector2 reference3 = ref location[2];
		reference3 = new Vector2((float)num * 1.25f, num2 / -4);
		ref Vector2 reference4 = ref location[3];
		reference4 = new Vector2(num / -4, num2 / 2);
		ref Vector2 reference5 = ref location[4];
		reference5 = new Vector2((float)num * 1.25f, num2 / 2);
		ref Vector2 reference6 = ref location[5];
		reference6 = new Vector2(num / -4, (float)num2 * 1.25f);
		ref Vector2 reference7 = ref location[6];
		reference7 = new Vector2(num / 2, (float)num2 * 1.25f);
		ref Vector2 reference8 = ref location[7];
		reference8 = new Vector2((float)num * 1.25f, (float)num2 * 1.25f);
		for (int i = 0; i < 10; i++)
		{
			posReg.Add(position);
		}
		pnumber = target;
		angle = (float)random.Next(720) / 100f + (float)Math.PI * 2f;
		movingAngle = (float)random.Next(100, 360) / 100f + (float)Math.PI * 2f;
		if (random.Next(100) < 50)
		{
			movingAngle *= -1f;
		}
		cadence = (float)random.Next(5, 10) / 100f + (float)Math.PI * 2f;
		if (random.Next(100) < 50)
		{
			cadence *= -1f;
		}
		targetWeight = 0f;
		delay = (float)random.Next(60, 100) / 3500f;
		spawning = 0f;
		spawnRatio = 0;
		scion = 10;
		change = 0u;
		dist = 100f;
		Damage = 2f;
		Energy = 1;
		experience = 1;
		scale = 1f;
		score = 1;
		life = 0u;
		this.speed = (float)random.Next(300, 500) / 100f;
		switch (enemyType)
		{
		case 0:
			spawning = 1f;
			scale = health / 2f;
			speed = 0f;
			Damage = 1f;
			score = 1;
			delay *= 10.01f;
			Energy = 10;
			experience = 0;
			movingAngle = (float)random.Next(-30, 30) / 1000f;
			switch (random.Next(3))
			{
			case 1:
				se = SpriteEffects.FlipHorizontally;
				break;
			case 2:
				se = SpriteEffects.FlipVertically;
				break;
			default:
				se = SpriteEffects.None;
				break;
			}
			break;
		case 2:
			this.speed = (float)random.Next(300, 350) / 100f;
			Damage = 5f;
			Health = 25f;
			delay *= 0.5f;
			score = 5;
			Energy = 5;
			experience = 1;
			dist = 250f;
			break;
		case 12:
			this.speed = (float)random.Next(450, 700) / 100f;
			Damage = 10f;
			Health = 300f;
			delay *= 0.5f;
			score = 7;
			Energy = 20;
			experience = 1;
			dist = 400f;
			break;
		case 3:
			this.speed = 0.0001f;
			Damage = 10f;
			score = 7;
			Health = 40f;
			delay *= 0.5f;
			Energy = 20;
			spawnRatio = 50;
			experience = 10;
			break;
		case 4:
			this.speed = 0f;
			Damage = 0f;
			score = 0;
			Health = 20f;
			delay *= 0.5f;
			Energy = 5;
			spawnRatio = 5;
			experience = 0;
			break;
		case 5:
			col = new Color(100, 250, 255);
			scale = 0.75f;
			this.speed = (float)random.Next(600, 800) / 100f;
			dist = 100f;
			break;
		case 6:
			col = new Color(50, 10, 5);
			scale = 0.75f;
			Health = 3f;
			spawnRatio = 60 + random.Next(120);
			Energy = 10;
			this.speed = 0f;
			break;
		case 7:
			col = new Color(64, 5, 1);
			scale = 0.6f;
			Health = 2f;
			this.speed = (float)random.Next(500, 700) / 100f;
			dist = random.Next(250, 500);
			change = (uint)random.Next(600, 800);
			break;
		case 8:
			col = new Color(128, 64, 32);
			scale = 0.5f;
			Health = 1f;
			this.speed = (float)random.Next(250, 350) / 100f;
			dist = random.Next(240, 280);
			change = (uint)random.Next(500, 1000);
			break;
		case 9:
			col = new Color(255, 64, 8);
			scale = 0.5f;
			Health = 12f;
			this.speed = (float)random.Next(300, 450) / 100f;
			dist = random.Next(240, 280);
			change = (uint)random.Next(500, 1000);
			scion = 9;
			while (followers.Count < 10)
			{
				AddFollower();
			}
			Energy = 20;
			break;
		case 10:
			col = new Color(255, 64, 8);
			scale = 0.5f;
			Health = 0.5f;
			this.speed = (float)random.Next(200, 250) / 100f;
			dist = random.Next(240, 280);
			change = (uint)random.Next(500, 1000);
			delay = 0.1f;
			break;
		case 11:
			scale = 0.5f;
			Health = 0.5f;
			if (Game1.gameState == GameState.Sidescroller)
			{
				Health = 5f;
			}
			this.speed = (float)random.Next(200, 250) / 100f;
			dist = random.Next(240, 280);
			change = (uint)random.Next(500, 1000);
			delay = 0.1f;
			break;
		case 99:
			this.speed = 0.0001f;
			Damage = 20f;
			score = 1;
			Health = 2000f;
			delay = 0.1f;
			Energy = 500;
			spawnRatio = 1;
			experience = 1000;
			scale = 1f;
			break;
		case 100:
			col = new Color(128, 64, 32);
			Health = 3f;
			Damage = 100f;
			delay = 0.05f;
			scale = 1f;
			this.speed = (float)random.Next(250, 350) / 100f;
			dist = random.Next(440, 680);
			change = (uint)(random.Next(5, 100) * 10);
			break;
		}
		float num3 = Game1.difficulty;
		if (Game1.gameState == GameState.Challenge)
		{
			num3 = 0.65f;
		}
		Health *= num3;
		maximunHealth = Health;
		topSpeed = this.speed * num3;
		topScale = scale;
		this.speed = 0f;
		scale = 0f;
		speedSaved = topSpeed;
		Damage *= num3;
		Active = true;
	}

	public void AddFollower()
	{
		followers.Add(new Enemy(texture, textureSpawning, textureSpawning2, position, direction, speed, 10, 1, txArrow, random.Next(100), 3f));
	}

	public void Update(int pnumber, Vector2 dest, int maxX, int maxY)
	{
		if (!Active)
		{
			return;
		}
		destiny = dest;
		this.pnumber = pnumber;
		spawning += delay;
		spawning = MathHelper.Clamp(spawning, 0f, 1f);
		float num = MathHelper.Clamp((float)life / 10f, 0f, 20f);
		if (Vector2.Distance(position, posReg[posReg.Count - 1]) > num)
		{
			posReg.RemoveAt(0);
			posReg.Add(position);
		}
		if (frozen > 0f)
		{
			frozen--;
			speed = 0f;
			topSpeed = 0f;
		}
		else
		{
			topSpeed = speedSaved;
			life++;
		}
		if (life > 100000)
		{
			life = 0u;
		}
		if (enemyType < 100)
		{
			OLDposition = position;
		}
		if (spawning >= 1f)
		{
			switch (enemyType)
			{
			case 2:
			{
				Vector2 vector = followAndShoot(ref destiny);
				break;
			}
			case 3:
				if (scale < 1f)
				{
					scale = MathHelper.Lerp(scale, topScale, 0.2f);
				}
				speed = MathHelper.Lerp(speed, topSpeed, (float)life / 500f);
				angle += 0.01f;
				if (frozen <= 0f)
				{
					drawingAngle = angle;
				}
				break;
			case 4:
				if (scale < 1f)
				{
					scale = MathHelper.Lerp(scale, topScale, 0.2f);
				}
				speed = MathHelper.Lerp(speed, topSpeed, (float)life / 500f);
				if (frozen <= 0f)
				{
					drawingAngle += 0.81f;
				}
				angle += 0.82f;
				Health--;
				break;
			case 5:
			{
				Vector2 vector = randomMovement();
				break;
			}
			case 6:
				if (scale < 1f)
				{
					scale = MathHelper.Lerp(scale, topScale, 0.2f);
				}
				if (frozen <= 0f)
				{
					drawingAngle += 0.001f + (float)Math.Sin(life / 50) / 20f;
				}
				if (life > 1560)
				{
					Active = false;
				}
				break;
			case 7:
				if (change != 0 && life > change)
				{
					Vector2 vector = cowards(ref destiny);
					if ((float)life > (float)change * 1.25f)
					{
						life = 0u;
					}
				}
				else
				{
					Vector2 vector = circlesAttackOnDist(ref destiny);
				}
				break;
			case 8:
				if (change != 0 && life > change)
				{
					Vector2 vector = followAndShoot(ref destiny);
					if ((float)life > (float)change * 1.1f)
					{
						life = 0u;
					}
				}
				else
				{
					Vector2 vector = cowards(ref destiny);
				}
				break;
			case 9:
			{
				Vector2 vector = serpentMovement();
				if (followers.Count < 10)
				{
					AddFollower();
				}
				UpdateFollowers(pnumber, maxX, maxY);
				break;
			}
			case 10:
			{
				Vector2 vector = defaultAttack(ref destiny);
				break;
			}
			case 11:
			{
				Vector2 vector = oscillion(ref destiny);
				if (life > 300 && Game1.gameState != GameState.Challenge && Game1.gameState != GameState.Sidescroller)
				{
					Health -= 0.01f;
				}
				break;
			}
			case 12:
			{
				Vector2 vector = followAndShoot(ref destiny);
				isShooting = true;
				break;
			}
			case 99:
			{
				Vector2 vector = boss();
				break;
			}
			case 100:
				if (change != 0 && life > change)
				{
					if (position.Y < OLDposition.Y - 10f)
					{
						Vector2 vector = defaultAttack(ref OLDposition);
					}
					else
					{
						Vector2 vector = verticalFall(ref destiny);
					}
					if (Vector2.Distance(position, OLDposition) < 5f)
					{
						life = 0u;
					}
				}
				else
				{
					Vector2 vector = invader01(ref destiny);
				}
				if (position.Y > 900f)
				{
					position.Y -= 1100f;
				}
				if (position.X > 700f)
				{
					position.X -= 600f;
				}
				if (position.X < 0f)
				{
					position.X += 700f;
				}
				break;
			default:
			{
				Vector2 vector = defaultAttack(ref destiny);
				break;
			}
			}
		}
		if (position.X > 1920f || position.X < -640f || position.Y > 1080f || position.Y < -360f)
		{
			movingAngle = 0f;
			speed /= 2f;
			if (random.Next(100) < 30)
			{
				cadence = 0f - cadence;
			}
			else if (random.Next(1000) > 500)
			{
				cadence /= 2f;
			}
		}
		if (position.X >= 1920f)
		{
			position.X--;
		}
		if (position.X <= -640f)
		{
			position.X++;
		}
		if (position.Y <= -360f)
		{
			position.Y++;
		}
		if (position.Y >= 1080f)
		{
			position.Y--;
		}
		if (Health <= 0f)
		{
			Active = false;
		}
	}

	private void UpdateFollowers(int pnumber, int maxX, int maxY)
	{
		for (int i = 0; i < followers.Count; i++)
		{
			followers[i].Update(pnumber, position, maxX, maxY);
			followers[i].position = new Vector2(MathHelper.Lerp(followers[i].position.X, posReg[followers.Count - 1 - i].X, 1f), MathHelper.Lerp(followers[i].position.Y, posReg[followers.Count - 1 - i].Y, 1f));
			followers[i].scale = MathHelper.Lerp(followers[i].scale, 0.2f + (float)(followers.Count - 1 - i) / 50f, 1f);
			if (followers[i].Health <= 0f)
			{
				followers[i].Active = false;
			}
			if (!followers[i].Active)
			{
				followers.RemoveAt(i);
			}
		}
	}

	private Vector2 boss()
	{
		Vector2 zero = Vector2.Zero;
		if (frozen > 0f)
		{
			jump = 0;
			return zero;
		}
		state = stateControl.Update();
		change = (uint)(maximunHealth * 0.99f);
		if (change < 100)
		{
			change = 100u;
		}
		if (Health < (float)change && Health > 100f && state != EnemState.shoot && state != EnemState.prepare)
		{
			maximunHealth = change;
			jump++;
		}
		if (jump > 0)
		{
			jump++;
			if (jump > 100)
			{
				jump = 0;
				int num;
				do
				{
					num = random.Next(7);
				}
				while (num == lastLocation);
				lastLocation = num;
				spawning = 0f;
				scale = 0f;
			}
		}
		position = location[lastLocation];
		if (scale < 1f)
		{
			scale = MathHelper.Lerp(scale, topScale, 0.2f);
		}
		return zero;
	}

	private Vector2 randomMovement()
	{
		Vector2 zero = Vector2.Zero;
		if (frozen > 0f)
		{
			return zero;
		}
		if (scale < 1f)
		{
			scale = MathHelper.Lerp(scale, topScale, 0.2f);
		}
		zero = positionRandom - position;
		angle = (float)Math.Atan2(zero.Y, zero.X);
		position.X += (int)(Math.Cos(angle) * (double)speed);
		position.Y += (int)(Math.Sin(angle) * (double)speed);
		if (Vector2.Distance(position, positionRandom) < dist)
		{
			positionRandom = new Vector2(random.Next(-560, 1800), random.Next(-300, 1100));
			speed = 0f;
		}
		drawingAngle = MathHelper.Lerp(drawingAngle, angle, 0.1f);
		drawingAngle = angle;
		speed = MathHelper.Lerp(speed, topSpeed, 0.1f);
		return zero;
	}

	private Vector2 serpentMovement()
	{
		Vector2 zero = Vector2.Zero;
		if (frozen > 0f)
		{
			return zero;
		}
		if (scale < 1f)
		{
			scale = MathHelper.Lerp(scale, topScale, 0.2f);
		}
		zero = positionRandom - position;
		angle = Math2.TurnToFace(position, positionRandom, angle, ((float)Math.Sin((float)life / 10f) + 2f) / 100f);
		position += Math2.AdvanceAngle(angle, speed);
		if (life % change == 0 || Vector2.Distance(position, positionRandom) < 20f)
		{
			positionRandom = new Vector2(random.Next(-560, 1800), random.Next(-300, 1100));
			change = (uint)random.Next(300, 1000);
		}
		drawingAngle = MathHelper.Lerp(drawingAngle, angle, 0.1f);
		drawingAngle = angle;
		speed = MathHelper.Lerp(speed, topSpeed, 0.1f);
		return zero;
	}

	private Vector2 defaultAttack(ref Vector2 destiny)
	{
		Vector2 zero = Vector2.Zero;
		if (frozen > 0f)
		{
			return zero;
		}
		speed = MathHelper.Lerp(speed, topSpeed, (float)life / 500f);
		if (scale < 0.99f)
		{
			scale = MathHelper.Lerp(scale, topScale, 0.2f);
		}
		if (targetWeight < 1f)
		{
			if (Game1.gameState == GameState.Challenge)
			{
				targetWeight += 0.01f;
			}
			else
			{
				targetWeight += 0.0025f;
			}
		}
		targetWeight = MathHelper.Clamp(targetWeight, 0f, 1f);
		zero = destiny - position;
		destinyAngle = Math2.TurnToFace(position, destiny, destinyAngle, speed / 100f);
		movingAngle += cadence;
		if (position.X > 1920f || position.X < -640f || position.Y > 1080f || position.Y < -360f)
		{
			movingAngle = 0f;
			speed /= 2f;
			if (random.Next(100) < 30)
			{
				cadence = 0f - cadence;
			}
			else if (random.Next(1000) > 500)
			{
				cadence /= 2f;
			}
		}
		angle = destinyAngle + (1f - targetWeight) * movingAngle / 60f;
		if (frozen <= 0f)
		{
			drawingAngle = angle;
		}
		position.X += (float)Math.Cos(angle) * speed;
		position.Y += (float)Math.Sin(angle) * speed;
		return zero;
	}

	private Vector2 oscillion(ref Vector2 destiny)
	{
		Vector2 zero = Vector2.Zero;
		if (frozen > 0f)
		{
			return zero;
		}
		if (targetWeight < 1f)
		{
			targetWeight += 0.0025f;
		}
		if (scale < 0.99f)
		{
			scale = MathHelper.Lerp(scale, topScale, 0.2f);
		}
		position.X += (float)Math.Cos((float)life / 95f) * (float)Math.Sin((float)life / 129f) * 0.01f;
		position.Y += (float)Math.Sin((float)life / 85f) * (float)Math.Cos((float)life / 116f) * 0.01f;
		if (frozen <= 0f)
		{
			drawingAngle += (float)Math.Sin((float)life / 75f) * 0.04f;
		}
		return zero;
	}

	private Vector2 invader01(ref Vector2 destiny)
	{
		Vector2 zero = Vector2.Zero;
		if (frozen > 0f)
		{
			return zero;
		}
		speed = MathHelper.Lerp(speed, topSpeed, (float)life / 500f);
		isShooting = false;
		if (random.Next(100) < 2)
		{
			isShooting = true;
		}
		if (scale < 0.99f)
		{
			scale = MathHelper.Lerp(scale, topScale, 0.2f);
		}
		if (targetWeight < 1f)
		{
			targetWeight += 0.0025f;
		}
		targetWeight = MathHelper.Clamp(targetWeight, 0f, 1f);
		zero = destiny - position;
		destinyAngle = (float)Math.Atan2(zero.Y, zero.X);
		movingAngle += cadence;
		if (position.X > 1920f || position.X < -640f || position.Y > 1080f || position.Y < -360f)
		{
			movingAngle = 0f;
			speed /= 2f;
			if (random.Next(100) < 30)
			{
				cadence = 0f - cadence;
			}
			else if (random.Next(1000) > 500)
			{
				cadence /= 2f;
			}
		}
		angle = destinyAngle + (1f - targetWeight) * movingAngle / 60f;
		drawingAngle = (float)Math.PI / 2f;
		position.X += (float)Math.Sin((float)life / (speed * 20f)) * 1f;
		position.Y += (float)Math.Sin((float)life / (speed * 2f)) * 0.5f;
		return zero;
	}

	private Vector2 verticalFall(ref Vector2 destiny)
	{
		Vector2 zero = Vector2.Zero;
		speed = MathHelper.Lerp(speed, topSpeed, (float)life / 500f);
		if (scale < 0.99f)
		{
			scale = MathHelper.Lerp(scale, topScale, 0.2f);
		}
		if (targetWeight < 1f)
		{
			targetWeight += 0.0025f;
		}
		targetWeight = MathHelper.Clamp(targetWeight, 0f, 1f);
		zero = destiny - position;
		destinyAngle = (float)Math.Atan2(zero.Y, zero.X);
		movingAngle += cadence;
		if (position.X > 1920f || position.X < -640f || position.Y > 1080f || position.Y < -360f)
		{
			movingAngle = 0f;
			speed /= 2f;
			if (random.Next(100) < 30)
			{
				cadence = 0f - cadence;
			}
			else if (random.Next(1000) > 500)
			{
				cadence /= 2f;
			}
		}
		angle = destinyAngle + (1f - targetWeight) * movingAngle / 60f;
		drawingAngle = (float)Math.PI / 2f;
		position.X += (float)(Math.Cos((float)life / (speed / 0.12f)) * 5.0);
		position.Y += speed / 0.5f;
		return zero;
	}

	private Vector2 cowards(ref Vector2 destiny)
	{
		Vector2 zero = Vector2.Zero;
		if (frozen > 0f)
		{
			return zero;
		}
		speed = MathHelper.Lerp(speed, topSpeed, 0.1f);
		positionRandom = position + new Vector2((float)Math.Sin((float)life / 50f) * 7f, (float)Math.Sin((float)life / 45f) * 8f);
		scale = MathHelper.Lerp(scale, topScale + (float)Math.Sin(life / 88) * 0.1f, 0.2f);
		if (Vector2.Distance(position, destiny) < dist)
		{
			targetWeight = MathHelper.Clamp(targetWeight, 1f, 0.8f);
		}
		else
		{
			targetWeight = MathHelper.Clamp(targetWeight, 0f, 0.8f);
		}
		destiny = positionRandom * targetWeight + destiny * (1f - targetWeight);
		zero = destiny - position;
		destinyAngle = (float)Math.Atan2(zero.Y, zero.X);
		movingAngle += cadence;
		angle = destinyAngle;
		drawingAngle += MathHelper.Clamp((float)Math.Sin(life / 100) * Vector2.Distance(position, destiny) / 200f, -0.1f, 0.1f);
		position.X += (float)(Math.Cos(angle) * (double)speed + (double)((float)Math.Sin(life / 55) * 3f));
		position.Y += (float)(Math.Sin(angle) * (double)speed + (double)((float)Math.Sin(life / 66) * 3f));
		return zero;
	}

	private Vector2 circlesAttackOnDist(ref Vector2 destiny)
	{
		Vector2 zero = Vector2.Zero;
		if (frozen > 0f)
		{
			return zero;
		}
		speed = MathHelper.Lerp(speed, topSpeed, 0.1f);
		positionRandom = position + new Vector2((float)Math.Cos((float)life / 20f) * 26f, (float)Math.Sin((float)life / 15f) * 22f);
		scale = MathHelper.Lerp(scale, topScale + (float)Math.Sin(life / 88) * 0.1f, 0.2f);
		if (Vector2.Distance(position, destiny) < dist)
		{
			targetWeight = MathHelper.Clamp(targetWeight, 1f, 0.2f);
		}
		else
		{
			targetWeight = MathHelper.Clamp(targetWeight, 0f, 0.1f);
		}
		destiny = destiny * targetWeight + positionRandom * (1f - targetWeight);
		zero = destiny - position;
		destinyAngle = (float)Math.Atan2(zero.Y, zero.X);
		movingAngle += cadence;
		angle = destinyAngle;
		if (frozen > 0f)
		{
			return zero;
		}
		drawingAngle += MathHelper.Clamp((float)Math.Sin(life / 150), -0.5f, 0.5f);
		position.X += (int)(Math.Cos(angle) * (double)speed + (double)((float)Math.Sin(life / 55) * 2f));
		position.Y += (int)(Math.Sin(angle) * (double)speed + (double)((float)Math.Sin(life / 66) * 2f));
		return zero;
	}

	private Vector2 followAndShoot(ref Vector2 destiny)
	{
		Vector2 zero = Vector2.Zero;
		if (frozen > 0f)
		{
			return zero;
		}
		if (Vector2.Distance(position, destiny) < dist)
		{
			speed = MathHelper.Lerp(speed, 0f, 0.1f);
			isShooting = true;
		}
		else
		{
			speed = MathHelper.Lerp(speed, topSpeed, 0.05f);
		}
		if (scale < 0.99f)
		{
			scale = MathHelper.Lerp(scale, topScale, 0.2f);
		}
		if (targetWeight < 1f)
		{
			targetWeight += 0.01f;
		}
		targetWeight = MathHelper.Clamp(targetWeight, 0f, 1f);
		zero = destiny - position;
		destinyAngle = (float)Math.Atan2(zero.Y, zero.X);
		movingAngle += cadence;
		if (enemyType == 12)
		{
			angle = Math2.TurnToFace(position, destiny, angle, 0.15f);
		}
		else
		{
			angle = Math2.TurnToFace(position, destiny, angle, 0.05f);
		}
		drawingAngle = angle;
		position.X += (int)(Math.Cos(angle) * (double)speed);
		position.Y += (int)(Math.Sin(angle) * (double)speed);
		return zero;
	}

	public void UpdateAsteroid()
	{
		position += direction;
		if (position.X < -740f)
		{
			position.X = 1919f;
		}
		if (position.X > 2020f)
		{
			position.X = -639f;
		}
		if (position.Y < -500f)
		{
			position.Y = 1199f;
		}
		if (position.Y > 1300f)
		{
			position.Y = -399f;
		}
		scale = MathHelper.Clamp(topScale, 0.2f, 1f);
		drawingAngle += movingAngle;
		if (Health <= 0f)
		{
			Active = false;
		}
	}

	public void Draw(SpriteBatch spriteBatch, float opac)
	{
		Draw(spriteBatch, player1Active: false, Vector2.Zero, player2Active: false, Vector2.Zero, player3Active: false, Vector2.Zero, player4Active: false, Vector2.Zero, opac);
	}

	public void Draw(SpriteBatch spriteBatch, bool player1Active, Vector2 player1Position, bool player2Active, Vector2 player2Position, bool player3Active, Vector2 player3Position, bool player4Active, Vector2 player4Position, float opac)
	{
		if (!Active)
		{
			return;
		}
		Color color = Color.White;
		if (frozen > 0f)
		{
			color = new Color(0.5f, 0.8f, 1.2f, 2f);
		}
		if (enemyType > 0 && enemyType != 10)
		{
			if (player1Active)
			{
				spriteBatch.Draw(txArrow, player1Position, null, new Color(1f, 1f, 1f, 0.5f) * opac * 0.1f, (float)Math.Atan2(position.Y - player1Position.Y, position.X - player1Position.X), new Vector2((float)txArrow.Width / 2f, (float)txArrow.Height / 2f), 1f, SpriteEffects.None, 0f);
			}
			if (player2Active)
			{
				spriteBatch.Draw(txArrow, player2Position, null, new Color(1f, 1f, 1f, 0.5f) * opac * 0.1f, (float)Math.Atan2(position.Y - player2Position.Y, position.X - player2Position.X), new Vector2((float)txArrow.Width / 2f, (float)txArrow.Height / 2f), 1f, SpriteEffects.None, 0f);
			}
			if (player3Active)
			{
				spriteBatch.Draw(txArrow, player3Position, null, new Color(1f, 1f, 1f, 0.5f) * opac * 0.1f, (float)Math.Atan2(position.Y - player3Position.Y, position.X - player3Position.X), new Vector2((float)txArrow.Width / 2f, (float)txArrow.Height / 2f), 1f, SpriteEffects.None, 0f);
			}
			if (player4Active)
			{
				spriteBatch.Draw(txArrow, player4Position, null, new Color(1f, 1f, 1f, 0.5f) * opac * 0.1f, (float)Math.Atan2(position.Y - player4Position.Y, position.X - player4Position.X), new Vector2((float)txArrow.Width / 2f, (float)txArrow.Height / 2f), 1f, SpriteEffects.None, 0f);
			}
		}
		if (spawning < 1f && enemyType != 10)
		{
			spriteBatch.Draw(textureSpawning, position, null, new Color(1f, (1f - spawning) / 2f, (1f - spawning) / 5f, spawning / 2f), angle * spawning / 10f, new Vector2(textureSpawning.Width / 2, textureSpawning.Height / 2), (1f - spawning) * 0.5f + 0.1f, SpriteEffects.None, 0f);
			spriteBatch.Draw(textureSpawning2, position, null, new Color(1f, (1f - spawning) / 1.2f, (1f - spawning) / 2f, spawning / 2f), angle / 10f / (spawning / 10f) / (float)Math.PI, new Vector2(textureSpawning2.Width / 2, textureSpawning2.Height / 2), (1f - spawning) * 1.5f + 0.2f, SpriteEffects.None, 0f);
			spriteBatch.Draw(textureSpawning2, position, null, new Color(1f, (1f - spawning) / 1.2f, (1f - spawning) / 2f, spawning / 2f), angle * 13.11f * (spawning * (float)Math.PI), new Vector2(textureSpawning2.Width / 2, textureSpawning2.Height / 2), (1f - spawning) * 2.5f + 0.3f, SpriteEffects.None, 0f);
			return;
		}
		switch (enemyType)
		{
		case 1:
			spriteBatch.Draw(texture, position, null, color, drawingAngle, new Vector2(Width / 2, Height / 2), scale / 2f, SpriteEffects.None, 0.0915f);
			break;
		case 2:
			spriteBatch.Draw(texture, position, null, color, drawingAngle, new Vector2(Width / 2, Height / 2), scale / 2f, SpriteEffects.None, 0.0915f);
			break;
		case 9:
		{
			spriteBatch.Draw(texture, position, null, color, drawingAngle, new Vector2(Width / 2, Height / 2), scale / 2f, se, 0.0915f);
			for (int i = 0; i < followers.Count; i++)
			{
				followers[i].Draw(spriteBatch, opac);
			}
			break;
		}
		case 99:
			spriteBatch.Draw(texture, position + new Vector2(-100f, 50f), null, Color.Black * 0.25f, 0f, new Vector2(Width / 2, Height / 2), scale * 1.1f, se, 0.0924f);
			spriteBatch.Draw(texture, position, null, color, 0f, new Vector2(Width / 2, Height / 2), scale, se, 0.0925f);
			if (jump > 70)
			{
				spriteBatch.Draw(texture, position, null, new Color((float)(jump - 70) / 20f, (float)(jump - 70) / 20f, (float)(jump - 70) / 20f, (float)(jump - 70) / 20f), 0f, new Vector2(Width / 2, Height / 2), scale, se, 0.0926f);
			}
			break;
		default:
			spriteBatch.Draw(texture, position, null, color, drawingAngle, new Vector2(Width / 2, Height / 2), scale / 2f, se, 0.0915f);
			break;
		}
	}

	public void DrawNodBar(SpriteBatch spriteBatch, GraphicsDevice GraphicsDevice, float GOHUDopacity, Texture2D whitePixel, Texture2D txColonyCORE)
	{
		Color color = new Color(0.101960786f, 16f / 85f, 1f) * (GOHUDopacity / 50f);
		Vector2 vector = new Vector2(GraphicsDevice.Viewport.Width / 2 - txColonyCORE.Width / 2 + 235, GraphicsDevice.Viewport.Height - txColonyCORE.Height - 40 + 25);
		Rectangle destinationRectangle = new Rectangle((int)vector.X, (int)vector.Y, (int)(Health / 2000f * 550f), 18);
		spriteBatch.Draw(whitePixel, destinationRectangle, Color.Red);
	}
}

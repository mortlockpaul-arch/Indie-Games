using System;
using Infinity.Scenes;
using InfinityLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ParticleLibrary;

namespace Infinity.GameObjects;

public class MissileManager : DrawableGameComponent
{
	private readonly TimeSpan SightDefaultWait = new TimeSpan(0, 0, 0, 0, 100);

	private Random random = new Random();

	private HormingMissile[] missiles;

	private GameSettings gameSettings;

	public ParticleSystem SmokeParticle { get; set; }

	public HormingMissile[] Missiles => missiles;

	public TimeSpan LockWait { get; private set; }

	public int Reserb { get; private set; }

	public event Action<Vector3> Explosion;

	public event Action<MainGame.SoundFlag> EntrySE;

	public MissileManager(Game game)
		: base(game)
	{
	}

	public override void Initialize()
	{
		ContentManager content = ((GameComponent)this).Game.Content;
		gameSettings = content.Load<GameSettings>("GameSettings");
		missiles = new HormingMissile[32];
		for (int i = 0; i < missiles.Length; i++)
		{
			missiles[i] = new HormingMissile(((GameComponent)this).Game);
			missiles[i].Dispose();
			missiles[i].Explosion += delegate(Vector3 position)
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				if (Explosion != null)
				{
					Explosion(position);
				}
			};
		}
		((DrawableGameComponent)this).Initialize();
	}

	public void Dispose(int index)
	{
		if (index >= 0)
		{
			missiles[index].Dispose();
		}
	}

	public void Clear()
	{
		HormingMissile[] array = missiles;
		foreach (HormingMissile hormingMissile in array)
		{
			hormingMissile.Dispose();
		}
	}

	public override void Update(GameTime gameTime)
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (LockWait.TotalSeconds > 0.0)
		{
			LockWait -= gameTime.ElapsedGameTime;
		}
		for (int i = 0; i < missiles.Length; i++)
		{
			HormingMissile hormingMissile = missiles[i];
			if (!hormingMissile.Use)
			{
				continue;
			}
			if (hormingMissile.Limit > 180)
			{
				if (Explosion != null)
				{
					Explosion(hormingMissile.GetPosition());
				}
				DisableReserb(i);
				hormingMissile.Dispose();
			}
			else
			{
				hormingMissile.Update(gameTime.ElapsedGameTime);
				if (SmokeParticle != null)
				{
					SmokeParticle.AddParticle(hormingMissile.GetPosition(), Vector3.Zero);
				}
			}
		}
		((GameComponent)this).Update(gameTime);
	}

	public override void Draw(GameTime gameTime)
	{
		HormingMissile[] array = missiles;
		foreach (HormingMissile hormingMissile in array)
		{
			if (hormingMissile.Use)
			{
				hormingMissile.Draw(gameTime);
			}
		}
		((DrawableGameComponent)this).Draw(gameTime);
	}

	public bool LockCheck(EnemyData enemy)
	{
		if (LockWait.TotalSeconds > 0.0)
		{
			return false;
		}
		int missileIndex = GetMissileIndex();
		if (missileIndex < 0)
		{
			return false;
		}
		Reserb |= 1 << missileIndex;
		enemy.LockOn(missileIndex);
		LockWait = SightDefaultWait;
		return true;
	}

	private int GetMissileIndex()
	{
		for (int i = 0; i < missiles.Length && i < gameSettings.MissileMaxCount; i++)
		{
			int num = 1 << i;
			if ((Reserb & num) == 0)
			{
				return i;
			}
		}
		return -1;
	}

	public void CreateMissile(Vector3 position, EnemyData enemy)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (enemy != null && enemy.LockOnIndex >= 0)
		{
			_ = enemy.LockOnIndex;
			HormingMissile hormingMissile = missiles[enemy.LockOnIndex];
			if (!hormingMissile.Use)
			{
				hormingMissile.Dispose();
				hormingMissile.Use = true;
				hormingMissile.Enable = true;
				hormingMissile.Visible = true;
				hormingMissile.Position = position;
				hormingMissile.Velocity.X = (float)random.NextDouble() * 0.1f - 0.05f;
				hormingMissile.Velocity.Y = -0.05f;
				hormingMissile.Velocity.Z = 0.5f;
				hormingMissile.Limit = 0;
				hormingMissile.Target = enemy;
				EntrySE(MainGame.SoundFlag.Missile);
			}
		}
	}

	public void DisableReserb(int index)
	{
		if (index >= 0)
		{
			int num = 1 << index;
			Reserb &= ~num;
		}
	}

	public void DisableReserb()
	{
		Reserb = 0;
	}
}

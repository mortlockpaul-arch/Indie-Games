using System;
using System.IO;
using Eyehook.Framework;
using Loot.Effects;
using Loot.Statuses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Dungeon;

public abstract class Character : BinaryRW, DamageSource
{
	protected int level;

	private Location location;

	private Location lastLocation;

	public Stats Stats;

	public Status Status = new Status();

	private int hp;

	private int maxHP;

	protected TimeSpan attackDelay = TimeSpan.Zero;

	private TimeSpan attackDelayDuration = TimeSpan.FromMilliseconds(350.0);

	protected float MoveSpeed = 5f;

	protected Vector2 currentOffset;

	protected Vector2 moveOffset;

	protected float moveLerp;

	public int Level => level;

	public abstract string Name { get; }

	public Location Location => location;

	public Location LastLocation => lastLocation;

	public virtual int HP
	{
		get
		{
			return hp;
		}
		set
		{
			hp = value;
			if (hp < 0)
			{
				hp = 0;
			}
			if (hp > maxHP)
			{
				hp = maxHP;
			}
		}
	}

	public virtual int MaxHP
	{
		get
		{
			return maxHP;
		}
		set
		{
			maxHP = value;
		}
	}

	public virtual bool CanFly => false;

	public virtual bool CanOpenDoors => false;

	public virtual bool IsDead => hp <= 0;

	public bool IsCritical => (double)hp <= 0.25 * (double)MaxHP;

	public bool IsBusy => IsAttacking || IsMoving;

	public bool IsAttacking => attackDelay > TimeSpan.Zero;

	protected virtual TimeSpan AttackDelayDuration => attackDelayDuration;

	protected virtual float CritOdds => 0f;

	public bool IsMoving => currentOffset != Vector2.Zero;

	public Vector2 Offset => currentOffset;

	public Character()
	{
	}

	public Character(BinaryReader reader)
	{
		Read(reader);
	}

	public virtual void SetLocation(Location loc)
	{
		moveOffset = Vector2.Zero;
		currentOffset = Vector2.Zero;
		moveLerp = 0f;
		lastLocation = loc;
		location = loc;
	}

	public virtual void MoveTo(Location loc)
	{
		if (!Status.Is<StatusFreeze>())
		{
			lastLocation = location;
			moveOffset = Location.ViewOffset(loc);
			currentOffset = moveOffset;
			moveLerp = 1f;
			location = loc;
		}
	}

	public void Update(GameTime gameTime)
	{
		UpdateStatus(gameTime);
		if (IsDead)
		{
			OnDeath();
			return;
		}
		UpdateAttackDelay(gameTime);
		UpdateMovement(gameTime);
		if (!IsBusy)
		{
			UpdateAI(gameTime);
		}
		UpdateSprite(gameTime);
	}

	public virtual void UpdateStatus(GameTime gameTime)
	{
		Status.Update(gameTime, this);
	}

	public virtual void UpdateAttackDelay(GameTime gameTime)
	{
		if (attackDelay > TimeSpan.Zero)
		{
			attackDelay -= gameTime.ElapsedGameTime;
			if (attackDelay < TimeSpan.Zero)
			{
				attackDelay = TimeSpan.Zero;
			}
		}
	}

	public virtual void UpdateMovement(GameTime gameTime)
	{
		if (!((double)moveLerp <= 0.0))
		{
			moveLerp -= MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
			currentOffset = Vector2.Lerp(Vector2.Zero, moveOffset, moveLerp);
			if (!((double)moveLerp > 0.0))
			{
				currentOffset = Vector2.Zero;
				moveLerp = 0f;
				DM.EndMove(this);
			}
		}
	}

	public virtual void UpdateAI(GameTime gameTime)
	{
	}

	public virtual void UpdateSprite(GameTime gameTime)
	{
	}

	public virtual void Attack(Character defender)
	{
		if (Status.Is<StatusFreeze>())
		{
			return;
		}
		int num = 0;
		bool flag = false;
		if (defender.Status.Is<StatusFreeze>())
		{
			flag = true;
		}
		else if ((double)CritOdds > 0.0 && DM.Random.NextDouble() <= (double)CritOdds)
		{
			flag = true;
		}
		if (flag)
		{
			num = calcDmg(Stats.DMG.Damage + Stats.DMG.Damage, defender.Stats.DEF);
			if (num <= 0)
			{
				num = 1;
			}
			flag = true;
		}
		else if (DM.Random.NextDouble() <= (double)hitOdds(Stats.DEX, defender.Stats.DEX))
		{
			num = calcDmg(Stats.DMG.Damage, defender.Stats.DEF);
			if (num <= 0)
			{
				num = 1;
			}
		}
		else
		{
			Miss();
		}
		if (num > 0)
		{
			defender.OnHit(num, flag, this);
		}
		attackDelay = AttackDelayDuration;
	}

	private float hitOdds(float atkDex, float defDex)
	{
		return (float)(1.0 / (1.0 + Math.Pow(8.0, 0.0 - ((double)atkDex / (double)defDex - 0.5))));
	}

	private int calcDmg(int atkDmg, int defDef)
	{
		return (int)((double)atkDmg * (1.0 - (double)defMod(defDef)));
	}

	private float defMod(int def)
	{
		return (float)(1.0 / (1.0 + Math.Pow(4.0, 0.0 - (double)((float)def / 50f))) - 0.5) * 1.8f;
	}

	protected virtual void Miss()
	{
	}

	public virtual void OnHit(int dmg, bool crit, DamageSource src)
	{
		if (Status.Is<StatusFreeze>())
		{
			DM.AddEffect(FXIce.GetFX(Location));
			Status.RemoveAll<StatusFreeze>();
		}
		if (dmg > 0)
		{
			HP -= dmg;
			if (DM.Map.IsDiscovered(Location))
			{
				DM.AddEffect(FXHit.GetFX(this, crit));
			}
		}
	}

	public abstract void OnDeath();

	public abstract void Draw(SpriteBatch spriteBatch);

	public virtual void Read(BinaryReader reader)
	{
		level = reader.ReadInt32();
		location.Read(reader);
		Stats.Read(reader);
		Status = new Status(reader);
		hp = reader.ReadInt32();
		maxHP = reader.ReadInt32();
	}

	public virtual void Write(BinaryWriter writer)
	{
		writer.Write(level);
		location.Write(writer);
		Stats.Write(writer);
		Status.Write(writer);
		writer.Write(hp);
		writer.Write(maxHP);
	}
}

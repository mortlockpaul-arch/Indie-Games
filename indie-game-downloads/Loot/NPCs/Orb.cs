using System;
using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.NPCs;

public class Orb : NPC
{
	private enum State
	{
		Ready,
		Deploying,
		Deployed,
		Undeploying,
		Attacking,
		Recharging
	}

	private static Texture2D texture;

	private AnimatedSprite moving;

	private AnimatedSprite deploying;

	private StillSprite deployed;

	private AnimatedSprite undeploying;

	private RandomSprite attacking;

	private StillSprite hit;

	private State state;

	private int damage;

	private TimeSpan orbRechargeDelay;

	private TimeSpan orbRechargeDuration = TimeSpan.FromMilliseconds(150.0);

	private TimeSpan orbAttackDelay;

	private TimeSpan orbAttackDuration = TimeSpan.FromMilliseconds(300.0);

	private MoveMap moveMap;

	private float hitRotation;

	private TimeSpan hitTimer = TimeSpan.Zero;

	private TimeSpan hitDuration = TimeSpan.FromMilliseconds(250.0);

	private NPC target;

	public override string Name => "Orb";

	protected override NPCSprite.Info SpriteInfo => null;

	public override bool CanFly => true;

	public Orb(BinaryReader reader)
		: base(reader)
	{
		IsAlly = true;
		Init();
	}

	public Orb(int level, Location loc)
		: base(level, loc)
	{
		IsAlly = true;
		Init();
		UpdateStats();
		CreateMoveMap();
	}

	public void Reset()
	{
		target = null;
		state = State.Ready;
		hitTimer = TimeSpan.Zero;
		orbRechargeDelay = TimeSpan.Zero;
		orbAttackDelay = TimeSpan.Zero;
		CreateMoveMap();
	}

	private void Init()
	{
		state = State.Ready;
		moving = new AnimatedSprite(texture, new Rectangle(0, 0, 64, 64), new Vector2(32f, 32f), 8, TimeSpan.FromMilliseconds(100.0));
		deploying = new AnimatedSprite(texture, new Rectangle(0, 64, 64, 64), new Vector2(32f, 32f), 4, TimeSpan.FromMilliseconds(100.0));
		deployed = new StillSprite(texture, new Rectangle(256, 64, 64, 64), new Vector2(32f, 32f));
		undeploying = new AnimatedSprite(texture, new Rectangle(256, 64, 64, 64), new Vector2(32f, 32f), 4, TimeSpan.FromMilliseconds(100.0));
		attacking = new RandomSprite(texture, new Rectangle(0, 128, 64, 64), new Vector2(32f, 64f), 8, TimeSpan.FromMilliseconds(100.0));
		hit = new StillSprite(texture, new Rectangle(0, 192, 64, 64), new Vector2(32f, 32f));
	}

	public void UpdateStats()
	{
		MaxHP = DM.Player.MaxHP / 2;
		HP = MaxHP;
		Stats.DMG = DMGRange.Zero;
		Stats.DEF = 0;
		Stats.DEX = 0;
		Stats.LCK = 0;
		damage = (int)Math.Ceiling((double)(DM.Player.Level * DM.Player.SkillSet.Orb.Level) * 0.25);
	}

	public void CreateMoveMap()
	{
		moveMap = new MoveMap(DM.Map, base.Location, 50);
	}

	public static void Load(ContentManager content)
	{
		texture = content.Load<Texture2D>("Sprites\\Mobs\\Orb");
	}

	public override void OnHit(int dmg, bool crit, DamageSource src)
	{
		HP -= dmg;
		hitTimer = hitDuration;
		hitRotation = (float)(Math.PI * 2.0 * DM.Random.NextDouble());
	}

	public override void OnDeath()
	{
		DM.RemoveNPC(this);
		DM.AddEffect(FXText.GetFX(DM.Player.Location, "Orb Destroyed!", new Color(255, 229, 155), new Color(62, 40, 26)));
		DM.AddEffect(FXOrbGore.GetFX(this));
		DM.Player.Orb = null;
	}

	public void CastNova()
	{
		nova();
		DM.RemoveNPC(this);
		DM.AddEffect(FXOrbNova.GetFX(this));
		DM.Player.Orb = null;
	}

	private void nova()
	{
		for (int i = base.Location.Row - 2; i <= base.Location.Row + 2; i++)
		{
			for (int j = base.Location.Col - 2; j <= base.Location.Col + 2; j++)
			{
				Location location = new Location(i, j);
				if (DM.Map.IsValid(location) && (double)base.Location.Distance(location) <= 2.5)
				{
					NPC nPC = DM.Map.GetNPC(location);
					if (nPC != null && !nPC.IsAlly && DM.LineOfSight(base.Location, location))
					{
						nPC.OnHit(damage * 2, crit: true, this);
					}
				}
			}
		}
	}

	public override void UpdateSprite(GameTime gameTime)
	{
		switch (state)
		{
		case State.Ready:
			moving.Update(gameTime);
			break;
		case State.Deploying:
			deploying.Update(gameTime);
			if (deploying.HasLooped)
			{
				state = State.Deployed;
			}
			break;
		case State.Undeploying:
			undeploying.Update(gameTime);
			if (undeploying.HasLooped)
			{
				state = State.Ready;
				moving.Reset();
			}
			break;
		case State.Attacking:
			attacking.Update(gameTime);
			break;
		case State.Deployed:
			break;
		}
	}

	public override void UpdateStatus(GameTime gameTime)
	{
		base.UpdateStatus(gameTime);
		if (hitTimer > TimeSpan.Zero)
		{
			hitTimer -= gameTime.ElapsedGameTime;
		}
	}

	public override void UpdateAI(GameTime gameTime)
	{
		if (state == State.Ready)
		{
			Location location = base.Location;
			NPC nPC = FindTarget();
			if (nPC != null)
			{
				if ((double)nPC.Location.Distance(base.Location) < 1.5)
				{
					state = State.Deploying;
					deploying.Reset();
					return;
				}
				moveMap.Generate(nPC.Location);
				location = moveMap.PathToTarget(base.Location);
			}
			else if ((double)base.Location.Distance(DM.Player.Location) > 1.5)
			{
				location = DM.PathToPlayer(base.Location);
			}
			if (location != base.Location)
			{
				DM.MoveNPC(this, location);
			}
		}
		if (state == State.Deploying || state == State.Undeploying)
		{
			return;
		}
		if (state == State.Deployed)
		{
			target = FindTargetInRange();
			if (target != null)
			{
				state = State.Attacking;
				attacking.Reset();
				orbAttackDelay = orbAttackDuration;
				PlaySound.Zot();
			}
			else
			{
				state = State.Undeploying;
				undeploying.Reset();
			}
		}
		else if (state == State.Attacking)
		{
			orbAttackDelay -= gameTime.ElapsedGameTime;
			if (orbAttackDelay <= TimeSpan.Zero)
			{
				target.OnHit(damage, crit: false, this);
				state = State.Recharging;
				orbRechargeDelay = orbRechargeDuration;
			}
		}
		else if (state == State.Recharging)
		{
			orbRechargeDelay -= gameTime.ElapsedGameTime;
			if (orbRechargeDelay <= TimeSpan.Zero)
			{
				state = State.Deployed;
			}
		}
	}

	private NPC FindTargetInRange()
	{
		for (int i = base.Location.Row - 1; i <= base.Location.Row + 1; i++)
		{
			for (int j = base.Location.Col - 1; j <= base.Location.Col + 1; j++)
			{
				NPC nPC = DM.Map.GetNPC(new Location(i, j));
				if (nPC != null && !nPC.IsAlly)
				{
					return nPC;
				}
			}
		}
		return null;
	}

	private NPC FindTarget()
	{
		moveMap.Generate(base.Location);
		NPC result = null;
		byte b = byte.MaxValue;
		for (int i = base.Location.Row - 3; i <= base.Location.Row + 3; i++)
		{
			for (int j = base.Location.Col - 3; j <= base.Location.Col + 3; j++)
			{
				Location location = new Location(i, j);
				if (!DM.Map.IsValid(location))
				{
					continue;
				}
				NPC nPC = DM.Map.GetNPC(location);
				if (nPC != null && !nPC.IsAlly)
				{
					byte b2 = moveMap.TargetDistance(location);
					if (b2 < b)
					{
						result = nPC;
						b = b2;
					}
				}
			}
		}
		return result;
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		Draw(spriteBatch, Color.White);
	}

	public override void Draw(SpriteBatch spriteBatch, Color color)
	{
		Vector2 vector = DungeonView.Camera.Position(this);
		float zoom = DungeonView.Camera.Zoom;
		DrawHPBar(spriteBatch, vector, color);
		switch (state)
		{
		case State.Ready:
			moving.Draw(spriteBatch, vector, color, zoom);
			break;
		case State.Deploying:
			deploying.Draw(spriteBatch, vector, color, zoom);
			break;
		case State.Deployed:
			deployed.Draw(spriteBatch, vector, color, zoom);
			break;
		case State.Undeploying:
			undeploying.Draw(spriteBatch, vector, color, zoom);
			break;
		case State.Attacking:
			deployed.Draw(spriteBatch, vector, color, zoom);
			break;
		case State.Recharging:
			deployed.Draw(spriteBatch, vector, color, zoom);
			break;
		}
		if (hitTimer > TimeSpan.Zero)
		{
			hit.Rotation = hitRotation;
			hit.Draw(spriteBatch, vector, Color.White, zoom);
		}
		if (state == State.Attacking)
		{
			attacking.Rotation = (float)Math.Atan2(target.Location.Col - base.Location.Col, base.Location.Row - target.Location.Row);
			vector.Y += 4f;
			attacking.Draw(spriteBatch, vector, Color.White, zoom);
		}
	}

	public static Orb LoadOrb(BinaryReader reader)
	{
		return (!reader.ReadBoolean()) ? null : new Orb(reader);
	}

	public static void SaveOrb(BinaryWriter writer, Orb orb)
	{
		if (orb == null)
		{
			writer.Write(value: false);
			return;
		}
		writer.Write(value: true);
		orb.Write(writer);
	}

	public override void Read(BinaryReader reader)
	{
		base.Read(reader);
		Init();
		damage = reader.ReadInt32();
	}

	public override void Write(BinaryWriter writer)
	{
		base.Write(writer);
		writer.Write(damage);
	}
}

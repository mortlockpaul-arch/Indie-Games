using System;
using System.IO;
using Eyehook.Framework;
using Loot.Awardments;
using Loot.Core;
using Loot.Dungeon;
using Loot.Effects;
using Loot.Items;
using Loot.Items.Misc;
using Loot.Items.Potions;
using Loot.Items.Scrolls;
using Loot.NPCs;
using Loot.Screens;
using Loot.Skills;
using Loot.Statuses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.PC;

public abstract class Player : Character
{
	private const string potionCue = "potion";

	private TimeSpan attackDelayDuration = TimeSpan.FromMilliseconds(350.0);

	private int depth;

	public SkillSet SkillSet = new SkillSet();

	public Stats Base = default(Stats);

	public int XP;

	public int PrevXP;

	public int NextXP;

	public int StatPoints;

	public int SkillPoints;

	public int Gold;

	public int HealthPotions;

	public float ShopBonus;

	private bool dying;

	private TimeSpan deathTimer = TimeSpan.Zero;

	private readonly TimeSpan deathDuration = TimeSpan.FromMilliseconds(750.0);

	public Lantern Lantern = new Lantern();

	public Item[] Inventory = new Item[32];

	public Equipment[] Equipment = new Equipment[4];

	public Orb Orb;

	public Direction Facing = Direction.South;

	public ThoughtBubble Thought;

	private float attackAngle;

	private TimeSpan playTime = TimeSpan.Zero;

	private TimeSpan idleTime = TimeSpan.Zero;

	private static readonly TimeSpan IdleSpan = TimeSpan.FromMilliseconds(100.0);

	public static int SaveFileVersion = 0;

	public int KillCount;

	public bool HasChangedEquipment;

	private bool prevCrit;

	private bool statusPoison;

	private bool statusLevitate;

	private bool statusProtect;

	private bool statusInvisible;

	private bool getRichUnlocked;

	private bool tripleThreatUnlocked;

	public Vector2 LightOffset = Vector2.Zero;

	private Vector2 wallPoint = Vector2.Zero;

	private Color levelUpColor = new Color(255, 202, 0);

	private Color levelUpShadowColor = new Color(151, 100, 0);

	private Color xpColor = new Color(0, 0, 0);

	private Color xpShadowColor = Color.Transparent;

	private float levDelta;

	private float levOffset;

	private Color invisColor = new Color(0, 0, 0) * 0.25f;

	public override string Name => "Anonymous";

	protected override TimeSpan AttackDelayDuration => attackDelayDuration;

	public abstract string ClassName { get; }

	public abstract PlayerClassType ClassType { get; }

	public abstract PlayerSprite Sprite { get; }

	public abstract Sprite Portrait { get; }

	public override bool CanOpenDoors => true;

	public TimeSpan PlayTime => playTime;

	public int Depth => depth;

	public abstract Skill PrimarySkill { get; }

	protected override float CritOdds => (float)(0.02500000037252903 + (double)SkillSet.Perception.Level / 20.0);

	public override bool IsDead
	{
		get
		{
			if (HP > 0)
			{
				if (dying)
				{
					dying = false;
					DungeonView.Camera.ZoomTo(1f);
					Profile.Awardments.Unlock(Awardment.CloseCall);
				}
				return false;
			}
			if (dying)
			{
				return deathTimer == TimeSpan.Zero;
			}
			dying = true;
			deathTimer = deathDuration;
			DungeonView.Camera.ZoomTo(2f);
			return false;
		}
	}

	public bool IsDying => dying;

	public Armor Armor => (Armor)Equipment[0];

	public Weapon Weapon => (Weapon)Equipment[1];

	public Amulet Amulet => (Amulet)Equipment[2];

	public Ring Ring => (Ring)Equipment[3];

	public bool Frenzy => SkillSet.Frenzy.Active;

	private float AttackRotation
	{
		get
		{
			double num = 1.0 - attackDelay.TotalSeconds / AttackDelayDuration.TotalSeconds;
			return ((double)attackAngle <= 0.0) ? ((float)((double)attackAngle + Math.PI / 4.0 - Math.PI / 2.0 * num)) : ((float)((double)attackAngle - Math.PI / 4.0 + Math.PI / 2.0 * num));
		}
	}

	protected Player(BinaryReader reader)
		: base(reader)
	{
	}

	protected Player()
	{
		depth = 1;
		level = 1;
		StatPoints = 0;
		SkillPoints = 0;
		XP = 0;
		CalcNextLevelXP();
		MaxHP = 300;
		HP = MaxHP;
		if (DM.GameOptions.Difficulty == Difficulty.Easy)
		{
			SkillSet.Regen.Level = 1;
		}
	}

	public virtual void Intro()
	{
	}

	public void SetDepth(int depth)
	{
		this.depth = depth;
	}

	public void OnNextLevel()
	{
		AddOrb();
	}

	public void AddOrb()
	{
		if (Orb != null && !DM.NPCList.Contains(Orb))
		{
			Orb.SetLocation(base.Location);
			DM.AddNPC(Orb);
			Orb.Reset();
		}
	}

	protected override void Miss()
	{
		PlaySound.Miss();
	}

	public override void OnHit(int dmg, bool crit, DamageSource src)
	{
		prevCrit = base.IsCritical;
		if (statusProtect)
		{
			dmg /= 2;
		}
		base.OnHit(dmg, crit, src);
		if (base.IsCritical && !prevCrit)
		{
			PlaySound.Critical();
		}
		SkillSet.Regen.OnHit();
		int num = SkillSet.Thorns.Level;
		if (num > 0 && src is NPC nPC)
		{
			DM.AddEffect(FXThorns.GetFX(nPC));
			nPC.HP -= num * 3;
		}
	}

	private bool Resurrect()
	{
		int num = 0;
		CharmResurrect charmResurrect = null;
		for (int i = 0; i < Inventory.Length; i++)
		{
			Item item = Inventory[i];
			if (item != null && item is CharmResurrect)
			{
				num = i;
				charmResurrect = (CharmResurrect)item;
				break;
			}
		}
		if (charmResurrect == null)
		{
			return false;
		}
		charmResurrect.Resurrect();
		Inventory[num] = null;
		return true;
	}

	public override void OnDeath()
	{
		if (!Resurrect())
		{
			SaveScreen.GameOver();
		}
	}

	public void AddDefaultItems()
	{
		Gold = 500;
		AddItem(new PotionHealth());
		AddItem(new PotionHealth());
		AddItem(new PotionHealth());
		AddItem(new PotionOil());
		AddItem(new ScrollIdentify());
		if (DM.GameOptions.Difficulty == Difficulty.Easy)
		{
			AddItem(new CharmResurrect());
		}
	}

	public override void UpdateStatus(GameTime gameTime)
	{
		base.UpdateStatus(gameTime);
		statusPoison = Status.Is<StatusPoison>();
		statusLevitate = Status.Is<StatusLevitate>();
		statusProtect = Status.Is<StatusProtect>();
		statusInvisible = Status.Is<StatusInvisible>();
		if (!tripleThreatUnlocked && statusLevitate && statusProtect && statusInvisible)
		{
			Profile.Awardments.Unlock(Awardment.TripleThreat);
			tripleThreatUnlocked = true;
		}
		if (!getRichUnlocked && Gold > 10000)
		{
			Profile.Awardments.Unlock(Awardment.ImRich);
			getRichUnlocked = true;
		}
		playTime += gameTime.ElapsedGameTime;
		Lantern.Update(gameTime);
		SkillSet.Freeze.Update(gameTime);
		SkillSet.Frenzy.Update(gameTime);
		SkillSet.Regen.Update(gameTime);
		SkillSet.Orb.Update(gameTime);
		SkillSet.Poison.Update(gameTime);
		if (statusLevitate)
		{
			levDelta += (float)(gameTime.ElapsedGameTime.TotalSeconds * 3.1415927410125732);
			if ((double)levDelta > 6.2831854820251465)
			{
				levDelta -= (float)Math.PI * 2f;
			}
			levOffset = (float)(Math.Sin(levDelta) * 8.0) - 16f;
		}
		else
		{
			levDelta = 0f;
			if ((double)levOffset < 0.0)
			{
				levOffset += (float)(gameTime.ElapsedGameTime.TotalSeconds * 32.0);
				if ((double)levOffset > 0.0)
				{
					levOffset = 0f;
				}
			}
		}
		if (dying)
		{
			deathTimer -= gameTime.ElapsedGameTime;
			if (deathTimer < TimeSpan.Zero)
			{
				deathTimer = TimeSpan.Zero;
			}
		}
	}

	public override void MoveTo(Location loc)
	{
		LightOffset = Vector2.Zero;
		wallPoint = Vector2.Zero;
		if (base.Location.Row != loc.Row && base.Location.Col != loc.Col)
		{
			Location location = new Location(base.Location.Row, loc.Col);
			Location location2 = new Location(loc.Row, base.Location.Col);
			if (DM.Map.IsWall(location))
			{
				Location location3 = location - loc;
				wallPoint = new Vector2(location.Col - loc.Col, location.Row - loc.Row) * 64f;
			}
			else if (DM.Map.IsWall(location2))
			{
				Location location4 = location2 - loc;
				wallPoint = new Vector2(location2.Col - loc.Col, location2.Row - loc.Row) * 64f;
			}
		}
		base.MoveTo(loc);
	}

	public override void UpdateMovement(GameTime gameTime)
	{
		base.UpdateMovement(gameTime);
		if ((double)moveLerp > 0.0 && wallPoint != Vector2.Zero)
		{
			LightOffset = currentOffset - wallPoint;
			LightOffset.Normalize();
			LightOffset *= 64f;
			LightOffset += wallPoint - currentOffset;
		}
		else
		{
			LightOffset = Vector2.Zero;
		}
	}

	public override void UpdateSprite(GameTime gameTime)
	{
		Sprite.Update(gameTime);
		if (base.IsMoving)
		{
			idleTime = TimeSpan.Zero;
		}
		else if (idleTime < IdleSpan)
		{
			idleTime += gameTime.ElapsedGameTime;
			if (idleTime >= IdleSpan)
			{
				Sprite.Reset();
			}
		}
	}

	public override void Attack(Character defender)
	{
		if (!Frenzy)
		{
			Vector2 vector = new Vector2((defender.Location.Col - base.Location.Col) * 64 / 2, (defender.Location.Row - base.Location.Row) * 64 / 2);
			attackAngle = (float)Math.Atan2(vector.X, 0.0 - (double)vector.Y);
			if ((double)attackAngle == 3.1415927410125732)
			{
				attackAngle = -(float)Math.PI;
			}
			base.Attack(defender);
		}
	}

	public bool IsLucky()
	{
		return DM.Random.Next(100 + Stats.LCK) > 50;
	}

	public bool IsLucky(int value)
	{
		return DM.Random.Next(100 + Stats.LCK) > value;
	}

	public void CalcStats()
	{
		Stats = Base;
		for (int i = 0; i < Equipment.Length; i++)
		{
			Equipment equipment = Equipment[i];
			if (equipment != null)
			{
				Stats += equipment.Stats;
			}
		}
		if (Stats.LCK >= 100)
		{
			Profile.Awardments.Unlock(Awardment.Lucky);
			if ((Stats.DMG.Min + Stats.DMG.Max) / 2 >= 100 && Stats.DEF >= 100 && Stats.DEX >= 100)
			{
				Profile.Awardments.Unlock(Awardment.Invincible);
			}
		}
	}

	private void calcHealthPotions()
	{
		HealthPotions = 0;
		for (int i = 0; i < Inventory.Length; i++)
		{
			if (Inventory[i] is Potion potion && potion is PotionHealth)
			{
				HealthPotions++;
			}
		}
	}

	public void CalcNextLevelXP()
	{
		PrevXP = (base.Level - 1) * (base.Level - 1) * 23;
		NextXP = base.Level * base.Level * 23;
	}

	public virtual void AddXP(int xp)
	{
		int num = (int)Math.Round((double)xp * (double)(DM.GameOptions.Difficulty switch
		{
			Difficulty.Easy => 1.3333334f, 
			Difficulty.Normal => 1f, 
			Difficulty.Hard => 0.75f, 
			_ => throw new Exception("Unknown difficulty: " + DM.GameOptions.Difficulty), 
		}));
		XP += num;
		if (XP >= NextXP)
		{
			levelUp();
			DM.AddEffect(FXText.GetFX(base.Location, "Level Up!", levelUpColor, levelUpShadowColor));
		}
		else
		{
			DM.AddEffect(FXText.GetFX(base.Location, '+', num, " XP", xpColor, xpShadowColor));
		}
	}

	private void levelUp()
	{
		PlaySound.LevelUp();
		level++;
		StatPoints += 3;
		if (base.Level % 3 == 0)
		{
			SkillPoints++;
			Profile.Awardments.Unlock(Awardment.SkillUp);
		}
		MaxHP += 25;
		HP = MaxHP;
		CalcNextLevelXP();
		if (Orb != null)
		{
			Orb.UpdateStats();
		}
	}

	public void SortInventory()
	{
		Array.Sort(Inventory, Item.Comparator);
	}

	public Equipment GetEquipment(Type type)
	{
		if (type.IsSubclassOf(typeof(Armor)))
		{
			return DM.Player.Armor;
		}
		if (type.IsSubclassOf(typeof(Weapon)))
		{
			return DM.Player.Weapon;
		}
		if (type.IsSubclassOf(typeof(Ring)))
		{
			return DM.Player.Ring;
		}
		if (type.IsSubclassOf(typeof(Amulet)))
		{
			return DM.Player.Amulet;
		}
		throw new Exception("Unknown equipment type: " + GetType());
	}

	public int GetInventoryId(Item item)
	{
		for (int i = 0; i < Inventory.Length; i++)
		{
			if (Inventory[i] != null && Inventory[i] == item)
			{
				return i;
			}
		}
		return -1;
	}

	public void Equip(int id)
	{
		HasChangedEquipment = true;
		Item item = Inventory[id];
		Item item2 = item;
		Item item3 = item2;
		if (!(item3 is Armor))
		{
			if (!(item3 is Weapon))
			{
				if (!(item3 is Amulet))
				{
					if (!(item3 is Ring))
					{
						throw new Exception("Unknown equipment type: " + item);
					}
					equip(3, id);
				}
				else
				{
					equip(2, id);
				}
			}
			else
			{
				equip(1, id);
			}
		}
		else
		{
			equip(0, id);
		}
		CalcStats();
	}

	public void Unequip(int i)
	{
		if (Equipment[i] != null)
		{
			if (Equipment[i].Cursed)
			{
				Message.Display(Equipment[i].Name + " is cursed.");
			}
			else if (AddItem(Equipment[i]))
			{
				Equipment[i] = null;
				CalcStats();
			}
			else
			{
				Message.Display("Inventory full.");
			}
		}
	}

	private void equip(int eqId, int invId)
	{
		if (Equipment[eqId] != null && Equipment[eqId].Cursed)
		{
			Message.Display(Equipment[eqId].Name + " is cursed.");
			return;
		}
		Equipment equipment = Equipment[eqId];
		Equipment[eqId] = (Equipment)Inventory[invId];
		Equipment[eqId].Identify();
		Inventory[invId] = equipment;
		int num = 0;
		for (int i = 0; i < Equipment.Length; i++)
		{
			Equipment equipment2 = Equipment[i];
			if (equipment2 != null && equipment2.IsEpic)
			{
				num++;
			}
		}
		if (num > 0)
		{
			Profile.Awardments.Unlock(Awardment.EpicGet);
		}
		if (num == Equipment.Length)
		{
			Profile.Awardments.Unlock(Awardment.EpicSet);
		}
		if (Equipment[eqId].Cursed)
		{
			Profile.Awardments.Unlock(Awardment.Cursed);
		}
	}

	public void SetArmor(Armor armor)
	{
		Equipment[0] = armor;
		CalcStats();
	}

	public void ClearArmor()
	{
		Equipment[0] = null;
		CalcStats();
	}

	public virtual bool AddItem(Item item)
	{
		for (int i = 0; i < Inventory.Length; i++)
		{
			if (Inventory[i] == null)
			{
				Inventory[i] = item;
				calcHealthPotions();
				return true;
			}
		}
		return false;
	}

	public virtual void AddItemOrDrop(Item item)
	{
		if (item != null && !AddItem(item))
		{
			DM.Map.SetItem(DM.Map.FindNearest(DM.Player.Location, DM.Map.IsOpenFloor), item);
		}
	}

	public bool InventoryFull()
	{
		for (int i = 0; i < Inventory.Length; i++)
		{
			if (Inventory[i] == null)
			{
				return false;
			}
		}
		return true;
	}

	public void ConsumeHealthPotion()
	{
		for (int i = 0; i < Inventory.Length; i++)
		{
			if (Inventory[i] is PotionHealth potionHealth && HP < MaxHP)
			{
				potionHealth.Activate(i);
				break;
			}
		}
	}

	public void RemoveItem(int id)
	{
		Inventory[id] = null;
		calcHealthPotions();
	}

	public void DestroyItem(int id)
	{
		Item item = Inventory[id];
		if (item != null)
		{
			RemoveItem(id);
			Message.Display(item.Name + " destroyed.");
		}
	}

	public void setFacing(Location newLoc)
	{
		if (!Frenzy && !(newLoc == DM.Player.Location))
		{
			if (newLoc.Row < DM.Player.Location.Row)
			{
				DM.Player.Facing = Direction.North;
			}
			else if (newLoc.Row > DM.Player.Location.Row)
			{
				DM.Player.Facing = Direction.South;
			}
			else if (newLoc.Col < DM.Player.Location.Col)
			{
				DM.Player.Facing = Direction.West;
			}
			else if (newLoc.Col > DM.Player.Location.Col)
			{
				DM.Player.Facing = Direction.East;
			}
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		Vector2 center = DungeonView.Center;
		float zoom = DungeonView.Camera.Zoom;
		center.Y += levOffset * zoom;
		if ((double)levOffset != 0.0)
		{
			PlayerSprite.Levitate.Draw(spriteBatch, center, Color.White, zoom);
		}
		if ((base.IsAttacking || Frenzy) && Weapon != null)
		{
			Weapon.DrawAttack(spriteBatch, center, zoom, AttackRotation);
		}
		if (statusInvisible)
		{
			Sprite.DrawInvis(spriteBatch, center, zoom);
		}
		else if (statusPoison)
		{
			Sprite.DrawPoisoned(spriteBatch, center, zoom);
			if (Armor != null)
			{
				Armor.DrawOnPlayer(spriteBatch, center, zoom);
			}
		}
		else
		{
			Sprite.Draw(spriteBatch, center, zoom);
			if (Armor != null)
			{
				Armor.DrawOnPlayer(spriteBatch, center, zoom);
			}
		}
		if (statusProtect)
		{
			Sprite.DrawProtect(spriteBatch, center, zoom);
		}
	}

	public static Player Load(BinaryReader reader)
	{
		PlayerClassType playerClassType = (PlayerClassType)reader.ReadInt32();
		return playerClassType switch
		{
			PlayerClassType.Berserker => new Berserker(reader), 
			PlayerClassType.Shaman => new Shaman(reader), 
			PlayerClassType.Tinkerer => new Tinkerer(reader), 
			PlayerClassType.Gambler => new Gambler(reader), 
			PlayerClassType.Goblin => new Goblin(reader), 
			PlayerClassType.Peasant => new Peasant(reader), 
			_ => throw new Exception("Unknown PlayerClassType: " + playerClassType), 
		};
	}

	public static void Save(BinaryWriter writer, Player player)
	{
		writer.Write((int)player.ClassType);
		player.Write(writer);
	}

	public override void Read(BinaryReader reader)
	{
		base.Read(reader);
		playTime = TimeSpan.FromSeconds(reader.ReadDouble());
		depth = reader.ReadInt32();
		XP = reader.ReadInt32();
		Gold = reader.ReadInt32();
		if (SaveFileVersion > 0)
		{
			KillCount = reader.ReadInt32();
			HasChangedEquipment = reader.ReadBoolean();
		}
		else
		{
			KillCount = 0;
			HasChangedEquipment = true;
		}
		Base.Read(reader);
		SkillSet = new SkillSet(reader);
		StatPoints = reader.ReadInt32();
		SkillPoints = reader.ReadInt32();
		for (int i = 0; i < 4; i++)
		{
			Equipment[i] = (Equipment)ItemRegistry.Load(reader);
		}
		for (int j = 0; j < 32; j++)
		{
			Inventory[j] = ItemRegistry.Load(reader);
		}
		Orb = Loot.NPCs.Orb.LoadOrb(reader);
		Lantern.Read(reader);
		ShopBonus = reader.ReadSingle();
		calcHealthPotions();
		CalcNextLevelXP();
		CalcStats();
	}

	public override void Write(BinaryWriter writer)
	{
		base.Write(writer);
		writer.Write(playTime.TotalSeconds);
		writer.Write(depth);
		writer.Write(XP);
		writer.Write(Gold);
		writer.Write(KillCount);
		writer.Write(HasChangedEquipment);
		Base.Write(writer);
		SkillSet.Write(writer);
		writer.Write(StatPoints);
		writer.Write(SkillPoints);
		for (int i = 0; i < 4; i++)
		{
			ItemRegistry.Save(writer, Equipment[i]);
		}
		for (int j = 0; j < 32; j++)
		{
			ItemRegistry.Save(writer, Inventory[j]);
		}
		Loot.NPCs.Orb.SaveOrb(writer, Orb);
		Lantern.Write(writer);
		writer.Write(ShopBonus);
	}
}

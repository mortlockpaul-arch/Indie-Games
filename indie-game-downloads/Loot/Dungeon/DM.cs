using System;
using System.Collections.Generic;
using System.IO;
using Eyehook.Framework;
using Loot.Awardments;
using Loot.Core;
using Loot.Effects;
using Loot.Encounters;
using Loot.Items;
using Loot.Maps;
using Loot.NPCs;
using Loot.PC;
using Loot.Screens;
using Loot.TileSets;
using Loot.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;

namespace Loot.Dungeon;

public static class DM
{
	public delegate Location LocationDelegate();

	public delegate void EnterCallback();

	public delegate Item ItemProvider();

	private const int VERSION = 3;

	public const int MaxDepth = 51;

	private const string saveFileName = "save.dat";

	public static GameOptions GameOptions;

	public static Random Random = new Random();

	public static float TwoPiTimer = 0f;

	private static Player player;

	private static Map map;

	private static MoveMap playerMoveMap;

	private static PotionMaster potionMaster;

	private static ScrollMaster scrollMaster;

	private static EncounterList encounterList;

	private static List<NPC> npcList = new List<NPC>();

	private static List<FX> fxList = new List<FX>();

	private static List<FX> poofList = new List<FX>();

	private static int prevDepth = 0;

	private static TimeSpan respawnTimer = TimeSpan.Zero;

	private static TimeSpan respawnInterval = TimeSpan.FromSeconds(3.0);

	private static TimeSpan reaperTime = TimeSpan.FromMinutes(3.0);

	public static int KillCount;

	private static readonly Color hpEffectColor = new Color(255, 152, 152);

	private static readonly Color hpEffectShadowColor = new Color(204, 0, 0);

	private static int spawnOdds => player.Depth / 3 + 10;

	public static Player Player => player;

	public static Map Map => map;

	public static PotionMaster PotionMaster => potionMaster;

	public static ScrollMaster ScrollMaster => scrollMaster;

	public static List<NPC> NPCList => npcList;

	public static List<FX> EffectList => fxList;

	public static List<FX> PoofList => poofList;

	public static int D6 => Random.Next(6) + 1;

	public static int D20 => Random.Next(20) + 1;

	public static void Reset()
	{
		prevDepth = 0;
		player = null;
		map = null;
		potionMaster = null;
		scrollMaster = null;
		encounterList = null;
		npcList.Clear();
		fxList.Clear();
		poofList.Clear();
		DungeonView.Reset();
		Tips.Reset();
	}

	public static void NewGame(PlayerClassType classType, GameOptions gameOptions)
	{
		Reset();
		GameOptions = gameOptions;
		player = PlayerFactory.CreatePlayer(classType);
		player.AddDefaultItems();
		player.Sprite.Reset();
		potionMaster = new PotionMaster();
		scrollMaster = new ScrollMaster();
		encounterList = new EncounterList();
		SaveScreen.NewGame(1, () => map.FindRandom(map.IsOpenFloor), player.Intro);
	}

	public static void SaveNewGame(int depth, LocationDelegate playerLocation)
	{
		Graveyard.Load();
		npcList.Clear();
		fxList.Clear();
		poofList.Clear();
		player.SetDepth(depth);
		GenerateMap(depth);
		SetPlayerLoc(playerLocation());
		RemoveGank(player.Location, 4);
		player.OnNextLevel();
		Save.NewGame();
		MC.Game.ResetElapsedTime();
	}

	public static void GoToDepth(int depth, LocationDelegate playerLocation)
	{
		Save.ExitLevel();
		npcList.Clear();
		fxList.Clear();
		poofList.Clear();
		player.Thought = null;
		prevDepth = player.Depth;
		player.SetDepth(depth);
		bool flag = SetMap(depth);
		SetPlayerLoc(playerLocation());
		if (flag)
		{
			RemoveGank(player.Location, 4);
		}
		player.OnNextLevel();
		if (player.SkillSet.Perception.Level == 10)
		{
			DiscoverAll();
		}
		Save.EnterLevel(flag);
		MC.Game.ResetElapsedTime();
	}

	public static void SaveAndQuit()
	{
		Save.QuitGame();
		MC.Game.ResetElapsedTime();
	}

	private static bool SetMap(int depth)
	{
		map = null;
		try
		{
			using StorageContainer container = MC.StorageManager.OpenContainer(Profile.Container);
			if (Loot.Maps.Map.SaveFileExists(container, depth))
			{
				LoadMap(container, depth);
			}
		}
		catch (Exception)
		{
			throw new ResetIOException();
		}
		if (map != null)
		{
			return false;
		}
		GenerateMap(depth);
		return true;
	}

	public static void OnEnterDepth(int depth, EnterCallback callback)
	{
		callback?.Invoke();
		switch (depth)
		{
		case 2:
			if (Profile.Preferences.Tips)
			{
				CautionaryNote.Display();
			}
			else
			{
				Profile.Awardments.Unlock(Awardment.IntoTheUnknown);
			}
			break;
		case 25:
			Profile.Awardments.Unlock(Awardment.Halfway);
			break;
		}
	}

	private static void GenerateMap(int depth)
	{
		map = MapFactory.GetMap(depth);
		if (depth % 3 == 0)
		{
			AddShop();
			map.SetWidget(new WidgetSlots(map.FindRandom(map.IsOpenFloor)));
		}
		if (prevDepth <= player.Depth)
		{
			if (depth == 1)
			{
				map.SetWidget(new WidgetAwardmentChest(map.FindRandom(map.IsOpenFloor)));
			}
			if (depth % 3 == 2 && depth < 50)
			{
				AddEncounter();
			}
			if ((depth - 1) % 6 == 3)
			{
				map.SetWidget(new WidgetGlowingHole(map.FindRandom(map.IsOpenFloor)));
			}
			Tombstone tombstone = Graveyard.GetTombstone(depth);
			if (tombstone != null)
			{
				Map.SetWidget(new WidgetTombstone(Map.FindRandom(Map.IsOpenFloor), tombstone));
			}
		}
		map.Finish();
		AddNPCs();
		AddItems(ItemFactory.Random);
	}

	public static void RemoveGank(Location loc, int radius)
	{
		for (int i = loc.Row - radius; i <= loc.Row + radius; i++)
		{
			for (int j = loc.Col - radius; j <= loc.Col + radius; j++)
			{
				Location loc2 = new Location(i, j);
				if (Map.IsValid(loc2) && (double)loc.Distance(loc2) <= (double)radius)
				{
					NPC nPC = Map.GetNPC(loc2);
					if (nPC != null)
					{
						RemoveNPC(nPC);
					}
				}
			}
		}
	}

	private static void AddShop()
	{
		Location loc = Map.FindRandom(Map.MatchNorthWall);
		if (Map.GetWidget(loc) != null)
		{
			map.ClearWidget(loc);
			map.ClearWidget(loc.S);
		}
		Map.SetTile(loc, Map.GetTile(TileId.Floor));
		Shop shop = new Shop();
		for (int i = 0; i < 8; i++)
		{
			shop.AddItem(ItemRegistry.ShopItem(Player.Depth));
		}
		shop.Sort();
		Map.SetWidget(new WidgetShop(loc, shop));
	}

	private static void AddEncounter()
	{
		Location loc = Map.FindRandom(Map.IsOpenFloor);
		Map.SetWidget(new WidgetEncounter(encounterList.Next(loc)));
	}

	public static void SetPlayerLoc(Location loc)
	{
		Widget widget = map.GetWidget(loc);
		if (widget != null)
		{
			widget.OnMove();
			widget.OnStep();
		}
		player.SetLocation(loc);
		Discover(player.Location);
		poofList.Clear();
		playerMoveMap = new MoveMap(map, player.Location);
	}

	public static void Demo()
	{
		Reset();
		GameOptions = new GameOptions(Difficulty.Normal);
		DungeonView.Center = new Vector2(900f, 360f);
		DemoPlayer demoPlayer = PlayerFactory.CreateDemoPlayer();
		demoPlayer.SetLevel(15);
		demoPlayer.MaxHP = 300 + 25 * demoPlayer.Level;
		player = demoPlayer;
		if (player.PrimarySkill != null)
		{
			player.PrimarySkill.Level = 6;
		}
		player.Stats.DMG = new DMGRange(25, 25);
		player.Stats.DEX = 25;
		player.SetDepth(15);
		map = new MapCavern(15);
		player.SetLocation(map.FindRandom(map.IsOpenFloor));
		potionMaster = new PotionMaster();
		scrollMaster = new ScrollMaster();
		AddItems(ItemFactory.Random);
		AddNPCs();
		Discover(player.Location);
		poofList.Clear();
		playerMoveMap = new MoveMap(map, player.Location);
	}

	public static void Update(GameTime gameTime)
	{
		TwoPiTimer += (float)(gameTime.ElapsedGameTime.TotalSeconds * 6.2831854820251465);
		if ((double)TwoPiTimer > 6.2831854820251465)
		{
			TwoPiTimer -= (float)Math.PI * 2f;
		}
		Respawn(gameTime);
		for (int i = 0; i < NPCList.Count; i++)
		{
			NPCList[i].Update(gameTime);
		}
		Player.Update(gameTime);
		for (int j = 0; j < fxList.Count; j++)
		{
			fxList[j].Update(gameTime);
		}
		for (int k = 0; k < poofList.Count; k++)
		{
			poofList[k].Update(gameTime);
		}
		Map.Update(gameTime);
		WidgetSprite.UpdateAnimatedSprites(gameTime);
		if (KillCount >= 4)
		{
			Profile.Awardments.Unlock(Awardment.BloodBath);
		}
	}

	private static void Respawn(GameTime gameTime)
	{
		respawnTimer -= gameTime.ElapsedGameTime;
		if (respawnTimer > TimeSpan.Zero)
		{
			return;
		}
		respawnTimer = respawnInterval;
		if (100.0 * (double)NPCList.Count / (double)map.OpenFloorCount > (double)spawnOdds)
		{
			return;
		}
		Location loc = Map.FindRandom(Map.IsOpenFloor);
		if (!((double)player.Location.Distance(loc) <= 5.0))
		{
			NPC nPC;
			if (GameOptions.Difficulty == Difficulty.Hard && map.Age >= reaperTime && !player.IsLucky())
			{
				PlaySound.Death();
				nPC = new Reaper(player.Depth, loc);
			}
			else
			{
				nPC = NPCRegistry.Random(player.Depth, loc);
			}
			AddNPC(nPC);
			if (map.IsDiscovered(nPC.Location))
			{
				AddEffect(new FXPoof(nPC));
			}
		}
	}

	public static Location PathToPlayer(Location start)
	{
		return playerMoveMap.PathToTarget(start);
	}

	public static void MovePlayer(Location newLoc)
	{
		Player.setFacing(newLoc);
		if (player.Location == newLoc || !CanMove(player, newLoc))
		{
			return;
		}
		player.Thought = null;
		NPC nPC = map.GetNPC(newLoc);
		if (nPC != null && !nPC.IsAlly)
		{
			if (nPC.Offset == Vector2.Zero)
			{
				player.Attack(nPC);
			}
		}
		else
		{
			if (player.IsDying)
			{
				return;
			}
			map.GetWidget(newLoc)?.OnMove();
			if (Map.IsDoor(newLoc))
			{
				if (Map.GetTile(newLoc).IsSecretDoor())
				{
					AddEffect(FXText.GetFX(newLoc, "A Secret Door!", new Color(255, 203, 0), new Color(101, 67, 0)));
					PlaySound.SecretDoor();
				}
				else
				{
					PlaySound.Door();
				}
				map.SetOpenDoor(newLoc);
			}
			Item item = map.GetItem(newLoc);
			if (item != null && !(item is Gold) && player.InventoryFull())
			{
				player.Thought = ThoughtBubble.InventoryFull;
			}
			player.MoveTo(newLoc);
			playerMoveMap.Generate(player.Location);
			Discover(player.Location);
		}
	}

	public static void EndMove(Character character)
	{
		if (character == player)
		{
			Item item = map.GetItem(player.Location);
			if (item != null && item.onStep())
			{
				RemoveItem(player.Location);
			}
			map.GetWidget(player.Location)?.OnStep();
		}
	}

	public static void AddItems(ItemProvider itemProvider)
	{
		if (Player.Depth > 1 && Random.Next(100) < 40)
		{
			int num = Random.Next(4);
			for (int i = 0; i < num; i++)
			{
				Location location = Map.FindRandom(Map.IsOpenFloor);
				if (!(location == Location.Zero))
				{
					Map.SetWidget(new WidgetChestClosed(location));
					continue;
				}
				break;
			}
		}
		if (player.Depth > 4 && Random.Next(100) < 20)
		{
			Location location2 = Map.FindRandom(Map.IsOpenFloor);
			if (location2 != Location.Zero)
			{
				Map.SetWidget(new WidgetTombstoneTrap(location2));
			}
		}
		for (int j = 1; j < Map.Rows - 1; j++)
		{
			for (int k = 0; k < Map.Cols - 1; k++)
			{
				Location loc = new Location(j, k);
				if (Map.IsOpenFloor(loc))
				{
					Item item = itemProvider();
					Map.SetItem(loc, item);
				}
			}
		}
	}

	public static void AddNPCs()
	{
		int depth = Player.Depth;
		for (int i = 1; i < Map.Rows - 1; i++)
		{
			for (int j = 0; j < Map.Cols - 1; j++)
			{
				if (Random.Next(100) <= spawnOdds)
				{
					Location loc = new Location(i, j);
					if (Map.IsOpenFloor(loc))
					{
						AddNPC(NPCRegistry.Random(depth, loc));
					}
				}
			}
		}
	}

	public static void AddNPC(NPC npc)
	{
		if (npc != null && CanMove(npc, npc.Location))
		{
			map.SetNPC(npc.Location, npc);
			NPCList.Add(npc);
		}
	}

	public static void MoveNPC(NPC npc, Location loc)
	{
		map.SetNPC(npc.Location, null);
		npc.MoveTo(loc);
		map.SetNPC(npc.Location, npc);
	}

	public static void RemoveItem(Location loc)
	{
		map.SetItem(loc, null);
	}

	public static void RemoveNPC(NPC npc)
	{
		NPCList.Remove(npc);
		map.SetNPC(npc.Location, null);
	}

	public static void KillNPC(NPC npc)
	{
		RemoveNPC(npc);
		AddEffect(FXKill.GetFX(npc));
		if (Profile.Preferences.Gore && Map.IsFloor(npc.Location) && !Map.IsOpenDoor(npc.Location))
		{
			Map.SetOrnament(npc.Location, Ornament.RandomGore());
		}
		player.AddXP(npc.XP);
		KillCount++;
		player.KillCount++;
		if (player.KillCount == 100)
		{
			Profile.Awardments.Unlock(Awardment.Slayer);
		}
		else if (player.KillCount == 1000)
		{
			Profile.Awardments.Unlock(Awardment.Destroyer);
		}
	}

	public static bool CanMove(Character c, Location loc)
	{
		return !Map.IsWall(loc) && (c.CanOpenDoors || !Map.IsDoor(loc));
	}

	public static void Discover(Location src)
	{
		for (int i = src.Row - 4; i <= src.Row + 4; i++)
		{
			for (int j = src.Col - 4; j <= src.Col + 4; j++)
			{
				Location location = new Location(i, j);
				if (!map.IsDiscovered(location) && LineOfSight(src, location))
				{
					map.Discover(location);
					AddPoof(location);
				}
			}
		}
	}

	public static void DiscoverAll()
	{
		for (int i = 0; i < map.Rows; i++)
		{
			for (int j = 0; j < map.Cols; j++)
			{
				Location location = new Location(i, j);
				if (map.IsFloor(location) || map.IsDoor(location))
				{
					Discover(location);
				}
			}
		}
	}

	public static bool LineOfSight(Location src, Location dest)
	{
		if (!map.IsValid(dest))
		{
			return false;
		}
		float num = src.Distance(dest);
		if ((double)num < 1.5)
		{
			return true;
		}
		if ((double)num > 4.0 * (double)player.Lantern.Radius)
		{
			return false;
		}
		Vector2 vector = new Vector2(src.Col, src.Row);
		Vector2 vector2 = new Vector2(dest.Col - src.Col, dest.Row - src.Row);
		vector2.Normalize();
		Location location;
		do
		{
			vector += vector2;
			location = new Location((int)Math.Round(vector.Y), (int)Math.Round(vector.X));
			if (location == dest)
			{
				return true;
			}
		}
		while (map.IsFloor(location));
		return false;
	}

	public static void AddEffect(FX effect)
	{
		if (effect != null)
		{
			fxList.Remove(effect);
			fxList.Add(effect);
		}
	}

	public static void RemoveEffect(FX effect)
	{
		fxList.Remove(effect);
	}

	public static bool HasEffect(FX effect)
	{
		return fxList.Contains(effect);
	}

	public static void ClearEffects()
	{
		fxList.Clear();
	}

	public static void AddHPEffect(Location loc, int hp)
	{
		AddEffect(FXText.GetFX(loc, '+', hp, " HP", hpEffectColor, hpEffectShadowColor));
	}

	public static void AddPoof(Location loc)
	{
		FXFogPoof fX = FXFogPoof.GetFX(loc);
		if (!poofList.Contains(fX))
		{
			poofList.Add(fX);
		}
	}

	public static void RemovePoof(FX poof)
	{
		poofList.Remove(poof);
	}

	public static int GameOver()
	{
		if (player.Depth == 51)
		{
			Profile.Awardments.Unlock(Awardment.Escaped);
			if (GameOptions.Difficulty == Difficulty.Hard)
			{
				Profile.Awardments.Unlock(Awardment.Victory);
			}
			switch (player.ClassType)
			{
			case PlayerClassType.Berserker:
				Profile.Awardments.Unlock(Awardment.EscapeBerserker);
				break;
			case PlayerClassType.Shaman:
				Profile.Awardments.Unlock(Awardment.EscapeShaman);
				break;
			case PlayerClassType.Tinkerer:
				Profile.Awardments.Unlock(Awardment.EscapeTinkerer);
				break;
			case PlayerClassType.Gambler:
				Profile.Awardments.Unlock(Awardment.EscapeGambler);
				break;
			case PlayerClassType.Goblin:
				Profile.Awardments.Unlock(Awardment.EscapeGoblin);
				break;
			case PlayerClassType.Peasant:
				Profile.Awardments.Unlock(Awardment.EscapePeasant);
				break;
			}
			if (Profile.Awardments.IsUnlocked(Awardment.EscapeBerserker) && Profile.Awardments.IsUnlocked(Awardment.EscapeShaman) && Profile.Awardments.IsUnlocked(Awardment.EscapeTinkerer) && Profile.Awardments.IsUnlocked(Awardment.EscapeGambler) && Profile.Awardments.IsUnlocked(Awardment.EscapeGoblin) && Profile.Awardments.IsUnlocked(Awardment.EscapePeasant))
			{
				Profile.Awardments.Unlock(Awardment.EscapeAllClasses);
			}
			if (player.SkillSet.Regen.Level == 0)
			{
				Profile.Awardments.Unlock(Awardment.Heartbreaker);
			}
			if (!player.HasChangedEquipment)
			{
				Profile.Awardments.Unlock(Awardment.WhyChange);
			}
			if (player.PlayTime <= TimeSpan.FromMinutes(50.0))
			{
				Profile.Awardments.Unlock(Awardment.Fast);
			}
			if (GameOptions.Difficulty == Difficulty.Hard && player.PlayTime <= TimeSpan.FromMinutes(85.0))
			{
				Profile.Awardments.Unlock(Awardment.HardAndFast);
			}
		}
		if (player.Depth < 51)
		{
			Graveyard.AddTombstone(player);
		}
		int result = HighScores.Add(GameOptions.Difficulty, player.Name, (int)player.ClassType, player.Level, player.Depth, player.PlayTime);
		Save.GameOver();
		MC.Game.ResetElapsedTime();
		return result;
	}

	public static bool SaveFileExists()
	{
		return MC.StorageManager.Exists(Profile.Container, "save.dat");
	}

	public static void Load()
	{
		Reset();
		Graveyard.Load();
		try
		{
			using StorageContainer container = MC.StorageManager.OpenContainer(Profile.Container);
			LoadGame(container);
			LoadMap(container, player.Depth);
		}
		catch (Exception)
		{
			throw new ResetIOException();
		}
		if (map == null)
		{
			GenerateMap(player.Depth);
		}
		player.AddOrb();
	}

	public static void LoadGame(StorageContainer container)
	{
		using Stream input = container.OpenFile("save.dat", FileMode.Open, FileAccess.Read);
		using BinaryReader binaryReader = new BinaryReader(input);
		Read(binaryReader);
		binaryReader.Close();
	}

	public static void LoadMap(StorageContainer container, int depth)
	{
		map = new Map(container, depth);
		if (map.GameId != GameOptions.GameId)
		{
			map = null;
			return;
		}
		npcList.Clear();
		for (int i = 0; i < Map.Rows; i++)
		{
			for (int j = 0; j < Map.Cols; j++)
			{
				NPC nPC = Map.GetNPC(new Location(i, j));
				if (nPC != null)
				{
					npcList.Add(nPC);
				}
			}
		}
		playerMoveMap = new MoveMap(map, player.Location);
	}

	public static void DeleteGame(StorageContainer container)
	{
		container.DeleteFile("save.dat");
		string[] fileNames = container.GetFileNames("*.map");
		foreach (string file in fileNames)
		{
			container.DeleteFile(file);
		}
	}

	public static void SaveGame(StorageContainer container)
	{
		using Stream output = container.OpenFile("save.dat", FileMode.Create);
		using BinaryWriter binaryWriter = new BinaryWriter(output);
		Write(binaryWriter);
		binaryWriter.Close();
	}

	public static void SaveMap(StorageContainer container)
	{
		using Stream output = container.OpenFile(map.SaveFileName, FileMode.Create);
		using BinaryWriter binaryWriter = new BinaryWriter(output);
		map.Write(binaryWriter);
		binaryWriter.Close();
	}

	public static void Read(BinaryReader reader)
	{
		int num = reader.ReadInt32();
		GameOptions = ((num != 1) ? new GameOptions(reader) : new GameOptions());
		scrollMaster = new ScrollMaster(reader);
		potionMaster = new PotionMaster(reader);
		encounterList = new EncounterList(reader);
		Loot.PC.Player.SaveFileVersion = ((num >= 3) ? 1 : 0);
		player = Loot.PC.Player.Load(reader);
	}

	public static void Write(BinaryWriter writer)
	{
		writer.Write(3);
		GameOptions.Write(writer);
		scrollMaster.Write(writer);
		potionMaster.Write(writer);
		encounterList.Save(writer);
		Loot.PC.Player.Save(writer, player);
	}
}

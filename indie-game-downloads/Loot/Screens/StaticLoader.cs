using Eyehook.Framework;
using Loot.Awardments;
using Loot.Core;
using Loot.Dungeon;
using Loot.Effects;
using Loot.Encounters;
using Loot.Items;
using Loot.NPCs;
using Loot.PC;
using Loot.Platforms;
using Loot.Skills;
using Loot.Slots;
using Loot.Statuses;
using Loot.TileSets;
using Loot.Widgets;
using Microsoft.Xna.Framework.Content;

namespace Loot.Screens;

public static class StaticLoader
{
	private static bool Loaded;

	public static void LoadAllContent()
	{
		if (!Loaded)
		{
			ItemRegistry.Initialize();
			NPCRegistry.Initialize();
			EncounterRegistry.Initialize();
			WidgetRegistry.Initialize();
			StatusRegistry.Initialize();
			ContentManager content = MC.Content;
			Text.LoadContent(content);
			Awardment.LoadContent(content);
			ItemSprite.LoadContent(content);
			JunkSprite.LoadContent(content);
			ArmorSprite.LoadContent(content);
			WeaponSprite.LoadContent(content);
			RingSprite.LoadContent(content);
			AmuletSprite.LoadContent(content);
			PotionSprite.LoadContent(content);
			ScrollSprite.LoadContent(content);
			PlayerSprite.LoadContent(content);
			Orb.Load(content);
			NPC.LoadContent(content);
			NPCSprite.LoadContent(content);
			FX.LoadContent(content);
			Fog.LoadContent(content);
			Ornament.LoadContent(content);
			ThoughtBubble.LoadContent(content);
			Tips.LoadContent(content);
			WidgetSprite.LoadContent(content);
			ButtonSprite.LoadContent(content);
			MenuBox.LoadContent(content);
			MessageBox.LoadContent(content);
			Picture.LoadContent(content);
			Pixel.LoadContent(content);
			SkillSprite.LoadContent(content);
			TileSetCave.LoadContent(content);
			TileSetMine.LoadContent(content);
			TileSetWooden.LoadContent(content);
			TileSetPoison.LoadContent(content);
			TileSetLava.LoadContent(content);
			TileSetDungeon.LoadContent(content);
			DungeonView.Load(content);
			PauseScreen.Load(content);
			SaveScreen.Load(content);
			PlatformerScreen.Load(content);
			StatScreen.Load(content);
			SkillScreen.Load(content);
			InventoryScreen.Load(content);
			ShopScreen.Load(content);
			HighScoreScreen.Load(content);
			ControlsScreen.Load(content);
			CreditScreen.Load(content);
			RippleScreen.Load(content);
			DialogScreen.Load(content);
			GameOverScreen.Load(content);
			AwardmentScreen.Load(content);
			SuitSprite.LoadContent(content);
			Loaded = true;
			MC.Game.ResetElapsedTime();
		}
	}
}

using System;
using System.Collections.Generic;
using Eyehook.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Awardments;

public class Awardment
{
	public readonly int Id;

	public readonly Sprite Icon;

	public readonly string Name;

	public readonly string LockedText;

	public readonly string UnlockedText;

	public readonly int Points;

	private static int maxPoints;

	public static List<int> Ids;

	private static Dictionary<int, Awardment> registry;

	public static Awardment IntoTheUnknown;

	public static Awardment SkillUp;

	public static Awardment PoisonMaster;

	public static Awardment FrenzyMaster;

	public static Awardment FreezeMaster;

	public static Awardment OrbMaster;

	public static Awardment RegenMaster;

	public static Awardment JackOfAllTrades;

	public static Awardment EpicGet;

	public static Awardment EpicSet;

	public static Awardment BloodBath;

	public static Awardment TripleThreat;

	public static Awardment Lucky;

	public static Awardment ImRich;

	public static Awardment SuperChain;

	public static Awardment LeapOfFaith;

	public static Awardment CloseCall;

	public static Awardment Spooky;

	public static Awardment FearTheReaper;

	public static Awardment Halfway;

	public static Awardment Fast;

	public static Awardment Escaped;

	public static Awardment HardAndFast;

	public static Awardment Victory;

	public static Awardment EscapeBerserker;

	public static Awardment EscapeShaman;

	public static Awardment EscapeTinkerer;

	public static Awardment EscapeGambler;

	public static Awardment EscapeGoblin;

	public static Awardment EscapePeasant;

	public static Awardment EscapeAllClasses;

	public static Awardment CurseBreaker;

	public static Awardment Cursed;

	public static Awardment WhyChange;

	public static Awardment Slayer;

	public static Awardment Destroyer;

	public static Awardment Heartbreaker;

	public static Awardment Invincible;

	public static Awardment SwordAndSworcery;

	public static Awardment Jackpot;

	public static Awardment Encounter;

	public static Awardment Toeless;

	public static Awardment BlessedByElves;

	public static Awardment OldGods;

	public static Awardment Streaker;

	public static Awardment UndeadRising;

	public static Awardment Anvil;

	public static Awardment Drinkaholic;

	private static Texture2D awardTexture;

	public static int MaxPoints => maxPoints;

	private Awardment(int id, Sprite icon, string name, string lockedText, string unlockedText, int points)
	{
		Id = id;
		Icon = icon;
		Name = name;
		LockedText = lockedText;
		UnlockedText = unlockedText;
		Points = points;
	}

	public static Awardment Get(int id)
	{
		return registry[id];
	}

	public static void LoadContent(ContentManager content)
	{
		maxPoints = 0;
		Ids = new List<int>();
		registry = new Dictionary<int, Awardment>();
		awardTexture = content.Load<Texture2D>("Sprites\\Awardment\\Awardments");
		IntoTheUnknown = Create(0, "Into The Unknown", "Go forth!", "You have reached depth 2.", 5);
		SkillUp = Create(1, "Skill Up!", "It'll happen.", "You gained a skill point.", 5);
		PoisonMaster = Create(2, "Poison Master", "You require more training.", "You have mastered Poison!", 10);
		FrenzyMaster = Create(3, "Frenzy Master", "You require more training.", "You have mastered Frenzy!", 10);
		FreezeMaster = Create(4, "Freeze Master", "You require more training.", "You have mastered Freeze!", 10);
		OrbMaster = Create(5, "Orb Master", "You require more training.", "You have mastered Orb!", 10);
		RegenMaster = Create(6, "Regen Master", "You require more training.", "You have mastered Regen!", 10);
		JackOfAllTrades = Create(7, "Jack of All Trades", "You require more training.", "You learned all 9 skills!", 10);
		EpicGet = Create(8, "Epic Get", "Equip an epic item.", "You equipped an epic item!", 5);
		EpicSet = Create(9, "Epic Set", "Equip a full epic set.", "You equipped an epic set!", 25);
		BloodBath = Create(10, "Blood Bath", "Well, more like a shower.", "It's a blood bath!", 5);
		TripleThreat = Create(11, "Triple Threat", "Become a triple threat.", "You became a triple threat.", 5);
		Lucky = Create(12, "Lucky!", "Become very lucky.", "... almost too lucky!", 5);
		ImRich = Create(13, "Get Rich", "Save your gold.", "How do you carry all that gold?", 5);
		SuperChain = Create(14, "Super Chain", "Perform a super chain.", "You executed a level 10 max chain!", 10);
		LeapOfFaith = Create(15, "Leap Of Faith", "Hope for the best.", "You made a leap of faith!", 10);
		CloseCall = Create(16, "Close Call", "Cheat death.", "You cheated death!", 5);
		Spooky = Create(17, "Spooky!", "Visit your own grave.", "What a chilling experience!", 5);
		FearTheReaper = Create(18, "Fear the Reaper", "You should.", "You felt death's icy touch!", 5);
		Halfway = Create(19, "Half Way", "Make it half way to the exit.", "You made it half way to the exit!", 10);
		Fast = Create(20, "Fast", "Escape quickly.", "You escaped quickly!", 10);
		Escaped = Create(21, "Escaped!", "Escape the dungeon.", "You escaped the dungeon!", 20);
		HardAndFast = Create(22, "Hard and Fast", "Escape quickly (Hard Mode).", "You escaped quickly!", 25);
		Victory = Create(23, "Victory!", "Escape the dungeon (Hard Mode).", "You escaped the dungeon!", 30);
		EscapeBerserker = Create(24, "Oh, me aching head!", "Escape with the Berserker.", "You escaped with the Berserker!", 10);
		EscapeShaman = Create(25, "Foul Creatures!", "Escape with the Shaman.", "You escaped with the Shaman!", 10);
		EscapeTinkerer = Create(26, "Mother?", "Escape with the Tinkerer.", "You escaped with the Tinkerer!", 10);
		EscapeGambler = Create(27, "Bit of a Catch!", "Escape with the Gambler.", "You escaped with the Gambler!", 10);
		EscapeGoblin = Create(28, "Big Shop!", "Escape with the Goblin.", "You escaped with the Goblin!", 10);
		EscapePeasant = Create(29, "Sweet and Wooly.", "Escape with the Peasant.", "You escaped with the Peasant!", 20);
		EscapeAllClasses = Create(30, "Master", "Escape with all classes.", "You escaped with all classes!", 30);
		CurseBreaker = Create(31, "Curse Breaker", "Destroy a cursed item.", "You destroyed a cursed item.", 10);
		Cursed = Create(32, "Cursed Loot", "It's the name of the game.", "You've been cursed!", 5);
		WhyChange = Create(33, "Why Change?", "Never change your equipment.", "Rags are the new plate mail.", 25);
		Slayer = Create(34, "Slayer", "Kill 100 monsters.", "You killed 100 monsters!", 5);
		Destroyer = Create(35, "Destroyer", "Kill 1000 monsters.", "You killed 1000 monsters!", 10);
		Heartbreaker = Create(36, "Heartbreaker", "Escape with 0 points on Regen.", "How many potions did that take?", 20);
		Invincible = Create(37, "Invincible", "Get all stats above 100.", "You laugh in the face of death!", 20);
		SwordAndSworcery = Create(38, "Sworcery", "Enchant an item.", "You enchanted an item.", 5);
		Jackpot = Create(39, "Jackpot!", "Defeat the one armed bandit!", "You hit the jackpot!", 10);
		Encounter = Create(40, "I Have to Read?!", "Yes, you do.", "You can read!  Good job!", 5);
		Toeless = Create(41, "Toeless", "A witch in need...", "That smarts!", 5);
		BlessedByElves = Create(42, "Blessed", "A lady awaits...", "You were blessed by the elves!", 5);
		OldGods = Create(43, "Old Gods", "The gods demand sacrifice...", "You worshipped an old god.", 5);
		Streaker = Create(44, "Streaker", "Enjoy the hot springs...", "You lost your armor!", 5);
		UndeadRising = Create(45, "Undead Rising", "Probably not a good idea, but...", "You raised an undead army!", 5);
		Anvil = Create(46, "Pale Blue Glow", "A magic anvil awaits...", "You enchanted your weapon!", 5);
		Drinkaholic = Create(47, "Time Well Spent", "An Orc walks into a bar...", "You drank an Orc under the table!", 5);
	}

	private static Awardment Create(int id, string name, string lockedText, string unlockedText, int points)
	{
		if (Ids.Contains(id))
		{
			throw new Exception("Awardment id already in use: " + id);
		}
		Awardment awardment = new Awardment(id, new StillSprite(awardTexture, new Rectangle(id % 8 * 64, id / 8 * 64, 64, 64), new Vector2(32f, 32f)), name, lockedText, unlockedText, points);
		maxPoints += points;
		Ids.Add(id);
		registry.Add(id, awardment);
		return awardment;
	}
}

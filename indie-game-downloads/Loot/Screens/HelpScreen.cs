using System.Collections.Generic;
using Eyehook.Framework;
using Loot.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Loot.Screens;

public class HelpScreen
{
	private class HelpDialogScreen : DialogScreen
	{
		private string id;

		private DialogCallback cancelCallback;

		private string backText = "\u0082\u0083Back";

		private Color backTextColor = new Color(128, 128, 128);

		public HelpDialogScreen(string id, string title, string text, DialogCallback cancelCallback, DialogCallback defaultCallback, params DialogOption[] options)
			: base(title, text, defaultCallback, options)
		{
			this.id = id;
			this.cancelCallback = cancelCallback;
		}

		public void Select(int id)
		{
			selected = id;
		}

		public override void update(GameTime gameTime)
		{
			if (MC.GamePadManager.isNewButtonDown(Buttons.A))
			{
				screenHistory[id] = selected;
			}
			if (MC.GamePadManager.isNewButtonDown(Buttons.B) && cancelCallback != null)
			{
				PlaySound.MenuCancel();
				screenHistory[id] = 0;
				MC.ScreenManager.removeScreen(this);
				cancelCallback();
			}
			else
			{
				base.update(gameTime);
			}
		}

		public override void draw(GameTime gameTime)
		{
			base.draw(gameTime);
			if (options != null)
			{
				base.spriteBatch.Begin();
				Text.Draw(base.spriteBatch, new Vector2(1024f, 634f), backText, backTextColor);
				base.spriteBatch.End();
			}
		}
	}

	private static HelpDialogScreen curScreen;

	private static Dictionary<string, int> screenHistory = new Dictionary<string, int>();

	public static void Display()
	{
		screenHistory.Clear();
		mainMenu();
	}

	private static void exit()
	{
	}

	private static void mainMenu()
	{
		Display("mainMenu", "Help", null, exit, new DialogOption("How to Play", howTo), new DialogOption("The Dungeon", dungeon), new DialogOption("Stats", stats), new DialogOption("Skills", skills), new DialogOption("Items", inventory), new DialogOption("Monsters", mobs));
	}

	private static void dungeon()
	{
		Display("dungeon", "The Dungeon", null, mainMenu, new DialogOption("Light", dLight), new DialogOption("Exits", dExit), new DialogOption("Doors", dDoor), new DialogOption("Traps", dTrap), new DialogOption("Shops", dShop), new DialogOption("Encounters", dEncounter), new DialogOption("Death", dDeath));
	}

	private static void dLight()
	{
		Display("dLight", "Light", "Your lantern lights your way.\n\nWhen your lantern gets low on oil, the level gets darker and less is revealed.  When it runs out of oil... well, it gets dark!\n\nOil is available for purchase in the goblin shops, so it's always a good idea to keep a supply in your inventory.", dungeon, dungeon);
	}

	private static void dExit()
	{
		Display("dExit", "Exits", "Every level has an exit going up (except the first) and an exit going down. To use an exit, step on it and press the left trigger \u0094\u0095.\n\nIf you zoom all the way out with the right thumbstick \u008a\u008b, exits are marked with up and down arrows.\n\nIf you still can't find the exit, it is probably hidden behind a secret door (see doors for more details).", dungeon, dungeon);
	}

	private static void dDoor()
	{
		Display("dDoor", "Doors", "Open doors by moving toward them.\n\nCursed Loot also has secret doors that look just like walls.  Luckily, your lantern will light them up.  So, keep your eyes peeled for lit walls!\n\nIf you have \"Tips\" enabled, secret doors on dungeon levels 1-6 will be highlighted.", dungeon, dungeon);
	}

	private static void dTrap()
	{
		Display("dTrap", "Traps", "Cursed Loot is filled with traps!\n\nYou can levitate to avoid traps that are on the ground (like spike traps).  However, wall spears will still get you, so watch out!\n\nAlso, try to use traps to your advantage.  They hurt monsters, too!  Be aware that flying creatures (like bats) will not be hurt by traps on the ground.", dungeon, dungeon);
	}

	private static void dShop()
	{
		Display("dShop", "Shops", "Cursed Loot has a goblin shop every few levels, so pick up that gold!\n\nTo enter a shop, move to it and press the left trigger \u0094\u0095.\n\nWhile in the shop, you can switch between the shop and your inventory by pressing the back button \u008c\u008d.\n\nPress \u0080\u0081 on items to display a menu.", dungeon, dungeon);
	}

	private static void dEncounter()
	{
		Display("dEncounter", "Encounters", "Gold question marks indicate encounters.  To activate an encounter press the left trigger \u0094\u0095.\n\nEncounters have numerous branching paths and are affected by your choices, stats, and even your class.\n\nBe careful!  Bad luck and bad choices can lead to dire consequences, but the rewards can be great!", dungeon, dungeon);
	}

	private static void dDeath()
	{
		Display("dDeath", "Death", "Death is permanent in Cursed Loot, so don't die!\n\nIf you do happen to die, there is a small consolation.  On your next visit to the dungeon, a grave containing one of your equipped items will appear on the level that you died.\n\nSo, keep an eye out for it.  More loot is never a bad thing!", dungeon, dungeon);
	}

	private static void howTo()
	{
		Display("howTo", "Escape the Dungeon!", "Each game of Cursed Loot has 50 randomly generated levels filled with loot, monsters, and encounters.", mainMenu, new DialogOption("How do I fight?", howToKill), new DialogOption("How do I stay alive?", howToLive), new DialogOption("How do I get stronger?", howToLevel));
	}

	private static void howToKill()
	{
		Display("howToKill", "Fighting", "Use the left thumbstick \u0088\u0089 to move around and to attack creatures.\n\nYour stats affect your ability to hit creatures, and the amount of damage you inflict.\n\nYou also begin the game with one skill, and you can learn more as the game progresses.  Good use of skills will often mean the difference between life and death!", howTo, howTo);
	}

	private static void howToLive()
	{
		Display("howToLive", "Health", "Your health is represented by Hit Points (HPs).  Whenever you are injured, you lose HPs.  When your HPs reach 0, you are dead!\n\nThe red bar at the bottom of the screen is your current health.  When it begins to run low, you should use a Health Potion.\n\nYou can quickly use a Health Potion by pulling the right trigger \u0096\u0097.", howTo, howTo);
	}

	private static void howToLevel()
	{
		Display("howToLevel", "Experience [1/2]", "Every time you kill a creature, you are granted experience points (XP).  After you gain enough XP, your level will increase.\n\nYour progress toward the next level is displayed in the gray bar beneath your health bar.\n\nWhen you reach the next level, \"Level Up\" will be displayed.", howTo, howToLevel2);
	}

	private static void howToLevel2()
	{
		Display("howToLevel2", "Experience [2/2]", "Gaining a level increases your maximum health.  You are also awarded stat points, and, every few levels, a skill point.\n\nPress the left bumper \u0090\u0091 to assign stat points and the right bumper \u0092\u0093 to assign skill points.\n\nSee \"Stats\" & \"Skills\" for more information.", howTo, howTo);
	}

	private static void inventory()
	{
		Display("inventory", "Inventory [1/3]", "Walk over an item to pick it up.  When you pick up an item, it is added to your inventory.\n\nTo manage your inventory, press the back button \u008c\u008d.\n\nYou have room for up to 32 items (+4 equipped items) in your inventory.  If your inventory is full, you can delete items or sell them in a shop.", mainMenu, intentory2);
	}

	private static void intentory2()
	{
		Display("inventory2", "Inventory [2/3]", "You can equip four types of items:\nWeapons, Armor, Rings, and Amulets.\n\nEquipping an item modifies your stats.\n\nInformation about equipment is displayed next to it, showing its stats and then the difference between it and the currently equipped item's stats.", mainMenu, intentory3);
	}

	private static void intentory3()
	{
		Display("inventory3", "Inventory [3/3]", "Some items are unidentified.  You can equip or use the item to identify it, but be careful!\n\nSome equipment is cursed and can only be removed with a \"Remove Curse Scroll.\"\n\nIt is much safer to use an \"Identify Scroll\" on unidentified items, but the choice is yours.", mainMenu, mainMenu);
	}

	private static void stats()
	{
		Display("stats", "Stats", "Stats affect the following:\n\nDMG: How much damage you inflict.\nDEF: How much damage you absorb.\nDEX: Your ability to hit/dodge.\nLCK: Affects loot & encounters.\n\nYou can view your current stats by pressing the left bumper \u0090\u0091.", mainMenu, mainMenu);
	}

	private static void skills()
	{
		Display("skills", "Skills", null, mainMenu, new DialogOption("Overview", skillOverview), new DialogOption("Chaining", chain), new DialogOption("Active Skills", activeSkills), new DialogOption("Passive Skills", passiveSkills));
	}

	private static void skillOverview()
	{
		Display("skillOverview", "Skill Overview", "At the beginning of the game, you can use your primary skill.\n\nEvery few levels, you will be given a skill point that can be assigned to a skill of your choosing.  If you add it to your primary skill, it will increase by 2, otherwise the skill will increase by 1. (Max 10)\n\nYou can view your current skills by pressing the right bumper \u0092\u0093.", skills, skills);
	}

	private static void chain()
	{
		Display("chain", "Chaining Skills", "Chaining gives you the ability to reuse your skills faster than you would be able to normally.\n\nAfter you use a skill, watch the skill button display on the top-right of the screen.\n\nYou will see a larger version of the skill button zoom in on itself.  If you hit the skill button when the two meet, you will chain your skill.", skills, skills);
	}

	private static void activeSkills()
	{
		Display("activeSkills", "Active Skills", null, skills, new DialogOption("Poison", skillPoison), new DialogOption("Frenzy", skillFrenzy), new DialogOption("Freeze", skillFreeze), new DialogOption("Orb", skillOrb));
	}

	private static void skillPoison()
	{
		Display("skillPoison", "Poison", "Poison is the Gambler's primary skill.  (Press \u0080\u0081 to activate)\n\nAll surrounding creatures will be poisoned and suffer damage over time.\n\nIncreasing the Poison Skill Level will increase the damage it inflicts over time.", activeSkills, activeSkills);
	}

	private static void skillFrenzy()
	{
		Display("skillFrenzy", "Frenzy", "Frenzy is the Berserker's primary skill.  (Press \u0082\u0083 to activate)\n\nAll surrounding creatures will be hit by your weapon.\n\nIncreasing the Frenzy Skill Level will increase its damage.", activeSkills, activeSkills);
	}

	private static void skillFreeze()
	{
		Display("skillFreeze", "Freeze", "Freeze is the Shaman's primary skill.  (Press \u0084\u0085 to activate)\n\nAll surrounding creatures will be frozen.  Hitting a frozen creature guarantees a critical hit.\n\nIncreasing the Freeze Skill Level will increase its duration.", activeSkills, activeSkills);
	}

	private static void skillOrb()
	{
		Display("skillOrb", "Orb", "Orb is the Tinkerer's primary skill.  (Press \u0086\u0087 to activate)\n\nThe orb will fight alongside you.  Its health and damage increase with both your level and the Orb Skill Level.\n\nIf you press \u0086\u0087 while the Orb is active, it will detonate, causing critical damage to all surrounding enemies.", activeSkills, activeSkills);
	}

	private static void passiveSkills()
	{
		Display("passiveSkills", "Passive Skills", "Passive skills work automatically.", skills, new DialogOption("Perception", skillPerception), new DialogOption("Stat Boost", skillStatBoost), new DialogOption("Regenerate", skillRegenerate), new DialogOption("Stealth", skillStealth), new DialogOption("Thorns", skillThorns));
	}

	private static void skillPerception()
	{
		Display("skillPerception", "Perception", "With perception, you will be able to see hidden doors, see in the dark, and discover your enemies weaknesses (improving your chance for critical hits).\n\nLevel 10 Perception automatically reveals the entire level layout.", passiveSkills, passiveSkills);
	}

	private static void skillStatBoost()
	{
		Display("skillStatBoost", "Stat Boost", "Stat Boost improves all your stats, by increasing amounts, based upon its level.\n\nLevel 10 Stat Boost will increase all of your stats by 10.", passiveSkills, passiveSkills);
	}

	private static void skillRegenerate()
	{
		Display("skillRegenerate", "Regenerate", "If you have been injured, Regenerate will restore your hit points over time.  However, it becomes inactive for a brief period after you have been hit.\n\nIncreasing the Regenerate Skill Level will increase the speed that it heals you.", passiveSkills, passiveSkills);
	}

	private static void skillStealth()
	{
		Display("skillStealth", "Stealth", "Stealth makes it more difficult for monsters to find you.\n\nWith level 10 Stealth, a monster must be right next to you to discover you.", passiveSkills, passiveSkills);
	}

	private static void skillThorns()
	{
		Display("skillThorns", "Thorns", "Thorns deals damage to enemies that hit you.\n\nIncreasing the level of Thorns will increase the damage dealt to enemies.", passiveSkills, passiveSkills);
	}

	private static void mobs()
	{
		Display("mobs", "Monsters", "Cursed Loot is filled with a wide variety of monsters, and they all want you dead!\n\nWatch out for glowing monsters.  They are elite and have more health and deal more damage than their regular counterparts.\n\nGood luck and good hunting!", mainMenu, mainMenu);
	}

	private static void Display(string id, string title, string text, DialogCallback cancelCallback, DialogCallback defaultCallback)
	{
		curScreen = new HelpDialogScreen(id, title, text, cancelCallback, defaultCallback, (DialogOption[])null);
		if (screenHistory.ContainsKey(id))
		{
			curScreen.Select(screenHistory[id]);
		}
		MC.ScreenManager.addScreen(curScreen);
	}

	private static void Display(string id, string title, string text, DialogCallback cancelCallback, params DialogOption[] options)
	{
		curScreen = new HelpDialogScreen(id, title, text, cancelCallback, null, options);
		if (screenHistory.ContainsKey(id))
		{
			curScreen.Select(screenHistory[id]);
		}
		MC.ScreenManager.addScreen(curScreen);
	}
}

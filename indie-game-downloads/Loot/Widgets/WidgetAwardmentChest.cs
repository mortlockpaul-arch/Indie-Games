using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Items;
using Loot.Items.Amulets;
using Loot.Items.Armors;
using Loot.Items.Misc;
using Loot.Items.Potions;
using Loot.Items.Rings;
using Loot.Items.Weapons;
using Loot.Screens;

namespace Loot.Widgets;

public class WidgetAwardmentChest : WidgetAction
{
	protected override Sprite Sprite => WidgetSprite.AwardmentChest;

	public WidgetAwardmentChest(Location loc)
		: base(loc)
	{
	}

	public WidgetAwardmentChest(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		PlaySound.Encounter();
		MC.ScreenManager.addScreen(new RippleScreen(openIt));
	}

	private void openIt()
	{
		DM.Map.ClearWidget(Location);
		int points = Profile.Awardments.Points;
		int gp = points;
		if (points == 0)
		{
			empty();
		}
		else if (points <= 50)
		{
			reward1(gp);
		}
		else if (points <= 100)
		{
			reward2(gp);
		}
		else if (points <= 200)
		{
			reward3(gp);
		}
		else if (points <= 300)
		{
			reward4(gp);
		}
		else if (points < 400)
		{
			reward5(gp);
		}
		else if (points < 500)
		{
			reward6(gp);
		}
		else
		{
			reward7(gp);
		}
	}

	private void empty()
	{
		Dialog.Display("Awardment Chest", "The chest is empty, except for an old scrap of paper which reads:\n\n\"Earn Awardments to unlock gold and items for your next visit!\"", new DialogOption("Ok"));
	}

	private void reward1(int gp)
	{
		DM.Player.Gold += gp;
		Dialog.Display("Awardment Chest", "The chest contains " + gp + " Gold Pieces!", new DialogOption("Cha-ching!"));
	}

	private void reward2(int gp)
	{
		DM.Player.Gold += gp;
		DM.Player.AddItemOrDrop(new PotionHealth());
		Dialog.Display("Awardment Chest", "The chest contains a Health Potion and " + gp + " Gold Pieces!", new DialogOption("Cha-ching!"));
	}

	private void reward3(int gp)
	{
		DM.Player.Gold += gp;
		Item item = new RingGold(EquipmentTier.Dwarven);
		item.Identify();
		Item item2 = new AmuletWooden(EquipmentTier.Dwarven);
		item2.Identify();
		DM.Player.AddItemOrDrop(new PotionHealth());
		DM.Player.AddItemOrDrop(item);
		DM.Player.AddItemOrDrop(item2);
		Dialog.Display("Awardment Chest", "The chest contains a Health Potion, a Dwarven Ring and Amulet, and " + gp + " Gold Pieces!", new DialogOption("Cha-ching!"));
	}

	private void reward4(int gp)
	{
		DM.Player.Gold += gp;
		Item item = new WeaponDagger(EquipmentTier.Dwarven);
		item.Identify();
		Item item2 = new ArmorLeather(EquipmentTier.Dwarven);
		item2.Identify();
		Item item3 = new RingGold(EquipmentTier.Dwarven);
		item3.Identify();
		Item item4 = new AmuletWooden(EquipmentTier.Dwarven);
		item4.Identify();
		DM.Player.AddItemOrDrop(new PotionHealth());
		DM.Player.AddItemOrDrop(item2);
		DM.Player.AddItemOrDrop(item);
		DM.Player.AddItemOrDrop(item3);
		DM.Player.AddItemOrDrop(item4);
		Dialog.Display("Awardment Chest", "The chest contains a Health Potion, a full Dwarven Set, and " + gp + " Gold Pieces!", new DialogOption("Cha-ching!"));
	}

	private void reward5(int gp)
	{
		DM.Player.Gold += gp;
		Item item = new WeaponDagger(EquipmentTier.Elven);
		item.Identify();
		Item item2 = new ArmorLeather(EquipmentTier.Elven);
		item2.Identify();
		Item item3 = new RingGold(EquipmentTier.Elven);
		item3.Identify();
		Item item4 = new AmuletWooden(EquipmentTier.Elven);
		item4.Identify();
		DM.Player.AddItemOrDrop(new PotionHealth());
		DM.Player.AddItemOrDrop(item2);
		DM.Player.AddItemOrDrop(item);
		DM.Player.AddItemOrDrop(item3);
		DM.Player.AddItemOrDrop(item4);
		Dialog.Display("Awardment Chest", "The chest contains a Health Potion, a full Elven Set, and " + gp + " Gold Pieces!", new DialogOption("Cha-ching!"));
	}

	private void reward6(int gp)
	{
		DM.Player.Gold += gp;
		Item item = new WeaponDagger(EquipmentTier.Epic);
		item.Identify();
		Item item2 = new ArmorLeather(EquipmentTier.Epic);
		item2.Identify();
		Item item3 = new RingGold(EquipmentTier.Epic);
		item3.Identify();
		Item item4 = new AmuletWooden(EquipmentTier.Epic);
		item4.Identify();
		DM.Player.AddItemOrDrop(new PotionHealth());
		DM.Player.AddItemOrDrop(item2);
		DM.Player.AddItemOrDrop(item);
		DM.Player.AddItemOrDrop(item3);
		DM.Player.AddItemOrDrop(item4);
		Dialog.Display("Awardment Chest", "The chest contains a Health Potion, a full Epic Set, and " + gp + " Gold Pieces!", new DialogOption("Cha-ching!"));
	}

	private void reward7(int gp)
	{
		DM.Player.Gold += gp;
		DM.Player.AddItemOrDrop(new CharmResurrect());
		Dialog.Display("Awardment Chest", "The chest contains a Charm of Resurrection and " + gp + " Gold Pieces!", new DialogOption("Cha-ching!"));
	}
}

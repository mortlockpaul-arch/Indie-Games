using System.IO;
using Eyehook.Framework;

namespace Loot.Widgets;

public static class WidgetRegistry
{
	private static TypeRegistry<Widget> registry = new TypeRegistry<Widget>();

	public static void Initialize()
	{
		Register<WidgetEncounter>("W0");
		Register<WidgetExitDown>("W1");
		Register<WidgetExitUp>("W2");
		Register<WidgetExitOut>("W3");
		Register<WidgetFountainEmpty>("W4");
		Register<WidgetFountainHP>("W5");
		Register<WidgetShop>("W6");
		Register<WidgetTrapSpike>("W7");
		Register<WidgetPoisonVent>("W8");
		Register<WidgetWallSword>("W9");
		Register<WidgetWallGear>("W10");
		Register<WidgetLavaVent>("W11");
		Register<WidgetFountainXP>("W12");
		Register<WidgetTombstone>("W13");
		Register<WidgetGlowingHole>("W14");
		Register<WidgetHole>("W15");
		Register<WidgetAwardmentChest>("W16");
		Register<WidgetChestClosed>("W17");
		Register<WidgetChestOpen>("W18");
		Register<WidgetFire>("W19");
		Register<WidgetTombstoneTrap>("W20");
		Register<WidgetSlots>("W21");
	}

	private static void Register<T>(string id) where T : Widget
	{
		registry.Register<T>(id);
	}

	public static void Save(BinaryWriter writer, Widget widget)
	{
		registry.Save(writer, widget);
	}

	public static Widget Load(BinaryReader reader)
	{
		return registry.Load(reader);
	}
}

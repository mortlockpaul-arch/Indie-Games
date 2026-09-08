using System;
using Eyehook.Framework;
using Loot.Awardments;
using Loot.Dungeon;
using Loot.Screens;
using Microsoft.Xna.Framework;

namespace Loot;

public class LootGame : EyehookGame
{
	public const string Version = "2.1.0";

	private const bool vsync = true;

	public static AwardmentComponent AwardComponent;

	public LootGame()
		: base("CursedLoot", fixedTimeStep: false, vSync: true)
	{
		AwardComponent = new AwardmentComponent(this);
		base.Components.Add(AwardComponent);
	}

	protected override void Initialize()
	{
		base.Initialize();
		GraphicUtil.Initialize(this);
		Shadow.Initialize(this);
	}

	public override void ResetGame()
	{
		DM.Reset();
	}

	public override void DisplayStartScreen()
	{
		MC.ScreenManager.BackgroundColor = new Color(64, 64, 64);
		MC.ScreenManager.addScreen(new StartScreen());
	}

	private static void Main(string[] args)
	{
		try
		{
			using LootGame lootGame = new LootGame();
			lootGame.Run();
		}
		catch (Exception)
		{
		}
	}
}

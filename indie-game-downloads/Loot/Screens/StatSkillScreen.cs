using System;
using Eyehook.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Loot.Screens;

public class StatSkillScreen : Screen
{
	public enum Mode
	{
		Stat,
		Skill
	}

	private SlidingScreen activeScreen;

	private StatScreen statScreen;

	private SkillScreen skillScreen;

	public StatSkillScreen(Mode mode)
		: base(modal: true)
	{
		statScreen = new StatScreen();
		skillScreen = new SkillScreen();
		statScreen.SetMode(SlidingScreen.Mode.Off);
		statScreen.Disabled = true;
		skillScreen.SetMode(SlidingScreen.Mode.Off);
		skillScreen.Disabled = true;
		switch (mode)
		{
		case Mode.Stat:
			activeScreen = statScreen;
			statScreen.SetMode(SlidingScreen.Mode.SlideOn);
			statScreen.Disabled = false;
			break;
		case Mode.Skill:
			activeScreen = skillScreen;
			skillScreen.SetMode(SlidingScreen.Mode.SlideOn);
			skillScreen.Disabled = false;
			break;
		default:
			throw new Exception("Unknown mode: " + mode);
		}
	}

	public override void transitionOn()
	{
		MC.ScreenManager.insertBefore(statScreen, this);
		MC.ScreenManager.insertBefore(skillScreen, this);
	}

	public override void transitionOff()
	{
		MC.ScreenManager.removeScreen(statScreen);
		MC.ScreenManager.removeScreen(skillScreen);
	}

	public override void update(GameTime gameTime)
	{
		if (MC.GamePadManager.isNewButtonDown(Buttons.Start))
		{
			MC.ScreenManager.addScreen(new PauseScreen());
			return;
		}
		if (statScreen.IsOff && skillScreen.IsOff)
		{
			MC.ScreenManager.removeScreen(this);
			return;
		}
		if (statScreen.IsOff && skillScreen.IsOn)
		{
			activeScreen = skillScreen;
		}
		else if (statScreen.IsOn && skillScreen.IsOff)
		{
			activeScreen = statScreen;
		}
		statScreen.Disabled = activeScreen == skillScreen;
		skillScreen.Disabled = activeScreen == statScreen;
		if (MC.GamePadManager.isNewButtonDown(Buttons.B) && activeScreen.IsOn)
		{
			activeScreen.SetMode(SlidingScreen.Mode.SlideOff);
			return;
		}
		if (MC.GamePadManager.isNewButtonDown(Buttons.LeftShoulder))
		{
			if (activeScreen != statScreen)
			{
				activeScreen = statScreen;
				if (!activeScreen.IsOff)
				{
					return;
				}
			}
		}
		else if (MC.GamePadManager.isNewButtonDown(Buttons.RightShoulder) && activeScreen != skillScreen)
		{
			activeScreen = skillScreen;
			if (!activeScreen.IsOff)
			{
				return;
			}
		}
		statScreen.update(gameTime);
		skillScreen.update(gameTime);
	}
}

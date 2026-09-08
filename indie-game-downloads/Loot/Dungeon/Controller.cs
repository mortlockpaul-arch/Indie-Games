using System;
using Eyehook.Framework;
using Loot.Screens;
using Loot.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Loot.Dungeon;

public class Controller
{
	private TimeSpan inputDelay;

	private bool rtDown;

	private bool ltDown;

	public Controller()
	{
		inputDelay = TimeSpan.Zero;
	}

	public void Update(GameTime gameTime)
	{
		if (MC.GamePadManager.isNewButtonDown(Buttons.Start))
		{
			MC.ScreenManager.addScreen(new PauseScreen());
			return;
		}
		TimeSpan elapsedGameTime = gameTime.ElapsedGameTime;
		if (inputDelay > TimeSpan.Zero)
		{
			inputDelay -= elapsedGameTime;
			if (inputDelay < TimeSpan.Zero)
			{
				inputDelay = TimeSpan.Zero;
			}
		}
		if (!DM.Player.IsDying && inputDelay == TimeSpan.Zero)
		{
			if (MC.GamePadManager.isButtonDown(Buttons.RightStick))
			{
				DungeonView.Camera.ZoomReset();
				inputDelay = TimeSpan.FromMilliseconds(500.0);
			}
			else if ((double)MC.GamePadManager.GamePadState.ThumbSticks.Right.Y > 0.4000000059604645)
			{
				DungeonView.Camera.ZoomAdjust((float)elapsedGameTime.TotalSeconds);
			}
			else if ((double)MC.GamePadManager.GamePadState.ThumbSticks.Right.Y < -0.4000000059604645)
			{
				DungeonView.Camera.ZoomAdjust(0f - (float)elapsedGameTime.TotalSeconds);
			}
		}
		if ((double)MC.GamePadManager.GamePadState.Triggers.Right < 0.10000000149011612)
		{
			rtDown = false;
		}
		if ((double)MC.GamePadManager.GamePadState.Triggers.Left < 0.10000000149011612)
		{
			ltDown = false;
		}
		if (MC.GamePadManager.isNewButtonDown(Buttons.Back))
		{
			InventoryScreen.Display(null);
			return;
		}
		if (!ltDown && (double)MC.GamePadManager.GamePadState.Triggers.Left > 0.800000011920929)
		{
			ltDown = true;
			Widget widget = DM.Map.GetWidget(DM.Player.Location);
			if (widget != null)
			{
				widget.OnClick();
				return;
			}
		}
		else if (!rtDown && (double)MC.GamePadManager.GamePadState.Triggers.Right > 0.800000011920929)
		{
			rtDown = true;
			DM.Player.ConsumeHealthPotion();
			return;
		}
		if (MC.GamePadManager.isNewButtonDown(Buttons.A))
		{
			DM.Player.SkillSet.Poison.Cast();
		}
		if (MC.GamePadManager.isNewButtonDown(Buttons.B))
		{
			DM.Player.SkillSet.Frenzy.Cast();
		}
		if (MC.GamePadManager.isNewButtonDown(Buttons.X))
		{
			DM.Player.SkillSet.Freeze.Cast();
		}
		if (MC.GamePadManager.isNewButtonDown(Buttons.Y))
		{
			DM.Player.SkillSet.Orb.Cast();
		}
		if (!statSkillScreen() && !DM.Player.IsBusy)
		{
			GamePadDir gamePadDir = MC.GamePadManager.get8Dir();
			if (gamePadDir != GamePadDir.NONE)
			{
				DM.MovePlayer(autoAim(hugWall(gamePadDir)));
			}
		}
	}

	private bool statSkillScreen()
	{
		if (MC.GamePadManager.isNewButtonDown(Buttons.LeftShoulder))
		{
			MC.ScreenManager.addScreen(new StatSkillScreen(StatSkillScreen.Mode.Stat));
			return true;
		}
		if (!MC.GamePadManager.isNewButtonDown(Buttons.RightShoulder))
		{
			return false;
		}
		MC.ScreenManager.addScreen(new StatSkillScreen(StatSkillScreen.Mode.Skill));
		return true;
	}

	private GamePadDir hugWall(GamePadDir dir)
	{
		Location location = DM.Player.Location;
		if (!DM.CanMove(DM.Player, getLoc(DM.Player.Location, dir)))
		{
			switch (dir)
			{
			case GamePadDir.NE:
				if (DM.CanMove(DM.Player, location.N))
				{
					return GamePadDir.N;
				}
				if (DM.CanMove(DM.Player, location.E))
				{
					return GamePadDir.E;
				}
				break;
			case GamePadDir.SE:
				if (DM.CanMove(DM.Player, location.S))
				{
					return GamePadDir.S;
				}
				if (DM.CanMove(DM.Player, location.E))
				{
					return GamePadDir.E;
				}
				break;
			case GamePadDir.SW:
				if (DM.CanMove(DM.Player, location.S))
				{
					return GamePadDir.S;
				}
				if (DM.CanMove(DM.Player, location.W))
				{
					return GamePadDir.W;
				}
				break;
			case GamePadDir.NW:
				if (DM.CanMove(DM.Player, location.N))
				{
					return GamePadDir.N;
				}
				if (DM.CanMove(DM.Player, location.W))
				{
					return GamePadDir.W;
				}
				break;
			}
		}
		return dir;
	}

	private Location autoAim(GamePadDir dir)
	{
		Location location = DM.Player.Location;
		Location loc = getLoc(location, dir);
		if (DM.Map.IsEnemy(loc))
		{
			return loc;
		}
		switch (dir)
		{
		case GamePadDir.N:
		{
			Location loc6 = getLoc(location, GamePadDir.NW);
			if (DM.Map.IsEnemy(loc6))
			{
				return loc6;
			}
			Location loc7 = getLoc(location, GamePadDir.NE);
			if (DM.Map.IsEnemy(loc7))
			{
				return loc7;
			}
			break;
		}
		case GamePadDir.NE:
		{
			Location loc8 = getLoc(location, GamePadDir.N);
			if (DM.Map.IsEnemy(loc8))
			{
				return loc8;
			}
			Location loc9 = getLoc(location, GamePadDir.E);
			if (DM.Map.IsEnemy(loc9))
			{
				return loc9;
			}
			break;
		}
		case GamePadDir.E:
		{
			Location loc16 = getLoc(location, GamePadDir.NE);
			if (DM.Map.IsEnemy(loc16))
			{
				return loc16;
			}
			Location loc17 = getLoc(location, GamePadDir.SE);
			if (DM.Map.IsEnemy(loc17))
			{
				return loc17;
			}
			break;
		}
		case GamePadDir.SE:
		{
			Location loc12 = getLoc(location, GamePadDir.E);
			if (DM.Map.IsEnemy(loc12))
			{
				return loc12;
			}
			Location loc13 = getLoc(location, GamePadDir.S);
			if (DM.Map.IsEnemy(loc13))
			{
				return loc13;
			}
			break;
		}
		case GamePadDir.S:
		{
			Location loc14 = getLoc(location, GamePadDir.SW);
			if (DM.Map.IsEnemy(loc14))
			{
				return loc14;
			}
			Location loc15 = getLoc(location, GamePadDir.SE);
			if (DM.Map.IsEnemy(loc15))
			{
				return loc15;
			}
			break;
		}
		case GamePadDir.SW:
		{
			Location loc4 = getLoc(location, GamePadDir.S);
			if (DM.Map.IsEnemy(loc4))
			{
				return loc4;
			}
			Location loc5 = getLoc(location, GamePadDir.W);
			if (DM.Map.IsEnemy(loc5))
			{
				return loc5;
			}
			break;
		}
		case GamePadDir.W:
		{
			Location loc10 = getLoc(location, GamePadDir.NW);
			if (DM.Map.IsEnemy(loc10))
			{
				return loc10;
			}
			Location loc11 = getLoc(location, GamePadDir.SW);
			if (DM.Map.IsEnemy(loc11))
			{
				return loc11;
			}
			break;
		}
		case GamePadDir.NW:
		{
			Location loc2 = getLoc(location, GamePadDir.N);
			if (DM.Map.IsEnemy(loc2))
			{
				return loc2;
			}
			Location loc3 = getLoc(location, GamePadDir.W);
			if (DM.Map.IsEnemy(loc3))
			{
				return loc3;
			}
			break;
		}
		}
		return getLoc(location, dir);
	}

	private Location getLoc(Location loc, GamePadDir dir)
	{
		return dir switch
		{
			GamePadDir.N => loc.N, 
			GamePadDir.NE => loc.NE, 
			GamePadDir.E => loc.E, 
			GamePadDir.SE => loc.SE, 
			GamePadDir.S => loc.S, 
			GamePadDir.SW => loc.SW, 
			GamePadDir.W => loc.W, 
			GamePadDir.NW => loc.NW, 
			GamePadDir.NONE => loc, 
			_ => throw new ArgumentException("Unknown Direction: " + dir), 
		};
	}
}

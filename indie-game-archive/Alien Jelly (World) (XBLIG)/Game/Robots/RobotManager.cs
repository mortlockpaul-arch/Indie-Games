using System.Collections.Generic;
using Game.Atoms;
using Game.Grids;
using Game.Scenes.Play;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Robots;

public class RobotManager
{
	public PlayUniverse universe;

	public AtomManager atoms;

	public List<Robot> robots;

	public RobotManager(PlayUniverse oUniverse)
	{
		universe = oUniverse;
		atoms = universe.atoms;
		Init();
	}

	public void Init()
	{
		robots = new List<Robot>();
	}

	public void Update(GameTime elapsed)
	{
		for (int i = 0; i < robots.Count; i++)
		{
			robots[i].Update(elapsed);
		}
	}

	public void Reverse(GameTime elapsed)
	{
		for (int i = 0; i < robots.Count; i++)
		{
			robots[i].Reverse(elapsed);
		}
	}

	public void RenderEffect(ref Effect effect)
	{
		for (int i = 0; i < robots.Count; i++)
		{
			robots[i].RenderEffect(ref effect);
		}
	}

	public void Add(Robot oQByte)
	{
		robots.Add(oQByte);
	}

	public void Remove(Robot oQByte)
	{
		robots.Remove(oQByte);
	}

	public void Flush()
	{
		while (robots.Count > 0)
		{
			Remove(robots[0]);
		}
	}

	public void Dispose()
	{
		for (int i = 0; i < robots.Count; i++)
		{
			robots[i].Dispose();
			robots[i] = null;
		}
		robots.Clear();
	}

	public Robot At(float xX, float xY, float xZ, Robot oThisRobot)
	{
		Robot result = null;
		for (int i = 0; i < robots.Count; i++)
		{
			if ((oThisRobot != robots[i] && !robots[i].moving && xX > robots[i].X - Grid.SPACING.X && xX < robots[i].X + Grid.SPACING.X && xY > robots[i].Y - Grid.SPACING.Y && xY < robots[i].Y + Grid.SPACING.Y && xZ > robots[i].Z - Grid.SPACING.Z && xZ < robots[i].Z + Grid.SPACING.Z) || (robots[i].moving && ((xX > robots[i].moveFrom.X - Grid.SPACING.X && xX < robots[i].moveFrom.X + Grid.SPACING.X && xY > robots[i].moveFrom.Y - Grid.SPACING.Y && xY < robots[i].moveFrom.Y + Grid.SPACING.Y && xZ > robots[i].moveFrom.Z - Grid.SPACING.Z && xZ < robots[i].moveFrom.Z + Grid.SPACING.Z) || (xX > robots[i].moveTo.X - Grid.SPACING.X && xX < robots[i].moveTo.X + Grid.SPACING.X && xY > robots[i].moveTo.Y - Grid.SPACING.Y && xY < robots[i].moveTo.Y + Grid.SPACING.Y && xZ > robots[i].moveTo.Z - Grid.SPACING.Z && xZ < robots[i].moveTo.Z + Grid.SPACING.Z))))
			{
				result = robots[i];
				break;
			}
		}
		return result;
	}

	public void Snap_All()
	{
		for (int i = 0; i < robots.Count; i++)
		{
			robots[i].SnapToGrid();
		}
	}
}

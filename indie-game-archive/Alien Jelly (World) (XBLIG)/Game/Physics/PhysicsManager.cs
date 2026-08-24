using System;
using System.Collections.Generic;
using GKEngine.Entities;
using Game.Atoms;
using Game.Grids;
using Game.Scenes.Play;
using Microsoft.Xna.Framework;

namespace Game.Physics;

public class PhysicsManager
{
	public static float GRAVITY = -0.0005f;

	public static float VELOCITY_MAX = 2f;

	private Base3D _flipBase = new Base3D();

	public PlayUniverse universe;

	public AtomManager atoms;

	public List<PhysicsItem> stack;

	public bool active;

	public int lengthStack;

	private Matrix inverse;

	public PhysicsManager(PlayUniverse oUniverse)
	{
		universe = oUniverse;
		atoms = universe.atoms;
		stack = new List<PhysicsItem>();
		lengthStack = 0;
		inverse = Matrix.Invert(Matrix.Identity);
	}

	public void Start()
	{
		active = true;
	}

	public void Stop()
	{
		active = false;
	}

	public void Dispose()
	{
		stack.Clear();
		lengthStack = 0;
	}

	public void Add(PhysicsItem oItem)
	{
		if (!stack.Contains(oItem))
		{
			stack.Add(oItem);
			lengthStack = stack.Count;
			oItem.physics = this;
		}
	}

	public void Remove(PhysicsItem oItem)
	{
		if (stack.Contains(oItem))
		{
			oItem.physics = null;
			stack.Remove(oItem);
			lengthStack = stack.Count;
		}
	}

	public void Update(GameTime oGameTime)
	{
		if (active)
		{
			for (int i = 0; i < lengthStack; i++)
			{
				stack[i].PhysicsUpdate((float)oGameTime.ElapsedGameTime.TotalMilliseconds);
			}
		}
	}

	public void UpdateSlow(float elapsed)
	{
	}

	public void Flip(Vector3 vAxis, float xAmount)
	{
		for (int i = 0; i < lengthStack; i++)
		{
			_flipBase.matrix = Matrix.Multiply(Matrix.Multiply(stack[i]._base.matrix, inverse), Matrix.CreateFromAxisAngle(vAxis, (float)Math.PI / 2f * xAmount));
			stack[i].position = _flipBase.position;
			if (!stack[i].physicsFlipNoRotate)
			{
				stack[i].rotation = _flipBase.rotation;
			}
			stack[i].Event_Flip_Update();
		}
	}

	public PhysicsItem At(float xX, float xY, float xZ)
	{
		PhysicsItem result = null;
		for (int i = 0; i < lengthStack; i++)
		{
			if (xX > stack[i].X - Grid.SPACING.X * Grid.ERROR_MARGIN && xX < stack[i].X + Grid.SPACING.X * Grid.ERROR_MARGIN && xY > stack[i].Y - Grid.SPACING.Y * Grid.ERROR_MARGIN && xY < stack[i].Y + Grid.SPACING.Y * Grid.ERROR_MARGIN && xZ > stack[i].Z - Grid.SPACING.Z * Grid.ERROR_MARGIN && xZ < stack[i].Z + Grid.SPACING.Z * Grid.ERROR_MARGIN)
			{
				result = stack[i];
				break;
			}
		}
		return result;
	}

	private int SortHeightComp(PhysicsItem oEnt1, PhysicsItem oEnt2)
	{
		if (oEnt1 == null)
		{
			if (oEnt2 == null)
			{
				return 0;
			}
			return -1;
		}
		return oEnt2?.Y.CompareTo(oEnt1.Y) ?? 1;
	}

	public int NextTop(int xX, int xY, int xZ)
	{
		int result = atoms.grid.fromY;
		for (int num = xY; num >= atoms.grid.fromY; num--)
		{
			if (At((float)xX * Grid.SPACING.X, (float)num * Grid.SPACING.Y, (float)xZ * Grid.SPACING.Z) != null)
			{
				result = num + 1;
				break;
			}
		}
		return result;
	}

	public void Event_Flip_Start()
	{
		for (int i = 0; i < lengthStack; i++)
		{
			stack[i].Event_Flip_Start();
		}
	}

	public void Event_Flip_End()
	{
		stack.Sort(SortHeightComp);
		for (int i = 0; i < lengthStack; i++)
		{
			stack[i].Event_Flip_End();
		}
	}
}

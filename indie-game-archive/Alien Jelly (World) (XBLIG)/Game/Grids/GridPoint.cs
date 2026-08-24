using System;
using GKEngine.Entities;
using Microsoft.Xna.Framework;

namespace Game.Grids;

public class GridPoint
{
	private Vector3 _vector = default(Vector3);

	private Base3D _base3D;

	private int _X;

	private int _Y;

	private int _Z;

	public virtual int X
	{
		get
		{
			return _X;
		}
		set
		{
			_X = value;
			if (_base3D != null)
			{
				_base3D.X = (float)_X * Grid.SPACING.X;
			}
		}
	}

	public virtual int Y
	{
		get
		{
			return _Y;
		}
		set
		{
			_Y = value;
			if (_base3D != null)
			{
				_base3D.Y = (float)_Y * Grid.SPACING.Y;
			}
		}
	}

	public virtual int Z
	{
		get
		{
			return _Z;
		}
		set
		{
			_Z = value;
			if (_base3D != null)
			{
				_base3D.Z = (float)_Z * Grid.SPACING.Z;
			}
		}
	}

	public GridPoint(int xX, int xY, int xZ)
	{
		X = xX;
		Y = xY;
		Z = xZ;
	}

	public GridPoint()
	{
		X = 0;
		Y = 0;
		Z = 0;
	}

	public GridPoint(Base3D oBase)
	{
		FromPosition(oBase.position);
		Link(oBase);
	}

	public void FromPosition(Vector3 oVector)
	{
		X = (int)(Math.Round(Math.Abs(oVector.X / Grid.SPACING.X)) * (double)Math.Sign(oVector.X));
		Y = (int)(Math.Round(Math.Abs(oVector.Y / Grid.SPACING.Y)) * (double)Math.Sign(oVector.Y));
		Z = (int)(Math.Round(Math.Abs(oVector.Z / Grid.SPACING.Z)) * (double)Math.Sign(oVector.Z));
	}

	public void FromVector3(Vector3 oVector)
	{
		X = (int)(Math.Round(Math.Abs(oVector.X)) * (double)Math.Sign(oVector.X));
		Y = (int)(Math.Round(Math.Abs(oVector.Y)) * (double)Math.Sign(oVector.Y));
		Z = (int)(Math.Round(Math.Abs(oVector.Z)) * (double)Math.Sign(oVector.Z));
	}

	public void FromPoint(GridPoint oPoint)
	{
		X = oPoint.X;
		Y = oPoint.Y;
		Z = oPoint.Z;
	}

	public Vector3 ToVector3()
	{
		_vector.X = X;
		_vector.Y = Y;
		_vector.Z = Z;
		return new Vector3(X, Y, Z);
	}

	public void ToPosition(ref Vector3 vPosition)
	{
		vPosition.X = (float)X * Grid.SPACING.X;
		vPosition.Y = (float)Y * Grid.SPACING.Y;
		vPosition.Z = (float)Z * Grid.SPACING.Z;
	}

	public void Link(Base3D oBase3D)
	{
		_base3D = oBase3D;
		LinkRefresh();
	}

	public void LinkRefresh()
	{
		if (_base3D != null)
		{
			_base3D.X = (float)_X * Grid.SPACING.X;
			_base3D.Y = (float)_Y * Grid.SPACING.Y;
			_base3D.Z = (float)_Z * Grid.SPACING.Z;
		}
	}

	public override string ToString()
	{
		return "{X:" + X + " Y:" + Y + " Z:" + Z + "}";
	}

	public static GridPoint Modulus(ref Vector3 vInc)
	{
		GridPoint gridPoint = new GridPoint();
		if (Math.Abs(vInc.X) >= Grid.SPACING.X)
		{
			gridPoint.X = (int)Math.Floor(Math.Abs(vInc.X) / Grid.SPACING.X) * Math.Sign(vInc.X);
			vInc.X -= (float)gridPoint.X * Grid.SPACING.X;
		}
		if (Math.Abs(vInc.Y) >= Grid.SPACING.Y)
		{
			gridPoint.Y = (int)Math.Floor(Math.Abs(vInc.Y) / Grid.SPACING.Y) * Math.Sign(vInc.Y);
			vInc.Y -= (float)gridPoint.Y * Grid.SPACING.Y;
		}
		if (Math.Abs(vInc.Z) >= Grid.SPACING.Z)
		{
			gridPoint.Z = (int)Math.Floor(Math.Abs(vInc.Z) / Grid.SPACING.Z) * Math.Sign(vInc.Z);
			vInc.Z -= (float)gridPoint.Z * Grid.SPACING.Z;
		}
		return gridPoint;
	}

	public static Vector3 ToPosition(int xX, int xY, int xZ)
	{
		return new Vector3((float)xX * Grid.SPACING.X, (float)xY * Grid.SPACING.Y, (float)xZ * Grid.SPACING.Z);
	}
}

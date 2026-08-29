using System;
using Microsoft.Xna.Framework;

namespace GKEngine.Entities;

public class Point3D
{
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
		}
	}

	public Point3D(int xX, int xY, int xZ)
	{
		X = xX;
		Y = xY;
		Z = xZ;
	}

	public Point3D()
	{
		X = 0;
		Y = 0;
		Z = 0;
	}

	public Point3D(Vector3 vVector)
	{
		FromVector3(vVector);
	}

	public virtual void FromVector3(Vector3 oVector)
	{
		X = (int)Math.Round(oVector.X);
		Y = (int)Math.Round(oVector.Y);
		Z = (int)Math.Round(oVector.Z);
	}

	public virtual Vector3 ToVector3()
	{
		return new Vector3(X, Y, Z);
	}

	public virtual void Copy(Point3D oPoint)
	{
		X = oPoint.X;
		Y = oPoint.Y;
		Z = oPoint.Z;
	}

	public override string ToString()
	{
		return "Point3D {X:" + X + " Y:" + Y + " Z:" + Z + "}";
	}

	public static Point3D Clone(Point3D oPoint)
	{
		return new Point3D(oPoint.X, oPoint.Y, oPoint.Z);
	}
}

using System;
using System.Collections.Generic;
using GKEngine;
using GKEngine.Utils;
using Microsoft.Xna.Framework;

namespace Game.Grids;

public class Grid
{
	private static Vector3 SPACING_MAX = new Vector3(80f, 80f, 80f);

	private static Vector3 SPACING_MIN = new Vector3(20f, 20f, 20f);

	public static Vector3 SPACING = new Vector3(20f, 20f, 20f);

	public static float ERROR_MARGIN = 0.95f;

	public static float COLLISION_MARGIN = 0.01f;

	public int fromX;

	public int fromY;

	public int fromZ;

	public int toX;

	public int toY;

	public int toZ;

	public IGridable[,,] grid;

	private int _width;

	private int _height;

	private int _depth;

	public int width => toX - fromX + 1;

	public int height => toY - fromY + 1;

	public int depth => toZ - fromZ + 1;

	public Grid(int xFromX, int xToX, int xFromY, int xToY, int xFromZ, int xToZ)
	{
		fromX = xFromX;
		fromY = xFromY;
		fromZ = xFromZ;
		toX = xToX;
		toY = xToY;
		toZ = xToZ;
		_width = width;
		_height = height;
		_depth = depth;
		grid = new IGridable[_width, _height, _depth];
		Flush();
	}

	public void Flush()
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				for (int k = 0; k < depth; k++)
				{
					grid[i, j, k] = null;
				}
			}
		}
	}

	public void Flush(Type oType)
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				for (int k = 0; k < depth; k++)
				{
					if (grid[i, j, k] != null && grid[i, j, k].gridType == oType)
					{
						grid[i, j, k] = null;
					}
				}
			}
		}
	}

	public void Remove(GUID oItemGUID)
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				for (int k = 0; k < depth; k++)
				{
					if (grid[i, j, k] != null && grid[i, j, k].gridGUID == oItemGUID)
					{
						grid[i, j, k] = null;
					}
				}
			}
		}
	}

	public void Remove(IGridable oItem)
	{
		for (int i = 0; i < oItem.gridArea.Length; i++)
		{
			int num = oItem.gridPoint.X + oItem.gridArea[i].X + fromX * -1;
			int num2 = oItem.gridPoint.Y + oItem.gridArea[i].Y + fromY * -1;
			int num3 = oItem.gridPoint.Z + oItem.gridArea[i].Z + fromZ * -1;
			if (grid[num, num2, num3] == oItem)
			{
				grid[num, num2, num3] = null;
			}
		}
	}

	public void RandomPos(out int xX, out int xY, out int xZ)
	{
		xX = GameEngine.random.Next(fromX, toX + 1);
		xY = GameEngine.random.Next(fromY, toY + 1);
		xZ = GameEngine.random.Next(fromZ, toZ + 1);
	}

	public void Add(IGridable oItem)
	{
		for (int i = 0; i < oItem.gridArea.Length; i++)
		{
			int num = oItem.gridPoint.X + oItem.gridArea[i].X - fromX;
			int num2 = oItem.gridPoint.Y + oItem.gridArea[i].Y - fromY;
			int num3 = oItem.gridPoint.Z + oItem.gridArea[i].Z - fromZ;
			grid[num, num2, num3] = oItem;
		}
	}

	public void Refresh(IGridable oItem)
	{
		Remove(oItem.gridGUID);
		Add(oItem);
	}

	public void SetTo(IGridable oItem, int xX, int xY, int xZ)
	{
		grid[oItem.gridPoint.X - fromX, oItem.gridPoint.Y - fromY, oItem.gridPoint.Z - fromZ] = null;
		grid[xX - fromX, xY - fromY, xZ - fromZ] = oItem;
	}

	public bool CanFit(GridPoint[] aArea, int xX, int xY, int xZ, List<IGridable> aSelected)
	{
		bool result = false;
		if (InBounds(xX, xY, xZ, aArea) && IsEmpty(xX, xY, xZ, aArea, aSelected))
		{
			result = true;
		}
		return result;
	}

	public bool CanFit(GridPoint[] aArea, int xX, int xY, int xZ)
	{
		bool result = false;
		if (InBounds(xX, xY, xZ, aArea) && IsEmpty(xX, xY, xZ, aArea))
		{
			result = true;
		}
		return result;
	}

	public IGridable At(int xX, int xY, int xZ)
	{
		IGridable result = null;
		if (InBounds(xX, xY, xZ))
		{
			result = grid[xX - fromX, xY - fromY, xZ - fromZ];
		}
		return result;
	}

	public IGridable At(float xX, float xY, float xZ)
	{
		IGridable result = null;
		int num = (int)Math.Round(Math.Abs(xX) * (float)Math.Sign(xX));
		int num2 = (int)Math.Round(Math.Abs(xY) * (float)Math.Sign(xY));
		int num3 = (int)Math.Round(Math.Abs(xZ) * (float)Math.Sign(xZ));
		if (InBounds(num, num2, num3))
		{
			result = grid[num - fromX, num2 - fromY, num3 - fromZ];
		}
		return result;
	}

	public IGridable At(Vector3 vPosition)
	{
		IGridable result = null;
		int num = (int)Math.Round(Math.Abs(vPosition.X / SPACING.X) * (float)Math.Sign(vPosition.X));
		int num2 = (int)Math.Round(Math.Abs(vPosition.Y / SPACING.Y) * (float)Math.Sign(vPosition.Y));
		int num3 = (int)Math.Round(Math.Abs(vPosition.Z / SPACING.Z) * (float)Math.Sign(vPosition.Z));
		if (InBounds(num, num2, num3))
		{
			result = grid[num - fromX, num2 - fromY, num3 - fromZ];
		}
		return result;
	}

	public bool InBounds(int xX, int xY, int xZ)
	{
		if (xX <= toX && xX >= fromX && xY <= toY && xY >= fromY && xZ <= toZ)
		{
			return xZ >= fromZ;
		}
		return false;
	}

	public bool InBounds(int xX, int xY, int xZ, GridPoint[] aPoints)
	{
		bool result = true;
		for (int i = 0; i < aPoints.Length; i++)
		{
			if (!InBounds(xX + aPoints[i].X, xY + aPoints[i].Y, xZ + aPoints[i].Z))
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public bool IsEmpty(int xX, int xY, int xZ, GridPoint[] aPoints)
	{
		bool result = true;
		for (int i = 0; i < aPoints.Length; i++)
		{
			if (grid[xX + aPoints[i].X - fromX, xY + aPoints[i].Y - fromY, xZ + aPoints[i].Z - fromZ] != null)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public bool IsEmpty(int xX, int xY, int xZ, IGridable oItem)
	{
		bool result = true;
		for (int i = 0; i < oItem.gridArea.Length; i++)
		{
			IGridable gridable = grid[xX + oItem.gridArea[i].X - fromX, xY + oItem.gridArea[i].Y - fromY, xZ + oItem.gridArea[i].Z - fromZ];
			if (gridable != null && gridable.gridGUID.value != oItem.gridGUID.value)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public bool IsEmpty(int xX, int xY, int xZ, GridPoint[] aPoints, List<IGridable> aPiers)
	{
		bool result = true;
		for (int i = 0; i < aPoints.Length; i++)
		{
			IGridable gridable = grid[xX + aPoints[i].X - fromX, xY + aPoints[i].Y - fromY, xZ + aPoints[i].Z - fromZ];
			if (gridable != null && !aPiers.Contains(gridable))
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public int Top(int xX, int xZ)
	{
		int num;
		for (num = toY; num >= fromY; num--)
		{
			if (At(xX, num, xZ) != null)
			{
				num++;
				break;
			}
		}
		return num;
	}

	public int NextTop(int xX, int xY, int xZ)
	{
		int result = fromY;
		for (int num = xY; num >= fromY; num--)
		{
			if (At(xX, num, xZ) != null)
			{
				result = num + 1;
				break;
			}
		}
		return result;
	}

	public override string ToString()
	{
		string text = "";
		for (int i = 0; i < height; i++)
		{
			text = text + "Grid for Y:" + (i + fromY) + "\n";
			for (int j = 0; j < depth; j++)
			{
				for (int k = 0; k < width; k++)
				{
					text = text + ((grid[k, i, j] == null) ? "0" : "1") + "\t";
				}
				text += "\n";
			}
		}
		return text;
	}

	public static void Lerp(float xRatio)
	{
		SPACING = Vector3.Lerp(SPACING_MIN, SPACING_MAX, xRatio);
	}

	public static bool Contains(IGridable oItem, List<IGridable> aItems)
	{
		bool result = false;
		int count = aItems.Count;
		for (int i = 0; i < count; i++)
		{
			if (aItems[i].gridGUID.value == oItem.gridGUID.value)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public static bool BoxCollide(Vector3 oSubject, Vector3 oObject, ref Vector3 oError, ref Vector3 oDelta, ref BoundingBox oVolume)
	{
		Vector3 vector = SPACING / 2f;
		bool result = false;
		oDelta.X = oObject.X - oSubject.X;
		oDelta.Y = oObject.Y - oSubject.Y;
		oDelta.Z = oObject.Z - oSubject.Z;
		if (Math.Abs(oDelta.X) < SPACING.X - oError.X && Math.Abs(oDelta.Y) < SPACING.Y - oError.Y && Math.Abs(oDelta.Z) < SPACING.Z - oError.Z)
		{
			oVolume.Min.X = ((oSubject.X - vector.X > oObject.X - vector.X) ? (oSubject.X - vector.X) : (oObject.X - vector.X));
			oVolume.Max.X = ((oSubject.X + vector.X < oObject.X + vector.X) ? (oSubject.X + vector.X) : (oObject.X + vector.X));
			oVolume.Min.Y = ((oSubject.Y - vector.Y > oObject.Y - vector.Y) ? (oSubject.Y - vector.Y) : (oObject.Y - vector.Y));
			oVolume.Max.Y = ((oSubject.Y + vector.Y < oObject.Y + vector.Y) ? (oSubject.Y + vector.Y) : (oObject.Y + vector.Y));
			oVolume.Min.Z = ((oSubject.Z - vector.Z > oObject.Z - vector.Z) ? (oSubject.Z - vector.Z) : (oObject.Z - vector.Z));
			oVolume.Max.Z = ((oSubject.Z + vector.Z < oObject.Z + vector.Z) ? (oSubject.Z + vector.Z) : (oObject.Z + vector.Z));
			result = true;
		}
		return result;
	}

	public static bool BoxCollide(Vector3 oSubject, Vector3 oObject, ref Vector3 oDelta)
	{
		_ = SPACING / 2f;
		bool result = false;
		oDelta.X = oObject.X - oSubject.X;
		oDelta.Y = oObject.Y - oSubject.Y;
		oDelta.Z = oObject.Z - oSubject.Z;
		if (Math.Abs(oDelta.X) < SPACING.X && Math.Abs(oDelta.Y) < SPACING.Y && Math.Abs(oDelta.Z) < SPACING.Z)
		{
			result = true;
		}
		return result;
	}
}

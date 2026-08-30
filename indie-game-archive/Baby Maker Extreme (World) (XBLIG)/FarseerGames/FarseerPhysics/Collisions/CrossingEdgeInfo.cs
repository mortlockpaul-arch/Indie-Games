using System;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Collisions;

public class CrossingEdgeInfo : IComparable
{
	private Vector2 _egdeVertex1;

	private Vector2 _edgeVertex2;

	private EdgeAlignment _alignment;

	private Vector2 _crossingPoint;

	public Vector2 EdgeVertex1
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _egdeVertex1;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_egdeVertex1 = value;
		}
	}

	public Vector2 EdgeVertex2
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _edgeVertex2;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_edgeVertex2 = value;
		}
	}

	public EdgeAlignment CheckLineAlignment
	{
		get
		{
			return _alignment;
		}
		set
		{
			_alignment = value;
		}
	}

	public Vector2 CrossingPoint
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _crossingPoint;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_crossingPoint = value;
		}
	}

	public CrossingEdgeInfo(Vector2 edgeVertex1, Vector2 edgeVertex2, Vector2 crossingPoint, EdgeAlignment checkLineAlignment)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		_egdeVertex1 = edgeVertex1;
		_edgeVertex2 = edgeVertex2;
		_alignment = checkLineAlignment;
		_crossingPoint = crossingPoint;
	}

	public int CompareTo(object obj)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		CrossingEdgeInfo crossingEdgeInfo = (CrossingEdgeInfo)obj;
		int result = 0;
		switch (_alignment)
		{
		case EdgeAlignment.Vertical:
			if (_crossingPoint.X < crossingEdgeInfo.CrossingPoint.X)
			{
				result = -1;
			}
			else if (_crossingPoint.X > crossingEdgeInfo.CrossingPoint.X)
			{
				result = 1;
			}
			break;
		case EdgeAlignment.Horizontal:
			if (_crossingPoint.Y < crossingEdgeInfo.CrossingPoint.Y)
			{
				result = -1;
			}
			else if (_crossingPoint.Y > crossingEdgeInfo.CrossingPoint.Y)
			{
				result = 1;
			}
			break;
		}
		return result;
	}
}

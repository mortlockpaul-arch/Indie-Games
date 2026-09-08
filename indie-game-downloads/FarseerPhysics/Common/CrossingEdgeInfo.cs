using System;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common;

public sealed class CrossingEdgeInfo : IComparable
{
	private EdgeAlignment _alignment;

	private Vector2 _crossingPoint;

	private Vector2 _edgeVertex2;

	private Vector2 _egdeVertex1;

	public Vector2 EdgeVertex1
	{
		get
		{
			return _egdeVertex1;
		}
		set
		{
			_egdeVertex1 = value;
		}
	}

	public Vector2 EdgeVertex2
	{
		get
		{
			return _edgeVertex2;
		}
		set
		{
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
			return _crossingPoint;
		}
		set
		{
			_crossingPoint = value;
		}
	}

	public CrossingEdgeInfo(Vector2 edgeVertex1, Vector2 edgeVertex2, Vector2 crossingPoint, EdgeAlignment checkLineAlignment)
	{
		_egdeVertex1 = edgeVertex1;
		_edgeVertex2 = edgeVertex2;
		_alignment = checkLineAlignment;
		_crossingPoint = crossingPoint;
	}

	public int CompareTo(object obj)
	{
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

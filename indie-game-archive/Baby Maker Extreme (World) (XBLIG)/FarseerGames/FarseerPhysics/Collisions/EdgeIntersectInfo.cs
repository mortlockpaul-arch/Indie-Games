using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Collisions;

public class EdgeIntersectInfo
{
	[CompilerGenerated]
	private Vector2 _003CIntersectionPoint_003Ek__BackingField;

	public Edge EdgeOne { get; private set; }

	public Edge EdgeTwo { get; private set; }

	public Vector2 IntersectionPoint
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CIntersectionPoint_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CIntersectionPoint_003Ek__BackingField = value;
		}
	}

	public EdgeIntersectInfo(Edge edgeOne, Edge edgeTwo, Vector2 intersectionPoint)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		EdgeOne = edgeOne;
		EdgeTwo = edgeTwo;
		IntersectionPoint = intersectionPoint;
	}
}

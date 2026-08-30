using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Collisions;

public class Edge
{
	[CompilerGenerated]
	private Vector2 _003CEdgeStart_003Ek__BackingField;

	[CompilerGenerated]
	private Vector2 _003CEdgeEnd_003Ek__BackingField;

	public Vector2 EdgeStart
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CEdgeStart_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CEdgeStart_003Ek__BackingField = value;
		}
	}

	public Vector2 EdgeEnd
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CEdgeEnd_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CEdgeEnd_003Ek__BackingField = value;
		}
	}

	public Edge(Vector2 edgeStart, Vector2 edgeEnd)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		EdgeStart = edgeStart;
		EdgeEnd = edgeEnd;
	}
}

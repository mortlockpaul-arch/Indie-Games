using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Collisions;

public struct Feature
{
	public float Distance;

	public Vector2 Normal;

	public Vector2 Position;

	public Feature(ref Vector2 position, ref Vector2 normal, float distance)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Position = position;
		Normal = normal;
		Distance = distance;
	}

	public override int GetHashCode()
	{
		return (int)(Normal.X + Normal.Y + Position.X + Position.Y + Distance);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Feature))
		{
			return false;
		}
		return Equals((Feature)obj);
	}

	public bool Equals(Feature other)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (Normal == other.Normal && Position == other.Position)
		{
			return Distance == other.Distance;
		}
		return false;
	}

	public bool Equals(ref Feature other)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (Normal == other.Normal && Position == other.Position)
		{
			return Distance == other.Distance;
		}
		return false;
	}

	public static bool operator ==(Feature first, Feature second)
	{
		return first.Equals(ref second);
	}

	public static bool operator !=(Feature first, Feature second)
	{
		return !first.Equals(ref second);
	}
}

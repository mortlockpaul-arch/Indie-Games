using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Collisions;

internal struct PolygonCollisionResult
{
	public bool Intersect;

	public Vector2 MinimumTranslationVector;

	public int bestEdgeIndex;
}

using FarseerGames.FarseerPhysics.Collisions;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Interfaces;

public interface INarrowPhaseCollider
{
	void Collide(Geom geomA, Geom geomB, ContactList contactList);

	bool Intersect(Geom geom, ref Vector2 point);
}

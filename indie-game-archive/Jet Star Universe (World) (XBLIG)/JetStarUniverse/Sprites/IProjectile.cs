using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace JetStarUniverse.Sprites;

public interface IProjectile
{
	List<Projectile> Projectiles { get; set; }

	Rectangle SourceProjectile { get; set; }
}

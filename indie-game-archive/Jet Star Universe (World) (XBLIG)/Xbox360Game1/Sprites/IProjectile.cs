using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Xbox360Game1.Sprites;

public interface IProjectile
{
	List<Projectile> Projectiles { get; set; }

	Rectangle SourceProjectile { get; set; }
}

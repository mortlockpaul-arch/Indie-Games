using Microsoft.Xna.Framework;

namespace SpaceBlast.Weapons;

internal abstract class WeaponRound
{
	public abstract bool Update();

	public abstract void Draw();

	public abstract BoundingSphere GetBoundingSphere();

	public abstract int GetHitDamage();
}

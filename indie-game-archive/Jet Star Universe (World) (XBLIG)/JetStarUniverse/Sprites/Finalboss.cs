namespace JetStarUniverse.Sprites;

public class Finalboss : Miniboss
{
	public Finalboss(int width, int height)
		: base(width, height)
	{
		base.Life = 100;
		base.Projectiles.Add(new Projectile());
		base.Projectiles.Add(new Projectile());
		base.Projectiles.Add(new Projectile());
		base.Projectiles.Add(new Projectile());
		base.Projectiles.Add(new Projectile());
	}
}

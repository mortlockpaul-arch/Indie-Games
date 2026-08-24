using Microsoft.Xna.Framework;

namespace ZP2K9.particles;

public class Explode
{
	public Vector2 loc;

	public float splash;

	public int damage;

	public bool exists;

	public void Init(Vector2 loc, float splash, int damage)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		this.loc = loc;
		this.splash = splash;
		this.damage = damage;
		exists = true;
	}
}

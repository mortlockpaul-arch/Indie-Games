using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9.particles.debris;

public class WaterSplash
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, float size)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		p.loc = loc;
		p.orig = loc;
		p.traj = traj;
		p.size = size;
		p.frame = Rand.GetRandomFloat(0.5f, 1f);
		p.dir = Rand.GetRandomFloat(-2f, 2f);
		p.alpha = true;
	}

	public static void Update(Particle p, float fTime)
	{
		ref Vector2 traj = ref p.traj;
		traj.Y += fTime * Game1.gravity;
		if (p.loc.Y > p.orig.Y)
		{
			p.frame = -1f;
		}
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(544, 0, 96, 96), new Color(new Vector4(0.3f, 0.39f, 0.4f, p.frame)), p.frame * p.dir, new Vector2(48f, 48f), p.size * Scroll.zoom, (SpriteEffects)0, 1f);
	}
}

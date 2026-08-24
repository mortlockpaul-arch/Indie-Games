using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9.particles.shot;

public class MuzzleFlash
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, float size)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		p.loc = loc;
		p.traj = traj;
		p.size = size;
		p.angle = Rand.GetRandomFloat(0f, 6f);
		p.alpha = true;
		p.frame = 0.05f;
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(0, 0, 64, 64), new Color(new Vector4(p.frame * 20f, p.frame * 10f, p.frame * 5f, 1f)), p.frame * 5f + p.angle, new Vector2(32f, 32f), p.size * Scroll.zoom + (0.05f - p.frame) * 10f, (SpriteEffects)0, 1f);
		if (p.size >= 0.5f)
		{
			Game1.postGlowMgr.Add(Scroll.GetLoc(p.loc), 1f, 0.5f, 0.1f, 2f * p.frame, 2f);
		}
	}
}

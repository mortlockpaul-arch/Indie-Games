using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9.particles.shot;

public class Shell
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		p.loc = loc;
		p.traj = traj + Rand.GetRandomVec2(-50f, 50f, -50f, 50f);
		p.frame = 5f;
		p.dir = Rand.GetRandomFloat(-10f, 10f);
		p.bounce = true;
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(16, 64, 16, 16), new Color(new Vector4(1f, 1f, 1f, p.frame)), p.angle, new Vector2(8f, 8f), Scroll.zoom * 0.4f, (SpriteEffects)0, 1f);
	}
}

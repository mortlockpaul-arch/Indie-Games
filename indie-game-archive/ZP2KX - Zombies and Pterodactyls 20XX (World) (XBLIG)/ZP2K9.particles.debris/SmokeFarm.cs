using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.debris;

public class SmokeFarm
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		p.loc = loc;
		p.traj = traj;
		p.frame = Rand.GetRandomFloat(0.25f, 0.5f);
		p.bounce = true;
		p.alpha = true;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		float num = p.frame - fTime;
		if ((int)(p.frame * 10f) != (int)(num * 10f))
		{
			Game1.pMan.AddParticle(2, p.loc, Rand.GetRandomVec2(-10f, 10f, -100f, -90f), p.frame * Rand.GetRandomFloat(0.4f, 0.8f), 0, 0);
		}
		p.BaseUpdate(map, c, fTime);
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(0, 160, 64, 64), new Color(new Vector4(1f, 1f, 1f, p.frame)), Rand.GetRandomRadian(), new Vector2(32f, 32f), Rand.GetRandomFloat(0.2f, 1f), (SpriteEffects)0, 1f);
		Game1.postGlowMgr.Add(Scroll.GetLoc(p.loc), 1f, 0.9f, 0.8f, 1f, 0f, 0.1f * p.frame);
	}
}

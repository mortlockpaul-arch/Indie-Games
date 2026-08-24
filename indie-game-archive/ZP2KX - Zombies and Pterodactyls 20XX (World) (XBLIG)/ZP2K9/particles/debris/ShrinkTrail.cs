using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.debris;

public class ShrinkTrail
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, float size)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		p.size = size;
		p.frame = Rand.GetRandomFloat(0.5f, 1f);
		p.loc = loc;
		p.traj = traj;
		p.angle = Rand.GetRandomRadian();
		p.dir = Rand.GetRandomFloat(-1f, 1f);
		p.alpha = true;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		p.angle += p.dir * fTime;
		ref Vector2 traj = ref p.traj;
		traj.X += fTime * Rand.GetRandomFloat(10f, 40f);
		ref Vector2 traj2 = ref p.traj;
		traj2.Y -= fTime * Rand.GetRandomFloat(10f, 40f);
		p.BaseUpdate(map, c, fTime);
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(64, 0, 64, 64), new Color(new Vector4(0.1f, 1f, 0.4f, 1f) * p.frame), p.angle, new Vector2(32f, 32f), p.size * Scroll.zoom + (1f - p.frame), (SpriteEffects)0, 1f);
	}
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yuki_Win;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.debris;

public class LaserTrail
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, float size)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		p.size = size;
		p.frame = 1f;
		p.loc = loc;
		p.traj = traj;
		p.angle = Trig.GetAngle(default(Vector2), traj);
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
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 3; i++)
		{
			sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(256 + i * 64, 0, 64, 64), new Color(new Vector4(0.1f, 0.5f, 1f, 1f) * p.frame), p.angle, new Vector2(32f, 32f), p.size * Scroll.zoom * new Vector2(2f, 1.01f - p.frame) * 5f, (SpriteEffects)0, 1f);
		}
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(64, 0, 64, 64), new Color(new Vector4(0.1f, 0.5f, 1f, 1f) * p.frame * 0.5f), p.angle, new Vector2(32f, 32f), p.size * Scroll.zoom * new Vector2(2f, 2f - p.frame) * 5f, (SpriteEffects)0, 1f);
	}
}

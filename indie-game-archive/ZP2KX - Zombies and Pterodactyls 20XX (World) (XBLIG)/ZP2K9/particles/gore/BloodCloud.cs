using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.characters;
using ZP2K9.map;

namespace ZP2K9.particles.gore;

public class BloodCloud
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, float size)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		p.loc = loc;
		p.traj = traj;
		p.frame = 0.3f;
		p.angle = Rand.GetRandomFloat(0f, 6.28f);
		p.size = size;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		ref Vector2 traj = ref p.traj;
		traj.Y += fTime * Game1.gravity * 0.3f;
		p.BaseUpdate(map, c, fTime);
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)((1f - p.frame / 0.3f) * 9f);
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(num * 64, 96, 64, 64), new Color(new Vector4(0.5f, 0f, 0f, 0.5f)), p.angle, new Vector2(32f, 32f), Scroll.zoom * p.size, (SpriteEffects)0, 1f);
	}
}

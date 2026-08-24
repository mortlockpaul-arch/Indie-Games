using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9.characters;

public class FlashLight
{
	public Vector2 orig;

	public Vector2 flashVec;

	public Vector2 goalVec;

	public bool active;

	public void Update()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		flashVec += (goalVec - flashVec) * Game1.frameTime * 10f;
		if (float.IsNaN(flashVec.X))
		{
			flashVec.X = 0f;
		}
		if (float.IsNaN(flashVec.Y))
		{
			flashVec.Y = 0f;
		}
		int playerOne = Game1.netSession.GetPlayerOne();
		if (playerOne > -1 && Game1.character[playerOne] != null)
		{
			Vector2 val = goalVec;
			if (((Vector2)(ref Game1.character[playerOne].charKeys.shootVec)).Length() > 0.1f)
			{
				val = Game1.character[playerOne].charKeys.shootVec;
			}
			else if (((Vector2)(ref Game1.character[playerOne].charKeys.runVec)).Length() > 0.1f)
			{
				val = Game1.character[playerOne].charKeys.runVec;
				val.Y = 0f - val.Y;
			}
			((Vector2)(ref val)).Normalize();
			goalVec = val * 30f;
		}
	}

	public void Draw(SpriteBatch sprite)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		int playerOne = Game1.netSession.GetPlayerOne();
		if (playerOne > -1 && Game1.character[playerOne] != null)
		{
			orig = Scroll.GetLoc(Game1.character[playerOne].drawVec + new Vector2(0f, -50f));
		}
		sprite.Begin((SpriteBlendMode)2);
		for (int i = 0; i < 20; i++)
		{
			sprite.Draw(Game1.spritesTex, orig + flashVec * (float)i, (Rectangle?)new Rectangle(0, 832, 192, 192), new Color(1f, 0f, 0f, 0.25f), 0f, new Vector2(96f, 96f), 1f + (float)i / 8f, (SpriteEffects)0, 1f);
		}
		sprite.End();
	}
}

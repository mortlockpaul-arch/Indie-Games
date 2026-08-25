using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xCharEdit.Character;
using yMapEdit.map;

namespace ZP2K9.characters;

public class Pterodactyl
{
	public Vector2 loc;

	public Vector2 traj;

	public int frameIdx;

	public int face;

	public int anim;

	public int key;

	public float animFrame;

	public bool exists;

	public void Update()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		loc += traj * Game1.frameTime;
		ref Vector2 reference = ref traj;
		reference.Y -= Game1.frameTime * 100f;
		if (loc.Y < 0f)
		{
			exists = false;
		}
		animFrame += Game1.frameTime * 30f;
		Animation animation = Game1.charDef[1].GetAnimation(anim);
		KeyFrame keyFrame = animation.GetKeyFrame(key);
		if (animFrame > (float)keyFrame.duration)
		{
			animFrame -= keyFrame.duration;
			key++;
			keyFrame = animation.GetKeyFrame(key);
			if (key >= animation.getKeyFrameArray().Length)
			{
				key = 0;
			}
		}
		if (keyFrame.frameRef < 0)
		{
			key = 0;
		}
	}

	public void Draw(SpriteBatch sprite)
	{
		CharDef charDef = Game1.charDef[1];
		if (charDef.GetAnimation(anim).GetKeyFrame(key).lerp)
		{
			frameIdx = charDef.GetAnimation(anim).GetKeyFrame(key).frameRef;
			if (frameIdx < 0)
			{
				frameIdx = 0;
			}
			int idx = key + 1;
			if (charDef.GetAnimation(anim).GetKeyFrame(idx).duration <= 0)
			{
				idx = 0;
			}
			Draw(sprite, charDef.GetAnimation(anim).GetKeyFrame(idx).frameRef);
		}
		else
		{
			frameIdx = charDef.GetAnimation(anim).GetKeyFrame(key).frameRef;
			if (frameIdx < 0)
			{
				frameIdx = 0;
			}
			Draw(sprite, -1);
		}
	}

	public void Draw(SpriteBatch spriteBatch, int next)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		Rectangle value = default(Rectangle);
		CharDef charDef = Game1.charDef[1];
		Frame frame = charDef.GetFrame(frameIdx);
		float num = 0.9f * ScrollManager.zoom;
		Vector2 screenLoc = ScrollManager.GetScreenLoc(loc, 1f);
		Color val3 = default(Color);
		for (int i = 0; i < frame.GetPartArray().Length; i++)
		{
			Part part = frame.GetPart(i);
			if (part.idx <= -1)
			{
				continue;
			}
			float num2 = part.rotation;
			Vector2 val = part.location * num + screenLoc;
			Vector2 val2 = part.scaling * num;
			bool flag = false;
			if ((face == 1 && part.flip == 0) || (face == 0 && part.flip == 1))
			{
				flag = true;
			}
			if (face == 0)
			{
				num2 = 0f - num2;
				val.X -= part.location.X * num * 2f;
			}
			if (next > -1)
			{
				Frame frame2 = charDef.GetFrame(next);
				if (Frame.CanLerp(frame, frame2, i))
				{
					Part part2 = frame2.GetPart(i);
					Animation animation = charDef.GetAnimation(anim);
					KeyFrame keyFrame = animation.GetKeyFrame(key);
					float progress = animFrame / (float)keyFrame.duration;
					Vector2 location = part.location;
					Vector2 location2 = part2.location;
					float num3 = part.rotation;
					float num4 = part2.rotation;
					if (face == 0)
					{
						num3 = 0f - num3;
						num4 = 0f - num4;
						location.X -= part.location.X * 2f;
						location2.X -= part2.location.X * 2f;
					}
					val = Frame.LerpLoc(location, location2, progress) * num + screenLoc;
					num2 = Frame.LerpRotation(num3, num4, progress);
					val2 = Frame.LerpScale(part.scaling, part2.scaling, progress) * num;
				}
			}
			((Color)(ref val3))._002Ector(new Vector4(1f, 1f, 1f, 1f));
			if (part.idx < 1000)
			{
				Texture2D val4;
				switch (part.idx / 64)
				{
				case 0:
					val4 = Game1.charTex[charDef.charIdx].tex;
					value = Game1.charTex[charDef.charIdx].GetRect(part.idx);
					break;
				case 1:
					val4 = Game1.weapTex[charDef.weaponIdx].tex;
					value = Game1.weapTex[charDef.weaponIdx].GetRect(part.idx - 64);
					break;
				case 2:
					val4 = Game1.pteroTex[0].tex;
					value = Game1.pteroTex[0].GetRect(part.idx - 128);
					break;
				default:
					val4 = null;
					break;
				}
				if (val4 != null)
				{
					spriteBatch.Draw(val4, val, (Rectangle?)value, val3, num2, new Vector2((float)value.Width / 2f, (float)value.Height / 2f), val2, (SpriteEffects)(!flag), 1f);
				}
			}
		}
	}
}

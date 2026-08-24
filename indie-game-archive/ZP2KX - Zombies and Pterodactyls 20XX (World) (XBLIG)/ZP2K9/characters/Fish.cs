using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.particles;
using xCharEdit.Character;
using yMapEdit.map;

namespace ZP2K9.characters;

public class Fish
{
	public Vector2 loc;

	public Vector2 traj;

	public int frameIdx;

	public int face;

	public int anim;

	public int key;

	public float animFrame;

	public bool exists;

	public void Update(Character c)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		if (c == null)
		{
			return;
		}
		loc += traj * Game1.frameTime;
		ref Vector2 reference = ref traj;
		reference.Y += Game1.frameTime * 500f;
		if (loc.Y > 8192f)
		{
			exists = false;
		}
		if (anim == 2)
		{
			Game1.pMan.AddParticle(50, loc + Rand.GetRandomVec2(-80f, 80f, -100f, 40f), Rand.GetRandomVec2(0f, 0f, -50f, 0f), 0.1f, 0, 0);
		}
		animFrame += Game1.frameTime * 30f;
		Animation animation = Game1.charDef[2].GetAnimation(anim);
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
		if (keyFrame.frameRef >= 0)
		{
			return;
		}
		key = 0;
		switch (anim)
		{
		case 1:
		{
			anim = 2;
			Sound.PlayCue("hit1");
			Sound.PlayCue("hit2");
			Sound.PlayCue("hit3");
			if (c.hp >= 0)
			{
				KillManager.DoKill(c.lastHitBy, c.ID, 11);
			}
			c.hp = -50;
			c.StartKill(default(Vector2));
			for (int i = 0; i < 50; i++)
			{
				Game1.pMan.AddParticle(50, loc + Rand.GetRandomVec2(-80f, 80f, -100f, 40f), Rand.GetRandomVec2(0f, 0f, -50f, 0f), 0.1f, 0, 0);
			}
			break;
		}
		case 2:
			anim = 0;
			break;
		case 0:
			break;
		}
	}

	public void Draw(SpriteBatch sprite)
	{
		CharDef charDef = Game1.charDef[2];
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
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		Rectangle value = default(Rectangle);
		CharDef charDef = Game1.charDef[2];
		Frame frame = charDef.GetFrame(frameIdx);
		float num = 0.9f * ScrollManager.zoom;
		Vector2 screenLoc = ScrollManager.GetScreenLoc(loc, 1f);
		int num2 = 1 - face;
		float num3 = 1f;
		Vector2 val = Scroll.GetLoc(loc);
		if (val.Y > 400f)
		{
			num3 = 1f - (val.Y - 400f) * 0.01f;
		}
		val = Scroll.GetLoc(loc + new Vector2(0f, -80f));
		if (val.Y > 350f)
		{
			float num4 = (val.Y - 350f) * 0.01f;
			if (num4 > 1f)
			{
				num4 = 1f;
			}
			if (val.Y > 500f)
			{
				num4 -= (val.Y - 500f) * 0.04f;
			}
			if (num4 > 0f)
			{
				Game1.postGlowMgr.Add(val, 1f * num4, 0.6f * num4, 0.4f * num4, 0.1f, 4f, default(Vector2), 0f);
			}
		}
		Color val4 = default(Color);
		for (int i = 0; i < frame.GetPartArray().Length; i++)
		{
			Part part = frame.GetPart(i);
			if (part.idx <= -1)
			{
				continue;
			}
			float num5 = part.rotation;
			Vector2 val2 = part.location * num + screenLoc;
			Vector2 val3 = part.scaling * num;
			bool flag = false;
			if ((num2 == 1 && part.flip == 0) || (num2 == 0 && part.flip == 1))
			{
				flag = true;
			}
			if (num2 == 0)
			{
				num5 = 0f - num5;
				val2.X -= part.location.X * num * 2f;
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
					float num6 = part.rotation;
					float num7 = part2.rotation;
					if (num2 == 0)
					{
						num6 = 0f - num6;
						num7 = 0f - num7;
						location.X -= part.location.X * 2f;
						location2.X -= part2.location.X * 2f;
					}
					val2 = Frame.LerpLoc(location, location2, progress) * num + screenLoc;
					num5 = Frame.LerpRotation(num6, num7, progress);
					val3 = Frame.LerpScale(part.scaling, part2.scaling, progress) * num;
				}
			}
			((Color)(ref val4))._002Ector(new Vector4(num3, num3, num3, 1f));
			if (part.idx >= 1000)
			{
				continue;
			}
			Texture2D val5;
			switch (part.idx / 64)
			{
			case 0:
				val5 = Game1.charTex[charDef.charIdx].tex;
				value = Game1.charTex[charDef.charIdx].GetRect(part.idx);
				break;
			case 1:
				val5 = Game1.weapTex[charDef.weaponIdx].tex;
				value = Game1.weapTex[charDef.weaponIdx].GetRect(part.idx - 64);
				break;
			case 2:
				val5 = Game1.pteroTex[1].tex;
				value = Game1.pteroTex[1].GetRect(part.idx - 128);
				break;
			default:
				val5 = null;
				break;
			}
			if (val5 != null)
			{
				spriteBatch.Draw(val5, val2, (Rectangle?)value, val4, num5, new Vector2((float)value.Width / 2f, (float)value.Height / 2f), val3, (SpriteEffects)(!flag), 1f);
			}
			if (part.idx % 64 == 4)
			{
				float num8 = 1f;
				if (val2.Y > 500f)
				{
					num8 -= (val2.Y - 500f) * 0.04f;
				}
				if (num8 > 0f)
				{
					Game1.postGlowMgr.Add(val2, 1f, 1f, 1f, 0.2f * num8, 1f);
				}
			}
		}
	}
}

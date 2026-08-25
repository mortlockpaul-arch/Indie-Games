using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xCharEdit.Character;
using ZP2K9.characters.weapons;
using ZP2K9.particles;

namespace ZP2K9.characters;

public class BodySec
{
	public const int END_NONE = 0;

	public const int END_IDLE = 1;

	public const int END_JUMP = 2;

	public const int END_GETUP = 3;

	public const int END_FLY = 4;

	public const int END_DIE = 5;

	public float curFrame;

	public int anim;

	public int key;

	public int ID;

	private Vector2 torsoVec;

	public int endAction;

	public string animName;

	public BodySec(int ID)
	{
		this.ID = ID;
	}

	public void SetAnimNameFromInt(Character c)
	{
		animName = Game1.charDef[c.defIdx].GetAnimation(anim).name;
	}

	public void Update(Character c, float fTime)
	{
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		Animation animation = Game1.charDef[c.defIdx].GetAnimation(anim);
		KeyFrame keyFrame = animation.GetKeyFrame(key);
		float num = fTime;
		if (c.freeze > 0f || c.rainbowed > 0f)
		{
			num /= 5f;
		}
		bool flag = false;
		if (GameState.gameType == 4 && c.team == 1)
		{
			flag = true;
		}
		if (ID == 0)
		{
			if ((c.suit == 2 || flag) && c.charKeys.keyFloat)
			{
				switch (animName)
				{
				case "idlew":
				case "idlem":
				case "idles":
				case "idlea":
				case "idler":
				case "runw":
				case "runm":
				case "runs":
				case "runa":
				case "runr":
				case "runx":
					num *= 2f;
					break;
				}
			}
			if (c.perk[0] == 9)
			{
				switch (animName)
				{
				case "cart":
				case "roll":
				case "rollx":
					num *= 2f;
					break;
				}
			}
		}
		curFrame += num * 30f;
		int num2 = key;
		if (curFrame > (float)keyFrame.duration)
		{
			if (key != 0)
			{
				CheckTrig(c);
			}
			curFrame -= keyFrame.duration;
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
		if (ID == 1)
		{
			c.splitAnim = false;
		}
		switch (endAction)
		{
		case 1:
			SetAnim(c.GetAnimName(0), c);
			switch (c.state)
			{
			case 1:
				c.angle = 0f;
				break;
			case 2:
				c.angle = 1.57f;
				break;
			case 3:
				c.angle = 4.71f;
				break;
			case 4:
				c.angle = 3.14f;
				break;
			}
			break;
		case 4:
			SetAnim(c.GetAnimName(2), c);
			break;
		case 2:
			SetAnim(c.GetAnimName(2), c);
			switch (c.state)
			{
			case 2:
				c.traj.X = 300f;
				if (c.suit == 2)
				{
					c.traj *= 1.5f;
				}
				c.angle -= 6.28f;
				break;
			case 3:
				c.traj.X = -300f;
				if (c.suit == 2)
				{
					c.traj *= 1.5f;
				}
				c.angle += 6.28f;
				break;
			case 4:
				c.traj.Y = 400f;
				break;
			default:
				if (c.charKeys.jumpPower < 0.3f)
				{
					c.charKeys.jumpPower = 1f;
				}
				if (c.submerged)
				{
					c.charKeys.jumpPower = 1f;
				}
				c.traj.Y = -590f * c.charKeys.jumpPower;
				if (c.suit == 2)
				{
					c.traj *= 1.2f;
				}
				break;
			}
			c.state = 0;
			break;
		case 3:
			if (c.hp < 0)
			{
				key = num2;
				if (c.dyingFrame <= 0f)
				{
					c.dyingFrame = 1f;
				}
			}
			else
			{
				SetAnim(c.GetAnimName(8), c);
				endAction = 1;
			}
			if (c.ai != null)
			{
				c.ai.KillTrail();
			}
			break;
		case 5:
			c.hp = -1;
			c.lastHitBy = c.ID;
			break;
		}
	}

	public void SetAnim(string anim, Character c)
	{
		SetAnim(anim, c, overRide: false);
	}

	public void SetAnim(string anim, Character c, bool overRide)
	{
		for (int i = 0; i < Game1.charDef[c.defIdx].GetAnimationArray().Length; i++)
		{
			if (Game1.charDef[c.defIdx].GetAnimation(i).name == anim && (this.anim != i || overRide))
			{
				animName = anim;
				endAction = 0;
				this.anim = i;
				key = 0;
				curFrame = 0f;
				break;
			}
		}
	}

	public void CheckTrig(Character c)
	{
		if (ID == 1)
		{
			if (c.splitAnim)
			{
				CheckPartTrig(0, 0, all: false, c);
				CheckPartTrig(1, 1, all: false, c);
			}
		}
		else
		{
			CheckPartTrig(0, 0, all: true, c);
		}
	}

	public void CheckPartTrig(int ps, int sec, bool all, Character c)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1079: Unknown result type (might be due to invalid IL or missing references)
		//IL_107b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f92: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fcc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1053: Unknown result type (might be due to invalid IL or missing references)
		//IL_1055: Unknown result type (might be due to invalid IL or missing references)
		//IL_105c: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_10bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_10df: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0637: Unknown result type (might be due to invalid IL or missing references)
		//IL_0694: Unknown result type (might be due to invalid IL or missing references)
		//IL_069b: Unknown result type (might be due to invalid IL or missing references)
		//IL_071a: Unknown result type (might be due to invalid IL or missing references)
		//IL_071f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0724: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		//IL_115b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1162: Unknown result type (might be due to invalid IL or missing references)
		//IL_1167: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_11bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_11e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_11e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1219: Unknown result type (might be due to invalid IL or missing references)
		//IL_121b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1222: Unknown result type (might be due to invalid IL or missing references)
		//IL_1229: Unknown result type (might be due to invalid IL or missing references)
		//IL_127f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1281: Unknown result type (might be due to invalid IL or missing references)
		//IL_1288: Unknown result type (might be due to invalid IL or missing references)
		//IL_128f: Unknown result type (might be due to invalid IL or missing references)
		//IL_124c: Unknown result type (might be due to invalid IL or missing references)
		//IL_124e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1255: Unknown result type (might be due to invalid IL or missing references)
		//IL_125c: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_12bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_12c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_12e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_12e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1318: Unknown result type (might be due to invalid IL or missing references)
		//IL_131a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1321: Unknown result type (might be due to invalid IL or missing references)
		//IL_1328: Unknown result type (might be due to invalid IL or missing references)
		//IL_1348: Unknown result type (might be due to invalid IL or missing references)
		//IL_134a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1351: Unknown result type (might be due to invalid IL or missing references)
		//IL_1358: Unknown result type (might be due to invalid IL or missing references)
		//IL_1378: Unknown result type (might be due to invalid IL or missing references)
		//IL_137a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1381: Unknown result type (might be due to invalid IL or missing references)
		//IL_1388: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a08: Unknown result type (might be due to invalid IL or missing references)
		//IL_0988: Unknown result type (might be due to invalid IL or missing references)
		//IL_098a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d73: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dbc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e80: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e87: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cda: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0edc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a90: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a97: Unknown result type (might be due to invalid IL or missing references)
		//IL_0903: Unknown result type (might be due to invalid IL or missing references)
		//IL_0905: Unknown result type (might be due to invalid IL or missing references)
		//IL_090a: Unknown result type (might be due to invalid IL or missing references)
		//IL_090f: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0930: Unknown result type (might be due to invalid IL or missing references)
		//IL_0932: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0804: Unknown result type (might be due to invalid IL or missing references)
		int frameRef = Game1.charDef[c.defIdx].GetAnimation(c.bodySec[sec].anim).GetKeyFrame(c.bodySec[sec].key).frameRef;
		Frame frame = Game1.charDef[c.defIdx].GetFrame(frameRef);
		Vector2 val = default(Vector2);
		Vector2 loc = c.loc;
		float scale = c.scale;
		loc += new Vector2((float)Math.Cos(c.angle + 1.57f), (float)Math.Sin(c.angle + 1.57f)) * scale * 60f - new Vector2(0f, 60f) * scale;
		if (ps == 1 && sec == 1)
		{
			for (int i = 0; i < frame.GetPartArray().Length; i++)
			{
				Part part = frame.GetPart(i);
				if (part.idx > -1)
				{
					float num = part.rotation;
					Vector2 val2 = part.location * scale + loc;
					_ = part.scaling * scale;
					if ((c.face != 0 || part.flip != 0) && c.face == 1)
					{
						_ = part.flip;
						_ = 1;
					}
					if (c.face == 1)
					{
						num = 0f - num;
						val2.X -= part.location.X * scale * 2f;
					}
					val2 -= new Vector2((float)Math.Sin(num), (float)Math.Cos(num)) * scale * 24f;
					if (part.idx == 8 || part.idx == 9)
					{
						val = torsoVec - val2;
					}
				}
			}
		}
		Vector2 val4 = default(Vector2);
		Vector2 val8 = default(Vector2);
		for (int j = 0; j < frame.GetPartArray().Length; j++)
		{
			Part part2 = frame.GetPart(j);
			if (part2.idx <= -1)
			{
				continue;
			}
			float num2 = part2.rotation;
			Vector2 val3 = part2.location * scale + loc;
			_ = part2.scaling * scale;
			bool flag = false;
			if ((c.face == 0 && part2.flip == 0) || (c.face == 1 && part2.flip == 1))
			{
				flag = true;
			}
			if (c.face == 1)
			{
				num2 = 0f - num2;
				val3.X -= part2.location.X * scale * 2f;
			}
			new Color(new Vector4(1f, 1f, 1f, 1f));
			bool flag2 = false;
			if (ps == 0)
			{
				if (part2.idx >= 24 && part2.idx / 64 == 0)
				{
					flag2 = true;
				}
				if (part2.idx == 8 || part2.idx == 9)
				{
					torsoVec = val3;
					torsoVec -= new Vector2((float)Math.Sin(num2), (float)Math.Cos(num2)) * scale * 24f;
				}
			}
			else
			{
				if (part2.idx < 24 || part2.idx / 64 != 0)
				{
					flag2 = true;
				}
				val3 += val;
			}
			if (all)
			{
				flag2 = true;
			}
			if (!flag2 || part2.idx < 1000)
			{
				continue;
			}
			float num3 = part2.rotation;
			if (!flag)
			{
				num3 = 3.14f - num3;
			}
			val3 = Character.GetAngleAdjustedVec(loc, val3, c.angle);
			num3 += c.angle;
			((Vector2)(ref val4))._002Ector((float)Math.Cos(num3), (float)Math.Sin(num3));
			((Vector2)(ref val4)).Normalize();
			switch (part2.idx)
			{
			case 1000:
			case 1005:
			{
				((Vector2)(ref val8))._002Ector(c.charKeys.shootVec.X, c.charKeys.shootVec.Y);
				if (part2.idx - 1000 == 5)
				{
					((Vector2)(ref val8))._002Ector((c.face == 1) ? 1f : (-1f), 0.2f);
				}
				Vector2 val9 = (val4 + val8) / 2f;
				if (part2.idx - 1000 == 5)
				{
					for (int k = 0; k < 10; k++)
					{
						Game1.pMan.AddParticle(22, val3 + val9 * (float)(k * 3), val9 * 100f, (1f - (float)k * 0.07f) * 0.5f, 0, 0);
					}
				}
				else
				{
					switch (WeaponCatalog.weapons[c.weapon[c.curWeap]].projType)
					{
					case 2:
					{
						for (int m = 0; m < 9; m++)
						{
							Game1.pMan.AddParticle(22, val3 + val9 * (float)(m * 3), val9 * 100f, (1f - (float)m * 0.07f) * 0.35f, 0, 0);
						}
						break;
					}
					case 0:
					case 1:
					case 19:
					{
						for (int num5 = 0; num5 < 10; num5++)
						{
							Game1.pMan.AddParticle(22, val3 + val9 * (float)(num5 * 3), val9 * 100f, (1f - (float)num5 * 0.07f) * 0.5f, 0, 0);
						}
						break;
					}
					case 3:
					{
						for (int n = 0; n < 10; n++)
						{
							Game1.pMan.AddParticle(24, val3 + val9 * (float)(n * 3), val9 * 100f, (1f - (float)n * 0.07f) * 0.5f, 0, 0);
						}
						Game1.pMan.AddParticle(17, val3, val9 * 50f, 0f, 0, 0);
						break;
					}
					case 7:
					case 14:
					{
						for (int l = 0; l < 5; l++)
						{
							Game1.pMan.AddParticle(38, val3, val9 * (50f + (float)l * 30f), 0f, 0, 0);
						}
						break;
					}
					}
				}
				for (int num6 = 0; num6 < WeaponCatalog.weapons[c.weapon[c.curWeap]].burst; num6++)
				{
					((Vector2)(ref val8)).Normalize();
					Vector2 val10 = val8 * 2000f + Rand.GetRandomVec2(0f - WeaponCatalog.weapons[c.weapon[c.curWeap]].spread, WeaponCatalog.weapons[c.weapon[c.curWeap]].spread, 0f - WeaponCatalog.weapons[c.weapon[c.curWeap]].spread, WeaponCatalog.weapons[c.weapon[c.curWeap]].spread);
					if (part2.idx - 1000 == 5)
					{
						if (c.hp > -1)
						{
							Sound.PlayCue("pistol");
							c.lastHitBy = -1;
							c.hp = -1;
							if (c.face == 1)
							{
								c.face = 0;
								c.traj.X = 200f;
							}
							else
							{
								c.face = 1;
								c.traj.X = -200f;
							}
							KillManager.DoKill(c.ID, c.ID, 1);
							for (int num7 = 0; num7 < 6; num7++)
							{
								Game1.pMan.AddParticle(6, val3, val10 * Rand.GetRandomFloat(-0.2f, 0f), Rand.GetRandomFloat(0.8f, 1.2f), 0, 0);
								Game1.pMan.AddParticle(7, val3, val10 * Rand.GetRandomFloat(0.1f, 1f), Rand.GetRandomFloat(0f, 1.5f), 0, 0);
							}
						}
						continue;
					}
					switch (WeaponCatalog.weapons[c.weapon[c.curWeap]].projType)
					{
					case 12:
					{
						if (c.charge < 1f)
						{
							Game1.pMan.AddParticle(49, val3 + (c.drawVec - c.loc), Rand.GetRandomVec2(2f), 0.1f * c.charge, 0, c.ID);
							break;
						}
						Vector2 val11 = val3 - Scroll.scroll;
						if (((Vector2)(ref val11)).Length() < 300f)
						{
							Quake.SetQuake(0.3f);
						}
						Game1.pMan.AddParticle(47, val3, val10, WeaponCatalog.weapons[c.weapon[c.curWeap]].splash, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					}
					case 2:
						Game1.pMan.AddParticle(21, val3, val10, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 0:
						Game1.pMan.AddParticle(19, val3, val10, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 1:
						Game1.pMan.AddParticle(27, val3, val10, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 3:
						Game1.pMan.AddParticle(23, val3, val10 * 0.5f, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 19:
						Game1.pMan.AddParticle(67, val3, val10 * 0.25f, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 13:
						Game1.pMan.AddParticle(52, val3, val10 * 0.25f, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 14:
						Game1.pMan.AddParticle(54, val3, val10 * 0.65f, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 4:
						Game1.pMan.AddParticle(25, val3, val10 * 0.5f, WeaponCatalog.weapons[c.weapon[c.curWeap]].splash, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 17:
						Game1.pMan.AddParticle(64, val3, val10 * 0.5f, WeaponCatalog.weapons[c.weapon[c.curWeap]].splash, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 5:
						Game1.pMan.AddParticle(10, val3, val10 * 0.5f, WeaponCatalog.weapons[c.weapon[c.curWeap]].splash, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 9:
						Game1.pMan.AddParticle(34, val3, val10 * 0.4f, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 11:
						Game1.pMan.AddParticle(45, val3, val10 * 0.4f, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 6:
						Game1.pMan.AddParticle(31, val3, val10 * 0.5f, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 7:
						Game1.pMan.AddParticle(33, val3, val10 * 0.4f, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 8:
						Game1.pMan.AddParticle(35, val3, val10 * 0.5f, WeaponCatalog.weapons[c.weapon[c.curWeap]].splash, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 15:
						Game1.pMan.AddParticle(55, val3, val10 * 0.5f, WeaponCatalog.weapons[c.weapon[c.curWeap]].splash, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 10:
						Game1.pMan.AddParticle(44, val3, val10 * 0.3f + c.traj * 1.5f, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					case 16:
						Game1.pMan.AddParticle(65, val3, val10 * 0.3f + c.traj * 1.5f, 0f, WeaponCatalog.weapons[c.weapon[c.curWeap]].damage, c.ID);
						break;
					}
				}
				if (WeaponCatalog.weapons[c.weapon[c.curWeap]].fireRate <= 0.03f)
				{
					key += Rand.GetRandomInt(1, 3);
				}
				break;
			}
			case 1004:
			{
				Vector2 val6 = val4;
				((Vector2)(ref val6)).Normalize();
				Game1.pMan.AddParticle(20, val3, val6 * 1000f, 0f, 15, c.ID);
				Vector2 val7 = c.loc - Scroll.scroll;
				if (((Vector2)(ref val7)).Length() < 500f)
				{
					Sound.PlayCue("swing");
				}
				break;
			}
			case 1001:
			case 1006:
				if (WeaponCatalog.weapons[c.weapon[c.curWeap]].ammoType == 0 || WeaponCatalog.weapons[c.weapon[c.curWeap]].ammoType == 1 || part2.idx - 1000 == 6)
				{
					Game1.pMan.AddParticle(18, val3, val4 * 200f, 0f, 0, 0);
				}
				break;
			case 1002:
				Game1.pMan.AddParticle(26, val3, val4 * 200f, 0f, 0, 0);
				break;
			case 1003:
			{
				val4 = ((!(((Vector2)(ref c.charKeys.shootVec)).Length() > 0.6f)) ? c.grenVec : c.charKeys.shootVec);
				float num4 = ((Vector2)(ref val4)).Length();
				((Vector2)(ref val4)).Normalize();
				byte b = 33;
				Vector2 val5 = c.loc - Scroll.scroll;
				if (((Vector2)(ref val5)).LengthSquared() < 250000f)
				{
					Sound.PlayCue("throw");
				}
				if (c.grenAmmo[c.lastGren] > 0)
				{
					c.grenAmmo[c.lastGren]--;
					b = (byte)c.grenType[c.lastGren];
					if (c.perk[1] == 7)
					{
						val4 *= 1.1f;
					}
					switch (b)
					{
					case 33:
						Game1.pMan.AddParticle(11, val3, val4 * 1000f * num4, 0f, 0, c.ID);
						break;
					case 34:
						Game1.pMan.AddParticle(9, val3, val4 * 1000f * num4, 0f, 0, c.ID);
						break;
					case 35:
						Game1.pMan.AddParticle(14, val3, val4 * 1000f * num4, 0f, 0, c.ID);
						break;
					case 37:
						Game1.pMan.AddParticle(12, val3, val4 * 1000f * num4, 0f, 0, c.ID);
						break;
					case 36:
						Game1.pMan.AddParticle(13, val3, val4 * 1000f * num4, 0f, 0, c.ID);
						break;
					case 38:
						Game1.pMan.AddParticle(29, val3, val4 * 1000f * num4, 0f, 0, c.ID);
						break;
					case 41:
						Game1.pMan.AddParticle(59, val3, val4 * 1000f * num4, 0f, 0, c.ID);
						break;
					case 42:
						Game1.pMan.AddParticle(60, val3, val4 * 1000f * num4, 0f, 0, c.ID);
						break;
					case 43:
						Game1.pMan.AddParticle(61, val3, val4 * 1000f * num4, 0f, 0, c.ID);
						break;
					case 44:
						Game1.pMan.AddParticle(62, val3, val4 * 1000f * num4, 0f, 0, c.ID);
						break;
					}
					c.SortGrenades();
				}
				break;
			}
			}
		}
	}
}

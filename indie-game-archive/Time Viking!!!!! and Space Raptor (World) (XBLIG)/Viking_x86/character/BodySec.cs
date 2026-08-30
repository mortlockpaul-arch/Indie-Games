using IMAK3Z0MB1EGAEM.audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xCharEdit.Character;

namespace Viking_x86.character;

public class BodySec
{
	public const string RUN = "run";

	public const string IDLE = "idle";

	public const string ZAP = "zap";

	public const string FLY = "fly";

	public const string ATTACK = "attack";

	public const string LAND = "land";

	public const string UZAP = "uzap";

	public const string ULZAP = "ulzap";

	public const string WARPIN = "warpin";

	public const string WARP = "warp";

	public const string DIE = "die";

	public const int FLAG_DRAW_NORMAL = 0;

	public const int FLAG_DRAW_FIRST_LEGS = 1;

	public const int FLAG_DRAW_SECOND_LEGS = 2;

	private int key;

	private float curFrame;

	private int anim;

	public string animName;

	public Vector2 legAnchorVec;

	public Vector2 torsoAnchorVec;

	internal void Update(int sec, Character me)
	{
		Animation animation = CharDefMgr.charDef[me.defID].GetAnimation(anim);
		KeyFrame keyFrame = animation.GetKeyFrame(key);
		curFrame += Game1.frameTime * 30f;
		int num = key;
		if (curFrame > (float)keyFrame.duration)
		{
			curFrame -= keyFrame.duration;
			key++;
			keyFrame = animation.GetKeyFrame(key);
			if (key >= animation.getKeyFrameArray().Length)
			{
				key = 0;
				if (me.split && sec == 1)
				{
					me.split = false;
				}
			}
		}
		if (keyFrame.frameRef >= 0)
		{
			return;
		}
		key = 0;
		if (me.split && sec == 1)
		{
			me.split = false;
		}
		if (animName == "land" || animName == "attack")
		{
			me.SetAnimation("idle", 0, overRide: true);
		}
		else if (animName == "warpin")
		{
			me.SetAnimation("idle", 0, overRide: true);
		}
		else if (animName == "warp")
		{
			key = num;
		}
		else
		{
			if (!(animName == "die"))
			{
				return;
			}
			me.lives--;
			int lives = me.lives;
			if (me.lives >= 0)
			{
				if (me.defID == 0)
				{
					me.Init(Game1.vgame.world.GetBase(), 0, Rand.GetRandomInt(0, 2), 0);
				}
				else
				{
					me.Init(Game1.vgame.world.GetBase(), 1, Rand.GetRandomInt(0, 2), 0);
				}
				Sound.Play("warpin");
				me.loc.Y = Game1.vgame.world.GetMinY(me.loc.X);
				me.SetAnimation("warpin", 0, overRide: true);
				me.lives = lives;
				me.SetShield(0);
			}
			else
			{
				me.KillChar();
			}
		}
	}

	internal void Draw(Vector2 loc, float size, int face, Character me, int sec, int flag)
	{
		CharDef charDef = CharDefMgr.charDef[me.defID];
		int num = 0;
		if (charDef.GetAnimation(anim).GetKeyFrame(key).lerp)
		{
			num = charDef.GetAnimation(anim).GetKeyFrame(key).frameRef;
			if (num < 0)
			{
				num = 0;
			}
			int idx = key + 1;
			if (charDef.GetAnimation(anim).GetKeyFrame(idx).duration <= 0)
			{
				idx = 0;
			}
			Draw(loc, size, face, num, charDef.GetAnimation(anim).GetKeyFrame(idx).frameRef, me, sec, flag);
		}
		else
		{
			num = charDef.GetAnimation(anim).GetKeyFrame(key).frameRef;
			if (num < 0)
			{
				num = 0;
			}
			Draw(loc, size, face, num, -1, me, sec, flag);
		}
	}

	internal Vector2 GetAnchorVec(int sec, Vector2 loc, float size, int face, int frameIdx, int next, Character me)
	{
		CharDef charDef = CharDefMgr.charDef[me.defID];
		Frame frame = CharDefMgr.charDef[me.defID].GetFrame(frameIdx);
		float angle = VScroll.angle;
		face = 1 - face;
		for (int i = 0; i < frame.GetPartArray().Length; i++)
		{
			Part part = frame.GetPart(i);
			if (part.idx <= -1 || VikingGame.textures[charDef.texName].GetSpriteFlags(part.idx) != sec)
			{
				continue;
			}
			_ = part.rotation;
			if (float.IsNaN(part.location.X))
			{
				part.location.X = 0f;
			}
			if (float.IsNaN(part.location.Y))
			{
				part.location.Y = 0f;
			}
			Vector2 result = VScroll.GetRotatedVec2(part.location) * size + loc;
			_ = part.scaling * size;
			if ((face != 1 || part.flip != 0) && face == 0)
			{
				_ = part.flip;
				_ = 1;
			}
			if (face == 0)
			{
				_ = part.rotation;
				Vector2 location = part.location;
				location.X -= part.location.X * 2f;
				result = VScroll.GetRotatedVec2(location) * size + loc;
			}
			if (next > -1)
			{
				Frame frame2 = charDef.GetFrame(next);
				if (Frame.CanLerp(frame, frame2, i))
				{
					Part part2 = frame2.GetPart(i);
					Animation animation = charDef.GetAnimation(anim);
					KeyFrame keyFrame = animation.GetKeyFrame(key);
					float progress = curFrame / (float)keyFrame.duration;
					Vector2 location2 = part.location;
					Vector2 location3 = part2.location;
					float num = part.rotation;
					float num2 = part2.rotation;
					if (face == 0)
					{
						num = 0f - num;
						num2 = 0f - num2;
						location2.X -= part.location.X * 2f;
						location3.X -= part2.location.X * 2f;
					}
					result = VScroll.GetRotatedVec2(Frame.LerpLoc(location2, location3, progress)) * size + loc;
					Frame.LerpRotation(num, num2, progress);
					_ = Frame.LerpScale(part.scaling, part2.scaling, progress) * size;
				}
			}
			return result;
		}
		return loc;
	}

	internal void Draw(Vector2 loc, float size, int face, int frameIdx, int next, Character me, int sec, int flag)
	{
		Draw(loc, size, face, frameIdx, next, me, sec, anchorOnly: false, flag);
	}

	internal void Draw(Vector2 loc, float size, int face, int frameIdx, int next, Character me, int sec, bool anchorOnly, int flag)
	{
		Rectangle value = default(Rectangle);
		CharDef charDef = CharDefMgr.charDef[me.defID];
		Frame frame = CharDefMgr.charDef[me.defID].GetFrame(frameIdx);
		float angle = VScroll.angle;
		Vector2 origin = default(Vector2);
		Vector2 vector = default(Vector2);
		float foreBright = Game1.vgame.world.GetForeBright();
		if (!anchorOnly && sec == 1)
		{
			Vector2 vector2 = me.bodySec[0].legAnchorVec;
			Vector2 anchorVec = GetAnchorVec(3, loc, size, face, frameIdx, next, me);
			vector = vector2 - anchorVec;
		}
		face = 1 - face;
		bool flag2 = false;
		if (flag == 2)
		{
			flag2 = true;
		}
		for (int i = 0; i < frame.GetPartArray().Length; i++)
		{
			Part part = frame.GetPart(i);
			if (part.idx <= -1)
			{
				continue;
			}
			bool flag3 = true;
			switch (sec)
			{
			case 1:
				if (VikingGame.textures[charDef.texName].GetSpriteFlags(part.idx) != 2 && VikingGame.textures[charDef.texName].GetSpriteFlags(part.idx) != 3)
				{
					flag3 = false;
				}
				break;
			case 0:
				if (VikingGame.textures[charDef.texName].GetSpriteFlags(part.idx) != 1 && VikingGame.textures[charDef.texName].GetSpriteFlags(part.idx) != 3)
				{
					flag3 = false;
				}
				break;
			}
			if (!flag3)
			{
				continue;
			}
			float rotation = part.rotation + angle;
			if (float.IsNaN(part.location.X))
			{
				part.location.X = 0f;
			}
			if (float.IsNaN(part.location.Y))
			{
				part.location.Y = 0f;
			}
			Vector2 position = VScroll.GetRotatedVec2(part.location) * size + loc;
			Vector2 scale = part.scaling * size;
			bool flag4 = false;
			if ((face == 1 && part.flip == 0) || (face == 0 && part.flip == 1))
			{
				flag4 = true;
			}
			if (face == 0)
			{
				rotation = 0f - part.rotation + angle;
				Vector2 location = part.location;
				location.X -= part.location.X * 2f;
				position = VScroll.GetRotatedVec2(location) * size + loc;
			}
			if (next > -1)
			{
				Frame frame2 = charDef.GetFrame(next);
				if (Frame.CanLerp(frame, frame2, i))
				{
					Part part2 = frame2.GetPart(i);
					Animation animation = charDef.GetAnimation(anim);
					KeyFrame keyFrame = animation.GetKeyFrame(key);
					float progress = curFrame / (float)keyFrame.duration;
					Vector2 location2 = part.location;
					Vector2 location3 = part2.location;
					float num = part.rotation;
					float num2 = part2.rotation;
					if (face == 0)
					{
						num = 0f - num;
						num2 = 0f - num2;
						location2.X -= part.location.X * 2f;
						location3.X -= part2.location.X * 2f;
					}
					position = VScroll.GetRotatedVec2(Frame.LerpLoc(location2, location3, progress)) * size + loc;
					rotation = Frame.LerpRotation(num, num2, progress) + angle;
					scale = Frame.LerpScale(part.scaling, part2.scaling, progress) * size;
				}
			}
			float num3 = me.delta * 3f;
			if (num3 > 1f)
			{
				num3 = 1f;
			}
			Color color = new Color(new Vector4(foreBright, foreBright, foreBright, num3));
			if (part.idx >= 2000)
			{
				SpriteTools.End();
				int num4 = face;
				if (num4 == 1)
				{
					if (part.flip == 1)
					{
						num4 = 1 - num4;
					}
				}
				else if (part.flip == 1)
				{
					num4 = 1 - num4;
				}
				SpriteTools.BeginAlpha();
			}
			else
			{
				if (part.idx >= 1000)
				{
					continue;
				}
				Texture2D texture2D;
				if (part.idx < 64)
				{
					if (VikingGame.textures.ContainsKey(charDef.texName))
					{
						texture2D = VikingGame.textures[charDef.texName].texture;
						value = VikingGame.textures[charDef.texName].GetSpriteRect(part.idx);
						origin = VikingGame.textures[charDef.texName].GetSpriteOrigin(part.idx);
						origin.X -= value.X;
						origin.Y -= value.Y;
						if (!flag4)
						{
							origin.X = (float)value.Width - origin.X;
						}
					}
					else
					{
						texture2D = null;
					}
				}
				else
				{
					texture2D = null;
				}
				if (texture2D != null)
				{
					if (!anchorOnly)
					{
						position += vector;
						if ((sec != 0 || VikingGame.textures[charDef.texName].GetSpriteFlags(part.idx) != 3) && !flag2)
						{
							SpriteTools.sprite.Draw(texture2D, position, value, color, rotation, origin, scale, (!flag4) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
						}
					}
					switch (VikingGame.textures[charDef.texName].GetSpriteFlags(part.idx))
					{
					case 3:
						torsoAnchorVec = position;
						legAnchorVec = position;
						break;
					}
				}
				switch (flag)
				{
				case 1:
				{
					int spriteFlags2 = VikingGame.textures[charDef.texName].GetSpriteFlags(part.idx);
					if (spriteFlags2 == 1)
					{
						return;
					}
					break;
				}
				case 2:
				{
					int spriteFlags = VikingGame.textures[charDef.texName].GetSpriteFlags(part.idx);
					if (spriteFlags == 1)
					{
						flag2 = false;
						flag = 1;
					}
					break;
				}
				}
			}
		}
	}

	internal void SetAnim(int anim, string animName, bool overRide)
	{
		this.animName = animName;
		if (this.anim != anim || overRide)
		{
			this.anim = anim;
			key = 0;
			curFrame = 0f;
		}
	}
}

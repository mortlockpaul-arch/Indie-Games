using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using yMapEdit.map;

namespace ZP2K9.hud;

public class Pickup
{
	private int idx;

	private float frame;

	private int nameIdx;

	private float nameFrame;

	public void DoPickup(int idx)
	{
		this.idx = idx;
		frame = 3f;
	}

	public void DoName(int idx)
	{
		nameIdx = idx;
		nameFrame = 2f;
	}

	public void Update()
	{
		if (frame > 0f)
		{
			frame -= Game1.frameTime;
		}
		if (nameFrame > 0f)
		{
			nameFrame -= Game1.frameTime;
		}
	}

	public void Draw(SpriteBatch sprite, Color teamColor)
	{
		DrawPickup(sprite);
		DrawName(sprite);
	}

	private void DrawName(SpriteBatch sprite)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (!(nameFrame <= 0f))
		{
			float num = nameFrame * 2f;
			if (num > 1f)
			{
				num = 1f;
			}
			if (nameFrame > 1.9f)
			{
				num = (2f - nameFrame) * 10f;
			}
			if (num > 0.9f)
			{
				num = 0.9f;
			}
			Game1.text.size = 1.1f;
			Game1.text.color = new Color(1f, 1f, 1f, num);
			Game1.text.DrawString(new Vector2(390f, 550f), Special.names[nameIdx], 0, -1f, Game1.impact, sprite);
		}
	}

	public void DrawPickup(SpriteBatch sprite)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		if (frame <= 0f)
		{
			return;
		}
		Vector2 val = ScrollManager.screenSize / 2f;
		float num = frame * 2f;
		if (num > 1f)
		{
			num = 1f;
		}
		if (frame > 2.9f)
		{
			num = (3f - frame) * 10f;
		}
		float num2 = 120f;
		try
		{
			sprite.Draw(Game1.spritesTex, val + new Vector2(0f, num2 + 10f), (Rectangle?)new Rectangle(0, 768, 128, 64), new Color(1f, 1f, 1f, num * 0.5f), 0f, new Vector2(64f, 32f), new Vector2(Game1.impact.MeasureString(Special.names[idx]).X / 80f, 2.7f), (SpriteEffects)0, 1f);
			sprite.Draw(Game1.spritesTex, val + new Vector2(0f, num2), (Rectangle?)new Rectangle((idx - 1) % 16 * 64, 320 + (idx - 1) / 16 * 64, 64, 64), new Color(1f, 1f, 1f, num), 0f, new Vector2(32f, 32f), 1.2f, (SpriteEffects)0, 1f);
			Game1.text.size = 1.1f;
			Game1.text.color = new Color(1f, 1f, 1f, num);
			Game1.text.DrawString(val + new Vector2(0f, num2 + 30f), Special.names[idx], 1, -1f, Game1.impact, sprite);
		}
		catch
		{
		}
	}
}

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class TargetEffectTimestop : TargetEffect
{
	public float maxCountdown;

	public Vector2 screenPos;

	public Vector2 screenOffset;

	public Vector3 baseColor;

	public float rotation;

	public TargetEffectTimestop()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		screenPos = new Vector2(0.5f * (float)BaseGame.WIDTH, 0.5f * (float)BaseGame.HEIGHT);
		screenOffset = new Vector2(400f, 400f);
		baseColor = new Vector3(1.4f, 0.8f, 0.6f);
		rotation = 0f;
	}

	public override void Update(GameTime gametime)
	{
		countDown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		rotation = (0f - (maxCountdown - countDown)) * 1.2f * (float)Math.PI;
		if (countDown < 0f)
		{
			BaseGame.Get().targetFX.Remove(this);
		}
	}

	public override void Draw()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		float num = countDown / maxCountdown;
		BaseGame.Get().spriteBatch.Draw(TargetEffect.timeTex, screenPos, (Rectangle?)null, new Color(new Vector4(baseColor * num, num * 2f)), rotation, new Vector2(191f, 191f), 1.8f, (SpriteEffects)0, 0f);
	}

	public override void DrawInBack()
	{
	}
}

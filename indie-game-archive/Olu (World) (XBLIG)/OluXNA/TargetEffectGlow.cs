using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class TargetEffectGlow : TargetEffect
{
	public float maxCountdown;

	public Vector2 screenPos;

	public Vector3 baseColor;

	public override void Update(GameTime gametime)
	{
		countDown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (countDown < 0f)
		{
			BaseGame.Get().targetFX.Remove(this);
		}
	}

	public override void Draw()
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		float num = countDown / maxCountdown;
		BaseGame.Get().spriteBatch.Draw(TargetEffect.glowTex2, new Rectangle((int)(screenPos.X - 0.6f * (float)BaseGame.WIDTH), (int)(screenPos.Y - (float)BaseGame.HEIGHT * 0.25f), (int)((float)BaseGame.WIDTH * 1.2f), (int)((float)BaseGame.HEIGHT * 0.5f)), (Rectangle?)null, new Color(new Vector4(baseColor * num, num * 2f)), 0f, Vector2.Zero, (SpriteEffects)0, 0f);
	}

	public override void DrawInBack()
	{
	}
}

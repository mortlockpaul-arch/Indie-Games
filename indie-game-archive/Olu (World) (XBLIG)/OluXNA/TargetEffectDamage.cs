using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class TargetEffectDamage : TargetEffect
{
	public float maxCountdown;

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
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		Viewport viewport = BaseGame.Get().graphics.GraphicsDevice.Viewport;
		Vector3 val = ((Viewport)(ref viewport)).Project(BaseGame.Get().playerPos, BaseGame.Get().projectionMatrix, BaseGame.Get().viewMatrix, BaseGame.Get().world);
		BaseGame.Get().spriteBatch.Draw(TargetEffect.glowTex2, new Rectangle(0, (int)(val.Y - (float)BaseGame.HEIGHT * 0.25f), (int)(float)BaseGame.WIDTH, (int)((float)BaseGame.HEIGHT * 0.5f)), (Rectangle?)null, new Color(1f, 0.5f * countDown / maxCountdown, 0.5f * countDown / maxCountdown, countDown / maxCountdown * 2f), 0f, Vector2.Zero, (SpriteEffects)0, 0f);
	}

	public override void DrawInBack()
	{
	}
}

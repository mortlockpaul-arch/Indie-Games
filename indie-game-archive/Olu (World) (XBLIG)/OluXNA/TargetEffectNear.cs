using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class TargetEffectNear : TargetEffect
{
	protected float rotAmount;

	public override void Update(GameTime gametime)
	{
		if (!enem.exists)
		{
			BaseGame.Get().targetFX.Remove(this);
		}
		else
		{
			rotAmount += MathHelper.ToRadians(240f * (float)gametime.ElapsedGameTime.TotalSeconds);
		}
	}

	public override void Draw()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = eTarget.absolutePos();
		BaseGame.Get().graphics.GraphicsDevice.Vertices[0].SetSource(TargetEffect.vBuffer, 0, VertexPositionColor.SizeInBytes);
		if ((double)Vector3.Dot(Vector3.Normalize(val - BaseGame.Get().ActualCameraPos()), Vector3.Normalize(BaseGame.Get().cameraDir)) < Math.PI / 2.0)
		{
			Viewport viewport = BaseGame.Get().graphics.GraphicsDevice.Viewport;
			Vector3 val2 = ((Viewport)(ref viewport)).Project(val, BaseGame.Get().projectionMatrix, BaseGame.Get().viewMatrix, BaseGame.Get().world);
			val2.Z = 0f;
			if (eTarget.Visible())
			{
				BaseGame.Get().flatStack.PushMatrix();
				BaseGame.Get().flatStack.ApplyMatrix(Matrix.CreateRotationZ(rotAmount) * Matrix.CreateTranslation(val2));
				BaseGame.Get().graphics.GraphicsDevice.DrawPrimitives((PrimitiveType)4, TargetEffect.offsets[5], TargetEffect.size[5] / 3);
				BaseGame.Get().flatStack.PopMatrix();
				BaseGame.Get().spriteBatch.DrawString(BaseGame.Get().hud.HUDfont, "warning", new Vector2(val2.X, val2.Y), Color.White, 0f, BaseGame.Get().hud.HUDfont.MeasureString("warning") / 2f, 0.95f * HUD.textScale, (SpriteEffects)0, 0f);
			}
		}
	}

	public override void DrawInBack()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = eTarget.absolutePos();
		Viewport viewport = BaseGame.Get().graphics.GraphicsDevice.Viewport;
		Vector3 val2 = ((Viewport)(ref viewport)).Project(val, BaseGame.Get().projectionMatrix, BaseGame.Get().viewMatrix, BaseGame.Get().world);
		BaseGame.Get().spriteBatch.Draw(TargetEffect.glowTex, new Rectangle((int)(val2.X - (float)BaseGame.WIDTH * 0.15f), (int)(val2.Y - (float)BaseGame.HEIGHT * 0.15f), (int)((float)BaseGame.WIDTH * 0.3f), (int)((float)BaseGame.HEIGHT * 0.3f)), Color.Red);
	}
}

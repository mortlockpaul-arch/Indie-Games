using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class TargetEffectBase : TargetEffect
{
	public float rotAmount;

	public bool disablePowerScore;

	public bool skipSquare;

	public bool ignoreBeat;

	public static Texture2D shockTex;

	public TargetEffectBase()
	{
		countDown = 0.52f;
		activated = false;
	}

	public static void CreateShockFX()
	{
		shockTex = BaseGame.Get().content.Load<Texture2D>("Content/zoneOneShock");
		TargetEffect.timeTex = BaseGame.Get().content.Load<Texture2D>("Content/Timestop");
	}

	public override void Update(GameTime gametime)
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Invalid comparison between Unknown and I4
		if (!activated && countDown >= 0f)
		{
			countDown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		}
		else
		{
			if (!activated)
			{
				return;
			}
			if (countDown < 0f && wade.Check(BaseGame.Get().curBeat) && prev == null && waitBeat != BaseGame.Get().curBeat)
			{
				pos = eTarget.absolutePos();
				if (wade.cueName != "")
				{
					enem.HitSound(lockNum, (fillMode == eTarget.fillMode) ? (-20f) : 0f);
				}
				enem.hit(this);
				if (!BaseGame.Get().scoreFlow.isEmpty())
				{
					ScoreGroup scoreGroup = BaseGame.Get().scoreFlow.Pop();
					if (disablePowerScore || fillMode == eTarget.fillMode)
					{
						BaseGame.Get().score += scoreGroup.scores[0];
					}
					else
					{
						BaseGame.Get().score += scoreGroup.totalPoints * scoreGroup.scores[0] * 2;
						BaseGame.Get().powerScore[((int)fillMode != 2) ? 1u : 0u] += scoreGroup.totalPoints * scoreGroup.scores[0] * 2;
					}
				}
				BaseGame.Get().firstPass = false;
				countDown = 0.001f;
				Random random = new Random();
				rotAmount = (float)random.NextDouble() * 2f * (float)Math.PI;
				if (next != null)
				{
					next.prev = null;
					next.waitBeat = BaseGame.Get().curBeat;
					next = null;
				}
			}
			else if (countDown > 0f)
			{
				countDown += 0.5f * (float)gametime.ElapsedGameTime.TotalSeconds;
				if (countDown > 0.676f)
				{
					BaseGame.Get().targetFX.Remove(this);
				}
			}
		}
	}

	public override void DrawInBack()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		if (fillMode == eTarget.fillMode)
		{
			return;
		}
		BaseGame.Get().fogEffect.Parameters["xFogColor"].GetValueVector4();
		float num = 0.5f * (float)BaseGame.HEIGHT;
		Vector3 val = ((activated && (!activated || !(countDown <= 0f))) ? pos : eTarget.absolutePos());
		if (!((double)Vector3.Dot(Vector3.Normalize(val - BaseGame.Get().ActualCameraPos()), Vector3.Normalize(BaseGame.Get().cameraDir)) < Math.PI / 2.0))
		{
			return;
		}
		Viewport viewport = BaseGame.Get().graphics.GraphicsDevice.Viewport;
		Vector3 val2 = ((Viewport)(ref viewport)).Project(val, BaseGame.Get().projectionMatrix, BaseGame.Get().viewMatrix, BaseGame.Get().world);
		val2.Z = 0f;
		if (((activated && (!activated || !(countDown <= 0f))) || skipSquare) && countDown > 0f)
		{
			float num2 = num * countDown;
			if (!skipSquare)
			{
				BaseGame.Get().spriteBatch.Draw(shockTex, new Vector2(val2.X, val2.Y), (Rectangle?)null, new Color(new Vector4(BaseGame.Get().level.effectColor, 0.8f * (0.676f - countDown) / 0.676f)), rotAmount + countDown * 1f * (float)Math.PI, new Vector2((float)(shockTex.Width / 2), (float)(shockTex.Height / 2)), (0.8f + countDown) * num2 * 0.3f, (SpriteEffects)0, 0f);
			}
		}
	}

	public override void Draw()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.01768f * (float)BaseGame.HEIGHT;
		float num2 = 0.035f * (float)BaseGame.HEIGHT;
		if (fillMode != eTarget.fillMode)
		{
			num2 = 0.28f * (float)BaseGame.HEIGHT;
		}
		Vector3 val = ((activated && (!activated || !(countDown <= 0f))) ? pos : eTarget.absolutePos());
		BaseGame.Get().graphics.GraphicsDevice.Vertices[0].SetSource(TargetEffect.vBuffer, 0, VertexPositionColor.SizeInBytes);
		if (!((double)Vector3.Dot(Vector3.Normalize(val - BaseGame.Get().ActualCameraPos()), Vector3.Normalize(BaseGame.Get().cameraDir)) < Math.PI / 2.0))
		{
			return;
		}
		float num3 = num + num2 * countDown;
		Viewport viewport = BaseGame.Get().graphics.GraphicsDevice.Viewport;
		Vector3 val2 = ((Viewport)(ref viewport)).Project(val, BaseGame.Get().projectionMatrix, BaseGame.Get().viewMatrix, BaseGame.Get().world);
		val2.Z = 0f;
		if (eTarget.Visible())
		{
			if ((!activated || (activated && countDown <= 0f)) && !skipSquare)
			{
				BaseGame.Get().flatStack.PushMatrix();
				BaseGame.Get().flatStack.ApplyMatrix(Matrix.CreateScale(num3) * Matrix.CreateRotationZ((float)((double)(200f * countDown) * Math.PI / 180.0)) * Matrix.CreateTranslation(val2));
				BaseGame.Get().graphics.GraphicsDevice.DrawPrimitives((PrimitiveType)6, TargetEffect.offsets[0], TargetEffect.size[0] - 2);
				BaseGame.Get().graphics.GraphicsDevice.DrawPrimitives((PrimitiveType)3, TargetEffect.offsets[1], TargetEffect.size[1] - 1);
				BaseGame.Get().flatStack.PopMatrix();
			}
			else if (countDown > 0f)
			{
				num3 = num2 * countDown;
				BaseGame.Get().flatStack.PushMatrix();
				BaseGame.Get().flatStack.ApplyMatrix(Matrix.CreateTranslation(val2));
				BaseGame.Get().flatStack.PushMatrix();
				BaseGame.Get().flatStack.ApplyMatrix(Matrix.CreateScale(num3));
				BaseGame.Get().graphics.GraphicsDevice.DrawPrimitives((PrimitiveType)3, TargetEffect.offsets[2], TargetEffect.size[2] - 1);
				_ = skipSquare;
				BaseGame.Get().flatStack.PopMatrix();
				num3 += 2f;
				BaseGame.Get().flatStack.PushMatrix();
				BaseGame.Get().flatStack.ApplyMatrix(Matrix.CreateScale(num3));
				BaseGame.Get().graphics.GraphicsDevice.DrawPrimitives((PrimitiveType)3, TargetEffect.offsets[3], TargetEffect.size[3] - 1);
				BaseGame.Get().flatStack.PopMatrix();
				num3 += 2f;
				BaseGame.Get().flatStack.PushMatrix();
				BaseGame.Get().flatStack.ApplyMatrix(Matrix.CreateScale(num3));
				BaseGame.Get().graphics.GraphicsDevice.DrawPrimitives((PrimitiveType)3, TargetEffect.offsets[4], TargetEffect.size[4] - 1);
				BaseGame.Get().flatStack.PopMatrix();
				BaseGame.Get().flatStack.PopMatrix();
			}
		}
	}
}

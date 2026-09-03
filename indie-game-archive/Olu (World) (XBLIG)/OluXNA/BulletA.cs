using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class BulletA : Enemy
{
	private Vector3 vel;

	private float speed;

	private static VertexPositionColor[] bullNodes;

	private static int[] indices;

	private float rot;

	private static float rotInc = (float)Math.PI * 2f;

	public BulletA(Vector3 _start)
	{
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		if (bullNodes == null)
		{
			bullNodes = (VertexPositionColor[])(object)new VertexPositionColor[5];
			ref VertexPositionColor reference = ref bullNodes[0];
			reference = new VertexPositionColor(new Vector3(0.5f, 0.5f, 0f), Color.Orange);
			ref VertexPositionColor reference2 = ref bullNodes[1];
			reference2 = new VertexPositionColor(new Vector3(0.5f, -0.5f, 0f), Color.Orange);
			ref VertexPositionColor reference3 = ref bullNodes[2];
			reference3 = new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0f), Color.Orange);
			ref VertexPositionColor reference4 = ref bullNodes[3];
			reference4 = new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0f), Color.Orange);
			ref VertexPositionColor reference5 = ref bullNodes[4];
			reference5 = new VertexPositionColor(new Vector3(0f, 0f, -0.75f), Color.Yellow);
			indices = new int[18];
			indices[0] = 0;
			indices[1] = 1;
			indices[2] = 2;
			indices[3] = 0;
			indices[4] = 2;
			indices[5] = 3;
			indices[6] = 1;
			indices[7] = 0;
			indices[8] = 4;
			indices[9] = 0;
			indices[10] = 3;
			indices[11] = 4;
			indices[12] = 2;
			indices[13] = 1;
			indices[14] = 4;
			indices[15] = 3;
			indices[16] = 2;
			indices[17] = 4;
		}
		setPos(_start);
		vel = BaseGame.Get().cameraLoc - _start;
		vel = Vector3.Normalize(vel);
		speed = 10f;
		vel *= speed;
		state = 0;
		hitPoints = 1;
	}

	public override void draw(GameTime gametime)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (exists)
		{
			base.draw(gametime);
			BaseGame.Get().SwitchEffectTechnique("Colored");
			BaseGame.Get().fogEffect.Begin();
			BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].Begin();
			BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateRotationZ(rot) * Matrix.CreateTranslation(getPos()));
			BaseGame.Get().graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>((PrimitiveType)4, bullNodes, 0, 5, indices, 0, 6);
			BaseGame.Get().matStack.PopMatrix();
			BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].End();
			BaseGame.Get().fogEffect.End();
		}
	}

	public override void act(GameTime gametime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.Normalize(vel) + Vector3.Normalize(BaseGame.Get().playerPos + new Vector3(0f, 0f, 1.5f) - getPos());
		val *= 0.5f;
		vel = val * (float)((double)speed * gametime.ElapsedGameTime.TotalSeconds);
		setPos(getPos() + vel);
		rot += rotInc * (float)gametime.ElapsedGameTime.TotalSeconds;
		PlayerHit();
	}

	public override void start()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = pos;
		addTarget(new Vector3(0f, 0f, 0f), 1, 10);
		base.start();
		BaseGame.Get().actualEnem--;
		pos = val;
	}

	public override Enemy attack()
	{
		return new ECube();
	}

	public override string name()
	{
		return "[shot 0x0001]";
	}

	public override void HitSound(int lockNum, float volume)
	{
		if (lockNum <= 8)
		{
			BaseGame.Get().PlayCue("muteCrash", volume);
		}
	}

	public override void hit(TargetEffectBase toHit)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (!toHit.skipSquare)
		{
			BaseGame.Get().ps.AddParticles(getPos(), Vector3.Forward * 25f, 0f, 180f, Vector3.Zero, 0f, 0.25f, 0f, 0.2f, new Vector4(1f, 1f, 0f, 1f), 80, 0.0005f);
		}
		base.hit(toHit);
	}

	public override void die()
	{
		if (exists)
		{
			BaseGame.Get().actualEnem++;
			base.die();
		}
	}

	public override void leave()
	{
		if (exists)
		{
			BaseGame.Get().actualEnem++;
			base.leave();
		}
	}
}

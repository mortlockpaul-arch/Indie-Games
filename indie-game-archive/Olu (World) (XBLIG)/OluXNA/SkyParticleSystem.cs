using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class SkyParticleSystem : ParticleSystem
{
	public SkyParticleDeclaration[] skyVertices;

	public VertexDeclaration skyVertDec;

	public SkyParticleSystem()
	{
		skyVertices = new SkyParticleDeclaration[8000];
		curTime = 0f;
	}

	public override void LoadGraphics()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		VertexElement[] array = (VertexElement[])(object)new VertexElement[4]
		{
			new VertexElement((short)0, (short)0, (VertexElementFormat)2, (VertexElementMethod)0, (VertexElementUsage)0, (byte)0),
			new VertexElement((short)0, (short)12, (VertexElementFormat)1, (VertexElementMethod)0, (VertexElementUsage)5, (byte)0),
			new VertexElement((short)0, (short)20, (VertexElementFormat)2, (VertexElementMethod)0, (VertexElementUsage)5, (byte)1),
			new VertexElement((short)0, (short)32, (VertexElementFormat)4, (VertexElementMethod)0, (VertexElementUsage)10, (byte)0)
		};
		skyVertDec = new VertexDeclaration(BaseGame.Get().graphics.GraphicsDevice, array);
		vBuffer = new VertexBuffer(BaseGame.Get().graphics.GraphicsDevice, skyVertices.Length * SkyParticleDeclaration.SizeInBytes(), (BufferUsage)8);
		vBuffer.SetData<SkyParticleDeclaration>(skyVertices);
	}

	public void AddPlaneParticles(Vector3 corner1, Vector3 corner2, Vector3 vel, float velRand, float velSpread, float lifetime, float lifetimeRand, Vector4 col, int numParticles, float genTime, float aheadTime, float delayTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		AddPlaneParticles(corner1, corner2, vel, velRand, velSpread, lifetime, lifetimeRand, col, numParticles, genTime, aheadTime, delayTime, Vector3.Zero);
	}

	public void AddPlaneParticles(Vector3 corner1, Vector3 corner2, Vector3 vel, float velRand, float velSpread, float lifetime, float lifetimeRand, Vector4 col, int numParticles, float genTime, float aheadTime, float delayTime, Vector3 floatVel)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		int num = AllocateVertices(numParticles);
		if (floatVel == Vector3.Zero)
		{
			floatVel = vel;
		}
		Vector2 tex = default(Vector2);
		for (int i = 0; i < numParticles * 2; i += 2)
		{
			((Vector2)(ref tex))._002Ector((float)i * genTime + curTime - delayTime, lifetime + lifetimeRand * (float)BaseGame.Get().r.NextDouble());
			ref SkyParticleDeclaration reference = ref skyVertices[num + i];
			reference = new SkyParticleDeclaration(BaseGame.GetRandVect(vel * (1f + velRand * (float)BaseGame.Get().r.NextDouble()), velSpread), tex, BaseGame.GetRandPos(corner1, corner2), new Color(col));
			ref SkyParticleDeclaration reference2 = ref skyVertices[num + i + 1];
			reference2 = new SkyParticleDeclaration(floatVel, tex, skyVertices[num + i].Center + aheadTime * skyVertices[num + i].Velocity, new Color(col));
			skyVertices[num + i].Velocity = floatVel;
		}
		vBuffer.SetData<SkyParticleDeclaration>(SkyParticleDeclaration.SizeInBytes() * num, skyVertices, num, numParticles * 2, SkyParticleDeclaration.SizeInBytes());
	}

	public void AddBurstParticles(Vector3 center, Vector3 vel, float velRand, float velSpread, float lifetime, float lifetimeRand, Vector4 col, int numParticles, float genTime, float aheadTime)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		int num = AllocateVertices(numParticles);
		new Color(col);
		Vector2 tex = default(Vector2);
		for (int i = 0; i < numParticles * 2; i += 2)
		{
			BaseGame.GetRandVect(vel * (1f + velRand * (float)BaseGame.Get().r.NextDouble()), velSpread);
			((Vector2)(ref tex))._002Ector((float)i * genTime + curTime, lifetime + lifetimeRand * (float)BaseGame.Get().r.NextDouble());
			ref SkyParticleDeclaration reference = ref skyVertices[num + i];
			reference = new SkyParticleDeclaration(BaseGame.GetRandVect(vel * (1f + velRand * (float)BaseGame.Get().r.NextDouble()), velSpread), tex, center, new Color(col));
			ref SkyParticleDeclaration reference2 = ref skyVertices[num + i + 1];
			reference2 = new SkyParticleDeclaration(skyVertices[num + i].Velocity, tex, skyVertices[num + i].Center + aheadTime * skyVertices[num + i].Velocity, new Color(col));
		}
		vBuffer.SetData<SkyParticleDeclaration>(SkyParticleDeclaration.SizeInBytes() * num, skyVertices, num, numParticles * 2, SkyParticleDeclaration.SizeInBytes());
	}

	public void AddBackwardsBurstParticles(Vector3 center, Vector3 vel, float velRand, float velSpread, float lifetime, float lifetimeRand, Vector4 col, int numParticles, float genTime, float aheadTime)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		int num = AllocateVertices(numParticles);
		new Color(col);
		Vector2 tex = default(Vector2);
		for (int i = 0; i < numParticles * 2; i += 2)
		{
			BaseGame.GetRandVect(vel * (1f + velRand * (float)BaseGame.Get().r.NextDouble()), velSpread);
			((Vector2)(ref tex))._002Ector((float)i * genTime + curTime, lifetime + lifetimeRand * (float)BaseGame.Get().r.NextDouble());
			ref SkyParticleDeclaration reference = ref skyVertices[num + i];
			reference = new SkyParticleDeclaration(-BaseGame.GetRandVect(vel * (1f + velRand * (float)BaseGame.Get().r.NextDouble()), velSpread), tex, center, new Color(col));
			ref SkyParticleDeclaration reference2 = ref skyVertices[num + i + 1];
			reference2 = new SkyParticleDeclaration(skyVertices[num + i].Velocity, tex, skyVertices[num + i].Center + aheadTime * skyVertices[num + i].Velocity, new Color(col));
			ref SkyParticleDeclaration reference3 = ref skyVertices[num + i];
			reference3.Center -= skyVertices[num + i].Velocity * tex.Y;
			ref SkyParticleDeclaration reference4 = ref skyVertices[num + i + 1];
			reference4.Center -= skyVertices[num + i + 1].Velocity * tex.Y;
		}
		vBuffer.SetData<SkyParticleDeclaration>(SkyParticleDeclaration.SizeInBytes() * num, skyVertices, num, numParticles * 2, SkyParticleDeclaration.SizeInBytes());
	}

	public override int AllocateVertices(int number)
	{
		int result = curVertex;
		curVertex += number * 2;
		if (curVertex >= skyVertices.Length / 2)
		{
			curVertex = number * 2;
			result = 0;
		}
		return result;
	}

	public override void Draw(GameTime gametime)
	{
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().SwitchEffectTechnique("SkyParticle");
		BaseGame.Get().fogEffect.Parameters["curTime"].SetValue(curTime);
		BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = skyVertDec;
		BaseGame.Get().graphics.GraphicsDevice.Vertices[0].SetSource(vBuffer, 0, SkyParticleDeclaration.SizeInBytes());
		BaseGame.Get().fogEffect.Begin();
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].Begin();
		RawDraw();
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].End();
		BaseGame.Get().fogEffect.End();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		BaseGame.Get().SwitchEffectTechnique("Colored");
		BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
	}

	public override void RawDraw()
	{
		BaseGame.Get().graphics.GraphicsDevice.DrawPrimitives((PrimitiveType)2, 0, skyVertices.Length / 2);
	}
}

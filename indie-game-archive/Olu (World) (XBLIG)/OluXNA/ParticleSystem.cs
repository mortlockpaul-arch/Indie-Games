using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class ParticleSystem
{
	public ParticleDeclaration[] vertices;

	public VertexBuffer vBuffer;

	public VertexDeclaration vertDec;

	public int curVertex;

	public float curTime;

	public int bufSize = 33000;

	public object vertLock;

	public object bufLock;

	public object indexLock;

	public Texture2D glowTex;

	public virtual void LoadGraphics()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Expected O, but got Unknown
		curTime = 0f;
		vertices = new ParticleDeclaration[bufSize];
		VertexElement[] array = (VertexElement[])(object)new VertexElement[6]
		{
			new VertexElement((short)0, (short)0, (VertexElementFormat)2, (VertexElementMethod)0, (VertexElementUsage)0, (byte)0),
			new VertexElement((short)0, (short)12, (VertexElementFormat)1, (VertexElementMethod)0, (VertexElementUsage)5, (byte)0),
			new VertexElement((short)0, (short)20, (VertexElementFormat)2, (VertexElementMethod)0, (VertexElementUsage)5, (byte)1),
			new VertexElement((short)0, (short)32, (VertexElementFormat)4, (VertexElementMethod)0, (VertexElementUsage)10, (byte)0),
			new VertexElement((short)0, (short)36, (VertexElementFormat)1, (VertexElementMethod)0, (VertexElementUsage)5, (byte)3),
			new VertexElement((short)0, (short)44, (VertexElementFormat)0, (VertexElementMethod)0, (VertexElementUsage)5, (byte)4)
		};
		vertDec = new VertexDeclaration(BaseGame.Get().graphics.GraphicsDevice, array);
		vBuffer = new VertexBuffer(BaseGame.Get().graphics.GraphicsDevice, vertices.Length * ParticleDeclaration.SizeInBytes(), (BufferUsage)8);
		vBuffer.SetData<ParticleDeclaration>(vertices);
		glowTex = BaseGame.Get().content.Load<Texture2D>("Content\\glowParticle");
		vertLock = new object();
		bufLock = new object();
		indexLock = new object();
	}

	public void Update(GameTime gametime)
	{
		curTime += (float)gametime.ElapsedGameTime.TotalSeconds;
	}

	public virtual void Draw(GameTime gametime)
	{
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Particle");
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().fogEffect.Parameters["curTime"].SetValue(curTime);
		BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = vertDec;
		BaseGame.Get().graphics.GraphicsDevice.Vertices[0].SetSource(vBuffer, 0, ParticleDeclaration.SizeInBytes());
		BaseGame.Get().fogEffect.Parameters["BasicTexture"].SetValue((Texture)(object)glowTex);
		BaseGame.Get().fogEffect.Parameters["TextureEnabled"].SetValue(true);
		BaseGame.Get().fogEffect.Parameters["TextureMix"].SetValue(BaseGame.T_MUL);
		BaseGame.Get().fogEffect.Begin();
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].Begin();
		RawDraw();
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].End();
		BaseGame.Get().fogEffect.End();
		BaseGame.Get().graphics.GraphicsDevice.Vertices[0].SetSource((VertexBuffer)null, 0, 0);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		BaseGame.Get().fogEffect.Parameters["TextureEnabled"].SetValue(false);
		BaseGame.Get().SwitchEffectTechnique("Colored");
		BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
	}

	public virtual void RawDraw()
	{
		BaseGame.Get().graphics.GraphicsDevice.DrawPrimitives((PrimitiveType)4, 0, bufSize / 3);
	}

	public void AddParticles(Vector3 center, Vector3 vel, float velRand, float velSpread, Vector3 accel, float accelRand, float lifetime, float lifetimeRand, float friction, Vector4 col, int numParticles, float genTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		AddParticles(center, vel, velRand, velSpread, accel, accelRand, lifetime, lifetimeRand, friction, col, numParticles, genTime, 1f);
	}

	public void AddParticles(Vector3 center, Vector3 vel, float velRand, float velSpread, Vector3 accel, float accelRand, float lifetime, float lifetimeRand, float friction, Vector4 col, int numParticles, float genTime, float glow)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		thread_AddParticles(center, vel, velRand, velSpread, accel, accelRand, lifetime, lifetimeRand, friction, col, numParticles, genTime, glow);
	}

	public void thread_AddParticles(Vector3 center, Vector3 vel, float velRand, float velSpread, Vector3 accel, float accelRand, float lifetime, float lifetimeRand, float friction, Vector4 col, int numParticles, float genTime, float glow)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(0f, 1f, 0f);
		Vector3 val2 = Vector3.Normalize(Vector3.Cross(center, val));
		val = Vector3.Normalize(Vector3.Cross(val2, center));
		val2 *= 4f;
		val *= 4f;
		int num = AllocateVertices(numParticles);
		Vector4 val3 = default(Vector4);
		((Vector4)(ref val3))._002Ector(center, 1f);
		Color col2 = default(Color);
		((Color)(ref col2))._002Ector(col);
		Vector2 tex = default(Vector2);
		for (int i = 0; i < numParticles * 3; i += 3)
		{
			Vector3 randVect = BaseGame.GetRandVect(vel * (1f + velRand * (float)BaseGame.Get().r.NextDouble()), velSpread);
			((Vector2)(ref tex))._002Ector((float)i * genTime + curTime, lifetime + lifetimeRand * (float)BaseGame.Get().r.NextDouble());
			ref ParticleDeclaration reference = ref vertices[num + i];
			reference = new ParticleDeclaration(randVect, tex, val3 + new Vector4(-val2 / 2f + val / 4f, 0f), col2, new Vector2(0f, 0f), glow);
			ref ParticleDeclaration reference2 = ref vertices[num + i + 1];
			reference2 = new ParticleDeclaration(randVect, tex, val3 + new Vector4(val2 / 2f + val / 4f, 0f), col2, new Vector2(1f, 0f), glow);
			ref ParticleDeclaration reference3 = ref vertices[num + i + 2];
			reference3 = new ParticleDeclaration(randVect, tex, val3 + new Vector4(-3f * val / 4f, 0f), col2, new Vector2(0.5f, 1f), glow);
		}
		vBuffer.SetData<ParticleDeclaration>(ParticleDeclaration.SizeInBytes() * num, vertices, num, numParticles * 3, ParticleDeclaration.SizeInBytes());
	}

	public void AddParticlesFlat(Vector3 center, Vector3 vel, float velRand, float velSpread, Vector3 accel, float accelRand, float lifetime, float lifetimeRand, float friction, Vector4 col, int numParticles, float genTime, float glow)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		int num = AllocateVertices(numParticles);
		Vector4 val = default(Vector4);
		((Vector4)(ref val))._002Ector(center, 1f);
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(10f, 0f, 0f);
		Vector3 val3 = default(Vector3);
		((Vector3)(ref val3))._002Ector(0f, 10f, 0f);
		Color col2 = default(Color);
		((Color)(ref col2))._002Ector(col);
		Vector2 tex = default(Vector2);
		for (int i = 0; i < numParticles * 3; i += 3)
		{
			Vector3 randVect = BaseGame.GetRandVect(vel * (1f + velRand * (float)BaseGame.Get().r.NextDouble()), velSpread);
			randVect.Z = 0f;
			((Vector2)(ref tex))._002Ector((float)i * genTime + curTime, lifetime + lifetimeRand * (float)BaseGame.Get().r.NextDouble());
			ref ParticleDeclaration reference = ref vertices[num + i];
			reference = new ParticleDeclaration(randVect, tex, val + new Vector4(-val2 / 2f + val3 / 4f, 0f), col2, new Vector2(0f, 0f), glow);
			ref ParticleDeclaration reference2 = ref vertices[num + i + 1];
			reference2 = new ParticleDeclaration(randVect, tex, val + new Vector4(val2 / 2f + val3 / 4f, 0f), col2, new Vector2(1f, 0f), glow);
			ref ParticleDeclaration reference3 = ref vertices[num + i + 2];
			reference3 = new ParticleDeclaration(randVect, tex, val + new Vector4(-3f * val3 / 4f, 0f), col2, new Vector2(0.5f, 1f), glow);
		}
		vBuffer.SetData<ParticleDeclaration>(ParticleDeclaration.SizeInBytes() * num, vertices, num, numParticles * 3, ParticleDeclaration.SizeInBytes());
	}

	public void CommitVerts(int startIndex, int endIndex)
	{
		if (startIndex == endIndex)
		{
			return;
		}
		lock (bufLock)
		{
			lock (vertLock)
			{
				if (startIndex > endIndex)
				{
					vBuffer.SetData<ParticleDeclaration>(ParticleDeclaration.SizeInBytes() * startIndex, vertices, startIndex, (bufSize / 3 - startIndex + 1) * 3, ParticleDeclaration.SizeInBytes());
					vBuffer.SetData<ParticleDeclaration>(0, vertices, 0, endIndex * 3, ParticleDeclaration.SizeInBytes());
				}
				else
				{
					vBuffer.SetData<ParticleDeclaration>(ParticleDeclaration.SizeInBytes() * startIndex, vertices, startIndex, (endIndex - startIndex) * 3, ParticleDeclaration.SizeInBytes());
				}
			}
		}
	}

	public virtual int AllocateVertices(int number)
	{
		int result = curVertex;
		curVertex += number * 3;
		if (curVertex >= vertices.Length / 3)
		{
			curVertex = number * 3;
			result = 0;
		}
		return result;
	}
}

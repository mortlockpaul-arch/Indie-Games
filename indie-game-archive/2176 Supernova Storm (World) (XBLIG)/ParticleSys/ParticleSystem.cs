using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpaceBlast;

namespace ParticleSys;

public abstract class ParticleSystem
{
	private ParticleSettings settings = new ParticleSettings();

	private ContentManager content;

	private Effect particleEffect;

	private EffectParameter effectViewParameter;

	private EffectParameter effectProjectionParameter;

	private EffectParameter effectViewportHeightParameter;

	private EffectParameter effectTimeParameter;

	private ParticleVertex[] particles;

	private DynamicVertexBuffer vertexBuffer;

	private VertexDeclaration vertexDeclaration;

	private int firstActiveParticle;

	private int firstNewParticle;

	private int firstFreeParticle;

	private int firstRetiredParticle;

	private float currentTime;

	private int drawCounter;

	private static Random random = new Random();

	protected ParticleSystem(Game game, ContentManager content)
	{
		this.content = content;
	}

	public void Initialize()
	{
		InitializeSettings(settings);
		particles = new ParticleVertex[settings.MaxParticles];
	}

	protected abstract void InitializeSettings(ParticleSettings settings);

	public void LoadContent()
	{
		LoadParticleEffect();
		vertexDeclaration = new VertexDeclaration(MainGame.Instance.GraphicsDevice, ParticleVertex.VertexElements);
		int sizeInBytes = 32 * particles.Length;
		vertexBuffer = new DynamicVertexBuffer(MainGame.Instance.GraphicsDevice, sizeInBytes, BufferUsage.Points | BufferUsage.WriteOnly);
	}

	private void LoadParticleEffect()
	{
		Effect effect = content.Load<Effect>("Effects/ParticleEffect");
		particleEffect = effect.Clone(MainGame.Instance.GraphicsDevice);
		EffectParameterCollection parameters = particleEffect.Parameters;
		effectViewParameter = parameters["View"];
		effectProjectionParameter = parameters["Projection"];
		effectViewportHeightParameter = parameters["ViewportHeight"];
		effectTimeParameter = parameters["CurrentTime"];
		parameters["Duration"].SetValue((float)settings.Duration.TotalSeconds);
		parameters["DurationRandomness"].SetValue(settings.DurationRandomness);
		parameters["Gravity"].SetValue(settings.Gravity);
		parameters["EndVelocity"].SetValue(settings.EndVelocity);
		parameters["MinColor"].SetValue(settings.MinColor.ToVector4());
		parameters["MaxColor"].SetValue(settings.MaxColor.ToVector4());
		parameters["RotateSpeed"].SetValue(new Vector2(settings.MinRotateSpeed, settings.MaxRotateSpeed));
		parameters["StartSize"].SetValue(new Vector2(settings.MinStartSize, settings.MaxStartSize));
		parameters["EndSize"].SetValue(new Vector2(settings.MinEndSize, settings.MaxEndSize));
		Texture2D value = content.Load<Texture2D>(settings.TextureName);
		parameters["Texture"].SetValue(value);
		string name = ((settings.MinRotateSpeed != 0f || settings.MaxRotateSpeed != 0f) ? "RotatingParticles" : "NonRotatingParticles");
		particleEffect.CurrentTechnique = particleEffect.Techniques[name];
	}

	public void Update(GameTime gameTime)
	{
		if (gameTime == null)
		{
			throw new ArgumentNullException("gameTime");
		}
		currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
		RetireActiveParticles();
		FreeRetiredParticles();
		if (firstActiveParticle == firstFreeParticle)
		{
			currentTime = 0f;
		}
		if (firstRetiredParticle == firstActiveParticle)
		{
			drawCounter = 0;
		}
	}

	private void RetireActiveParticles()
	{
		float num = (float)settings.Duration.TotalSeconds;
		while (firstActiveParticle != firstNewParticle)
		{
			float num2 = currentTime - particles[firstActiveParticle].Time;
			if (num2 < num)
			{
				break;
			}
			particles[firstActiveParticle].Time = drawCounter;
			firstActiveParticle++;
			if (firstActiveParticle >= particles.Length)
			{
				firstActiveParticle = 0;
			}
		}
	}

	private void FreeRetiredParticles()
	{
		while (firstRetiredParticle != firstActiveParticle)
		{
			int num = drawCounter - (int)particles[firstRetiredParticle].Time;
			if (num < 3)
			{
				break;
			}
			firstRetiredParticle++;
			if (firstRetiredParticle >= particles.Length)
			{
				firstRetiredParticle = 0;
			}
		}
	}

	public void Draw()
	{
		try
		{
			GraphicsDevice graphicsDevice = MainGame.Instance.GraphicsDevice;
			graphicsDevice.Vertices[0].SetSource(null, 0, 0);
			if (vertexBuffer.IsContentLost)
			{
				vertexBuffer.SetData(particles);
			}
			if (firstNewParticle != firstFreeParticle)
			{
				AddNewParticlesToVertexBuffer();
			}
			if (firstActiveParticle != firstFreeParticle)
			{
				SetParticleRenderStates(graphicsDevice.RenderState);
				effectViewportHeightParameter.SetValue(graphicsDevice.Viewport.Height);
				effectTimeParameter.SetValue(currentTime);
				graphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, 32);
				graphicsDevice.VertexDeclaration = vertexDeclaration;
				particleEffect.Begin();
				foreach (EffectPass pass in particleEffect.CurrentTechnique.Passes)
				{
					pass.Begin();
					if (firstActiveParticle < firstFreeParticle)
					{
						graphicsDevice.DrawPrimitives(PrimitiveType.PointList, firstActiveParticle, firstFreeParticle - firstActiveParticle);
					}
					else
					{
						graphicsDevice.DrawPrimitives(PrimitiveType.PointList, firstActiveParticle, particles.Length - firstActiveParticle);
						if (firstFreeParticle > 0)
						{
							graphicsDevice.DrawPrimitives(PrimitiveType.PointList, 0, firstFreeParticle);
						}
					}
					pass.End();
				}
				particleEffect.End();
				graphicsDevice.RenderState.PointSpriteEnable = false;
				graphicsDevice.RenderState.DepthBufferWriteEnable = true;
				graphicsDevice.RenderState.AlphaBlendEnable = false;
				graphicsDevice.RenderState.AlphaBlendOperation = BlendFunction.Add;
				graphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
				graphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
			}
			drawCounter++;
		}
		catch (Exception ex)
		{
			MainGame.DebugMsg = "ParticleSystem::Draw - Exception: " + ex.Message;
		}
	}

	private void AddNewParticlesToVertexBuffer()
	{
		int num = 32;
		if (firstNewParticle < firstFreeParticle)
		{
			vertexBuffer.SetData(firstNewParticle * num, particles, firstNewParticle, firstFreeParticle - firstNewParticle, num, SetDataOptions.NoOverwrite);
		}
		else
		{
			vertexBuffer.SetData(firstNewParticle * num, particles, firstNewParticle, particles.Length - firstNewParticle, num, SetDataOptions.NoOverwrite);
			if (firstFreeParticle > 0)
			{
				vertexBuffer.SetData(0, particles, 0, firstFreeParticle, num, SetDataOptions.NoOverwrite);
			}
		}
		firstNewParticle = firstFreeParticle;
	}

	private void SetParticleRenderStates(RenderState renderState)
	{
		renderState.PointSpriteEnable = true;
		renderState.PointSizeMax = 256f;
		renderState.AlphaBlendEnable = true;
		renderState.AlphaBlendOperation = BlendFunction.Add;
		renderState.SourceBlend = settings.SourceBlend;
		renderState.DestinationBlend = settings.DestinationBlend;
		renderState.AlphaTestEnable = true;
		renderState.AlphaFunction = CompareFunction.Greater;
		renderState.ReferenceAlpha = 0;
		renderState.DepthBufferEnable = true;
		renderState.DepthBufferWriteEnable = false;
	}

	public void SetCamera(Matrix view, Matrix projection)
	{
		effectViewParameter.SetValue(view);
		effectProjectionParameter.SetValue(projection);
	}

	public void AddParticle(Vector3 position, Vector3 velocity)
	{
		int num = firstFreeParticle + 1;
		if (num >= particles.Length)
		{
			num = 0;
		}
		if (num != firstRetiredParticle)
		{
			velocity *= settings.EmitterVelocitySensitivity;
			float num2 = MathHelper.Lerp(settings.MinHorizontalVelocity, settings.MaxHorizontalVelocity, (float)random.NextDouble());
			double d = random.NextDouble() * 6.2831854820251465;
			velocity.X += num2 * (float)Math.Cos(d);
			velocity.Y += MathHelper.Lerp(settings.MinVerticalVelocity, settings.MaxVerticalVelocity, (float)random.NextDouble());
			Color color = new Color((byte)random.Next(255), (byte)random.Next(255), (byte)random.Next(255), (byte)random.Next(255));
			particles[firstFreeParticle].Position = position;
			particles[firstFreeParticle].Velocity = velocity;
			particles[firstFreeParticle].Random = color;
			particles[firstFreeParticle].Time = currentTime;
			firstFreeParticle = num;
		}
	}
}

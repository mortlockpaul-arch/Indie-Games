using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using SynapseGaming.LightingSystem.Rendering;

namespace Deep_waters;

public abstract class ParticleSystem
{
	public ParticleSettings settings = new ParticleSettings();

	private ContentManager content;

	private GraphicsDevice graphicdevice;

	private Effect particleEffect;

	private EffectParameter effectViewParameter;

	private EffectParameter effectProjectionParameter;

	private EffectParameter effectViewportScaleParameter;

	private EffectParameter effectTimeParameter;

	private ParticleVertex[] particles;

	private DynamicVertexBuffer vertexBuffer;

	private IndexBuffer indexBuffer;

	private int createdparticle;

	private int firstActiveParticle;

	private int firstNewParticle;

	private int firstFreeParticle;

	private int firstRetiredParticle;

	public Vector3 position;

	public Vector3 velocity;

	private float currentTime;

	public Vector3 Emitter;

	private int drawCounter;

	private static Random random = new Random();

	public bool ended;

	private string BoneName;

	private SceneObject sceneobj;

	private Texture2D texture;

	protected ParticleSystem(GraphicsDevice gdev, Texture2D text, Vector3 emitter)
	{
		texture = text;
		graphicdevice = gdev;
		Emitter = emitter;
		Initialize();
		LoadContent();
	}

	protected ParticleSystem(GraphicsDevice gdev, Texture2D text, Effect eff, Vector3 emitter)
	{
		texture = text;
		graphicdevice = gdev;
		Emitter = emitter;
		particleEffect = eff;
		Initialize();
		LoadContent();
	}

	protected ParticleSystem(GraphicsDevice gdev, Texture2D text)
	{
		texture = text;
		graphicdevice = gdev;
		Initialize();
		LoadContent();
	}

	public void Initialize()
	{
		InitializeSettings(settings);
		particles = new ParticleVertex[settings.MaxParticles * 4];
		for (int i = 0; i < settings.MaxParticles; i++)
		{
			particles[i * 4].Corner = new Short2(-1f, -1f);
			particles[i * 4 + 1].Corner = new Short2(1f, -1f);
			particles[i * 4 + 2].Corner = new Short2(1f, 1f);
			particles[i * 4 + 3].Corner = new Short2(-1f, 1f);
		}
	}

	protected abstract void InitializeSettings(ParticleSettings settings);

	protected void LoadContent()
	{
		LoadParticleEffect();
		vertexBuffer = new DynamicVertexBuffer(graphicdevice, ParticleVertex.VertexDeclaration, settings.MaxParticles * 4, BufferUsage.WriteOnly);
		ushort[] array = new ushort[settings.MaxParticles * 6];
		for (int i = 0; i < settings.MaxParticles; i++)
		{
			array[i * 6] = (ushort)(i * 4);
			array[i * 6 + 1] = (ushort)(i * 4 + 1);
			array[i * 6 + 2] = (ushort)(i * 4 + 2);
			array[i * 6 + 3] = (ushort)(i * 4);
			array[i * 6 + 4] = (ushort)(i * 4 + 2);
			array[i * 6 + 5] = (ushort)(i * 4 + 3);
		}
		indexBuffer = new IndexBuffer(graphicdevice, typeof(ushort), array.Length, BufferUsage.WriteOnly);
		indexBuffer.SetData(array);
	}

	private void LoadParticleEffect()
	{
		EffectParameterCollection parameters = particleEffect.Parameters;
		effectViewParameter = parameters["View"];
		effectProjectionParameter = parameters["Projection"];
		effectViewportScaleParameter = parameters["ViewportScale"];
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
		parameters["Texture"].SetValue(texture);
	}

	public void Update(GameTime gameTime)
	{
		if (gameTime == null)
		{
			throw new ArgumentNullException("gameTime");
		}
		if (settings.isUpdatable)
		{
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
	}

	private void RetireActiveParticles()
	{
		float num = (float)settings.Duration.TotalSeconds;
		while (firstActiveParticle != firstNewParticle)
		{
			float num2 = currentTime - particles[firstActiveParticle * 4].Time;
			if (num2 < num)
			{
				break;
			}
			particles[firstActiveParticle * 4].Time = drawCounter;
			firstActiveParticle++;
			if (firstActiveParticle >= settings.MaxParticles)
			{
				firstActiveParticle = 0;
			}
		}
	}

	private void FreeRetiredParticles()
	{
		while (firstRetiredParticle != firstActiveParticle)
		{
			int num = drawCounter - (int)particles[firstRetiredParticle * 4].Time;
			if (num < 3)
			{
				break;
			}
			firstRetiredParticle++;
			if (firstRetiredParticle >= settings.MaxParticles)
			{
				firstRetiredParticle = 0;
			}
		}
	}

	public void Draw(GameTime gameTime)
	{
		if (!settings.isVisible)
		{
			return;
		}
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
			graphicdevice.BlendState = settings.BlendState;
			graphicdevice.DepthStencilState = DepthStencilState.DepthRead;
			effectViewportScaleParameter.SetValue(new Vector2(0.5f / graphicdevice.Viewport.AspectRatio, -0.5f));
			effectTimeParameter.SetValue(currentTime);
			graphicdevice.SetVertexBuffer(vertexBuffer);
			graphicdevice.Indices = indexBuffer;
			foreach (EffectPass pass in particleEffect.CurrentTechnique.Passes)
			{
				pass.Apply();
				if (firstActiveParticle < firstFreeParticle)
				{
					graphicdevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, firstActiveParticle * 4, (firstFreeParticle - firstActiveParticle) * 4, firstActiveParticle * 6, (firstFreeParticle - firstActiveParticle) * 2);
					continue;
				}
				graphicdevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, firstActiveParticle * 4, (settings.MaxParticles - firstActiveParticle) * 4, firstActiveParticle * 6, (settings.MaxParticles - firstActiveParticle) * 2);
				if (firstFreeParticle > 0)
				{
					graphicdevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, firstFreeParticle * 4, 0, firstFreeParticle * 2);
				}
			}
			graphicdevice.DepthStencilState = DepthStencilState.Default;
		}
		drawCounter++;
	}

	private void AddNewParticlesToVertexBuffer()
	{
		int num = 36;
		if (firstNewParticle < firstFreeParticle)
		{
			vertexBuffer.SetData(firstNewParticle * num * 4, particles, firstNewParticle * 4, (firstFreeParticle - firstNewParticle) * 4, num, SetDataOptions.NoOverwrite);
		}
		else
		{
			vertexBuffer.SetData(firstNewParticle * num * 4, particles, firstNewParticle * 4, (settings.MaxParticles - firstNewParticle) * 4, num, SetDataOptions.NoOverwrite);
			if (firstFreeParticle > 0)
			{
				vertexBuffer.SetData(0, particles, 0, firstFreeParticle * 4, num, SetDataOptions.NoOverwrite);
			}
		}
		firstNewParticle = firstFreeParticle;
	}

	public void SetCamera(Matrix view, Matrix projection)
	{
		effectViewParameter.SetValue(view);
		effectProjectionParameter.SetValue(projection);
	}

	public void AddParticle()
	{
		if (settings.isabox)
		{
			position = Emitter + new Vector3((float)random.Next(-1, 1) * 0.5f, 0f, (float)random.Next(-1, 1) * 0.5f);
		}
		else
		{
			position = Emitter;
		}
		if (settings.totalparticles > 0 && createdparticle > settings.totalparticles)
		{
			ended = true;
			return;
		}
		int num = firstFreeParticle + 1;
		if (num >= settings.MaxParticles)
		{
			num = 0;
		}
		if (num != firstRetiredParticle)
		{
			float num2 = MathHelper.Lerp(settings.MinHorizontalVelocity, settings.MaxHorizontalVelocity, (float)random.NextDouble());
			double num3 = random.NextDouble() * 6.2831854820251465;
			velocity.X += num2 * (float)Math.Cos(num3);
			velocity.Z += num2 * (float)Math.Sin(num3);
			velocity.Y += MathHelper.Lerp(settings.MinVerticalVelocity, settings.MaxVerticalVelocity, (float)random.NextDouble());
			Color color = new Color((byte)random.Next(255), (byte)random.Next(255), (byte)random.Next(255), (byte)random.Next(255));
			for (int i = 0; i < 4; i++)
			{
				particles[firstFreeParticle * 4 + i].Position = position;
				particles[firstFreeParticle * 4 + i].Velocity = velocity;
				particles[firstFreeParticle * 4 + i].Random = color;
				particles[firstFreeParticle * 4 + i].Time = currentTime;
			}
			firstFreeParticle = num;
			createdparticle++;
		}
	}
}

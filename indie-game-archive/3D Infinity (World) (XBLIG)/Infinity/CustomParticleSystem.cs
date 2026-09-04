using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;

namespace Infinity;

public class CustomParticleSystem
{
	public Game game;

	public ContentManager content;

	public int maxParticles;

	public Effect particleEffect;

	public string textureAsset;

	public Texture2D texture;

	public EffectParameter effectWorldParameter;

	public EffectParameter effectViewParameter;

	public EffectParameter effectProjectionParameter;

	public EffectParameter effectViewportHeightParameter;

	public EffectParameter effectTextureParameter;

	public ParticleVertex[] particles;

	public DynamicVertexBuffer vertexBuffer;

	public VertexDeclaration vertexDeclaration;

	public CustomParticleSystem(Game game, ContentManager content, int maxParticles, string textureAsset)
	{
		this.game = game;
		this.content = content;
		this.maxParticles = maxParticles;
		this.textureAsset = textureAsset;
		LoadContent();
	}

	protected void LoadContent()
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Expected O, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Expected O, but got Unknown
		particles = new ParticleVertex[maxParticles];
		Random random = new Random();
		for (int i = 0; i < particles.Length; i++)
		{
			particles[i].Position.X = (float)random.NextDouble() * 100f - 100f;
			particles[i].Position.Y = (float)random.NextDouble() * 100f - 100f;
			particles[i].Position.Z = (float)random.NextDouble() * 100f - 100f;
			particles[i].Random = Color.White;
		}
		InitializeParticleEffect();
		texture = content.Load<Texture2D>(textureAsset);
		vertexDeclaration = new VertexDeclaration(game.GraphicsDevice, ParticleVertex.VertexElements);
		int num = 32 * particles.Length;
		vertexBuffer = new DynamicVertexBuffer(game.GraphicsDevice, num, (BufferUsage)72);
	}

	private void InitializeParticleEffect()
	{
		particleEffect = content.Load<Effect>("Effects/PointSprite");
		EffectParameterCollection parameters = particleEffect.Parameters;
		effectWorldParameter = parameters["World"];
		effectViewParameter = parameters["View"];
		effectProjectionParameter = parameters["Projection"];
		effectViewportHeightParameter = parameters["ViewportHeight"];
		effectTextureParameter = parameters["Texture"];
	}

	public void Draw(GameTime gameTime, SpriteBlendMode spriteBlendMode)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		GraphicsDevice graphicsDevice = game.GraphicsDevice;
		SetParticleRenderStates(graphicsDevice.RenderState, spriteBlendMode, texture.Width * 10);
		effectWorldParameter.SetValue(Matrix.Identity);
		EffectParameter obj = effectViewportHeightParameter;
		Viewport viewport = graphicsDevice.Viewport;
		obj.SetValue(((Viewport)(ref viewport)).Height);
		effectTextureParameter.SetValue((Texture)(object)texture);
		graphicsDevice.VertexDeclaration = vertexDeclaration;
		particleEffect.Begin();
		for (int i = 0; i < particleEffect.CurrentTechnique.Passes.Count; i++)
		{
			EffectPass val = particleEffect.CurrentTechnique.Passes[i];
			val.Begin();
			graphicsDevice.DrawUserPrimitives<ParticleVertex>((PrimitiveType)1, particles, 0, particles.Length);
			val.End();
		}
		particleEffect.End();
		graphicsDevice.RenderState.PointSpriteEnable = false;
		graphicsDevice.RenderState.DepthBufferWriteEnable = true;
	}

	public static void SetParticleRenderStates(RenderState renderState, SpriteBlendMode spriteBlendMode, int pointSizeMax)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		if ((int)spriteBlendMode != 0)
		{
			if ((int)spriteBlendMode == 1)
			{
				SetParticleRenderStates(renderState, (Blend)5, (Blend)6, pointSizeMax);
				renderState.DepthBufferEnable = true;
				renderState.DepthBufferWriteEnable = true;
			}
			else if ((int)spriteBlendMode == 2)
			{
				SetParticleRenderStates(renderState, (Blend)5, (Blend)2, pointSizeMax);
			}
		}
	}

	public static void SetParticleRenderStates(RenderState renderState, SpriteBlendMode spriteBlendMode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetParticleRenderStates(renderState, spriteBlendMode, 255);
	}

	public static void SetParticleRenderStates(RenderState renderState, Blend sourceBlend, Blend destinationBlend, int pointSizeMax)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		renderState.PointSpriteEnable = true;
		renderState.PointSizeMax = pointSizeMax;
		renderState.AlphaBlendEnable = true;
		renderState.AlphaBlendOperation = (BlendFunction)1;
		renderState.SourceBlend = sourceBlend;
		renderState.DestinationBlend = destinationBlend;
		renderState.AlphaTestEnable = true;
		renderState.AlphaFunction = (CompareFunction)5;
		renderState.ReferenceAlpha = 0;
		renderState.DepthBufferEnable = true;
		renderState.DepthBufferWriteEnable = false;
	}

	public void SetCamera(Matrix view, Matrix projection)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		effectViewParameter.SetValue(view);
		effectProjectionParameter.SetValue(projection);
	}
}

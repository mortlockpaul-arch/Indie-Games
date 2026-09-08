using System;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.GamerServices;

internal class AvatarRenderPassShiny : IDisposable
{
	public AvatarRenderPassShiny owner;

	public int referenceCount;

	public Effect effect;

	public VertexBuffer texCoordBufferMask;

	public SamplerState environmentMapMaskSmaplerState;

	public DynamicVertexBuffer texCoordBufferReflection;

	public Texture2D environmentTexture;

	public Texture2D environnmentMask;

	public AvatarRenderPassShiny(GraphicsDevice graphicsDevice, int vertexCount)
	{
		owner = this;
		referenceCount = 1;
		texCoordBufferReflection = new DynamicVertexBuffer(graphicsDevice, typeof(TexCoordVertex), vertexCount, BufferUsage.WriteOnly);
	}

	public AvatarRenderPassShiny(AvatarRenderPassShiny source)
	{
		referenceCount = 1;
		owner = source.owner;
		owner.AddReference();
		effect = source.effect.Clone();
		environmentTexture = source.environmentTexture;
		environnmentMask = source.environnmentMask;
		texCoordBufferMask = source.texCoordBufferMask;
		environmentMapMaskSmaplerState = source.environmentMapMaskSmaplerState;
		texCoordBufferReflection = new DynamicVertexBuffer(source.texCoordBufferReflection.GraphicsDevice, typeof(TexCoordVertex), source.texCoordBufferReflection.VertexCount, BufferUsage.WriteOnly);
	}

	~AvatarRenderPassShiny()
	{
		Dispose(disposing: false);
	}

	public void AddReference()
	{
		referenceCount++;
	}

	public void Release()
	{
		referenceCount--;
		if (referenceCount == 0)
		{
			if (owner != this)
			{
				owner.Release();
			}
			Dispose();
		}
	}

	private void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (effect != null)
			{
				effect.Dispose();
			}
			if (texCoordBufferReflection != null)
			{
				texCoordBufferReflection.Dispose();
			}
			if (this == owner)
			{
				if (texCoordBufferMask != null)
				{
					texCoordBufferMask.Dispose();
				}
				if (environmentMapMaskSmaplerState != null)
				{
					environmentMapMaskSmaplerState.Dispose();
				}
				if (environmentTexture != null)
				{
					environmentTexture.Dispose();
				}
				if (environnmentMask != null)
				{
					environnmentMask.Dispose();
				}
			}
		}
		effect = null;
		texCoordBufferMask = null;
		environmentMapMaskSmaplerState = null;
		texCoordBufferReflection = null;
		environmentTexture = null;
		environnmentMask = null;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}

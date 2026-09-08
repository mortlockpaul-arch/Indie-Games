using System;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.GamerServices;

internal abstract class AvatarRenderPass : IDisposable
{
	public int referenceCount;

	public AvatarRenderPass owner;

	public ShaderParameterUsage usage;

	public Effect effect;

	public IndexBuffer indexBuffer;

	public BlendState blendState;

	public Texture2D[] textures;

	public SamplerState samplerStatePrimary;

	public VertexBuffer texCoordBufferPrimary;

	public SamplerState samplerStateSecondary;

	public VertexBuffer texCoordBufferSecondary;

	public abstract int TextureLayer { set; }

	public AvatarRenderPass()
	{
		referenceCount = 1;
		owner = this;
	}

	~AvatarRenderPass()
	{
		Dispose(disposing: false);
	}

	protected void CopyTo(AvatarRenderPass target)
	{
		lock (owner)
		{
			target.owner = owner;
			target.owner.AddReference();
			target.usage = usage;
			target.indexBuffer = indexBuffer;
			target.blendState = blendState;
			target.textures = textures;
			target.samplerStatePrimary = samplerStatePrimary;
			target.texCoordBufferPrimary = texCoordBufferPrimary;
			target.samplerStateSecondary = samplerStateSecondary;
			target.texCoordBufferSecondary = texCoordBufferSecondary;
		}
	}

	public abstract AvatarRenderPass Clone();

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
			if (this == owner)
			{
				if (indexBuffer != null)
				{
					indexBuffer.Dispose();
				}
				if (blendState != null)
				{
					blendState.Dispose();
				}
				if (samplerStatePrimary != null)
				{
					samplerStatePrimary.Dispose();
				}
				if (samplerStateSecondary != null)
				{
					samplerStateSecondary.Dispose();
				}
				if (texCoordBufferPrimary != null)
				{
					texCoordBufferPrimary.Dispose();
				}
				if (texCoordBufferSecondary != null)
				{
					texCoordBufferSecondary.Dispose();
				}
				if (textures != null)
				{
					Texture2D[] array = textures;
					for (int i = 0; i < array.Length; i++)
					{
						array[i]?.Dispose();
					}
				}
			}
		}
		effect = null;
		indexBuffer = null;
		blendState = null;
		textures = null;
		samplerStatePrimary = null;
		texCoordBufferPrimary = null;
		samplerStateSecondary = null;
		texCoordBufferSecondary = null;
		owner = null;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}

using System;
using System.Collections.Generic;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.GamerServices;

internal class AvatarRenderableModel : IDisposable
{
	public AvatarRenderableModel owner;

	public int referenceCount;

	public int vertexCount;

	public IndexBuffer indexBuffer;

	public DynamicVertexBuffer vertexBuffer;

	public SkinnedVertex[] skinnedVertices;

	public List<AvatarRenderPass> opaquePasses = new List<AvatarRenderPass>();

	public List<AvatarRenderPass> transparentPasses = new List<AvatarRenderPass>();

	public AvatarRenderPassShiny shinyPass;

	public ComponentManifest Manifest { get; set; }

	public int ModelIndex { get; set; }

	protected AvatarRenderableModel()
	{
		referenceCount = 1;
		owner = this;
	}

	~AvatarRenderableModel()
	{
		Dispose(disposing: false);
	}

	public AvatarRenderableModel(GraphicsDevice device, int triangleCount, int vertexCount)
		: this()
	{
		this.vertexCount = vertexCount;
		vertexBuffer = new DynamicVertexBuffer(device, typeof(PositionColorVertex), vertexCount, BufferUsage.WriteOnly);
		indexBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, 3 * triangleCount, BufferUsage.WriteOnly);
	}

	public AvatarRenderableModel(GraphicsDevice device, AvatarRenderableModel source)
	{
		owner = source.owner;
		source.owner.AddReference();
		vertexCount = source.vertexCount;
		vertexBuffer = new DynamicVertexBuffer(device, typeof(PositionColorVertex), vertexCount, BufferUsage.WriteOnly);
		indexBuffer = source.indexBuffer;
		skinnedVertices = source.skinnedVertices;
		Manifest = source.Manifest;
		ModelIndex = source.ModelIndex;
		opaquePasses.AddRange(source.opaquePasses);
		transparentPasses.AddRange(source.transparentPasses);
		foreach (AvatarRenderPass opaquePass in opaquePasses)
		{
			opaquePass.AddReference();
		}
		foreach (AvatarRenderPass transparentPass in transparentPasses)
		{
			transparentPass.AddReference();
		}
		if (source.shinyPass != null)
		{
			shinyPass = new AvatarRenderPassShiny(source.shinyPass);
		}
		referenceCount = 1;
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
			if (this != owner)
			{
				owner.Release();
			}
			Dispose();
		}
	}

	protected void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (owner == this && indexBuffer != null)
			{
				indexBuffer.Dispose();
			}
			if (vertexBuffer != null)
			{
				vertexBuffer.Dispose();
			}
			foreach (AvatarRenderPass opaquePass in opaquePasses)
			{
				opaquePass.Release();
			}
			foreach (AvatarRenderPass transparentPass in transparentPasses)
			{
				transparentPass.Release();
			}
			if (shinyPass != null)
			{
				shinyPass.Release();
			}
		}
		owner = null;
		indexBuffer = null;
		vertexBuffer = null;
		skinnedVertices = null;
		Manifest = null;
		opaquePasses = null;
		transparentPasses = null;
		shinyPass = null;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}

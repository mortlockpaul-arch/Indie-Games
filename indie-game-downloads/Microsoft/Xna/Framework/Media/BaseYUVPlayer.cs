using System;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Media;

internal abstract class BaseYUVPlayer : IDisposable
{
	protected Effect shaderProgram;

	private nint stateChangesPtr;

	protected Texture2D[] yuvTextures = new Texture2D[3];

	private Viewport viewport;

	private static VertexPositionTexture[] vertices = new VertexPositionTexture[4]
	{
		new VertexPositionTexture(new Vector3(-1f, -1f, 0f), new Vector2(0f, 1f)),
		new VertexPositionTexture(new Vector3(1f, -1f, 0f), new Vector2(1f, 1f)),
		new VertexPositionTexture(new Vector3(-1f, 1f, 0f), new Vector2(0f, 0f)),
		new VertexPositionTexture(new Vector3(1f, 1f, 0f), new Vector2(1f, 0f))
	};

	private VertexBufferBinding vertBuffer;

	private Texture[] oldTextures = new Texture[3];

	private SamplerState[] oldSamplers = new SamplerState[3];

	private RenderTargetBinding[] oldTargets;

	private VertexBufferBinding[] oldBuffers;

	private BlendState prevBlend;

	private DepthStencilState prevDepthStencil;

	private RasterizerState prevRasterizer;

	private Viewport prevViewport;

	private FNA3D.FNA3D_RenderTargetBinding[] nativeVideoTexture = new FNA3D.FNA3D_RenderTargetBinding[3];

	private FNA3D.FNA3D_RenderTargetBinding[] nativeOldTargets = new FNA3D.FNA3D_RenderTargetBinding[4];

	protected RenderTargetBinding[] videoTexture;

	protected GraphicsDevice currentDevice;

	public bool IsDisposed { get; private set; }

	protected unsafe void GL_initialize(byte[] shaderProgramBytes)
	{
		shaderProgram = new Effect(currentDevice, shaderProgramBytes);
		stateChangesPtr = FNAPlatform.Malloc(sizeof(Effect.MOJOSHADER_effectStateChanges));
		vertBuffer = new VertexBufferBinding(new VertexBuffer(currentDevice, VertexPositionTexture.VertexDeclaration, 4, BufferUsage.WriteOnly));
		vertBuffer.VertexBuffer.SetData(vertices);
	}

	protected void GL_dispose()
	{
		if (currentDevice == null)
		{
			return;
		}
		currentDevice = null;
		if (shaderProgram != null)
		{
			shaderProgram.Dispose();
		}
		if (stateChangesPtr != IntPtr.Zero)
		{
			FNAPlatform.Free(stateChangesPtr);
		}
		if (vertBuffer.VertexBuffer != null)
		{
			vertBuffer.VertexBuffer.Dispose();
		}
		for (int i = 0; i < 3; i++)
		{
			if (yuvTextures[i] != null)
			{
				yuvTextures[i].Dispose();
			}
		}
	}

	protected void GL_setupTextures(int yWidth, int yHeight, int uvWidth, int uvHeight, SurfaceFormat surfaceFormat)
	{
		for (int i = 0; i < 3; i++)
		{
			if (yuvTextures[i] != null)
			{
				yuvTextures[i].Dispose();
			}
		}
		yuvTextures[0] = new Texture2D(currentDevice, yWidth, yHeight, mipMap: false, surfaceFormat);
		yuvTextures[1] = new Texture2D(currentDevice, uvWidth, uvHeight, mipMap: false, surfaceFormat);
		yuvTextures[2] = new Texture2D(currentDevice, uvWidth, uvHeight, mipMap: false, surfaceFormat);
		viewport = new Viewport(0, 0, yWidth, yHeight);
	}

	protected unsafe void GL_pushState()
	{
		FNA3D.FNA3D_BeginPassRestore(currentDevice.GLDevice, shaderProgram.glEffect, stateChangesPtr);
		for (int i = 0; i < 3; i++)
		{
			oldTextures[i] = currentDevice.Textures[i];
			oldSamplers[i] = currentDevice.SamplerStates[i];
			currentDevice.Textures[i] = yuvTextures[i];
			currentDevice.SamplerStates[i] = SamplerState.LinearClamp;
		}
		oldBuffers = currentDevice.GetVertexBuffers();
		currentDevice.SetVertexBuffers(vertBuffer);
		int renderTargetsNoAllocEXT = currentDevice.GetRenderTargetsNoAllocEXT(null);
		Array.Resize(ref oldTargets, renderTargetsNoAllocEXT);
		currentDevice.GetRenderTargetsNoAllocEXT(oldTargets);
		fixed (FNA3D.FNA3D_RenderTargetBinding* ptr = &nativeVideoTexture[0])
		{
			GraphicsDevice.PrepareRenderTargetBindings(ptr, videoTexture);
			FNA3D.FNA3D_SetRenderTargets(currentDevice.GLDevice, ptr, videoTexture.Length, IntPtr.Zero, DepthFormat.None, 0);
		}
		prevBlend = currentDevice.BlendState;
		prevDepthStencil = currentDevice.DepthStencilState;
		prevRasterizer = currentDevice.RasterizerState;
		currentDevice.BlendState = BlendState.Opaque;
		currentDevice.DepthStencilState = DepthStencilState.None;
		currentDevice.RasterizerState = RasterizerState.CullNone;
		prevViewport = currentDevice.Viewport;
		FNA3D.FNA3D_SetViewport(currentDevice.GLDevice, ref viewport.viewport);
	}

	protected unsafe void GL_popState()
	{
		FNA3D.FNA3D_EndPassRestore(currentDevice.GLDevice, shaderProgram.glEffect);
		currentDevice.BlendState = prevBlend;
		currentDevice.DepthStencilState = prevDepthStencil;
		currentDevice.RasterizerState = prevRasterizer;
		prevBlend = null;
		prevDepthStencil = null;
		prevRasterizer = null;
		if (oldTargets == null || oldTargets.Length == 0)
		{
			FNA3D.FNA3D_SetRenderTargets(currentDevice.GLDevice, IntPtr.Zero, 0, IntPtr.Zero, DepthFormat.None, 0);
		}
		else
		{
			IRenderTarget renderTarget = oldTargets[0].RenderTarget as IRenderTarget;
			fixed (FNA3D.FNA3D_RenderTargetBinding* ptr = &nativeOldTargets[0])
			{
				GraphicsDevice.PrepareRenderTargetBindings(ptr, oldTargets);
				FNA3D.FNA3D_SetRenderTargets(currentDevice.GLDevice, ptr, oldTargets.Length, renderTarget.DepthStencilBuffer, renderTarget.DepthStencilFormat, (byte)((renderTarget.RenderTargetUsage != RenderTargetUsage.DiscardContents) ? 1u : 0u));
			}
		}
		oldTargets = null;
		FNA3D.FNA3D_SetViewport(currentDevice.GLDevice, ref prevViewport.viewport);
		currentDevice.SetVertexBuffers(oldBuffers);
		oldBuffers = null;
		currentDevice.Textures.ignoreTargets = true;
		for (int i = 0; i < 3; i++)
		{
			if (oldTextures[i] == null || !oldTextures[i].IsDisposed)
			{
				currentDevice.Textures[i] = oldTextures[i];
			}
			currentDevice.SamplerStates[i] = oldSamplers[i];
			oldTextures[i] = null;
			oldSamplers[i] = null;
		}
		currentDevice.Textures.ignoreTargets = false;
	}

	protected void checkDisposed()
	{
		if (IsDisposed)
		{
			throw new ObjectDisposedException("VideoPlayer");
		}
	}

	internal BaseYUVPlayer()
	{
		IsDisposed = false;
		videoTexture = new RenderTargetBinding[1];
	}

	public virtual void Dispose()
	{
		if (!IsDisposed)
		{
			GL_dispose();
			if (videoTexture[0].RenderTarget != null)
			{
				videoTexture[0].RenderTarget.Dispose();
			}
			IsDisposed = true;
		}
	}
}

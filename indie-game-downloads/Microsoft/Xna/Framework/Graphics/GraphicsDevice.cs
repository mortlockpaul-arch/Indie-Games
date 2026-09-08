using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Microsoft.Xna.Framework.Graphics;

public class GraphicsDevice : IDisposable
{
	internal const int MAX_TEXTURE_SAMPLERS = 16;

	internal const int MAX_VERTEX_ATTRIBUTES = 16;

	internal const int MAX_RENDERTARGET_BINDINGS = 4;

	internal const int MAX_VERTEXTEXTURE_SAMPLERS = 4;

	private Rectangle INTERNAL_scissorRectangle;

	private Viewport INTERNAL_viewport;

	internal readonly nint GLDevice;

	internal readonly PipelineCache PipelineCache;

	private BlendState currentBlend;

	private BlendState nextBlend;

	private DepthStencilState currentDepthStencil;

	private DepthStencilState nextDepthStencil;

	private readonly bool[] modifiedSamplers = new bool[16];

	private readonly bool[] modifiedVertexSamplers = new bool[4];

	internal nint effectStateChangesPtr;

	private readonly List<GCHandle> resources = new List<GCHandle>();

	private readonly object resourcesLock = new object();

	private static Vector4 DiscardColor = new Color(68, 34, 136, 255).ToVector4();

	internal readonly RenderTargetBinding[] renderTargetBindings = new RenderTargetBinding[4];

	private FNA3D.FNA3D_RenderTargetBinding[] nativeTargetBindings = new FNA3D.FNA3D_RenderTargetBinding[4];

	private FNA3D.FNA3D_RenderTargetBinding[] nativeTargetBindingsNext = new FNA3D.FNA3D_RenderTargetBinding[4];

	internal int renderTargetCount = 0;

	private readonly RenderTargetBinding[] singleTargetCache = new RenderTargetBinding[1];

	private readonly VertexBufferBinding[] vertexBufferBindings = new VertexBufferBinding[16];

	private readonly FNA3D.FNA3D_VertexBufferBinding[] nativeBufferBindings = new FNA3D.FNA3D_VertexBufferBinding[16];

	private int vertexBufferCount = 0;

	private bool vertexBuffersUpdated = false;

	private nint userVertexBuffer;

	private nint userIndexBuffer;

	private int userVertexBufferSize;

	private int userIndexBufferSize;

	public bool IsDisposed { get; private set; }

	public GraphicsDeviceStatus GraphicsDeviceStatus => GraphicsDeviceStatus.Normal;

	public GraphicsAdapter Adapter { get; private set; }

	public GraphicsProfile GraphicsProfile { get; private set; }

	public PresentationParameters PresentationParameters { get; private set; }

	public DisplayMode DisplayMode
	{
		get
		{
			if (PresentationParameters.IsFullScreen)
			{
				FNA3D.FNA3D_GetBackbufferSize(GLDevice, out var w, out var h);
				return new DisplayMode(w, h, FNA3D.FNA3D_GetBackbufferSurfaceFormat(GLDevice));
			}
			return Adapter.CurrentDisplayMode;
		}
	}

	public TextureCollection Textures { get; private set; }

	public SamplerStateCollection SamplerStates { get; private set; }

	public TextureCollection VertexTextures { get; private set; }

	public SamplerStateCollection VertexSamplerStates { get; private set; }

	public BlendState BlendState
	{
		get
		{
			return nextBlend;
		}
		set
		{
			nextBlend = value;
		}
	}

	public DepthStencilState DepthStencilState
	{
		get
		{
			return nextDepthStencil;
		}
		set
		{
			nextDepthStencil = value;
		}
	}

	public RasterizerState RasterizerState { get; set; }

	public Rectangle ScissorRectangle
	{
		get
		{
			return INTERNAL_scissorRectangle;
		}
		set
		{
			INTERNAL_scissorRectangle = value;
			FNA3D.FNA3D_SetScissorRect(GLDevice, ref value);
		}
	}

	public Viewport Viewport
	{
		get
		{
			return INTERNAL_viewport;
		}
		set
		{
			INTERNAL_viewport = value;
			FNA3D.FNA3D_SetViewport(GLDevice, ref value.viewport);
		}
	}

	public Color BlendFactor
	{
		get
		{
			FNA3D.FNA3D_GetBlendFactor(GLDevice, out var blendFactor);
			return blendFactor;
		}
		set
		{
			FNA3D.FNA3D_SetBlendFactor(GLDevice, ref value);
		}
	}

	public int MultiSampleMask
	{
		get
		{
			return FNA3D.FNA3D_GetMultiSampleMask(GLDevice);
		}
		set
		{
			FNA3D.FNA3D_SetMultiSampleMask(GLDevice, value);
		}
	}

	public int ReferenceStencil
	{
		get
		{
			return FNA3D.FNA3D_GetReferenceStencil(GLDevice);
		}
		set
		{
			FNA3D.FNA3D_SetReferenceStencil(GLDevice, value);
		}
	}

	public IndexBuffer Indices { get; set; }

	public event EventHandler<EventArgs> DeviceLost;

	public event EventHandler<EventArgs> DeviceReset;

	public event EventHandler<EventArgs> DeviceResetting;

	public event EventHandler<ResourceCreatedEventArgs> ResourceCreated;

	public event EventHandler<ResourceDestroyedEventArgs> ResourceDestroyed;

	public event EventHandler<EventArgs> Disposing;

	internal void OnResourceCreated(object resource)
	{
		if (ResourceCreated != null)
		{
			ResourceCreated(this, new ResourceCreatedEventArgs(resource));
		}
	}

	internal void OnResourceDestroyed(string name, object tag)
	{
		if (ResourceDestroyed != null)
		{
			ResourceDestroyed(this, new ResourceDestroyedEventArgs(name, tag));
		}
	}

	public unsafe GraphicsDevice(GraphicsAdapter adapter, GraphicsProfile graphicsProfile, PresentationParameters presentationParameters)
	{
		if (presentationParameters == null)
		{
			throw new ArgumentNullException("presentationParameters");
		}
		Adapter = adapter;
		PresentationParameters = presentationParameters;
		GraphicsProfile = graphicsProfile;
		PresentationParameters.MultiSampleCount = MathHelper.ClosestMSAAPower(PresentationParameters.MultiSampleCount);
		try
		{
			GLDevice = FNA3D.FNA3D_CreateDevice(ref PresentationParameters.parameters, 1);
		}
		catch (Exception ex)
		{
			throw new NoSuitableGraphicsDeviceException(ex.Message);
		}
		Mouse.INTERNAL_BackBufferWidth = PresentationParameters.BackBufferWidth;
		Mouse.INTERNAL_BackBufferHeight = PresentationParameters.BackBufferHeight;
		TouchPanel.DisplayWidth = PresentationParameters.BackBufferWidth;
		TouchPanel.DisplayHeight = PresentationParameters.BackBufferHeight;
		BlendState = BlendState.Opaque;
		DepthStencilState = DepthStencilState.Default;
		RasterizerState = RasterizerState.CullCounterClockwise;
		FNA3D.FNA3D_GetMaxTextureSlots(GLDevice, out var textures, out var vertexTextures);
		Textures = new TextureCollection(textures, modifiedSamplers);
		SamplerStates = new SamplerStateCollection(textures, modifiedSamplers);
		VertexTextures = new TextureCollection(vertexTextures, modifiedVertexSamplers);
		VertexSamplerStates = new SamplerStateCollection(vertexTextures, modifiedVertexSamplers);
		Viewport = new Viewport(PresentationParameters.Bounds);
		ScissorRectangle = Viewport.Bounds;
		PipelineCache = new PipelineCache(this);
		effectStateChangesPtr = FNAPlatform.Malloc(sizeof(Effect.MOJOSHADER_effectStateChanges));
		Effect.MOJOSHADER_effectStateChanges* ptr = (Effect.MOJOSHADER_effectStateChanges*)effectStateChangesPtr;
		ptr->render_state_change_count = 0u;
		ptr->sampler_state_change_count = 0u;
		ptr->vertex_sampler_state_change_count = 0u;
	}

	~GraphicsDevice()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (IsDisposed)
		{
			return;
		}
		IsDisposed = true;
		if (!disposing)
		{
			return;
		}
		if (Disposing != null)
		{
			Disposing(this, EventArgs.Empty);
		}
		lock (resourcesLock)
		{
			GCHandle[] array = resources.ToArray();
			resources.Clear();
			GCHandle[] array2 = array;
			foreach (GCHandle gCHandle in array2)
			{
				object target = gCHandle.Target;
				if (target != null)
				{
					(target as IDisposable).Dispose();
				}
			}
		}
		if (userVertexBuffer != IntPtr.Zero)
		{
			FNA3D.FNA3D_AddDisposeVertexBuffer(GLDevice, userVertexBuffer);
		}
		if (userIndexBuffer != IntPtr.Zero)
		{
			FNA3D.FNA3D_AddDisposeIndexBuffer(GLDevice, userIndexBuffer);
		}
		FNAPlatform.Free(effectStateChangesPtr);
		FNA3D.FNA3D_DestroyDevice(GLDevice);
	}

	internal void AddResourceReference(GCHandle resourceReference)
	{
		lock (resourcesLock)
		{
			resources.Add(resourceReference);
		}
	}

	internal bool RemoveResourceReference(GCHandle resourceReference)
	{
		lock (resourcesLock)
		{
			int i = 0;
			for (int count = resources.Count; i < count; i++)
			{
				if (!(resources[i] != resourceReference))
				{
					resources[i] = resources[resources.Count - 1];
					resources.RemoveAt(resources.Count - 1);
					return true;
				}
			}
		}
		return false;
	}

	internal void QuietlyUpdateAdapter(GraphicsAdapter adapter)
	{
		Adapter = adapter;
	}

	public void Present()
	{
		if (renderTargetCount > 0)
		{
			throw new InvalidOperationException("Cannot present while render targets are bound");
		}
		FNA3D.FNA3D_SwapBuffers(GLDevice, IntPtr.Zero, IntPtr.Zero, PresentationParameters.parameters.deviceWindowHandle);
	}

	public void Present(Rectangle? sourceRectangle, Rectangle? destinationRectangle, nint overrideWindowHandle)
	{
		if (renderTargetCount > 0)
		{
			throw new InvalidOperationException("Cannot present while render targets are bound");
		}
		overrideWindowHandle = ((overrideWindowHandle != IntPtr.Zero) ? FNAPlatform.WrapWindow(overrideWindowHandle) : PresentationParameters.parameters.deviceWindowHandle);
		if (sourceRectangle.HasValue && destinationRectangle.HasValue)
		{
			Rectangle sourceRectangle2 = sourceRectangle.Value;
			Rectangle destinationRectangle2 = destinationRectangle.Value;
			FNA3D.FNA3D_SwapBuffers(GLDevice, ref sourceRectangle2, ref destinationRectangle2, overrideWindowHandle);
		}
		else if (sourceRectangle.HasValue)
		{
			Rectangle sourceRectangle3 = sourceRectangle.Value;
			FNA3D.FNA3D_SwapBuffers(GLDevice, ref sourceRectangle3, IntPtr.Zero, overrideWindowHandle);
		}
		else if (destinationRectangle.HasValue)
		{
			Rectangle destinationRectangle3 = destinationRectangle.Value;
			FNA3D.FNA3D_SwapBuffers(GLDevice, IntPtr.Zero, ref destinationRectangle3, overrideWindowHandle);
		}
		else
		{
			FNA3D.FNA3D_SwapBuffers(GLDevice, IntPtr.Zero, IntPtr.Zero, overrideWindowHandle);
		}
	}

	public void Reset()
	{
		Reset(PresentationParameters, Adapter);
	}

	public void Reset(PresentationParameters presentationParameters)
	{
		Reset(presentationParameters, Adapter);
	}

	public void Reset(PresentationParameters presentationParameters, GraphicsAdapter graphicsAdapter)
	{
		if (presentationParameters == null)
		{
			throw new ArgumentNullException("presentationParameters");
		}
		PresentationParameters = presentationParameters;
		Adapter = graphicsAdapter;
		PresentationParameters.MultiSampleCount = FNA3D.FNA3D_GetMaxMultiSampleCount(GLDevice, PresentationParameters.BackBufferFormat, MathHelper.ClosestMSAAPower(PresentationParameters.MultiSampleCount));
		if (DeviceResetting != null)
		{
			DeviceResetting(this, EventArgs.Empty);
		}
		FNA3D.FNA3D_ResetBackbuffer(GLDevice, ref PresentationParameters.parameters);
		Mouse.INTERNAL_BackBufferWidth = PresentationParameters.BackBufferWidth;
		Mouse.INTERNAL_BackBufferHeight = PresentationParameters.BackBufferHeight;
		TouchPanel.DisplayWidth = PresentationParameters.BackBufferWidth;
		TouchPanel.DisplayHeight = PresentationParameters.BackBufferHeight;
		Viewport = new Viewport(0, 0, PresentationParameters.BackBufferWidth, PresentationParameters.BackBufferHeight);
		ScissorRectangle = new Rectangle(0, 0, PresentationParameters.BackBufferWidth, PresentationParameters.BackBufferHeight);
		if (DeviceReset != null)
		{
			DeviceReset(this, EventArgs.Empty);
		}
	}

	public void Clear(Color color)
	{
		Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, color.ToVector4(), Viewport.MaxDepth, 0);
	}

	public void Clear(ClearOptions options, Color color, float depth, int stencil)
	{
		Clear(options, color.ToVector4(), depth, stencil);
	}

	public void Clear(ClearOptions options, Vector4 color, float depth, int stencil)
	{
		switch ((renderTargetCount != 0) ? (renderTargetBindings[0].RenderTarget as IRenderTarget).DepthStencilFormat : FNA3D.FNA3D_GetBackbufferDepthFormat(GLDevice))
		{
		case DepthFormat.None:
			options &= ClearOptions.Target;
			break;
		default:
			options &= ~ClearOptions.Stencil;
			break;
		case DepthFormat.Depth24Stencil8:
			break;
		}
		FNA3D.FNA3D_Clear(GLDevice, options, ref color, depth, stencil);
	}

	public void GetBackBufferData<T>(T[] data) where T : struct
	{
		GetBackBufferData(null, data, 0, data.Length);
	}

	public void GetBackBufferData<T>(T[] data, int startIndex, int elementCount) where T : struct
	{
		GetBackBufferData(null, data, startIndex, elementCount);
	}

	public void GetBackBufferData<T>(Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct
	{
		int x;
		int y;
		int w;
		int h;
		if (!rect.HasValue)
		{
			x = 0;
			y = 0;
			FNA3D.FNA3D_GetBackbufferSize(GLDevice, out w, out h);
		}
		else
		{
			x = rect.Value.X;
			y = rect.Value.Y;
			w = rect.Value.Width;
			h = rect.Value.Height;
		}
		int num = MarshalHelper.SizeOf<T>();
		Texture.ValidateGetDataFormat(FNA3D.FNA3D_GetBackbufferSurfaceFormat(GLDevice), num);
		GCHandle gCHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
		FNA3D.FNA3D_ReadBackbuffer(GLDevice, x, y, w, h, gCHandle.AddrOfPinnedObject() + startIndex * num, data.Length * num);
		gCHandle.Free();
	}

	public void SetRenderTarget(RenderTarget2D renderTarget)
	{
		if (renderTarget == null)
		{
			SetRenderTargets((RenderTargetBinding[])null);
			return;
		}
		singleTargetCache[0] = new RenderTargetBinding(renderTarget);
		SetRenderTargets(singleTargetCache);
	}

	public void SetRenderTarget(RenderTargetCube renderTarget, CubeMapFace cubeMapFace)
	{
		if (renderTarget == null)
		{
			SetRenderTargets((RenderTargetBinding[])null);
			return;
		}
		singleTargetCache[0] = new RenderTargetBinding(renderTarget, cubeMapFace);
		SetRenderTargets(singleTargetCache);
	}

	public unsafe void SetRenderTargets(params RenderTargetBinding[] renderTargets)
	{
		FNA3D.FNA3D_ApplyRasterizerState(GLDevice, ref RasterizerState.state);
		ApplySamplers();
		if (renderTargets == null && renderTargetCount == 0)
		{
			return;
		}
		if (renderTargets != null && renderTargets.Length == renderTargetCount)
		{
			bool flag = true;
			for (int i = 0; i < renderTargets.Length; i++)
			{
				if (renderTargets[i].RenderTarget != renderTargetBindings[i].RenderTarget || renderTargets[i].CubeMapFace != renderTargetBindings[i].CubeMapFace)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				return;
			}
		}
		int width;
		int height;
		RenderTargetUsage renderTargetUsage;
		if (renderTargets == null || renderTargets.Length == 0)
		{
			FNA3D.FNA3D_SetRenderTargets(GLDevice, IntPtr.Zero, 0, IntPtr.Zero, DepthFormat.None, (byte)((PresentationParameters.RenderTargetUsage != RenderTargetUsage.DiscardContents) ? 1u : 0u));
			width = PresentationParameters.BackBufferWidth;
			height = PresentationParameters.BackBufferHeight;
			renderTargetUsage = PresentationParameters.RenderTargetUsage;
			for (int j = 0; j < renderTargetCount; j++)
			{
				FNA3D.FNA3D_ResolveTarget(GLDevice, ref nativeTargetBindings[j]);
			}
			Array.Clear(renderTargetBindings, 0, renderTargetBindings.Length);
			Array.Clear(nativeTargetBindings, 0, nativeTargetBindings.Length);
			renderTargetCount = 0;
		}
		else
		{
			IRenderTarget renderTarget = renderTargets[0].RenderTarget as IRenderTarget;
			fixed (FNA3D.FNA3D_RenderTargetBinding* ptr = &nativeTargetBindingsNext[0])
			{
				PrepareRenderTargetBindings(ptr, renderTargets);
				FNA3D.FNA3D_SetRenderTargets(GLDevice, ptr, renderTargets.Length, renderTarget.DepthStencilBuffer, renderTarget.DepthStencilFormat, (byte)((renderTarget.RenderTargetUsage != RenderTargetUsage.DiscardContents) ? 1u : 0u));
			}
			width = renderTarget.Width;
			height = renderTarget.Height;
			renderTargetUsage = renderTarget.RenderTargetUsage;
			for (int k = 0; k < renderTargetCount; k++)
			{
				bool flag2 = false;
				for (int l = 0; l < renderTargets.Length; l++)
				{
					if (renderTargetBindings[k].RenderTarget == renderTargets[l].RenderTarget)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					FNA3D.FNA3D_ResolveTarget(GLDevice, ref nativeTargetBindings[k]);
				}
			}
			Array.Clear(renderTargetBindings, 0, renderTargetBindings.Length);
			Array.Copy(renderTargets, renderTargetBindings, renderTargets.Length);
			Array.Clear(nativeTargetBindings, 0, nativeTargetBindings.Length);
			Array.Copy(nativeTargetBindingsNext, nativeTargetBindings, renderTargets.Length);
			renderTargetCount = renderTargets.Length;
		}
		Viewport = new Viewport(0, 0, width, height);
		ScissorRectangle = new Rectangle(0, 0, width, height);
		if (renderTargetUsage == RenderTargetUsage.DiscardContents)
		{
			Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, DiscardColor, Viewport.MaxDepth, 0);
		}
	}

	public RenderTargetBinding[] GetRenderTargets()
	{
		RenderTargetBinding[] array = new RenderTargetBinding[renderTargetCount];
		Array.Copy(renderTargetBindings, array, renderTargetCount);
		return array;
	}

	public int GetRenderTargetsNoAllocEXT(RenderTargetBinding[] output)
	{
		if (output == null)
		{
			return renderTargetCount;
		}
		if (output.Length < renderTargetCount)
		{
			throw new ArgumentException("Output buffer size incorrect");
		}
		Array.Copy(renderTargetBindings, output, renderTargetCount);
		return renderTargetCount;
	}

	public void SetVertexBuffer(VertexBuffer vertexBuffer)
	{
		SetVertexBuffer(vertexBuffer, 0);
	}

	public void SetVertexBuffer(VertexBuffer vertexBuffer, int vertexOffset)
	{
		if (vertexBuffer == null)
		{
			if (vertexBufferCount != 0)
			{
				for (int i = 0; i < vertexBufferCount; i++)
				{
					vertexBufferBindings[i] = VertexBufferBinding.None;
				}
				vertexBufferCount = 0;
				vertexBuffersUpdated = true;
			}
			return;
		}
		if (vertexBufferBindings[0].VertexBuffer != vertexBuffer || vertexBufferBindings[0].VertexOffset != vertexOffset)
		{
			vertexBufferBindings[0] = new VertexBufferBinding(vertexBuffer, vertexOffset);
			vertexBuffersUpdated = true;
		}
		if (vertexBufferCount > 1)
		{
			for (int j = 1; j < vertexBufferCount; j++)
			{
				vertexBufferBindings[j] = VertexBufferBinding.None;
			}
			vertexBuffersUpdated = true;
		}
		vertexBufferCount = 1;
	}

	public void SetVertexBuffers(params VertexBufferBinding[] vertexBuffers)
	{
		if (vertexBuffers == null)
		{
			if (vertexBufferCount != 0)
			{
				for (int i = 0; i < vertexBufferCount; i++)
				{
					vertexBufferBindings[i] = VertexBufferBinding.None;
				}
				vertexBufferCount = 0;
				vertexBuffersUpdated = true;
			}
			return;
		}
		if (vertexBuffers.Length > vertexBufferBindings.Length)
		{
			throw new ArgumentOutOfRangeException("vertexBuffers", $"Max Vertex Buffers supported is {vertexBufferBindings.Length}");
		}
		int j;
		for (j = 0; j < vertexBuffers.Length; j++)
		{
			if (vertexBufferBindings[j].VertexBuffer != vertexBuffers[j].VertexBuffer || vertexBufferBindings[j].VertexOffset != vertexBuffers[j].VertexOffset || vertexBufferBindings[j].InstanceFrequency != vertexBuffers[j].InstanceFrequency)
			{
				vertexBufferBindings[j] = vertexBuffers[j];
				vertexBuffersUpdated = true;
			}
		}
		if (vertexBuffers.Length < vertexBufferCount)
		{
			for (; j < vertexBufferCount; j++)
			{
				vertexBufferBindings[j] = VertexBufferBinding.None;
			}
			vertexBuffersUpdated = true;
		}
		vertexBufferCount = vertexBuffers.Length;
	}

	public VertexBufferBinding[] GetVertexBuffers()
	{
		VertexBufferBinding[] array = new VertexBufferBinding[vertexBufferCount];
		Array.Copy(vertexBufferBindings, array, vertexBufferCount);
		return array;
	}

	public void DrawIndexedPrimitives(PrimitiveType primitiveType, int baseVertex, int minVertexIndex, int numVertices, int startIndex, int primitiveCount)
	{
		ApplyState();
		PrepareVertexBindingArray(baseVertex);
		FNA3D.FNA3D_DrawIndexedPrimitives(GLDevice, primitiveType, baseVertex, minVertexIndex, numVertices, startIndex, primitiveCount, Indices.buffer, Indices.IndexElementSize);
	}

	public void DrawInstancedPrimitives(PrimitiveType primitiveType, int baseVertex, int minVertexIndex, int numVertices, int startIndex, int primitiveCount, int instanceCount)
	{
		if (FNA3D.FNA3D_SupportsHardwareInstancing(GLDevice) == 0)
		{
			throw new NoSuitableGraphicsDeviceException("Your hardware does not support hardware instancing!");
		}
		ApplyState();
		PrepareVertexBindingArray(baseVertex);
		FNA3D.FNA3D_DrawInstancedPrimitives(GLDevice, primitiveType, baseVertex, minVertexIndex, numVertices, startIndex, primitiveCount, instanceCount, Indices.buffer, Indices.IndexElementSize);
	}

	public void DrawPrimitives(PrimitiveType primitiveType, int vertexStart, int primitiveCount)
	{
		ApplyState();
		PrepareVertexBindingArray(0);
		FNA3D.FNA3D_DrawPrimitives(GLDevice, primitiveType, vertexStart, primitiveCount);
	}

	public void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, short[] indexData, int indexOffset, int primitiveCount) where T : struct, IVertexType
	{
		ApplyState();
		GCHandle gCHandle = GCHandle.Alloc(vertexData, GCHandleType.Pinned);
		GCHandle gCHandle2 = GCHandle.Alloc(indexData, GCHandleType.Pinned);
		PrepareUserVertexBuffer(gCHandle.AddrOfPinnedObject(), numVertices, vertexOffset, VertexDeclarationCache<T>.VertexDeclaration);
		PrepareUserIndexBuffer(gCHandle2.AddrOfPinnedObject(), PrimitiveVerts(primitiveType, primitiveCount), indexOffset, 2);
		gCHandle2.Free();
		gCHandle.Free();
		FNA3D.FNA3D_DrawIndexedPrimitives(GLDevice, primitiveType, 0, 0, numVertices, 0, primitiveCount, userIndexBuffer, IndexElementSize.SixteenBits);
	}

	public void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, short[] indexData, int indexOffset, int primitiveCount, VertexDeclaration vertexDeclaration) where T : struct
	{
		ApplyState();
		GCHandle gCHandle = GCHandle.Alloc(vertexData, GCHandleType.Pinned);
		GCHandle gCHandle2 = GCHandle.Alloc(indexData, GCHandleType.Pinned);
		PrepareUserVertexBuffer(gCHandle.AddrOfPinnedObject(), numVertices, vertexOffset, vertexDeclaration);
		PrepareUserIndexBuffer(gCHandle2.AddrOfPinnedObject(), PrimitiveVerts(primitiveType, primitiveCount), indexOffset, 2);
		gCHandle2.Free();
		gCHandle.Free();
		FNA3D.FNA3D_DrawIndexedPrimitives(GLDevice, primitiveType, 0, 0, numVertices, 0, primitiveCount, userIndexBuffer, IndexElementSize.SixteenBits);
	}

	public void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, int[] indexData, int indexOffset, int primitiveCount) where T : struct, IVertexType
	{
		ApplyState();
		GCHandle gCHandle = GCHandle.Alloc(vertexData, GCHandleType.Pinned);
		GCHandle gCHandle2 = GCHandle.Alloc(indexData, GCHandleType.Pinned);
		PrepareUserVertexBuffer(gCHandle.AddrOfPinnedObject(), numVertices, vertexOffset, VertexDeclarationCache<T>.VertexDeclaration);
		PrepareUserIndexBuffer(gCHandle2.AddrOfPinnedObject(), PrimitiveVerts(primitiveType, primitiveCount), indexOffset, 4);
		gCHandle2.Free();
		gCHandle.Free();
		FNA3D.FNA3D_DrawIndexedPrimitives(GLDevice, primitiveType, 0, 0, numVertices, 0, primitiveCount, userIndexBuffer, IndexElementSize.ThirtyTwoBits);
	}

	public void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, int[] indexData, int indexOffset, int primitiveCount, VertexDeclaration vertexDeclaration) where T : struct
	{
		ApplyState();
		GCHandle gCHandle = GCHandle.Alloc(vertexData, GCHandleType.Pinned);
		GCHandle gCHandle2 = GCHandle.Alloc(indexData, GCHandleType.Pinned);
		PrepareUserVertexBuffer(gCHandle.AddrOfPinnedObject(), numVertices, vertexOffset, vertexDeclaration);
		PrepareUserIndexBuffer(gCHandle2.AddrOfPinnedObject(), PrimitiveVerts(primitiveType, primitiveCount), indexOffset, 4);
		gCHandle2.Free();
		gCHandle.Free();
		FNA3D.FNA3D_DrawIndexedPrimitives(GLDevice, primitiveType, 0, 0, numVertices, 0, primitiveCount, userIndexBuffer, IndexElementSize.ThirtyTwoBits);
	}

	public void DrawUserPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int primitiveCount) where T : struct, IVertexType
	{
		ApplyState();
		GCHandle gCHandle = GCHandle.Alloc(vertexData, GCHandleType.Pinned);
		PrepareUserVertexBuffer(gCHandle.AddrOfPinnedObject(), PrimitiveVerts(primitiveType, primitiveCount), vertexOffset, VertexDeclarationCache<T>.VertexDeclaration);
		gCHandle.Free();
		FNA3D.FNA3D_DrawPrimitives(GLDevice, primitiveType, 0, primitiveCount);
	}

	public void DrawUserPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int primitiveCount, VertexDeclaration vertexDeclaration) where T : struct
	{
		ApplyState();
		GCHandle gCHandle = GCHandle.Alloc(vertexData, GCHandleType.Pinned);
		PrepareUserVertexBuffer(gCHandle.AddrOfPinnedObject(), PrimitiveVerts(primitiveType, primitiveCount), vertexOffset, vertexDeclaration);
		gCHandle.Free();
		FNA3D.FNA3D_DrawPrimitives(GLDevice, primitiveType, 0, primitiveCount);
	}

	public void SetStringMarkerEXT(string text)
	{
		FNA3D.FNA3D_SetStringMarker(GLDevice, text);
	}

	private void ApplyState()
	{
		if (currentBlend != nextBlend)
		{
			FNA3D.FNA3D_SetBlendState(GLDevice, ref nextBlend.state);
			currentBlend = nextBlend;
		}
		if (currentDepthStencil != nextDepthStencil)
		{
			FNA3D.FNA3D_SetDepthStencilState(GLDevice, ref nextDepthStencil.state);
			currentDepthStencil = nextDepthStencil;
		}
		FNA3D.FNA3D_ApplyRasterizerState(GLDevice, ref RasterizerState.state);
		ApplySamplers();
	}

	private void ApplySamplers()
	{
		for (int i = 0; i < modifiedSamplers.Length; i++)
		{
			if (modifiedSamplers[i])
			{
				modifiedSamplers[i] = false;
				FNA3D.FNA3D_VerifySampler(GLDevice, i, (Textures[i] != null) ? Textures[i].texture : IntPtr.Zero, ref SamplerStates[i].state);
			}
		}
		for (int j = 0; j < modifiedVertexSamplers.Length; j++)
		{
			if (modifiedVertexSamplers[j])
			{
				modifiedVertexSamplers[j] = false;
				FNA3D.FNA3D_VerifyVertexSampler(GLDevice, j, (VertexTextures[j] != null) ? VertexTextures[j].texture : IntPtr.Zero, ref VertexSamplerStates[j].state);
			}
		}
	}

	private unsafe void PrepareVertexBindingArray(int baseVertex)
	{
		fixed (FNA3D.FNA3D_VertexBufferBinding* ptr = &nativeBufferBindings[0])
		{
			for (int i = 0; i < vertexBufferCount; i++)
			{
				VertexBuffer vertexBuffer = vertexBufferBindings[i].VertexBuffer;
				ptr[i].vertexBuffer = vertexBuffer.buffer;
				ptr[i].vertexDeclaration.vertexStride = vertexBuffer.VertexDeclaration.VertexStride;
				ptr[i].vertexDeclaration.elementCount = vertexBuffer.VertexDeclaration.elements.Length;
				ptr[i].vertexDeclaration.elements = vertexBuffer.VertexDeclaration.elementsPin;
				ptr[i].vertexOffset = vertexBufferBindings[i].VertexOffset;
				ptr[i].instanceFrequency = vertexBufferBindings[i].InstanceFrequency;
			}
			FNA3D.FNA3D_ApplyVertexBufferBindings(GLDevice, ptr, vertexBufferCount, (byte)(vertexBuffersUpdated ? 1u : 0u), baseVertex);
		}
		vertexBuffersUpdated = false;
	}

	private unsafe void PrepareUserVertexBuffer(nint vertexData, int numVertices, int vertexOffset, VertexDeclaration vertexDeclaration)
	{
		int num = numVertices * vertexDeclaration.VertexStride;
		int num2 = vertexOffset * vertexDeclaration.VertexStride;
		vertexDeclaration.GraphicsDevice = this;
		if (num > userVertexBufferSize)
		{
			if (userVertexBuffer != IntPtr.Zero)
			{
				FNA3D.FNA3D_AddDisposeVertexBuffer(GLDevice, userVertexBuffer);
			}
			userVertexBuffer = FNA3D.FNA3D_GenVertexBuffer(GLDevice, 1, BufferUsage.WriteOnly, num);
			userVertexBufferSize = num;
		}
		FNA3D.FNA3D_SetVertexBufferData(GLDevice, userVertexBuffer, 0, vertexData + num2, num, 1, 1, SetDataOptions.Discard);
		fixed (FNA3D.FNA3D_VertexBufferBinding* ptr = &nativeBufferBindings[0])
		{
			ptr->vertexBuffer = userVertexBuffer;
			ptr->vertexDeclaration.vertexStride = vertexDeclaration.VertexStride;
			ptr->vertexDeclaration.elementCount = vertexDeclaration.elements.Length;
			ptr->vertexDeclaration.elements = vertexDeclaration.elementsPin;
			ptr->vertexOffset = 0;
			ptr->instanceFrequency = 0;
			FNA3D.FNA3D_ApplyVertexBufferBindings(GLDevice, ptr, 1, 1, 0);
		}
		vertexBuffersUpdated = true;
	}

	private void PrepareUserIndexBuffer(nint indexData, int numIndices, int indexOffset, int indexElementSizeInBytes)
	{
		int num = numIndices * indexElementSizeInBytes;
		if (num > userIndexBufferSize)
		{
			if (userIndexBuffer != IntPtr.Zero)
			{
				FNA3D.FNA3D_AddDisposeIndexBuffer(GLDevice, userIndexBuffer);
			}
			userIndexBuffer = FNA3D.FNA3D_GenIndexBuffer(GLDevice, 1, BufferUsage.WriteOnly, num);
			userIndexBufferSize = num;
		}
		FNA3D.FNA3D_SetIndexBufferData(GLDevice, userIndexBuffer, 0, indexData + indexOffset * indexElementSizeInBytes, num, SetDataOptions.Discard);
	}

	internal unsafe static void PrepareRenderTargetBindings(FNA3D.FNA3D_RenderTargetBinding* b, RenderTargetBinding[] bindings)
	{
		int num = 0;
		while (num < bindings.Length)
		{
			Texture renderTarget = bindings[num].RenderTarget;
			IRenderTarget renderTarget2 = renderTarget as IRenderTarget;
			if (renderTarget is RenderTargetCube)
			{
				b->type = 1;
				b->data1 = renderTarget2.Width;
				b->data2 = (int)bindings[num].CubeMapFace;
			}
			else
			{
				b->type = 0;
				b->data1 = renderTarget2.Width;
				b->data2 = renderTarget2.Height;
			}
			b->levelCount = renderTarget2.LevelCount;
			b->multiSampleCount = renderTarget2.MultiSampleCount;
			b->texture = renderTarget.texture;
			b->colorBuffer = renderTarget2.ColorBuffer;
			num++;
			b++;
		}
	}

	private static int PrimitiveVerts(PrimitiveType primitiveType, int primitiveCount)
	{
		return primitiveType switch
		{
			PrimitiveType.TriangleList => primitiveCount * 3, 
			PrimitiveType.TriangleStrip => primitiveCount + 2, 
			PrimitiveType.LineList => primitiveCount * 2, 
			PrimitiveType.LineStrip => primitiveCount + 1, 
			PrimitiveType.PointListEXT => primitiveCount, 
			_ => throw new InvalidOperationException("Unrecognized primitive type!"), 
		};
	}
}

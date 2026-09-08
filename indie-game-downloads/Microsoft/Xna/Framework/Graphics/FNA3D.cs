using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using ObjCRuntime;
using SDL2;

namespace Microsoft.Xna.Framework.Graphics;

[SuppressUnmanagedCodeSecurity]
internal static class FNA3D
{
	public struct FNA3D_Viewport
	{
		public int x;

		public int y;

		public int w;

		public int h;

		public float minDepth;

		public float maxDepth;
	}

	public struct FNA3D_BlendState
	{
		public Blend colorSourceBlend;

		public Blend colorDestinationBlend;

		public BlendFunction colorBlendFunction;

		public Blend alphaSourceBlend;

		public Blend alphaDestinationBlend;

		public BlendFunction alphaBlendFunction;

		public ColorWriteChannels colorWriteEnable;

		public ColorWriteChannels colorWriteEnable1;

		public ColorWriteChannels colorWriteEnable2;

		public ColorWriteChannels colorWriteEnable3;

		public Color blendFactor;

		public int multiSampleMask;
	}

	public struct FNA3D_DepthStencilState
	{
		public byte depthBufferEnable;

		public byte depthBufferWriteEnable;

		public CompareFunction depthBufferFunction;

		public byte stencilEnable;

		public int stencilMask;

		public int stencilWriteMask;

		public byte twoSidedStencilMode;

		public StencilOperation stencilFail;

		public StencilOperation stencilDepthBufferFail;

		public StencilOperation stencilPass;

		public CompareFunction stencilFunction;

		public StencilOperation ccwStencilFail;

		public StencilOperation ccwStencilDepthBufferFail;

		public StencilOperation ccwStencilPass;

		public CompareFunction ccwStencilFunction;

		public int referenceStencil;
	}

	public struct FNA3D_RasterizerState
	{
		public FillMode fillMode;

		public CullMode cullMode;

		public float depthBias;

		public float slopeScaleDepthBias;

		public byte scissorTestEnable;

		public byte multiSampleAntiAlias;
	}

	public struct FNA3D_SamplerState
	{
		public TextureFilter filter;

		public TextureAddressMode addressU;

		public TextureAddressMode addressV;

		public TextureAddressMode addressW;

		public float mipMapLevelOfDetailBias;

		public int maxAnisotropy;

		public int maxMipLevel;
	}

	public struct FNA3D_VertexDeclaration
	{
		public int vertexStride;

		public int elementCount;

		public nint elements;
	}

	public struct FNA3D_VertexBufferBinding
	{
		public nint vertexBuffer;

		public FNA3D_VertexDeclaration vertexDeclaration;

		public int vertexOffset;

		public int instanceFrequency;
	}

	public struct FNA3D_RenderTargetBinding
	{
		public byte type;

		public int data1;

		public int data2;

		public int levelCount;

		public int multiSampleCount;

		public nint texture;

		public nint colorBuffer;
	}

	public struct FNA3D_PresentationParameters
	{
		public int backBufferWidth;

		public int backBufferHeight;

		public SurfaceFormat backBufferFormat;

		public int multiSampleCount;

		public nint deviceWindowHandle;

		public byte isFullScreen;

		public DepthFormat depthStencilFormat;

		public PresentInterval presentationInterval;

		public DisplayOrientation displayOrientation;

		public RenderTargetUsage renderTargetUsage;
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void FNA3D_LogFunc(nint msg);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int FNA3D_Image_ReadFunc(nint context, nint data, int size);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate void FNA3D_Image_SkipFunc(nint context, int n);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int FNA3D_Image_EOFFunc(nint context);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate void FNA3D_Image_WriteFunc(nint context, nint data, int size);

	private const string nativeLibName = "FNA3D";

	private static FNA3D_Image_ReadFunc readFunc = INTERNAL_Read;

	private static FNA3D_Image_SkipFunc skipFunc = INTERNAL_Skip;

	private static FNA3D_Image_EOFFunc eofFunc = INTERNAL_EOF;

	private static int readGlobal = 0;

	private static Dictionary<nint, Stream> readStreams = new Dictionary<nint, Stream>();

	private static FNA3D_Image_WriteFunc writeFunc = INTERNAL_Write;

	private static int writeGlobal = 0;

	private static Dictionary<nint, Stream> writeStreams = new Dictionary<nint, Stream>();

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern uint FNA3D_LinkedVersion();

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_HookLogFunctions(FNA3D_LogFunc info, FNA3D_LogFunc warn, FNA3D_LogFunc error);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern uint FNA3D_PrepareWindowAttributes();

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_GetDrawableSize(nint window, out int w, out int h);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern nint FNA3D_CreateDevice(ref FNA3D_PresentationParameters presentationParameters, byte debugMode);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_DestroyDevice(nint device);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_SwapBuffers(nint device, ref Rectangle sourceRectangle, ref Rectangle destinationRectangle, nint overrideWindowHandle);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_SwapBuffers(nint device, nint sourceRectangle, nint destinationRectangle, nint overrideWindowHandle);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_SwapBuffers(nint device, ref Rectangle sourceRectangle, nint destinationRectangle, nint overrideWindowHandle);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_SwapBuffers(nint device, nint sourceRectangle, ref Rectangle destinationRectangle, nint overrideWindowHandle);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_Clear(nint device, ClearOptions options, ref Vector4 color, float depth, int stencil);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_DrawIndexedPrimitives(nint device, PrimitiveType primitiveType, int baseVertex, int minVertexIndex, int numVertices, int startIndex, int primitiveCount, nint indices, IndexElementSize indexElementSize);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_DrawInstancedPrimitives(nint device, PrimitiveType primitiveType, int baseVertex, int minVertexIndex, int numVertices, int startIndex, int primitiveCount, int instanceCount, nint indices, IndexElementSize indexElementSize);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_DrawPrimitives(nint device, PrimitiveType primitiveType, int vertexStart, int primitiveCount);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_SetViewport(nint device, ref FNA3D_Viewport viewport);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_SetScissorRect(nint device, ref Rectangle scissor);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_GetBlendFactor(nint device, out Color blendFactor);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_SetBlendFactor(nint device, ref Color blendFactor);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern int FNA3D_GetMultiSampleMask(nint device);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_SetMultiSampleMask(nint device, int mask);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern int FNA3D_GetReferenceStencil(nint device);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_SetReferenceStencil(nint device, int reference);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_SetBlendState(nint device, ref FNA3D_BlendState blendState);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_SetDepthStencilState(nint device, ref FNA3D_DepthStencilState depthStencilState);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_ApplyRasterizerState(nint device, ref FNA3D_RasterizerState rasterizerState);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_VerifySampler(nint device, int index, nint texture, ref FNA3D_SamplerState sampler);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_VerifyVertexSampler(nint device, int index, nint texture, ref FNA3D_SamplerState sampler);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public unsafe static extern void FNA3D_ApplyVertexBufferBindings(nint device, FNA3D_VertexBufferBinding* bindings, int numBindings, byte bindingsUpdated, int baseVertex);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_SetRenderTargets(nint device, nint renderTargets, int numRenderTargets, nint depthStencilBuffer, DepthFormat depthFormat, byte preserveDepthStencilContents);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public unsafe static extern void FNA3D_SetRenderTargets(nint device, FNA3D_RenderTargetBinding* renderTargets, int numRenderTargets, nint depthStencilBuffer, DepthFormat depthFormat, byte preserveDepthStencilContents);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_ResolveTarget(nint device, ref FNA3D_RenderTargetBinding target);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_ResetBackbuffer(nint device, ref FNA3D_PresentationParameters presentationParameters);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_ReadBackbuffer(nint device, int x, int y, int w, int h, nint data, int dataLen);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_GetBackbufferSize(nint device, out int w, out int h);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern SurfaceFormat FNA3D_GetBackbufferSurfaceFormat(nint device);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern DepthFormat FNA3D_GetBackbufferDepthFormat(nint device);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern int FNA3D_GetBackbufferMultiSampleCount(nint device);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern nint FNA3D_CreateTexture2D(nint device, SurfaceFormat format, int width, int height, int levelCount, byte isRenderTarget);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern nint FNA3D_CreateTexture3D(nint device, SurfaceFormat format, int width, int height, int depth, int levelCount);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern nint FNA3D_CreateTextureCube(nint device, SurfaceFormat format, int size, int levelCount, byte isRenderTarget);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_AddDisposeTexture(nint device, nint texture);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_SetTextureData2D(nint device, nint texture, int x, int y, int w, int h, int level, nint data, int dataLength);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_SetTextureData3D(nint device, nint texture, int x, int y, int z, int w, int h, int d, int level, nint data, int dataLength);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_SetTextureDataCube(nint device, nint texture, int x, int y, int w, int h, CubeMapFace cubeMapFace, int level, nint data, int dataLength);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_SetTextureDataYUV(nint device, nint y, nint u, nint v, int yWidth, int yHeight, int uvWidth, int uvHeight, nint data, int dataLength);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_GetTextureData2D(nint device, nint texture, int x, int y, int w, int h, int level, nint data, int dataLength);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_GetTextureData3D(nint device, nint texture, int x, int y, int z, int w, int h, int d, int level, nint data, int dataLength);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_GetTextureDataCube(nint device, nint texture, int x, int y, int w, int h, CubeMapFace cubeMapFace, int level, nint data, int dataLength);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern nint FNA3D_GenColorRenderbuffer(nint device, int width, int height, SurfaceFormat format, int multiSampleCount, nint texture);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern nint FNA3D_GenDepthStencilRenderbuffer(nint device, int width, int height, DepthFormat format, int multiSampleCount);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_AddDisposeRenderbuffer(nint device, nint renderbuffer);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern nint FNA3D_GenVertexBuffer(nint device, byte dynamic, BufferUsage usage, int sizeInBytes);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_AddDisposeVertexBuffer(nint device, nint buffer);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_SetVertexBufferData(nint device, nint buffer, int offsetInBytes, nint data, int elementCount, int elementSizeInBytes, int vertexStride, SetDataOptions options);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_GetVertexBufferData(nint device, nint buffer, int offsetInBytes, nint data, int elementCount, int elementSizeInBytes, int vertexStride);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern nint FNA3D_GenIndexBuffer(nint device, byte dynamic, BufferUsage usage, int sizeInBytes);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_AddDisposeIndexBuffer(nint device, nint buffer);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_SetIndexBufferData(nint device, nint buffer, int offsetInBytes, nint data, int dataLength, SetDataOptions options);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_GetIndexBufferData(nint device, nint buffer, int offsetInBytes, nint data, int dataLength);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_CreateEffect(nint device, byte[] effectCode, int length, out nint effect, out nint effectData);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_CloneEffect(nint device, nint cloneSource, out nint effect, out nint effectData);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_AddDisposeEffect(nint device, nint effect);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_SetEffectTechnique(nint device, nint effect, nint technique);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_ApplyEffect(nint device, nint effect, uint pass, nint stateChanges);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_BeginPassRestore(nint device, nint effect, nint stateChanges);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_EndPassRestore(nint device, nint effect);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern nint FNA3D_CreateQuery(nint device);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_AddDisposeQuery(nint device, nint query);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_QueryBegin(nint device, nint query);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_QueryEnd(nint device, nint query);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern byte FNA3D_QueryComplete(nint device, nint query);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern int FNA3D_QueryPixelCount(nint device, nint query);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern byte FNA3D_SupportsDXT1(nint device);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern byte FNA3D_SupportsS3TC(nint device);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern byte FNA3D_SupportsBC7(nint device);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern byte FNA3D_SupportsHardwareInstancing(nint device);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern byte FNA3D_SupportsNoOverwrite(nint device);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern byte FNA3D_SupportsSRGBRenderTargets(nint device);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_GetMaxTextureSlots(nint device, out int textures, out int vertexTextures);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern int FNA3D_GetMaxMultiSampleCount(nint device, SurfaceFormat format, int preferredMultiSampleCount);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	private unsafe static extern void FNA3D_SetStringMarker(nint device, byte* text);

	public unsafe static void FNA3D_SetStringMarker(nint device, string text)
	{
		byte* ptr = SDL.Utf8EncodeHeap(text);
		FNA3D_SetStringMarker(device, ptr);
		Marshal.FreeHGlobal((nint)ptr);
	}

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	private unsafe static extern void FNA3D_SetTextureName(nint device, nint texture, byte* text);

	public unsafe static void FNA3D_SetTextureName(nint device, nint texture, string text)
	{
		byte* ptr = SDL.Utf8EncodeHeap(text);
		FNA3D_SetTextureName(device, texture, ptr);
		Marshal.FreeHGlobal((nint)ptr);
	}

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	private static extern nint FNA3D_Image_Load(FNA3D_Image_ReadFunc readFunc, FNA3D_Image_SkipFunc skipFunc, FNA3D_Image_EOFFunc eofFunc, nint context, out int width, out int height, out int len, int forceW, int forceH, byte zoom);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FNA3D_Image_Free(nint mem);

	[MonoPInvokeCallback(typeof(FNA3D_Image_ReadFunc))]
	private unsafe static int INTERNAL_Read(nint context, nint data, int size)
	{
		Stream stream;
		lock (readStreams)
		{
			stream = readStreams[context];
		}
		return stream.Read(new Span<byte>(((IntPtr)data).ToPointer(), size));
	}

	[MonoPInvokeCallback(typeof(FNA3D_Image_SkipFunc))]
	private static void INTERNAL_Skip(nint context, int n)
	{
		Stream stream;
		lock (readStreams)
		{
			stream = readStreams[context];
		}
		stream.Seek(n, SeekOrigin.Current);
	}

	[MonoPInvokeCallback(typeof(FNA3D_Image_EOFFunc))]
	private static int INTERNAL_EOF(nint context)
	{
		Stream stream;
		lock (readStreams)
		{
			stream = readStreams[context];
		}
		return (stream.Position == stream.Length) ? 1 : 0;
	}

	public static nint ReadImageStream(Stream stream, out int width, out int height, out int len, int forceW = -1, int forceH = -1, bool zoom = false)
	{
		nint num;
		lock (readStreams)
		{
			num = readGlobal++;
			readStreams.Add(num, stream);
		}
		nint result = FNA3D_Image_Load(readFunc, skipFunc, eofFunc, num, out width, out height, out len, forceW, forceH, (byte)(zoom ? 1u : 0u));
		lock (readStreams)
		{
			readStreams.Remove(num);
		}
		return result;
	}

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	private static extern void FNA3D_Image_SavePNG(FNA3D_Image_WriteFunc writeFunc, nint context, int srcW, int srcH, int dstW, int dstH, nint data);

	[DllImport("FNA3D", CallingConvention = CallingConvention.Cdecl)]
	private static extern void FNA3D_Image_SaveJPG(FNA3D_Image_WriteFunc writeFunc, nint context, int srcW, int srcH, int dstW, int dstH, nint data, int quality);

	[MonoPInvokeCallback(typeof(FNA3D_Image_WriteFunc))]
	private unsafe static void INTERNAL_Write(nint context, nint data, int size)
	{
		Stream stream;
		lock (writeStreams)
		{
			stream = writeStreams[context];
		}
		stream.Write(new ReadOnlySpan<byte>(((IntPtr)data).ToPointer(), size));
	}

	public static void WritePNGStream(Stream stream, int srcW, int srcH, int dstW, int dstH, nint data)
	{
		nint num;
		lock (writeStreams)
		{
			num = writeGlobal++;
			writeStreams.Add(num, stream);
		}
		FNA3D_Image_SavePNG(writeFunc, num, srcW, srcH, dstW, dstH, data);
		lock (writeStreams)
		{
			writeStreams.Remove(num);
		}
	}

	public static void WriteJPGStream(Stream stream, int srcW, int srcH, int dstW, int dstH, nint data, int quality)
	{
		nint num;
		lock (writeStreams)
		{
			num = writeGlobal++;
			writeStreams.Add(num, stream);
		}
		FNA3D_Image_SaveJPG(writeFunc, num, srcW, srcH, dstW, dstH, data, quality);
		lock (writeStreams)
		{
			writeStreams.Remove(num);
		}
	}
}

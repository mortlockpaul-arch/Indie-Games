#define DEBUG
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.Xna.Framework.Graphics;

public class IndexBuffer : GraphicsResource
{
	internal nint buffer;

	public BufferUsage BufferUsage { get; private set; }

	public int IndexCount { get; private set; }

	public IndexElementSize IndexElementSize { get; private set; }

	public IndexBuffer(GraphicsDevice graphicsDevice, IndexElementSize indexElementSize, int indexCount, BufferUsage bufferUsage)
		: this(graphicsDevice, indexElementSize, indexCount, bufferUsage, dynamic: false)
	{
	}

	public IndexBuffer(GraphicsDevice graphicsDevice, Type indexType, int indexCount, BufferUsage usage)
		: this(graphicsDevice, SizeForType(graphicsDevice, indexType), indexCount, usage, dynamic: false)
	{
	}

	protected IndexBuffer(GraphicsDevice graphicsDevice, Type indexType, int indexCount, BufferUsage usage, bool dynamic)
		: this(graphicsDevice, SizeForType(graphicsDevice, indexType), indexCount, usage, dynamic)
	{
	}

	protected IndexBuffer(GraphicsDevice graphicsDevice, IndexElementSize indexElementSize, int indexCount, BufferUsage usage, bool dynamic)
	{
		if (graphicsDevice == null)
		{
			throw new ArgumentNullException("graphicsDevice");
		}
		base.GraphicsDevice = graphicsDevice;
		IndexElementSize = indexElementSize;
		IndexCount = indexCount;
		BufferUsage = usage;
		int num = ((indexElementSize == IndexElementSize.ThirtyTwoBits) ? 4 : 2);
		buffer = FNA3D.FNA3D_GenIndexBuffer(base.GraphicsDevice.GLDevice, (byte)(dynamic ? 1u : 0u), usage, IndexCount * num);
	}

	protected override void Dispose(bool disposing)
	{
		if (!base.IsDisposed)
		{
			nint num = Interlocked.Exchange(ref buffer, IntPtr.Zero);
			if (num != IntPtr.Zero)
			{
				FNA3D.FNA3D_AddDisposeIndexBuffer(base.GraphicsDevice.GLDevice, num);
			}
		}
		base.Dispose(disposing);
	}

	public void GetData<T>(T[] data) where T : struct
	{
		GetData(0, data, 0, data.Length);
	}

	public void GetData<T>(T[] data, int startIndex, int elementCount) where T : struct
	{
		GetData(0, data, startIndex, elementCount);
	}

	public void GetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount) where T : struct
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (data.Length < startIndex + elementCount)
		{
			throw new InvalidOperationException("The array specified in the data parameter is not the correct size for the amount of data requested.");
		}
		if (BufferUsage == BufferUsage.WriteOnly)
		{
			throw new NotSupportedException("This IndexBuffer was created with a usage type of BufferUsage.WriteOnly. Calling GetData on a resource that was created with BufferUsage.WriteOnly is not supported.");
		}
		int num = MarshalHelper.SizeOf<T>();
		GCHandle gCHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
		FNA3D.FNA3D_GetIndexBufferData(base.GraphicsDevice.GLDevice, buffer, offsetInBytes, gCHandle.AddrOfPinnedObject() + startIndex * num, elementCount * num);
		gCHandle.Free();
	}

	public void SetData<T>(T[] data) where T : struct
	{
		GCHandle gCHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
		FNA3D.FNA3D_SetIndexBufferData(base.GraphicsDevice.GLDevice, buffer, 0, gCHandle.AddrOfPinnedObject(), data.Length * MarshalHelper.SizeOf<T>(), SetDataOptions.None);
		gCHandle.Free();
	}

	public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct
	{
		ErrorCheck(data, startIndex, elementCount);
		GCHandle gCHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
		FNA3D.FNA3D_SetIndexBufferData(base.GraphicsDevice.GLDevice, buffer, 0, gCHandle.AddrOfPinnedObject() + startIndex * MarshalHelper.SizeOf<T>(), elementCount * MarshalHelper.SizeOf<T>(), SetDataOptions.None);
		gCHandle.Free();
	}

	public void SetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount) where T : struct
	{
		ErrorCheck(data, startIndex, elementCount);
		GCHandle gCHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
		FNA3D.FNA3D_SetIndexBufferData(base.GraphicsDevice.GLDevice, buffer, offsetInBytes, gCHandle.AddrOfPinnedObject() + startIndex * MarshalHelper.SizeOf<T>(), elementCount * MarshalHelper.SizeOf<T>(), SetDataOptions.None);
		gCHandle.Free();
	}

	public void SetDataPointerEXT(int offsetInBytes, nint data, int dataLength, SetDataOptions options)
	{
		FNA3D.FNA3D_SetIndexBufferData(base.GraphicsDevice.GLDevice, buffer, offsetInBytes, data, dataLength, options);
	}

	[Conditional("DEBUG")]
	internal void ErrorCheck<T>(T[] data, int startIndex, int elementCount) where T : struct
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (data.Length < startIndex + elementCount)
		{
			throw new InvalidOperationException("The array specified in the data parameter is not the correct size for the amount of data requested.");
		}
	}

	protected internal override void GraphicsDeviceResetting()
	{
	}

	private static IndexElementSize SizeForType(GraphicsDevice graphicsDevice, Type type)
	{
		return Marshal.SizeOf(type) switch
		{
			2 => IndexElementSize.SixteenBits, 
			4 => IndexElementSize.ThirtyTwoBits, 
			_ => throw new ArgumentOutOfRangeException("type", "Index buffers can only be created for types that are sixteen or thirty two bits in length"), 
		};
	}
}

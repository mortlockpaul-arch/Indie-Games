using System.Buffers;

namespace System.IO.MemoryMappedFiles;

internal sealed class MemoryMappedFileMemoryManager : MemoryManager<byte>
{
	private unsafe byte* _pointer;

	private int _length;

	private MemoryMappedFile _mappedFile;

	private MemoryMappedViewAccessor _accessor;

	public unsafe MemoryMappedFileMemoryManager(byte* pointer, int length, MemoryMappedFile mappedFile, MemoryMappedViewAccessor accessor)
	{
		_pointer = pointer;
		_length = length;
		_mappedFile = mappedFile;
		_accessor = accessor;
	}

	internal unsafe static MemoryMappedFileMemoryManager CreateFromFileClamped(FileStream fileStream, MemoryMappedFileAccess access = MemoryMappedFileAccess.Read, HandleInheritability inheritability = HandleInheritability.None, bool leaveOpen = false)
	{
		int num = (int)Math.Min(2147483647L, fileStream.Length);
		MemoryMappedFile memoryMappedFile = MemoryMappedFile.CreateFromFile(fileStream, null, 0L, access, inheritability, leaveOpen);
		MemoryMappedViewAccessor memoryMappedViewAccessor = null;
		byte* pointer = null;
		try
		{
			memoryMappedViewAccessor = memoryMappedFile.CreateViewAccessor(0L, num, access);
			memoryMappedViewAccessor.SafeMemoryMappedViewHandle.AcquirePointer(ref pointer);
			return new MemoryMappedFileMemoryManager(pointer, num, memoryMappedFile, memoryMappedViewAccessor);
		}
		catch (Exception)
		{
			if (pointer != null)
			{
				memoryMappedViewAccessor.SafeMemoryMappedViewHandle.ReleasePointer();
			}
			memoryMappedViewAccessor?.Dispose();
			memoryMappedFile.Dispose();
			throw;
		}
	}

	protected unsafe override void Dispose(bool disposing)
	{
		_pointer = null;
		_length = -1;
		_accessor?.SafeMemoryMappedViewHandle.ReleasePointer();
		_accessor?.Dispose();
		_mappedFile?.Dispose();
		_accessor = null;
		_mappedFile = null;
	}

	public unsafe override Span<byte> GetSpan()
	{
		ThrowIfDisposed();
		return new Span<byte>(_pointer, _length);
	}

	public override MemoryHandle Pin(int elementIndex = 0)
	{
		ThrowIfDisposed();
		return default(MemoryHandle);
	}

	public override void Unpin()
	{
		ThrowIfDisposed();
	}

	private void ThrowIfDisposed()
	{
		ObjectDisposedException.ThrowIf(_length < 0, this);
	}
}

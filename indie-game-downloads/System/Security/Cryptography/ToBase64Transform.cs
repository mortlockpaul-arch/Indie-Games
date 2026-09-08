using System.Buffers.Text;

namespace System.Security.Cryptography;

public class ToBase64Transform : ICryptoTransform, IDisposable
{
	public int InputBlockSize => 3;

	public int OutputBlockSize => 4;

	public bool CanTransformMultipleBlocks => true;

	public virtual bool CanReuseTransform => true;

	public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
	{
		ThrowHelper.ValidateTransformBlock(inputBuffer, inputOffset, inputCount);
		int num = Math.DivRem(inputCount, InputBlockSize, out var result);
		if (num == 0)
		{
			ThrowHelper.ThrowArgumentOutOfRange(ThrowHelper.ExceptionArgument.inputCount);
		}
		if (outputBuffer == null)
		{
			ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.outputBuffer);
		}
		if (result != 0)
		{
			ThrowHelper.ThrowArgumentOutOfRange(ThrowHelper.ExceptionArgument.inputCount);
		}
		int num2 = checked(num * OutputBlockSize);
		if (num2 > outputBuffer.Length - outputOffset)
		{
			ThrowHelper.ThrowArgumentOutOfRange(ThrowHelper.ExceptionArgument.outputBuffer);
		}
		ReadOnlySpan<byte> bytes = new ReadOnlySpan<byte>(inputBuffer, inputOffset, inputCount);
		Span<byte> utf = outputBuffer.AsSpan(outputOffset, num2);
		Base64.EncodeToUtf8(bytes, utf, out var _, out var bytesWritten, isFinalBlock: false);
		return bytesWritten;
	}

	public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
	{
		ThrowHelper.ValidateTransformBlock(inputBuffer, inputOffset, inputCount);
		if (inputCount == 0)
		{
			return Array.Empty<byte>();
		}
		ReadOnlySpan<byte> bytes = new ReadOnlySpan<byte>(inputBuffer, inputOffset, inputCount);
		byte[] array = new byte[(Math.DivRem(inputCount, InputBlockSize, out var result) + ((result != 0) ? 1 : 0)) * OutputBlockSize];
		Base64.EncodeToUtf8(bytes, array, out var _, out var _);
		return array;
	}

	public void Dispose()
	{
		Clear();
	}

	public void Clear()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
	}

	~ToBase64Transform()
	{
		Dispose(disposing: false);
	}
}

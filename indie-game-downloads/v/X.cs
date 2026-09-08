using System;
using System.IO;

namespace v;

internal abstract class X : B, IDisposable
{
	protected byte[] HashValue;

	protected int HashSizeValue;

	protected int State;

	private bool a5h;

	public virtual bool CanTransformMultipleBlocks => true;

	public virtual bool CanReuseTransform => true;

	public virtual byte[] Hash
	{
		get
		{
			if (HashValue == null)
			{
				throw new v("No hash value computed.");
			}
			return HashValue;
		}
	}

	public virtual int HashSize => HashSizeValue;

	public virtual int InputBlockSize => 1;

	public virtual int OutputBlockSize => 1;

	protected X()
	{
		a5h = false;
	}

	public void Clear()
	{
		Dispose(disposing: true);
	}

	public byte[] ComputeHash(byte[] input)
	{
		if (input == null)
		{
			throw new ArgumentNullException("input");
		}
		return ComputeHash(input, 0, input.Length);
	}

	public byte[] ComputeHash(byte[] buffer, int offset, int count)
	{
		if (a5h)
		{
			throw new ObjectDisposedException("HashAlgorithm");
		}
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		if (offset < 0)
		{
			throw new ArgumentOutOfRangeException("offset", "< 0");
		}
		if (count < 0)
		{
			throw new ArgumentException("count", "< 0");
		}
		if (offset > buffer.Length - count)
		{
			throw new ArgumentException("offset + count", "Overflow");
		}
		HashCore(buffer, offset, count);
		HashValue = HashFinal();
		Initialize();
		return HashValue;
	}

	public byte[] ComputeHash(Stream inputStream)
	{
		if (a5h)
		{
			throw new ObjectDisposedException("HashAlgorithm");
		}
		byte[] array = new byte[4096];
		for (int num = inputStream.Read(array, 0, 4096); num > 0; num = inputStream.Read(array, 0, 4096))
		{
			HashCore(array, 0, num);
		}
		HashValue = HashFinal();
		Initialize();
		return HashValue;
	}

	public static X Create()
	{
		throw new Exception("HashAlgorithm.Create not supported.");
	}

	protected abstract void HashCore(byte[] rgb, int start, int size);

	protected abstract byte[] HashFinal();

	public abstract void Initialize();

	protected virtual void Dispose(bool disposing)
	{
		a5h = true;
	}

	void IDisposable.Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
	{
		if (inputBuffer == null)
		{
			throw new ArgumentNullException("inputBuffer");
		}
		if (inputOffset < 0)
		{
			throw new ArgumentOutOfRangeException("inputOffset", "< 0");
		}
		if (inputCount < 0)
		{
			throw new ArgumentException("inputCount");
		}
		if (inputOffset < 0 || inputOffset > inputBuffer.Length - inputCount)
		{
			throw new ArgumentException("inputBuffer");
		}
		if (outputBuffer != null)
		{
			if (outputOffset < 0)
			{
				throw new IndexOutOfRangeException("outputBuffer");
			}
			if (outputOffset > outputBuffer.Length - inputCount)
			{
				throw new IndexOutOfRangeException("outputBuffer");
			}
		}
		HashCore(inputBuffer, inputOffset, inputCount);
		if (outputBuffer != null)
		{
			Buffer.BlockCopy(inputBuffer, inputOffset, outputBuffer, outputOffset, inputCount);
		}
		return inputCount;
	}

	public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
	{
		if (inputBuffer == null)
		{
			throw new ArgumentNullException("inputBuffer");
		}
		if (inputCount < 0)
		{
			throw new ArgumentException("inputCount");
		}
		if (inputOffset > inputBuffer.Length - inputCount)
		{
			throw new ArgumentException("inputOffset + inputCount", "Overflow");
		}
		byte[] array = new byte[inputCount];
		Buffer.BlockCopy(inputBuffer, inputOffset, array, 0, inputCount);
		HashCore(inputBuffer, inputOffset, inputCount);
		HashValue = HashFinal();
		Initialize();
		return array;
	}
}

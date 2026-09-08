using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Mime;

internal sealed class EightBitStream : DelegatedStream, IEncodableStream
{
	private readonly bool _shouldEncodeLeadingDots;

	private bool _lastWriteEndedWithCr;

	[CompilerGenerated]
	private WriteStateInfoBase _003CWriteState_003Ek__BackingField;

	private WriteStateInfoBase WriteState => _003CWriteState_003Ek__BackingField ?? (_003CWriteState_003Ek__BackingField = new WriteStateInfoBase());

	public override bool CanRead => false;

	public override bool CanWrite => base.BaseStream.CanWrite;

	internal EightBitStream(Stream stream)
		: base(stream)
	{
	}

	internal EightBitStream(Stream stream, bool shouldEncodeLeadingDots)
		: this(stream)
	{
		_shouldEncodeLeadingDots = shouldEncodeLeadingDots;
	}

	protected override int ReadInternal(Span<byte> buffer)
	{
		throw new NotImplementedException();
	}

	protected override ValueTask<int> ReadAsyncInternal(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
	{
		throw new NotImplementedException();
	}

	protected override void WriteInternal(ReadOnlySpan<byte> buffer)
	{
		if (_shouldEncodeLeadingDots)
		{
			EncodeLines(buffer);
			base.BaseStream.Write(WriteState.Buffer.AsSpan(0, WriteState.Length));
			WriteState.BufferFlushed();
		}
		else
		{
			base.BaseStream.Write(buffer);
		}
	}

	protected override ValueTask WriteAsyncInternal(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (_shouldEncodeLeadingDots)
		{
			EncodeLines(buffer.Span);
			ValueTask result = base.BaseStream.WriteAsync(WriteState.Buffer.AsMemory(0, WriteState.Length), cancellationToken);
			WriteState.BufferFlushed();
			return result;
		}
		return base.BaseStream.WriteAsync(buffer, cancellationToken);
	}

	private void EncodeLines(ReadOnlySpan<byte> buffer)
	{
		for (int i = 0; i < buffer.Length; i++)
		{
			if (_lastWriteEndedWithCr)
			{
				_lastWriteEndedWithCr = false;
				if (buffer[i] == 10)
				{
					WriteState.AppendCRLF(includeSpace: false);
					continue;
				}
				WriteState.Append(13);
			}
			if (WriteState.CurrentLineLength == 0 && buffer[i] == 46)
			{
				WriteState.Append(46);
			}
			if (buffer[i] == 13)
			{
				_lastWriteEndedWithCr = true;
			}
			else
			{
				WriteState.Append(buffer[i]);
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		try
		{
			if (disposing && _lastWriteEndedWithCr)
			{
				_lastWriteEndedWithCr = false;
				base.BaseStream.WriteByte(13);
			}
		}
		finally
		{
			base.Dispose(disposing);
		}
	}

	public override async ValueTask DisposeAsync()
	{
		if (_lastWriteEndedWithCr)
		{
			_lastWriteEndedWithCr = false;
			await base.BaseStream.WriteAsync(new byte[1] { 13 }, CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	public int DecodeBytes(Span<byte> buffer)
	{
		throw new NotImplementedException();
	}

	public int EncodeBytes(ReadOnlySpan<byte> buffer)
	{
		throw new NotImplementedException();
	}

	public int EncodeString(string value, Encoding encoding)
	{
		throw new NotImplementedException();
	}

	public string GetEncodedString()
	{
		throw new NotImplementedException();
	}
}

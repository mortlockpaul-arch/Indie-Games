using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Mime;

internal sealed class QEncodedStream : DelegatedStream, IEncodableStream
{
	private sealed class ReadStateInfo
	{
		internal bool IsEscaped { get; set; }

		internal short Byte { get; set; } = -1;
	}

	private readonly WriteStateInfoBase _writeState;

	private readonly QEncoder _encoder;

	[CompilerGenerated]
	private ReadStateInfo _003CReadState_003Ek__BackingField;

	private static ReadOnlySpan<byte> HexDecodeMap => new byte[256]
	{
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 0, 1,
		2, 3, 4, 5, 6, 7, 8, 9, 255, 255,
		255, 255, 255, 255, 255, 10, 11, 12, 13, 14,
		15, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 10, 11, 12,
		13, 14, 15, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255
	};

	private ReadStateInfo ReadState => _003CReadState_003Ek__BackingField ?? (_003CReadState_003Ek__BackingField = new ReadStateInfo());

	internal WriteStateInfoBase WriteState => _writeState;

	public override bool CanRead => base.BaseStream.CanRead;

	public override bool CanWrite => base.BaseStream.CanWrite;

	internal QEncodedStream(WriteStateInfoBase wsi)
		: base(new MemoryStream())
	{
		_writeState = wsi;
		_encoder = new QEncoder(_writeState);
	}

	public override void Close()
	{
		FlushInternal();
		base.Close();
	}

	public unsafe int DecodeBytes(Span<byte> buffer)
	{
		fixed (byte* ptr = buffer)
		{
			byte* ptr2 = ptr;
			byte* ptr3 = ptr;
			byte* ptr4 = ptr + buffer.Length;
			if (ReadState.IsEscaped)
			{
				if (ReadState.Byte == -1)
				{
					if (buffer.Length == 1)
					{
						ReadState.Byte = *ptr2;
						return 0;
					}
					if (*ptr2 != 13 || ptr2[1] != 10)
					{
						byte b = HexDecodeMap[*ptr2];
						byte b2 = HexDecodeMap[ptr2[1]];
						if (b == byte.MaxValue)
						{
							throw new FormatException(System.SR.Format(System.SR.InvalidHexDigit, b));
						}
						if (b2 == byte.MaxValue)
						{
							throw new FormatException(System.SR.Format(System.SR.InvalidHexDigit, b2));
						}
						*(ptr3++) = (byte)((b << 4) + b2);
					}
					ptr2 += 2;
				}
				else
				{
					if (ReadState.Byte != 13 || *ptr2 != 10)
					{
						byte b3 = HexDecodeMap[ReadState.Byte];
						byte b4 = HexDecodeMap[*ptr2];
						if (b3 == byte.MaxValue)
						{
							throw new FormatException(System.SR.Format(System.SR.InvalidHexDigit, b3));
						}
						if (b4 == byte.MaxValue)
						{
							throw new FormatException(System.SR.Format(System.SR.InvalidHexDigit, b4));
						}
						*(ptr3++) = (byte)((b3 << 4) + b4);
					}
					ptr2++;
				}
				ReadState.IsEscaped = false;
				ReadState.Byte = -1;
			}
			while (ptr2 < ptr4)
			{
				if (*ptr2 != 61)
				{
					if (*ptr2 == 95)
					{
						*(ptr3++) = 32;
						ptr2++;
					}
					else
					{
						*(ptr3++) = *(ptr2++);
					}
					continue;
				}
				long num = ptr4 - ptr2;
				if (num != 1)
				{
					if (num != 2)
					{
						if (ptr2[1] != 13 || ptr2[2] != 10)
						{
							byte b5 = HexDecodeMap[ptr2[1]];
							byte b6 = HexDecodeMap[ptr2[2]];
							if (b5 == byte.MaxValue)
							{
								throw new FormatException(System.SR.Format(System.SR.InvalidHexDigit, b5));
							}
							if (b6 == byte.MaxValue)
							{
								throw new FormatException(System.SR.Format(System.SR.InvalidHexDigit, b6));
							}
							*(ptr3++) = (byte)((b5 << 4) + b6);
						}
						ptr2 += 3;
						continue;
					}
					ReadState.Byte = ptr2[1];
				}
				ReadState.IsEscaped = true;
				break;
			}
			return (int)(ptr3 - ptr);
		}
	}

	public int EncodeBytes(ReadOnlySpan<byte> buffer)
	{
		return _encoder.EncodeBytes(buffer, dontDeferFinalBytes: true, shouldAppendSpaceToCRLF: true);
	}

	public int EncodeString(string value, Encoding encoding)
	{
		return _encoder.EncodeString(value, encoding);
	}

	public string GetEncodedString()
	{
		return _encoder.GetEncodedString();
	}

	public override void Flush()
	{
		FlushInternal();
		base.Flush();
	}

	public override async Task FlushAsync(CancellationToken cancellationToken)
	{
		await FlushInternalAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		await base.FlushAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	private void FlushInternal()
	{
		if (_writeState != null && _writeState.Length > 0)
		{
			base.BaseStream.Write(WriteState.Buffer.AsSpan(0, WriteState.Length));
			WriteState.Reset();
		}
	}

	private async ValueTask FlushInternalAsync(CancellationToken cancellationToken)
	{
		if (_writeState != null && _writeState.Length > 0)
		{
			await base.BaseStream.WriteAsync(WriteState.Buffer.AsMemory(0, WriteState.Length), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			WriteState.Reset();
		}
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
		int num = 0;
		while (true)
		{
			num += EncodeBytes(buffer.Slice(num));
			if (num < buffer.Length)
			{
				FlushInternal();
				continue;
			}
			break;
		}
	}

	protected override async ValueTask WriteAsyncInternal(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
	{
		int written = 0;
		while (true)
		{
			written += EncodeBytes(buffer.Span.Slice(written));
			if (written >= buffer.Length)
			{
				break;
			}
			await FlushInternalAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}

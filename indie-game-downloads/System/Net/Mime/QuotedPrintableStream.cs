using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Mime;

internal sealed class QuotedPrintableStream : DelegatedStream, IEncodableStream
{
	private sealed class ReadStateInfo
	{
		internal bool IsEscaped { get; set; }

		internal short Byte { get; set; } = -1;
	}

	private readonly bool _encodeCRLF;

	private readonly int _lineLength;

	private WriteStateInfoBase _writeState;

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

	private static ReadOnlySpan<byte> HexEncodeMap => "0123456789ABCDEF"u8;

	public override bool CanRead => false;

	public override bool CanWrite => base.BaseStream.CanWrite;

	private ReadStateInfo ReadState => _003CReadState_003Ek__BackingField ?? (_003CReadState_003Ek__BackingField = new ReadStateInfo());

	internal WriteStateInfoBase WriteState => _writeState ?? (_writeState = new WriteStateInfoBase(1024, null, null, _lineLength));

	internal QuotedPrintableStream(Stream stream, int lineLength)
		: base(stream)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(lineLength, "lineLength");
		_lineLength = lineLength;
	}

	internal QuotedPrintableStream(Stream stream, bool encodeCRLF)
		: this(stream, 70)
	{
		_encodeCRLF = encodeCRLF;
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
					*(ptr3++) = *(ptr2++);
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
		int i;
		for (i = 0; i < buffer.Length; i++)
		{
			if ((_lineLength != -1 && WriteState.CurrentLineLength + 3 + 2 >= _lineLength && (buffer[i] == 32 || buffer[i] == 9 || buffer[i] == 13 || buffer[i] == 10)) || _writeState.CurrentLineLength + 3 + 2 >= 70)
			{
				if (WriteState.Buffer.Length - WriteState.Length < 3)
				{
					return i;
				}
				WriteState.Append(61);
				WriteState.AppendCRLF(includeSpace: false);
			}
			if (buffer[i] == 13 && i + 1 < buffer.Length && buffer[i + 1] == 10)
			{
				if (WriteState.Buffer.Length - WriteState.Length < (_encodeCRLF ? 6 : 2))
				{
					return i;
				}
				i++;
				if (_encodeCRLF)
				{
					WriteState.Append("=0D=0A"u8);
				}
				else
				{
					WriteState.AppendCRLF(includeSpace: false);
				}
				continue;
			}
			if ((buffer[i] < 32 && buffer[i] != 9) || buffer[i] == 61 || buffer[i] > 126)
			{
				if (WriteState.Buffer.Length - WriteState.Length < 3)
				{
					return i;
				}
				WriteState.Append(61);
				WriteState.Append(HexEncodeMap[buffer[i] >> 4]);
				WriteState.Append(HexEncodeMap[buffer[i] & 0xF]);
				continue;
			}
			if (WriteState.Buffer.Length - WriteState.Length < 1)
			{
				return i;
			}
			if ((buffer[i] == 9 || buffer[i] == 32) && i + 1 >= buffer.Length)
			{
				if (WriteState.Buffer.Length - WriteState.Length < 3)
				{
					return i;
				}
				WriteState.Append(61);
				WriteState.Append(HexEncodeMap[buffer[i] >> 4]);
				WriteState.Append(HexEncodeMap[buffer[i] & 0xF]);
			}
			else
			{
				WriteState.Append(buffer[i]);
			}
		}
		return i;
	}

	public int EncodeString(string value, Encoding encoding)
	{
		byte[] bytes = encoding.GetBytes(value);
		return EncodeBytes(bytes);
	}

	public string GetEncodedString()
	{
		return Encoding.ASCII.GetString(WriteState.Buffer, 0, WriteState.Length);
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

	private async ValueTask FlushInternalAsync(CancellationToken cancellationToken)
	{
		if (_writeState != null && _writeState.Length > 0)
		{
			await base.BaseStream.WriteAsync(WriteState.Buffer.AsMemory(0, WriteState.Length), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			WriteState.BufferFlushed();
		}
	}

	private void FlushInternal()
	{
		if (_writeState != null && _writeState.Length > 0)
		{
			base.BaseStream.Write(WriteState.Buffer, 0, WriteState.Length);
			WriteState.BufferFlushed();
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

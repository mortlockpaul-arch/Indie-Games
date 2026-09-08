using System.IO;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net;

internal sealed class Base64Stream : DelegatedStream, IEncodableStream
{
	private sealed class ReadStateInfo
	{
		internal byte Val { get; set; }

		internal byte Pos { get; set; }
	}

	private readonly Base64WriteStateInfo _writeState;

	private readonly Base64Encoder _encoder;

	[CompilerGenerated]
	private ReadStateInfo _003CReadState_003Ek__BackingField;

	private static ReadOnlySpan<byte> Base64DecodeMap => new byte[256]
	{
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		255, 255, 255, 62, 255, 255, 255, 63, 52, 53,
		54, 55, 56, 57, 58, 59, 60, 61, 255, 255,
		255, 255, 255, 255, 255, 0, 1, 2, 3, 4,
		5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
		15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
		25, 255, 255, 255, 255, 255, 255, 26, 27, 28,
		29, 30, 31, 32, 33, 34, 35, 36, 37, 38,
		39, 40, 41, 42, 43, 44, 45, 46, 47, 48,
		49, 50, 51, 255, 255, 255, 255, 255, 255, 255,
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

	public override bool CanRead => base.BaseStream.CanRead;

	public override bool CanWrite => base.BaseStream.CanWrite;

	private ReadStateInfo ReadState => _003CReadState_003Ek__BackingField ?? (_003CReadState_003Ek__BackingField = new ReadStateInfo());

	internal WriteStateInfoBase WriteState => _writeState;

	internal Base64Stream(Stream stream, Base64WriteStateInfo writeStateInfo)
		: base(stream)
	{
		_writeState = new Base64WriteStateInfo();
		_encoder = new Base64Encoder(_writeState, writeStateInfo.MaxLineLength);
	}

	internal Base64Stream(Base64WriteStateInfo writeStateInfo)
		: base(new MemoryStream())
	{
		_writeState = writeStateInfo;
		_encoder = new Base64Encoder(_writeState, writeStateInfo.MaxLineLength);
	}

	public override void Close()
	{
		if (_writeState != null && WriteState.Length > 0)
		{
			_encoder.AppendPadding();
			FlushInternal();
		}
		base.Close();
	}

	public unsafe int DecodeBytes(Span<byte> buffer)
	{
		fixed (byte* ptr = buffer)
		{
			byte* ptr2 = ptr;
			byte* ptr3 = ptr;
			byte* ptr4 = ptr + buffer.Length;
			while (ptr2 < ptr4)
			{
				if (*ptr2 == 13 || *ptr2 == 10 || *ptr2 == 61 || *ptr2 == 32 || *ptr2 == 9)
				{
					ptr2++;
					continue;
				}
				byte b = Base64DecodeMap[*ptr2];
				if (b == byte.MaxValue)
				{
					throw new FormatException(System.SR.MailBase64InvalidCharacter);
				}
				switch (ReadState.Pos)
				{
				case 0:
					ReadState.Val = (byte)(b << 2);
					ReadState.Pos++;
					break;
				case 1:
					*(ptr3++) = (byte)(ReadState.Val + (b >> 4));
					ReadState.Val = (byte)(b << 4);
					ReadState.Pos++;
					break;
				case 2:
					*(ptr3++) = (byte)(ReadState.Val + (b >> 2));
					ReadState.Val = (byte)(b << 6);
					ReadState.Pos++;
					break;
				case 3:
					*(ptr3++) = (byte)(ReadState.Val + b);
					ReadState.Pos = 0;
					break;
				}
				ptr2++;
			}
			return (int)(ptr3 - ptr);
		}
	}

	public int EncodeBytes(ReadOnlySpan<byte> buffer)
	{
		return _encoder.EncodeBytes(buffer, dontDeferFinalBytes: true, shouldAppendSpaceToCRLF: true);
	}

	internal int EncodeBytes(ReadOnlySpan<byte> buffer, bool dontDeferFinalBytes, bool shouldAppendSpaceToCRLF)
	{
		return _encoder.EncodeBytes(buffer, dontDeferFinalBytes, shouldAppendSpaceToCRLF);
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
		if (_writeState != null && WriteState.Length > 0)
		{
			FlushInternal();
		}
		base.Flush();
	}

	public override async Task FlushAsync(CancellationToken cancellationToken)
	{
		await FlushInternalAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		await base.FlushAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	private void FlushInternal()
	{
		base.BaseStream.Write(WriteState.Buffer.AsSpan(0, WriteState.Length));
		WriteState.Reset();
	}

	private async ValueTask FlushInternalAsync(CancellationToken cancellationToken)
	{
		await base.BaseStream.WriteAsync(WriteState.Buffer.AsMemory(0, WriteState.Length), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		WriteState.Reset();
	}

	protected override int ReadInternal(Span<byte> buffer)
	{
		int num;
		do
		{
			num = base.BaseStream.Read(buffer);
			if (num == 0)
			{
				return 0;
			}
			num = DecodeBytes(buffer.Slice(0, num));
		}
		while (num <= 0);
		return num;
	}

	protected override async ValueTask<int> ReadAsyncInternal(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
	{
		int num;
		do
		{
			num = await base.BaseStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			if (num == 0)
			{
				return 0;
			}
			num = DecodeBytes(buffer.Span.Slice(0, num));
		}
		while (num <= 0);
		return num;
	}

	protected override void WriteInternal(ReadOnlySpan<byte> buffer)
	{
		int num = 0;
		while (true)
		{
			num += EncodeBytes(buffer.Slice(num), dontDeferFinalBytes: false, shouldAppendSpaceToCRLF: false);
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
			written += EncodeBytes(buffer.Span.Slice(written), dontDeferFinalBytes: false, shouldAppendSpaceToCRLF: false);
			if (written >= buffer.Length)
			{
				break;
			}
			await FlushInternalAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}

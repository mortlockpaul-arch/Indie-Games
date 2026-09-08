using System.Text;

namespace System.Net.Mime;

internal abstract class ByteEncoder : IByteEncoder
{
	internal abstract WriteStateInfoBase WriteState { get; }

	protected abstract bool HasSpecialEncodingForCRLF { get; }

	public string GetEncodedString()
	{
		return Encoding.ASCII.GetString(WriteState.Buffer, 0, WriteState.Length);
	}

	public int EncodeBytes(ReadOnlySpan<byte> buffer, bool dontDeferFinalBytes, bool shouldAppendSpaceToCRLF)
	{
		WriteState.AppendHeader();
		bool hasSpecialEncodingForCRLF = HasSpecialEncodingForCRLF;
		int i;
		for (i = 0; i < buffer.Length; i++)
		{
			if (LineBreakNeeded(buffer[i]))
			{
				AppendPadding();
				WriteState.AppendCRLF(shouldAppendSpaceToCRLF);
			}
			if (hasSpecialEncodingForCRLF && buffer.Slice(i).StartsWith("\r\n"u8))
			{
				AppendEncodedCRLF();
				i++;
			}
			else
			{
				AppendEncodedByte(buffer[i]);
			}
		}
		if (dontDeferFinalBytes)
		{
			AppendPadding();
		}
		WriteState.AppendFooter();
		return i;
	}

	public int EncodeString(string value, Encoding encoding)
	{
		byte[] bytes;
		if (encoding == Encoding.Latin1)
		{
			bytes = encoding.GetBytes(value);
			return EncodeBytes(bytes, dontDeferFinalBytes: true, shouldAppendSpaceToCRLF: true);
		}
		WriteState.AppendHeader();
		bool hasSpecialEncodingForCRLF = HasSpecialEncodingForCRLF;
		int num = 0;
		bytes = new byte[encoding.GetMaxByteCount(2)];
		for (int i = 0; i < value.Length; i++)
		{
			int codepointSize = GetCodepointSize(value, i);
			int bytes2 = encoding.GetBytes(value, i, codepointSize, bytes, 0);
			Span<byte> span = bytes.AsSpan(0, bytes2);
			if (codepointSize == 2)
			{
				i++;
			}
			if (LineBreakNeeded(span))
			{
				AppendPadding();
				WriteState.AppendCRLF(includeSpace: true);
			}
			if (hasSpecialEncodingForCRLF && IsCRLF(span))
			{
				AppendEncodedCRLF();
			}
			else
			{
				AppendEncodedCodepoint(span);
			}
			num += bytes2;
		}
		AppendPadding();
		WriteState.AppendFooter();
		return num;
	}

	protected abstract void AppendEncodedCRLF();

	protected abstract bool LineBreakNeeded(byte b);

	protected abstract bool LineBreakNeeded(ReadOnlySpan<byte> bytes);

	protected abstract int GetCodepointSize(string value, int i);

	public abstract void AppendPadding();

	protected abstract void AppendEncodedByte(byte b);

	private void AppendEncodedCodepoint(ReadOnlySpan<byte> bytes)
	{
		ReadOnlySpan<byte> readOnlySpan = bytes;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			byte b = readOnlySpan[i];
			AppendEncodedByte(b);
		}
	}

	protected static bool IsSurrogatePair(string value, int i)
	{
		if (char.IsSurrogate(value[i]) && i + 1 < value.Length)
		{
			return char.IsSurrogatePair(value[i], value[i + 1]);
		}
		return false;
	}

	protected static bool IsCRLF(ReadOnlySpan<byte> buffer)
	{
		return buffer.SequenceEqual("\r\n"u8);
	}
}

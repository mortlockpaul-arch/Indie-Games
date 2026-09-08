using System.IO;

namespace XnaToFna;

public class CopyingStream : Stream
{
	public Stream Input;

	public bool LeaveInputOpen;

	public Stream Output;

	public bool LeaveOutputOpen;

	public bool Copy = true;

	public override bool CanRead => Input.CanRead;

	public override bool CanSeek => Input.CanSeek;

	public override bool CanWrite => Input.CanWrite;

	public override long Length => Input.Length;

	public override long Position
	{
		get
		{
			return Input.Position;
		}
		set
		{
			Seek(value, SeekOrigin.Begin);
		}
	}

	public CopyingStream(Stream input, Stream output)
		: this(input, leaveInputOpen: false, output, leaveOutputOpen: false)
	{
	}

	public CopyingStream(Stream input, bool leaveInputOpen, Stream output, bool leaveOutputOpen)
	{
		Input = input;
		LeaveInputOpen = leaveInputOpen;
		Output = output;
		LeaveOutputOpen = leaveOutputOpen;
	}

	public override void Flush()
	{
		Input.Flush();
		if (Copy)
		{
			Output.Flush();
		}
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		int num = Input.Read(buffer, offset, count);
		if (Copy)
		{
			Output.Write(buffer, offset, num);
		}
		return num;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		if (!Copy)
		{
			return Input.Seek(offset, origin);
		}
		long num = 0L;
		switch (origin)
		{
		case SeekOrigin.Begin:
			num = offset;
			break;
		case SeekOrigin.Current:
			num = Position + offset;
			break;
		case SeekOrigin.End:
			num = Input.Length - offset;
			break;
		}
		if (num == Position)
		{
			return Position;
		}
		if (num < Position)
		{
			Output.Seek(num, SeekOrigin.Begin);
			return Input.Seek(offset, origin);
		}
		byte[] array = new byte[offset - Position];
		for (int i = 0; i < array.Length; i += Input.Read(array, i, array.Length - i))
		{
		}
		Output.Write(array, 0, array.Length);
		return Position;
	}

	public override void SetLength(long value)
	{
		Input.SetLength(value);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		Input.Write(buffer, offset, count);
		if (Copy)
		{
			Output.Write(buffer, offset, count);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (!LeaveInputOpen)
		{
			Input.Dispose();
		}
		if (!LeaveOutputOpen)
		{
			Output.Dispose();
		}
		base.Dispose(disposing);
	}
}

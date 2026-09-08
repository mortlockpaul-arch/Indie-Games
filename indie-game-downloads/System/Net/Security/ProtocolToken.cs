using System.Buffers;

namespace System.Net.Security;

internal struct ProtocolToken
{
	internal SecurityStatusPal Status;

	internal byte[] Payload;

	internal int Size;

	internal bool RentBuffer;

	internal bool Failed
	{
		get
		{
			if (Status.ErrorCode != SecurityStatusPalErrorCode.OK)
			{
				return Status.ErrorCode != SecurityStatusPalErrorCode.ContinueNeeded;
			}
			return false;
		}
	}

	internal bool Done => Status.ErrorCode == SecurityStatusPalErrorCode.OK;

	internal int Available
	{
		get
		{
			if (Payload != null)
			{
				return Payload.Length - Size;
			}
			return 0;
		}
	}

	internal Span<byte> AvailableSpan
	{
		get
		{
			if (Payload != null)
			{
				return new Span<byte>(Payload, Size, Available);
			}
			return Span<byte>.Empty;
		}
	}

	internal void EnsureAvailableSpace(int size)
	{
		if (Available >= size)
		{
			return;
		}
		byte[] payload = Payload;
		Payload = (RentBuffer ? ArrayPool<byte>.Shared.Rent(Size + size) : new byte[Size + size]);
		if (payload != null)
		{
			payload.AsSpan().CopyTo(Payload);
			if (RentBuffer)
			{
				ArrayPool<byte>.Shared.Return(payload);
			}
		}
	}

	internal ReadOnlyMemory<byte> AsMemory()
	{
		return new ReadOnlyMemory<byte>(Payload, 0, Size);
	}

	internal void ReleasePayload()
	{
		byte[] payload = Payload;
		Payload = null;
		Size = 0;
		if (RentBuffer && payload != null)
		{
			ArrayPool<byte>.Shared.Return(payload);
		}
	}

	internal Exception GetException()
	{
		if (!Done)
		{
			return SslStreamPal.GetException(Status);
		}
		return null;
	}
}

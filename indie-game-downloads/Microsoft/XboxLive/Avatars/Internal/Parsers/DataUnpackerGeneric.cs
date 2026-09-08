using System;

namespace Microsoft.XboxLive.Avatars.Internal.Parsers;

public abstract class DataUnpackerGeneric<Type>
{
	public virtual int GetHeaderBitCount()
	{
		throw new NotImplementedException();
	}

	public virtual int GetPerDataBitCount()
	{
		throw new NotImplementedException();
	}

	public abstract void UnpackHeader(BitStream bitStream);

	public abstract void UnpackData(BitStream bitStream, out Type data);

	public DataUnpackerGeneric()
	{
	}
}
